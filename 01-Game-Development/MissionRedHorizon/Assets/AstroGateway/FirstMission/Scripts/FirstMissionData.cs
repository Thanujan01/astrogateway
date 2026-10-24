using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using AstroGateway.Integration;
using AstroGateway.MissionRules;

// First Mission sensor data: catalog, record schema, client-side validation, bounded offline store, upload queue
// and the scientific-analysis seam. Mirrors AstroGatewayAndroid/backend/sensor_records.py and shares
// Config/sensor_catalog_v1.json with it (a backend test checks both copies are byte-identical).
// Honesty rules enforced here:
//  - a client record is only ever SIMULATED or UNAVAILABLE;
//  - not-modelled sensors are UNAVAILABLE with no value;
//  - an analysis result can only be stored from a backend acknowledgement marked BACKEND-ANALYZED, and the
//    backend currently returns "not_connected" because no scientific model exists.
namespace AstroGateway.FirstMission
{
    public static class FirstMissionIds
    {
        public const string MissionId = "ag.mars.red-horizon.surface-ops";   // canonical (Step 4 proposal, approved 2026-10-20)
        public const int MissionVersion = 1;
        public const string BuildVariant = "first_mission_v1";
        public const string SimConfigVersion = "fm-sensors-1";
        public const int SchemaVersion = 1;
        public const string AnalysisInterface = "ag.science-analysis.v0";
    }

    public static class Provenance
    {
        public const string Simulated = "SIMULATED", Unavailable = "UNAVAILABLE", ModelEstimated = "MODEL-ESTIMATED", BackendAnalyzed = "BACKEND-ANALYZED";
    }

    // Upload state of one record (shown verbatim in the DATA tab).
    public static class UploadState
    {
        public const string CollectedLocally = "COLLECTED LOCALLY", WaitingForSync = "WAITING FOR SYNC", Uploading = "UPLOADING",
            Uploaded = "UPLOADED", Failed = "FAILED — RETRY AVAILABLE", Rejected = "REJECTED BY SERVER", Offline = "OFFLINE";
    }

    // Analysis state of one record. PROCESSING / ANALYSIS COMPLETE only ever come from the backend.
    public static class AnalysisState
    {
        public const string NotConnected = "NOT CONNECTED", Unavailable = "UNAVAILABLE", Processing = "PROCESSING", Complete = "ANALYSIS COMPLETE";
    }

    // ---------------- catalog ----------------
    public class SensorDef
    {
        public string id, type, kind, unit, method, pattern;
        public bool implemented;
        public double min, max;
        public string[] values = new string[0];
    }

    public class InstrumentDef
    {
        public string id, name, unavailableReason = "";
        public bool available, requiresStopped;
        public float maxRange;
        public string[] sensors = new string[0];
    }

    public class SensorCatalog
    {
        public readonly string Version;
        public readonly Dictionary<string, SensorDef> Sensors = new Dictionary<string, SensorDef>();
        public readonly List<InstrumentDef> Instruments = new List<InstrumentDef>();
        public readonly float CautionSlope, HazardSlope, CautionRough, HazardRough;
        public readonly int MaxBatch;

