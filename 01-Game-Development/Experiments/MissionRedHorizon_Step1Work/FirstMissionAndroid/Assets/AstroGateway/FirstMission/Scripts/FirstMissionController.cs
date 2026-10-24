using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using AstroGateway.CinematicHUD;
using AstroGateway.MarsEnvironment;
using AstroGateway.ProductionVisual;

// First Mission glue (Mars_FirstMission_v1 only). Owns the sensor store, the sync queue and the analysis seam,
// runs rover observations and evaluates the first-mission objectives. Shared gameplay scripts are not modified:
// the rover is driven through the same AstroGatewayHUDController input fields the existing touch controls use.
// Mission progress here is local guidance only; no score is computed (scoring rubric still pending approval).
namespace AstroGateway.FirstMission
{
    [Serializable]
    public class FirstMissionSettings
    {
        public int version = 2;
        public bool leftHanded, invertY, reducedMotion, autoSprint, runToggle;
        public float controlScale = 1, opacity = .85f, lookSensitivity = 1, shoulderSide = 1;
        public int joystickPosition = 1;   // 0 low, 1 standard, 2 raised
    }

    [Serializable]
    class FirstMissionProgress { public bool boarded, reachedSite; public float distance; }

    public class Objective { public string text; public bool done, info; }

    [DefaultExecutionOrder(700)]
    public class FirstMissionController : MonoBehaviour
    {
        public AstroGatewayHUDController hud;
        public ProductionModeCoordinator modes;
        public ProductionCameraRig legacyRig;
        public RoverSensorSuite sensors;
        public AresRoverPresentation ares;
        public TextAsset catalogJson;
        public string designatedStationId = "";
        public string storeFolderOverride = "";   // tests only
        public const string SaveFile = "AstroGateway_FirstMission_v1.json";
        public const string SettingsKey = "AstroGateway.FirstMission.v1.Settings", ProgressKey = "AstroGateway.FirstMission.v1.Progress";
        public const float SiteRadius = 15f;

        public SensorCatalog Catalog { get; private set; }
        public SensorRecordStore Store { get; private set; }
        public SensorSyncService Sync { get; private set; }
        public IScientificAnalysisService Analysis { get; private set; } = new NotConnectedAnalysisService();
        public FirstMissionSettings Settings { get; private set; } = new FirstMissionSettings();
        public ResearchTarget Designated { get; private set; }
        public bool Ready { get; private set; }

        // observation state
        public InstrumentDef Observing { get; private set; }
        public float ObservationProgress => Observing == null ? 0 : Mathf.Clamp01((Time.time - observeStart) / observeDuration);
        public string LastFeedback { get; private set; } = "Select an instrument";
        public List<SensorRecord> LastObservation { get; private set; } = new List<SensorRecord>();
        float observeStart, observeDuration;
        FirstMissionProgress progress = new FirstMissionProgress();

        void Start()
        {
            // After the HUD (300) and Visual Upgrade fixes (600) chose their files: this scene keeps its own save.
            hud.game.testSave = Path.Combine(Application.persistentDataPath, SaveFile);
            // Approved orientation: landscape only (both directions, so the camera cutout can sit on either side).
            Screen.autorotateToPortrait = false; Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true; Screen.autorotateToLandscapeRight = true;
            if (Application.isMobilePlatform) Screen.orientation = ScreenOrientation.AutoRotation;

            Catalog = new SensorCatalog(catalogJson.text);
            sensors.Catalog = Catalog;
            string folder = string.IsNullOrEmpty(storeFolderOverride) ? Path.Combine(Application.persistentDataPath, "AstroGateway_FirstMission") : storeFolderOverride;
            Store = new SensorRecordStore(Path.Combine(folder, "sensor_records_v1.json"));
            Sync = new SensorSyncService(Store, new NotConnectedUploader(), Catalog.MaxBatch);
            LoadSettings(); LoadProgress();
            sensors.RestoreDistance(progress.distance);
            Designated = sensors.stations.FirstOrDefault(s => s && s.stableId == designatedStationId)
                         ?? sensors.stations.Where(s => s).OrderBy(s => RoverSensorSuite.Planar(s.transform.position, hud.game.prototype.rover.transform.position)).FirstOrDefault();
            Ready = true;
        }

