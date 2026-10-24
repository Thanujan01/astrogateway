using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using AstroGateway.Integration;

// First Mission: HttpSensorRecordUploader + SensorSyncService against the LOCAL FastAPI app
// (AstroGatewayAndroid/backend/tests/local_mock_server.py: real routes and validation, mock Auth, in-memory
// repository). Batch mode, edit mode, no scene. Plain HTTP to 127.0.0.1 is allowed for the run only and the
// project setting is restored afterwards. This is NOT a hosted backend and NO AI model is involved.
// Env: FM_URL (default http://127.0.0.1:8767), FM_OUT (results folder).
namespace AstroGateway.FirstMission.EditorTools
{
    public static class FirstMissionBackendClientTest
    {
        const string FixtureUser = "11111111-1111-4111-8111-111111111111", FixtureIssuer = "https://fixture.supabase.co/auth/v1";
        static string Url => Environment.GetEnvironmentVariable("FM_URL") ?? "http://127.0.0.1:8767";
        static string Out => Environment.GetEnvironmentVariable("FM_OUT") ?? Path.GetFullPath("Docs/FirstMission/Backend");
        static readonly List<string> lines = new List<string>();
        static int pass, fail;
        static IEnumerator routine;
        static Func<bool> waitFor;
        static double deadline, started;
        static InsecureHttpOption priorHttp;

        public static void Run()
        {
            priorHttp = PlayerSettings.insecureHttpOption;
            PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
            started = EditorApplication.timeSinceStartup;
            routine = Tests();
            EditorApplication.update += Tick;
        }

        static void Tick()
        {
            try
            {
                if (EditorApplication.timeSinceStartup - started > 240) throw new Exception("timeout");
                if (waitFor != null)
                {
                    if (!waitFor() && EditorApplication.timeSinceStartup < deadline) return;
                    if (!waitFor()) Line("WARN  wait timed out");
                    waitFor = null;
                }
                if (!routine.MoveNext()) { Finish(); return; }
                if (routine.Current is Func<bool> f) { waitFor = f; deadline = EditorApplication.timeSinceStartup + 20; }
            }
            catch (Exception e) { Line("EXCEPTION: " + e); fail++; Finish(); }
        }

        static void Line(string s) { lines.Add(s); Debug.Log("[FirstMissionBackend] " + s); }
        static void Check(string n, bool ok, string d = "") { if (ok) pass++; else fail++; Line((ok ? "PASS  " : "FAIL  ") + n + (string.IsNullOrEmpty(d) ? "" : "  [" + d + "]")); }
        static string B64(string s) => Convert.ToBase64String(Encoding.UTF8.GetBytes(s)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        // Synthetic fixture token accepted only by the local mock Auth transport. Not a real credential.
        static string FixtureToken(string sessionId) =>
            B64("{\"alg\":\"ES256\"}") + "." + B64("{\"iss\":\"" + FixtureIssuer + "\",\"aud\":\"authenticated\",\"role\":\"authenticated\",\"sub\":\"" + FixtureUser +
            "\",\"session_id\":\"" + sessionId + "\",\"exp\":" + (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 600) + "}") + ".fixture-not-a-signature";

        static long code; static string text;
        static Func<bool> Raw(string method, string path, string token, string body = null)
        {
            code = -1;
            var req = new UnityWebRequest(Url + path, method) { downloadHandler = new DownloadHandlerBuffer() };
            if (body != null) { req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body)); req.SetRequestHeader("Content-Type", "application/json"); }
            req.SetRequestHeader("Authorization", "Bearer " + token);
            req.SendWebRequest().completed += _ => { text = req.downloadHandler.text; code = req.responseCode; req.Dispose(); };
            return () => code != -1;
        }