        public SensorCatalog(string json)
        {
            var raw = (Dictionary<string, object>)MiniJson.Parse(json);
            Version = (string)raw["catalog_version"];
            foreach (Dictionary<string, object> s in (List<object>)raw["sensors"])
            {
                var d = new SensorDef
                {
                    id = (string)s["sensor_id"], type = (string)s["sensor_type"], kind = (string)s["kind"], unit = (string)s["unit"],
                    method = (string)s["method"], implemented = (bool)s["implemented"],
                    min = s.TryGetValue("min", out var mn) ? Convert.ToDouble(mn) : 0, max = s.TryGetValue("max", out var mx) ? Convert.ToDouble(mx) : 0,
                    pattern = s.TryGetValue("pattern", out var p) ? (string)p : null,
                };
                if (s.TryGetValue("values", out var v)) d.values = ((List<object>)v).Cast<string>().ToArray();
                Sensors[d.id] = d;
            }
            foreach (Dictionary<string, object> i in (List<object>)raw["instruments"])
                Instruments.Add(new InstrumentDef
                {
                    id = (string)i["instrument_id"], name = (string)i["name"], available = (bool)i["available"],
                    requiresStopped = (bool)i["requires_stopped"], maxRange = Convert.ToSingle(i["max_range_m"]),
                    sensors = ((List<object>)i["sensors"]).Cast<string>().ToArray(),
                    unavailableReason = i.TryGetValue("unavailable_reason", out var r) ? (string)r : "",
                });
            var rule = (Dictionary<string, object>)((Dictionary<string, object>)raw["rules"])["traversability_v1"];
            CautionSlope = Convert.ToSingle(rule["caution_slope_deg"]); HazardSlope = Convert.ToSingle(rule["hazard_slope_deg"]);
            CautionRough = Convert.ToSingle(rule["caution_roughness_m"]); HazardRough = Convert.ToSingle(rule["hazard_roughness_m"]);
            MaxBatch = Convert.ToInt32(((Dictionary<string, object>)raw["limits"])["max_batch"]);
        }

        public InstrumentDef Instrument(string id) => Instruments.FirstOrDefault(i => i.id == id);
    }

    // ---------------- record ----------------
    [Serializable]
    public class SensorRecord
    {
        public int schemaVersion = FirstMissionIds.SchemaVersion;
        public string eventId, observationId, missionId = FirstMissionIds.MissionId, buildVariant = FirstMissionIds.BuildVariant;
        public int missionVersion = FirstMissionIds.MissionVersion;
        public string localSessionId;          // device-local mission session; the server session is the URL path
        public string sensorId, sensorType, instrumentId, stationId = "";
        public float simTimeS;
        public long simTick;
        public string capturedAtUtc;
        public Vector3 location;
        public bool hasValue;
        public double value;
        public string valueText = "", unit, provenance, method, simConfigVersion = FirstMissionIds.SimConfigVersion;
        // sync / analysis bookkeeping (local only, never sent)
        public string uploadState = UploadState.CollectedLocally, analysisState = AnalysisState.NotConnected;
        public string analysisProvenance = "", analysisModelVersion = "", analysisSummary = "", lastError = "";
        public bool hasConfidence;
        public float confidence;
        public int attempts;

        public string ValueLabel => provenance == Provenance.Unavailable ? "unavailable" : hasValue ? value.ToString("0.###", CultureInfo.InvariantCulture) + " " + unit
            : !string.IsNullOrEmpty(valueText) ? valueText : "(" + location.x.ToString("0.0", CultureInfo.InvariantCulture) + ", " + location.z.ToString("0.0", CultureInfo.InvariantCulture) + ") m";

        static string S(string v) => v == null ? "null" : "\"" + v.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
        static string F(double v) => v.ToString("R", CultureInfo.InvariantCulture);

        // Wire body for POST /v1/mission-sessions/{id}/sensor-records (snake_case, like the FastAPI models).
        public string ToWireJson()
        {
            var b = new StringBuilder("{");
            b.Append("\"schema_version\":").Append(schemaVersion);
            b.Append(",\"event_id\":").Append(S(eventId)).Append(",\"observation_id\":").Append(S(observationId));
            b.Append(",\"mission_id\":").Append(S(missionId)).Append(",\"mission_version\":").Append(missionVersion);
            b.Append(",\"build_variant\":").Append(S(buildVariant)).Append(",\"sensor_id\":").Append(S(sensorId));
            b.Append(",\"sensor_type\":").Append(S(sensorType)).Append(",\"instrument_id\":").Append(S(instrumentId));
            b.Append(",\"station_id\":").Append(string.IsNullOrEmpty(stationId) ? "null" : S(stationId));
            b.Append(",\"sim_time_s\":").Append(F(Math.Round(simTimeS, 3))).Append(",\"sim_tick\":").Append(simTick);
            b.Append(",\"captured_at\":").Append(S(capturedAtUtc));
            b.Append(",\"location\":{\"x_m\":").Append(F(Math.Round(location.x, 3))).Append(",\"y_m\":").Append(F(Math.Round(location.y, 3)))
             .Append(",\"z_m\":").Append(F(Math.Round(location.z, 3))).Append(",\"frame\":\"scene_local_m\"}");
            b.Append(",\"value\":").Append(hasValue ? F(value) : "null");
            b.Append(",\"value_text\":").Append(string.IsNullOrEmpty(valueText) ? "null" : S(valueText));
            b.Append(",\"unit\":").Append(S(unit)).Append(",\"provenance\":").Append(S(provenance));
            b.Append(",\"method\":").Append(S(method)).Append(",\"sim_config_version\":").Append(S(simConfigVersion)).Append("}");
            return b.ToString();
        }
    }