        float nextProgressSave;
        void Update()
        {
            if (!Ready) return;
            Sync.Tick(Time.unscaledTime);
            var p = hud.game.prototype;
            if (p.driving && !progress.boarded) { progress.boarded = true; SaveProgress(); }
            if (p.driving && Designated && !progress.reachedSite && RoverSensorSuite.Planar(p.rover.transform.position, Designated.transform.position) <= SiteRadius) { progress.reachedSite = true; SaveProgress(); }
            if (Time.unscaledTime > nextProgressSave && Mathf.Abs(sensors.DistanceTravelled - progress.distance) > 1) { nextProgressSave = Time.unscaledTime + 10; SaveProgress(); }

            if (Observing != null)
            {
                string reason = sensors.CanObserve(Observing, out _, out _);
                if (!hud.environment.paused)
                {
                    if (reason != null) Cancel("Observation cancelled • " + reason);
                    else if (ObservationProgress >= 1) Finish();
                }
            }
        }

        // ---------- observations ----------
        public string Check(InstrumentDef inst) => Observing != null ? "Observation in progress" : sensors.CanObserve(inst, out _, out _);

        public bool BeginObservation(InstrumentDef inst)
        {
            string reason = Check(inst);
            if (reason != null) { LastFeedback = reason; return false; }
            Observing = inst;
            observeStart = Time.time;
            observeDuration = inst.id == "rover.arm_sampler" && ares && ares.armDeploy ? ares.armDeploy.length : inst.id == "rover.context_camera" || inst.id == "rover.spectrometer" ? 3f : 1.5f;
            if (ares) { ares.armTarget = inst.id == "rover.arm_sampler" ? 1 : 0; ares.mastScanning = inst.id == "rover.context_camera"; }
            LastFeedback = "Observing with " + inst.name + "…";
            return true;
        }

        public void Cancel(string why)
        {
            Observing = null;
            if (ares) { ares.armTarget = 0; ares.mastScanning = false; }
            LastFeedback = why;
        }

        void Finish()
        {
            var inst = Observing;
            Observing = null;
            if (ares) { ares.armTarget = 0; ares.mastScanning = false; }
            var records = sensors.Observe(inst);
            var invalid = records.Select(r => SensorValidation.Validate(r, Catalog)).Where(e => e != null).ToList();
            if (invalid.Count > 0) { LastFeedback = "Observation discarded • local validation failed: " + invalid[0]; Debug.LogWarning("FirstMission: " + LastFeedback); return; }
            if (!Store.Add(records, out string why)) { LastFeedback = "Not stored • " + (string.IsNullOrEmpty(why) ? Store.LastError : why); return; }
            LastObservation = records;
            int simulated = records.Count(r => r.provenance == Provenance.Simulated), unavailable = records.Count - simulated;
            LastFeedback = $"{inst.name}: {simulated} simulated reading(s) stored locally" + (unavailable > 0 ? $", {unavailable} unavailable (not modelled)" : "");
        }

        // ---------- mission ----------
        bool Has(Func<SensorRecord, bool> f) => Store.Records.Any(f);
        bool AtSite(SensorRecord r) => Designated && RoverSensorSuite.Planar(r.location, Designated.transform.position) <= SiteRadius;