        static SensorRecord Rec(SensorCatalog c, string sensor, string instrument, Action<SensorRecord> set = null)
        {
            var d = c.Sensors[sensor];
            var r = new SensorRecord { eventId = Guid.NewGuid().ToString(), observationId = Guid.NewGuid().ToString(), localSessionId = Guid.NewGuid().ToString(), sensorId = sensor, sensorType = d.type, instrumentId = instrument, unit = d.unit, method = d.method, provenance = Provenance.Simulated, simTimeS = 12.5f, simTick = 12, capturedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture), location = new Vector3(-15, .03f, -93) };
            set?.Invoke(r);
            return r;
        }

        static IEnumerator Tests()
        {
            Line("Backend: " + Url + " — local FastAPI routes, mock Auth, in-memory repository (no hosted service, no AI model)");
            var catalog = new SensorCatalog(File.ReadAllText("Assets/AstroGateway/FirstMission/Config/sensor_catalog_v1.json"));
            string token = FixtureToken(Guid.NewGuid().ToString());
            yield return Raw("POST", "/v1/mission-sessions", token, "{\"launch_id\":\"" + Guid.NewGuid() + "\",\"mission_id\":\"mission-red-horizon-poc\"}");
            Check("Mission session created on the local server", code == 201, code + " " + text);
            string session = code == 201 ? (string)((Dictionary<string, object>)AstroGateway.MissionRules.MiniJson.Parse(text))["id"] : "";

            var identity = new ShellIdentityProvider();
            string dir = Path.Combine(Path.GetTempPath(), "FirstMissionBackendTest_" + Guid.NewGuid().ToString("N"));
            var store = new SensorRecordStore(Path.Combine(dir, "sensor_records_v1.json"));
            var uploader = new HttpSensorRecordUploader(Url, identity, () => session);
            var sync = new SensorSyncService(store, uploader, catalog.MaxBatch);
            var recs = new List<SensorRecord>
            {
                Rec(catalog, "terrain.slope", "rover.terrain_profiler", r => { r.hasValue = true; r.value = 4.2; }),
                Rec(catalog, "terrain.traversability", "rover.terrain_profiler", r => r.valueText = "nominal"),
                Rec(catalog, "sci.station_id", "rover.context_camera", r => { r.stationId = "site_3"; r.valueText = "site_3"; }),
                Rec(catalog, "nav.position", "rover.navigation"),
                Rec(catalog, "env.air_temperature", "rover.env_package", r => { r.provenance = Provenance.Unavailable; r.method = "not_modeled"; }),
            };
            Check("All test records pass local validation", recs.All(r => SensorValidation.Validate(r, catalog) == null), string.Join(";", recs.Select(r => SensorValidation.Validate(r, catalog))));
            store.Add(recs, out _);

            sync.Tick((float)EditorApplication.timeSinceStartup);
            Check("Without a signed-in session nothing is sent (OFFLINE)", !uploader.Online && store.Records.All(r => r.uploadState == UploadState.Offline));
            identity.SetSession(FixtureUser, "Fixture trainee", token);
            sync.Tick((float)EditorApplication.timeSinceStartup);
            yield return (Func<bool>)(() => store.Records.All(r => r.uploadState != UploadState.Uploading));
            Check("Records uploaded to the local backend and acknowledged", store.Records.All(r => r.uploadState == UploadState.Uploaded), string.Join(",", store.Records.Select(r => r.uploadState + "/" + r.lastError).Distinct()));
            Check("Backend analysis status is NOT CONNECTED / UNAVAILABLE (no model)", store.Records.Where(r => r.provenance == Provenance.Simulated).All(r => r.analysisState == AnalysisState.NotConnected) && store.Records.Where(r => r.provenance == Provenance.Unavailable).All(r => r.analysisState == AnalysisState.Unavailable));

            // Retry the same batch (e.g. app killed before the ack was saved): server says duplicate.
            foreach (var r in store.Records) r.uploadState = UploadState.WaitingForSync;
            sync.Tick((float)EditorApplication.timeSinceStartup + 1);
            yield return (Func<bool>)(() => store.Records.All(r => r.uploadState != UploadState.Uploading));
            Check("Retried records are acknowledged as duplicates, not stored twice", store.Records.All(r => r.uploadState == UploadState.Uploaded && r.lastError == "server already had this record"));
            string batch = "{\"records\":[" + store.Records[0].ToWireJson() + "]}";
            yield return Raw("POST", "/v1/mission-sessions/" + session + "/sensor-records", token, batch);
            Check("Server reports duplicates=1 accepted=0 for a resend", code == 200 && text.Contains("\"accepted\":0") && text.Contains("\"duplicates\":1"), text);

            // A tampered client record claiming an AI verdict is refused.
            var fake = JsonUtility.FromJson<SensorRecord>(JsonUtility.ToJson(store.Records[0])); fake.eventId = Guid.NewGuid().ToString(); fake.provenance = Provenance.BackendAnalyzed;
            yield return Raw("POST", "/v1/mission-sessions/" + session + "/sensor-records", token, "{\"records\":[" + fake.ToWireJson() + "]}");
            Check("Server rejects a client record claiming BACKEND-ANALYZED (422)", code == 422, code.ToString());
            var mod = JsonUtility.FromJson<SensorRecord>(JsonUtility.ToJson(store.Records[0])); mod.value = 9.9;
            yield return Raw("POST", "/v1/mission-sessions/" + session + "/sensor-records", token, "{\"records\":[" + mod.ToWireJson() + "]}");
            Check("Same event id with different content conflicts (409)", code == 409, code.ToString());
            yield return Raw("GET", "/v1/science/analysis-status", token);
            Check("Analysis status endpoint: not_connected, no model version", code == 200 && text.Contains("\"status\":\"not_connected\"") && text.Contains("\"model_version\":null"), text);

            // Server unreachable: transient failure with backoff, records kept.
            var downStore = new SensorRecordStore(Path.Combine(dir, "down", "sensor_records_v1.json"));
            downStore.Add(new[] { Rec(catalog, "nav.position", "rover.navigation") }, out _);
            var downSync = new SensorSyncService(downStore, new HttpSensorRecordUploader("http://127.0.0.1:8799", identity, () => session), catalog.MaxBatch);
            downSync.Tick((float)EditorApplication.timeSinceStartup);
            yield return (Func<bool>)(() => downStore.Records.All(r => r.uploadState != UploadState.Uploading));
            Check("Unreachable server: FAILED — RETRY AVAILABLE, record kept, retry scheduled", downStore.Records.All(r => r.uploadState == UploadState.Failed) && downSync.NextAttemptIn((float)EditorApplication.timeSinceStartup) > 0, downStore.Records[0].lastError + " / " + downSync.Status);

            // Revoked sign-in: stop until RETRY.
            yield return Raw("POST", "/v1/auth-session/revoke", token);
            store.Add(new[] { Rec(catalog, "nav.position", "rover.navigation") }, out _);
            sync.Tick((float)EditorApplication.timeSinceStartup + 5);
            yield return (Func<bool>)(() => store.Records.All(r => r.uploadState != UploadState.Uploading));
            Check("Revoked session: upload stops with sign-in required", sync.Status.StartsWith("Sign-in") || store.Records.Last().uploadState == UploadState.Failed, sync.Status + " / " + store.Records.Last().lastError);
            try { Directory.Delete(dir, true); } catch { }
        }

        static void Finish()
        {
            EditorApplication.update -= Tick;
            PlayerSettings.insecureHttpOption = priorHttp;
            AssetDatabase.SaveAssets();
            Directory.CreateDirectory(Out);
            var all = new List<string> { $"First Mission Unity client vs LOCAL backend — {DateTime.Now:yyyy-MM-dd HH:mm:ss}", $"PASS {pass}  FAIL {fail}", "Local FastAPI app with mock Auth + in-memory repository. Not a hosted service. No AI model exists, so no inference was called.", "" };
            all.AddRange(lines);
            File.WriteAllLines(Path.Combine(Out, "Backend_Client_Results.txt"), all);
            if (Application.isBatchMode) EditorApplication.Exit(fail == 0 ? 0 : 1);
        }
    }
}