    // Same rules as the backend (defence in depth; the server is authoritative).
    public static class SensorValidation
    {
        public static string Validate(SensorRecord r, SensorCatalog c)
        {
            if (r == null) return "null record";
            if (r.schemaVersion != FirstMissionIds.SchemaVersion || r.simConfigVersion != c.Version) return "schema/config version";
            if (!Guid.TryParse(r.eventId, out _) || !Guid.TryParse(r.observationId, out _)) return "event/observation id";
            if (r.provenance != Provenance.Simulated && r.provenance != Provenance.Unavailable) return "client provenance must be SIMULATED or UNAVAILABLE";
            if (!c.Sensors.TryGetValue(r.sensorId ?? "", out var s)) return "unknown sensor";
            var inst = c.Instrument(r.instrumentId);
            if (inst == null || !inst.sensors.Contains(r.sensorId)) return "sensor not on instrument";
            if (s.type != r.sensorType || s.unit != r.unit) return "type/unit mismatch";
            if (!Finite(r.location.x) || !Finite(r.location.y) || !Finite(r.location.z) || Mathf.Abs(r.location.x) > 200 || Mathf.Abs(r.location.z) > 200 || r.location.y < -50 || r.location.y > 200) return "location out of bounds";
            if (r.simTimeS < 0 || r.simTimeS > 86400 || r.simTick < 0) return "sim time";
            if (!DateTime.TryParse(r.capturedAtUtc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var t) || t.Kind != DateTimeKind.Utc) return "captured_at";
            if (!string.IsNullOrEmpty(r.stationId) && !System.Text.RegularExpressions.Regex.IsMatch(r.stationId, "^site_[0-9]{1,3}$")) return "station id";
            if (r.provenance == Provenance.Unavailable || !s.implemented || !inst.available)
                return r.provenance == Provenance.Unavailable && !r.hasValue && string.IsNullOrEmpty(r.valueText) ? null : "not-modelled sensor must be UNAVAILABLE without a value";
            if (r.method != s.method) return "method mismatch";
            switch (s.kind)
            {
                case "numeric": return r.hasValue && Finite(r.value) && r.value >= s.min && r.value <= s.max && string.IsNullOrEmpty(r.valueText) ? null : "numeric value missing/out of range";
                case "category": return !r.hasValue && s.values.Contains(r.valueText) ? null : "category not allowed";
                case "identifier":
                    if (r.hasValue || string.IsNullOrEmpty(r.valueText) || !System.Text.RegularExpressions.Regex.IsMatch(r.valueText, s.pattern)) return "identifier invalid";
                    return r.sensorId == "sci.station_id" && r.valueText != r.stationId ? "station mismatch" : null;
                case "location": return !r.hasValue && string.IsNullOrEmpty(r.valueText) ? null : "location carries no value";
            }
            return "unknown kind";
        }
        static bool Finite(double v) => !double.IsNaN(v) && !double.IsInfinity(v);
    }

    // ---------------- bounded offline store ----------------
    [Serializable] class StoreFile { public int version = 1; public List<SensorRecord> records = new List<SensorRecord>(); }

    // Persistent, bounded, de-duplicated. Writes go to a temp file then replace the old one, so a crash never
    // leaves a half-written store. When full, the oldest records the server already acknowledged are evicted;
    // pending observations are never dropped silently (Add refuses instead).
    public class SensorRecordStore
    {
        public readonly string Path;
        public readonly int MaxRecords;
        public const int MaxFileBytes = 1 << 20;
        readonly List<SensorRecord> records = new List<SensorRecord>();
        public IReadOnlyList<SensorRecord> Records => records;
        public string LastError { get; private set; } = "";
        public event Action Changed;

