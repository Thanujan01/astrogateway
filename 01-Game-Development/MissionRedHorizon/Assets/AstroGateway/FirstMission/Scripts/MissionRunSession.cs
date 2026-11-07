using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using AstroGateway.FirstMission.Missions;
using AstroGateway.NativeSlice;

namespace AstroGateway.FirstMission
{
    // One atomic, device-local checkpoint for the actual playable mission. No auth
    // tokens, passwords, server scores or scientific model results are stored here.
    public sealed class MissionRunSession : MonoBehaviour
    {
        public const string CheckpointName = "AstroGateway_MissionRun_v1.json";
        const int MaxBytes = 2 * 1024 * 1024;
        [Serializable] public sealed class Envelope { public string payload, sha256; }
        [Serializable] public sealed class Checkpoint
        {
            public int version = 1;
            public string owner, runId, sensorSession;
            public long savedAt;
            public SliceSnapshot world;
            public MissionProgressState chain;
            public FirstMissionProgress progress;
            public List<SensorRecord> records;
            public Vector3 roverAngles;
            public float yaw, pitch;
            public int weather;
        }
        public string Status { get; private set; } = "Local autosave ready";
        public bool Restarting { get; private set; }
        MissionDirector director;
        string runId = Guid.NewGuid().ToString();
        float nextSave;
        bool dirty, suspended;
        string CheckpointPath => NativeMissionContext.FilePath(CheckpointName);
        public static bool Automation
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.SessionState.GetBool("FirstMissionPlaytest.Run", false)
                    || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FM_OUT"));
#else
                return false;
#endif
            }
        }
        static string Digest(string data)
        {
            using (var hash = SHA256.Create())
                return BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLowerInvariant();
        }
        static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
        static void AtomicWrite(string path, string data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string temp = path + ".tmp";
            File.WriteAllText(temp, data);
            if (File.Exists(path)) File.Replace(temp, path, null); else File.Move(temp, path);
        }
        public static bool Valid(Checkpoint c, FirstMissionController mission)
        {
            if (c == null || c.version != 1 || c.owner != NativeMissionContext.Owner
                || !Guid.TryParse(c.runId, out _) || !Guid.TryParse(c.sensorSession, out _)
                || c.chain == null || c.progress == null || c.records == null || c.records.Count > 400
                || !mission.hud.game.ValidSnapshot(c.world)) return false;
            if (!Finite(c.progress.distance) || c.progress.distance < 0 || c.progress.distance > 10000000
                || !Finite(c.yaw) || Mathf.Abs(c.yaw) > 1000000000 || !Finite(c.pitch) || c.pitch < -8 || c.pitch > 55
                || !Finite(c.roverAngles.x) || !Finite(c.roverAngles.y) || !Finite(c.roverAngles.z)
                || c.weather < 0 || c.weather > 2 || c.chain.mission < 0 || c.chain.mission > 6) return false;
            var ids = new HashSet<string>();
            return c.records.All(r => r != null && ids.Add(r.eventId) && SensorValidation.Validate(r, mission.Catalog) == null);
        }
        static Checkpoint Read(string path)
        {
            if (!File.Exists(path) || new FileInfo(path).Length > MaxBytes) return null;
            var e = JsonUtility.FromJson<Envelope>(File.ReadAllText(path));
            if (e == null || e.payload == null || e.sha256 != Digest(e.payload)) return null;
            return JsonUtility.FromJson<Checkpoint>(e.payload);
        }
        // Called before sensor/progress files are loaded and before MissionDirector.Start.
        public static void RestoreAtStartup(FirstMissionController mission)
        {
            if (Automation || !SceneManager.GetActiveScene().path.EndsWith("Mars_FirstMission_Missions.unity")) return;
            string path = NativeMissionContext.FilePath(CheckpointName);
            if (!File.Exists(path)) return; // Preserve existing legacy saves on first upgrade.
            try
            {
                var c = Read(path);
                if (!Valid(c, mission)) throw new InvalidDataException();
                var p = mission.hud.game.prototype;
                // World restoration uses the existing validated boarding/control path.
                if (!mission.hud.game.RestoreSnapshot(c.world)) throw new InvalidDataException();
                p.rover.transform.rotation = Quaternion.Euler(c.roverAngles);
                p.yaw = c.yaw; p.pitch = c.pitch; p.SetWeather(c.weather);
                mission.sensors.LocalSessionId = c.sensorSession;
                string folder = NativeMissionContext.FilePath("AstroGateway_FirstMission");
                AtomicWrite(Path.Combine(folder, "sensor_records_v1.json"), JsonUtility.ToJson(new StoreFile { records = c.records }));
                PlayerPrefs.SetString(NativeMissionContext.PreferenceKey(FirstMissionController.ProgressKey), JsonUtility.ToJson(c.progress));
                PlayerPrefs.SetString(NativeMissionContext.PreferenceKey(MissionDirector.SaveKey), JsonUtility.ToJson(c.chain));
                PlayerPrefs.Save();
                mission.hud.game.notice = "Local mission resumed";
            }
            catch
            {
                Debug.LogWarning("Mission checkpoint unavailable; using existing local progress. Previous checkpoint preserved.");
            }
        }
        public void Initialize(MissionDirector value)
        {
            director = value; suspended = Automation;
            if (suspended) return;
            try { var previous = Read(CheckpointPath); if (previous != null && previous.owner == NativeMissionContext.Owner) runId = previous.runId; } catch { }
            director.StepStarted += OnStep;
            director.AllMissionsCompleted += MarkDirty;
            director.MissionFailed += OnFailed;
            director.Retried += MarkDirty;
            director.mission.Store.Changed += MarkDirty;
            nextSave = Time.unscaledTime + 10;
        }
        void OnStep(MissionDef m, MissionStep s) => MarkDirty();
        void OnFailed(string why) => MarkDirty();
        void MarkDirty() { dirty = true; }
        void LateUpdate()
        {
            if (suspended || Restarting || !director || !director.Ready) return;
            if (dirty || Time.unscaledTime >= nextSave) SaveNow();
        }
        public bool SaveNow()
        {
            if (suspended || Restarting || !director || !director.Ready) return false;
            try
            {
                var m = director.mission; var p = m.hud.game.prototype;
                var c = new Checkpoint { owner = NativeMissionContext.Owner, runId = runId,
                    sensorSession = m.sensors.LocalSessionId, savedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    world = m.hud.game.CaptureSnapshot(), chain = director.CaptureLocalProgress(),
                    progress = m.CaptureLocalProgress(), records = m.Store.Records.ToList(),
                    roverAngles = p.rover.transform.eulerAngles, yaw = p.yaw, pitch = p.pitch, weather = p.weather };
                if (!Valid(c, m)) throw new InvalidDataException();
                string payload = JsonUtility.ToJson(c);
                string json = JsonUtility.ToJson(new Envelope { payload = payload, sha256 = Digest(payload) });
                if (Encoding.UTF8.GetByteCount(json) > MaxBytes) throw new InvalidDataException();
                AtomicWrite(CheckpointPath, json);
                Status = "Saved locally";
                dirty = false; nextSave = Time.unscaledTime + 10;
                return true;
            }
            catch { Status = "Save failed - previous checkpoint preserved"; nextSave = Time.unscaledTime + 10; dirty = false; return false; }
        }
        // User confirmation is supplied by the in-game confirmation card. Keep a
        // recoverable archive of the old run; only this owner's run paths are reset.
        public bool RestartRun()
        {
            if (suspended || Restarting || !SaveNow()) return false;
            try
            {
                string archive = NativeMissionContext.FilePath("run-archives");
                archive = Path.Combine(archive, DateTime.UtcNow.ToString("yyyyMMddTHHmmss") + "-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(archive);
                File.Copy(CheckpointPath, Path.Combine(archive, CheckpointName));
                var m = director.mission;
                string world = NativeMissionContext.FilePath(FirstMissionController.SaveFile);
                string sensors = m.Store.Path;
                // All copies finish before any current-run file is removed.
                if (File.Exists(world)) File.Copy(world, Path.Combine(archive, "world.json"));
                if (File.Exists(sensors)) File.Copy(sensors, Path.Combine(archive, "sensor_records_v1.json"));
                if (File.Exists(world)) File.Delete(world);
                if (File.Exists(sensors)) File.Delete(sensors);
                PlayerPrefs.DeleteKey(NativeMissionContext.PreferenceKey(FirstMissionController.ProgressKey));
                PlayerPrefs.DeleteKey(NativeMissionContext.PreferenceKey(MissionDirector.SaveKey));
                PlayerPrefs.Save();
                File.Delete(CheckpointPath);
                Restarting = true;
                if (m.hud.modalOpen) m.hud.CloseModal();
                Time.timeScale = 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().path);
                return true;
            }
            catch { Status = "New run unavailable - your archived run is preserved"; return false; }
        }
        void OnApplicationPause(bool paused) { if (paused) SaveNow(); }
        void OnApplicationQuit() { SaveNow(); }
        void OnDestroy()
        {
            if (!director || suspended) return;
            director.StepStarted -= OnStep; director.AllMissionsCompleted -= MarkDirty;
            director.MissionFailed -= OnFailed; director.Retried -= MarkDirty;
            if (director.mission != null && director.mission.Store != null) director.mission.Store.Changed -= MarkDirty;
        }
    }
}