        public List<Objective> Objectives()
        {
            string site = Designated ? Designated.displayName : "the research site";
            string id = Designated ? Designated.stableId : "";
            bool context = Has(r => r.instrumentId == "rover.context_camera" && r.stationId == id && r.provenance == Provenance.Simulated);
            bool sample = Has(r => r.sensorId == "sci.sample_id" && r.stationId == id && r.provenance == Provenance.Simulated);
            bool terrain = Has(r => r.instrumentId == "rover.terrain_profiler" && r.sensorId == "terrain.slope" && r.provenance == Provenance.Simulated && AtSite(r));
            bool second = Has(r => r.sensorId == "sci.station_id" && r.provenance == Provenance.Simulated && r.stationId != id);
            return new List<Objective>
            {
                new Objective { text = "Board the ARES-VI research rover", done = progress.boarded || Store.Records.Count > 0 },
                new Objective { text = "Drive to " + site + " (within " + SiteRadius.ToString("0") + " m)", done = progress.reachedSite || context || sample },
                new Objective { text = "SCIENCE: profile the terrain at the site", done = terrain },
                new Objective { text = "SCIENCE: survey " + site + " with the mast context camera", done = context },
                new Objective { text = "SCIENCE: stop within 6 m and collect an arm sample", done = sample },
                new Objective { text = "DATA: observations stored on this device", done = context && sample && string.IsNullOrEmpty(Store.LastError) },
                new Objective { text = "Backend upload: " + (Sync.Uploader.Online ? "connected" : "NOT CONNECTED (no hosted backend yet)"), info = true },
                new Objective { text = "AI analysis: NOT CONNECTED (team model not yet available)", info = true },
                new Objective { text = "NEXT: survey a second research station", done = second },
            };
        }

        public string Instructions()
        {
            var o = Objectives().FirstOrDefault(x => !x.done && !x.info);
            return o == null ? "First mission objectives complete (local, unscored)." : o.text;
        }

        // ---------- persistence ----------
        public void SaveSettings() { PlayerPrefs.SetString(SettingsKey, JsonUtility.ToJson(Settings)); PlayerPrefs.Save(); }
        public void ResetSettingsForTests() { JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(new FirstMissionSettings()), Settings); } // keep the same object (UI closures hold it)
        void LoadSettings()
        {
            try { Settings = JsonUtility.FromJson<FirstMissionSettings>(PlayerPrefs.GetString(SettingsKey, "{}")) ?? new FirstMissionSettings(); }
            catch { Settings = new FirstMissionSettings(); }
            Settings.controlScale = Mathf.Clamp(Settings.controlScale == 0 ? 1 : Settings.controlScale, .85f, 1.25f);
            if (Settings.version < 2) { Settings.version = 2; Settings.opacity = .85f; Settings.lookSensitivity = 1; Settings.shoulderSide = 1; Settings.joystickPosition = 1; }
            Settings.opacity = Mathf.Clamp(Settings.opacity, .4f, 1f);
            Settings.lookSensitivity = Mathf.Clamp(Settings.lookSensitivity, .3f, 2.5f);
            Settings.shoulderSide = Settings.shoulderSide < 0 ? -1 : 1;
            Settings.joystickPosition = Mathf.Clamp(Settings.joystickPosition, 0, 2);
        }
        void SaveProgress() { progress.distance = sensors.DistanceTravelled; PlayerPrefs.SetString(ProgressKey, JsonUtility.ToJson(progress)); PlayerPrefs.Save(); }
        void LoadProgress()
        {
            try { progress = JsonUtility.FromJson<FirstMissionProgress>(PlayerPrefs.GetString(ProgressKey, "{}")) ?? new FirstMissionProgress(); }
            catch { progress = new FirstMissionProgress(); }
        }
        // Test hook: isolated store folder and fresh progress (the player's own data is never touched by the harness).
        public void UseStoreForTests(string folder, ISensorRecordUploader uploader = null, int maxRecords = 400)
        {
            Store = new SensorRecordStore(System.IO.Path.Combine(folder, "sensor_records_v1.json"), maxRecords);
            Sync = new SensorSyncService(Store, uploader ?? new NotConnectedUploader(), Catalog.MaxBatch);
            LastObservation = new List<SensorRecord>();
        }
        public void ResetProgressForTests() { progress = new FirstMissionProgress(); PlayerPrefs.DeleteKey(ProgressKey); }
        void OnApplicationPause(bool paused) { if (paused && Ready) { SaveProgress(); Store.Touch(); } }
    }
}