        public SensorRecordStore(string path, int maxRecords = 400) { Path = path; MaxRecords = maxRecords; Load(); }

        public int Pending => records.Count(r => r.uploadState != UploadState.Uploaded && r.uploadState != UploadState.Rejected);
        public bool Contains(string eventId) => records.Any(r => r.eventId == eventId);

        public bool Add(IEnumerable<SensorRecord> batch, out string reason)
        {
            var list = batch.Where(r => !Contains(r.eventId)).ToList();
            int overflow = records.Count + list.Count - MaxRecords;
            if (overflow > 0)
            {
                var evictable = records.Where(r => r.uploadState == UploadState.Uploaded).Take(overflow).ToList();
                if (evictable.Count < overflow) { reason = "Local storage full: " + Pending + " records still waiting for sync"; return false; }
                foreach (var e in evictable) records.Remove(e);
            }
            records.AddRange(list);
            reason = "";
            return Save();
        }

        public bool Save()
        {
            try
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
                var json = JsonUtility.ToJson(new StoreFile { records = records });
                if (Encoding.UTF8.GetByteCount(json) > MaxFileBytes) { LastError = "store over size limit"; return false; }
                string tmp = Path + ".tmp";
                File.WriteAllText(tmp, json);
                if (File.Exists(Path)) File.Replace(tmp, Path, null); else File.Move(tmp, Path);
                LastError = "";
                Changed?.Invoke();
                return true;
            }
            catch (Exception e) { LastError = "save failed: " + e.GetType().Name; return false; }
        }

        void Load()
        {
            records.Clear();
            try
            {
                if (!File.Exists(Path) || new FileInfo(Path).Length > MaxFileBytes) return;
                var f = JsonUtility.FromJson<StoreFile>(File.ReadAllText(Path));
                if (f == null || f.version != 1 || f.records == null) { LastError = "store unreadable; ignored"; return; }
                var seen = new HashSet<string>();
                foreach (var r in f.records.Take(MaxRecords))
                    if (r != null && Guid.TryParse(r.eventId, out _) && seen.Add(r.eventId))
                    {
                        if (r.uploadState == UploadState.Uploading) r.uploadState = UploadState.WaitingForSync; // interrupted mid-flight
                        records.Add(r);
                    }
            }
            catch (Exception e) { LastError = "load failed: " + e.GetType().Name; records.Clear(); }
        }

        public void Touch() => Save();
    }

    // ---------------- upload ----------------
    public class UploadAck
    {
        public string eventId, analysisStatus, analysisProvenance, modelVersion, summary;
        public bool duplicate, hasConfidence;
        public float confidence;
    }

    public class UploadResult
    {
        public long code;            // 0 = no connection
        public string error = "";
        public List<UploadAck> acks = new List<UploadAck>();
    }

    public interface ISensorRecordUploader
    {
        bool Online { get; }
        string Label { get; }
        void Upload(IList<SensorRecord> batch, Action<UploadResult> done);
    }

    // Default in this build: no hosted backend or sign-in exists, so nothing is transmitted.
    public class NotConnectedUploader : ISensorRecordUploader
    {
        public bool Online => false;
        public string Label => "Backend NOT CONNECTED • records kept on this device";
        public void Upload(IList<SensorRecord> batch, Action<UploadResult> done) => done(new UploadResult { code = 0, error = "not connected" });
    }

    // POST /v1/mission-sessions/{id}/sensor-records with the signed-in user's bearer token (never stored by the game).
    // Only used by the local-server verification test until a hosted backend is approved.
    public class HttpSensorRecordUploader : ISensorRecordUploader
    {
        readonly string baseUrl;
        readonly IIdentityProvider identity;
        readonly Func<string> serverSessionId;
        public HttpSensorRecordUploader(string baseUrl, IIdentityProvider identity, Func<string> serverSessionId)
        {
            var uri = new Uri(baseUrl);
            if (uri.Scheme != "https" && !(uri.IsLoopback && uri.Scheme == "http")) throw new ArgumentException("Backend URL must use HTTPS (plain HTTP only for localhost testing).");
            this.baseUrl = baseUrl.TrimEnd('/'); this.identity = identity; this.serverSessionId = serverSessionId;
        }
        public bool Online => !string.IsNullOrEmpty(identity?.AccessToken) && !string.IsNullOrEmpty(serverSessionId?.Invoke());
        public string Label => Online ? "Backend session active" : "Backend NOT CONNECTED • no signed-in session";

        public void Upload(IList<SensorRecord> batch, Action<UploadResult> done)
        {
            if (!Online) { done(new UploadResult { error = "not connected" }); return; }
            string body = "{\"records\":[" + string.Join(",", batch.Select(r => r.ToWireJson())) + "]}";
            var req = new UnityWebRequest(baseUrl + "/v1/mission-sessions/" + serverSessionId() + "/sensor-records", "POST")
            { downloadHandler = new DownloadHandlerBuffer(), uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body)), timeout = 15 };
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", "Bearer " + identity.AccessToken);
            req.SendWebRequest().completed += _ =>
            {
                var result = new UploadResult { code = req.result == UnityWebRequest.Result.ConnectionError ? 0 : req.responseCode };
                try
                {
                    if (result.code == 200) result.acks = ParseAcks(req.downloadHandler.text);
                    else result.error = result.code == 0 ? "no connection" : "HTTP " + result.code;
                }
                catch (Exception) { result.code = 502; result.error = "invalid server response"; result.acks.Clear(); }
                req.Dispose();
                done(result);
            };
        }

        public static List<UploadAck> ParseAcks(string json)
        {
            var root = (Dictionary<string, object>)MiniJson.Parse(json);
            var list = new List<UploadAck>();
            foreach (Dictionary<string, object> r in (List<object>)root["records"])
            {
                var a = (Dictionary<string, object>)r["analysis"];
                var ack = new UploadAck { eventId = (string)r["event_id"], duplicate = (bool)r["duplicate"], analysisStatus = (string)a["status"] };
                // Analysis fields are taken only when the server explicitly says it ran a model.
                if (ack.analysisStatus == "complete" && a.TryGetValue("provenance", out var p) && (string)p == Provenance.BackendAnalyzed)
                {
                    ack.analysisProvenance = Provenance.BackendAnalyzed;
                    ack.modelVersion = a.TryGetValue("model_version", out var mv) ? mv as string : null;
                    ack.summary = a.TryGetValue("result", out var res) ? res as string : null;
                    if (a.TryGetValue("confidence", out var cf) && cf is double d) { ack.hasConfidence = true; ack.confidence = (float)d; }
                }
                list.Add(ack);
            }
            return list;
        }
    }

    // Retry policy: one request at a time, oldest first, up to MaxBatch records; exponential backoff on transient
    // failures (no connection, 429, 5xx) from 2 s up to 5 min; 401/403 stop until RetryNow; 409/422 mark the
    // records rejected (kept on the device, never resent automatically). Duplicates acknowledged by the server
    // (retries) count as uploaded, so the same observation is never processed twice.
    public class SensorSyncService
    {
        public ISensorRecordUploader Uploader;
        readonly SensorRecordStore store;
        readonly int maxBatch;
        bool inFlight, authBlocked;
        int failures;
        float nextAttempt;
        public string Status { get; private set; } = "";
        public float NextAttemptIn(float now) => Mathf.Max(0, nextAttempt - now);

        public SensorSyncService(SensorRecordStore store, ISensorRecordUploader uploader, int maxBatch) { this.store = store; Uploader = uploader; this.maxBatch = maxBatch; }

        public void RetryNow() { authBlocked = false; failures = 0; nextAttempt = 0; foreach (var r in store.Records) if (r.uploadState == UploadState.Failed) r.uploadState = UploadState.WaitingForSync; }

        public void Tick(float now)
        {
            bool online = Uploader != null && Uploader.Online;
            foreach (var r in store.Records)
            {
                bool open = r.uploadState != UploadState.Uploaded && r.uploadState != UploadState.Rejected && r.uploadState != UploadState.Uploading;
                if (open && !online) r.uploadState = UploadState.Offline;
                else if (open && online && r.uploadState == UploadState.Offline) r.uploadState = UploadState.WaitingForSync;
                else if (open && r.uploadState == UploadState.CollectedLocally && online) r.uploadState = UploadState.WaitingForSync;
            }
            Status = !online ? Uploader?.Label ?? "No uploader" : authBlocked ? "Sign-in rejected • RETRY after signing in" : inFlight ? "Uploading…" : store.Pending == 0 ? "All records synced" : failures > 0 ? "Retry in " + Mathf.CeilToInt(NextAttemptIn(now)) + " s" : "Waiting for sync";
            if (!online || inFlight || authBlocked || now < nextAttempt) return;
            foreach (var r in store.Records) if (r.uploadState == UploadState.Failed) r.uploadState = UploadState.WaitingForSync;
            var batch = store.Records.Where(r => r.uploadState == UploadState.WaitingForSync).Take(maxBatch).ToList();
            if (batch.Count == 0) return;
            inFlight = true;
            foreach (var r in batch) { r.uploadState = UploadState.Uploading; r.attempts++; }
            Uploader.Upload(batch, result => Complete(batch, result, now));
        }

        void Complete(List<SensorRecord> batch, UploadResult result, float startedAt)
        {
            inFlight = false;
            if (result.code == 200 && result.acks.Count == batch.Count && batch.All(b => result.acks.Any(a => a.eventId == b.eventId)))
            {
                failures = 0;
                foreach (var r in batch)
                {
                    var a = result.acks.First(x => x.eventId == r.eventId);
                    r.uploadState = UploadState.Uploaded; r.lastError = a.duplicate ? "server already had this record" : "";
                    if (a.analysisProvenance == Provenance.BackendAnalyzed)
                    {
                        r.analysisState = AnalysisState.Complete; r.analysisProvenance = Provenance.BackendAnalyzed;
                        r.analysisModelVersion = a.modelVersion ?? ""; r.analysisSummary = a.summary ?? ""; r.hasConfidence = a.hasConfidence; r.confidence = a.confidence;
                    }
                    else if (string.IsNullOrEmpty(r.analysisProvenance))
                        r.analysisState = r.provenance == Provenance.Unavailable ? AnalysisState.Unavailable : AnalysisState.NotConnected; // never overwrite a stored server result
                }
            }
            else if (result.code == 401 || result.code == 403)
            {
                authBlocked = true;
                foreach (var r in batch) { r.uploadState = UploadState.Failed; r.lastError = "sign-in required"; }
            }
            else if (result.code == 409 || result.code == 422)
            {
                foreach (var r in batch) { r.uploadState = UploadState.Rejected; r.lastError = result.code == 409 ? "conflicts with a stored record" : "failed server validation"; }
            }
            else
            {
                failures++;
                nextAttempt = startedAt + Mathf.Min(300, 2 * Mathf.Pow(2, failures - 1)) * UnityEngine.Random.Range(.85f, 1.15f);
                // Shown as FAILED — RETRY AVAILABLE; Tick puts them back in the queue when the backoff ends.
                foreach (var r in batch) { r.uploadState = UploadState.Failed; r.lastError = string.IsNullOrEmpty(result.error) ? "HTTP " + result.code : result.error; }
            }
            store.Touch();
        }
    }

    // ---------------- scientific analysis seam ----------------
    public enum ModelConnection { NotConnected, Connected }

    // Versioned seam for the team's future scientific model. Results are produced by the backend and arrive with
    // sensor-record acknowledgements; the client has no inference method on purpose, so it cannot fabricate one.
    public interface IScientificAnalysisService
    {
        string InterfaceVersion { get; }
        ModelConnection Connection { get; }
        string ModelVersion { get; }
        string StatusLabel { get; }
    }

    public class NotConnectedAnalysisService : IScientificAnalysisService
    {
        public string InterfaceVersion => FirstMissionIds.AnalysisInterface;
        public ModelConnection Connection => ModelConnection.NotConnected;
        public string ModelVersion => "none";
        public string StatusLabel => "AI MODEL • NOT CONNECTED (team model not yet available)";
    }
}
