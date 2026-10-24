using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AstroGateway.FirstMission.Nav;

// Survival mission chain for Mars_FirstMission_Missions ("storm day"): six missions built on the real scene (the placed mission
// structures, the rover, the research stations and the existing science objectives). Each step has a guide text
// for the in-game popup, a navigation target (guidance only, the player is never moved) and a completion rule.
// Charging and repair are game rules (SIMULATED); nothing here is telemetry. Progress is saved locally.
namespace AstroGateway.FirstMission.Missions
{
    public class MissionStep
    {
        public string objective, guide, hint, targetId;
        public Func<bool> done;
        public Func<float> progress;          // optional 0..1 bar (charge, repair, checkpoints)
        public string progressLabel = "";
        public bool repair;                    // shows the HOLD TO REPAIR control when in range
    }

    public class MissionDef
    {
        public string id, title, brief, reward;
        public List<MissionStep> steps = new List<MissionStep>();
    }

    [Serializable]
    class MissionSave { public int mission, step; public float charge; public bool repaired; public int checkpoints; public bool finished; public float o2 = 40; public bool stormStarted, batteryDrained; public float stormEta = 200; }

    [DefaultExecutionOrder(710)]
    public class MissionDirector : MonoBehaviour
    {
        public FirstMissionController mission;
        public HUD.ExplorerHUD ui;
        public GeneratorState generator;
        public ChargingPadState charging;
        public List<CheckpointState> checkpoints = new List<CheckpointState>();
        public Transform dockBay, tentsAnchor;
        public Material particleTemplate;
        public const string SaveKey = "AstroGateway.FirstMission.v1.Missions";
        public const float PadRadius = 2.8f, PanelRadius = 2.6f, GateRadius = 5.5f, DockRadius = 4f;
        public const float ChargeSeconds = 6f, RepairSeconds = 3.5f;

        public readonly List<MissionDef> Missions = new List<MissionDef>();
        public int MissionIndex => save.mission;
        public int StepIndex => save.step;
        public bool AllComplete => save.finished;
        public MissionDef Current => save.finished || Missions.Count == 0 ? null : Missions[Mathf.Clamp(save.mission, 0, Missions.Count - 1)];
        public MissionStep Step => Current == null ? null : Current.steps[Mathf.Clamp(save.step, 0, Current.steps.Count - 1)];
        public float Charge => save.charge;
        public float Battery => mission.hud.game.battery;
        public bool Repaired => save.repaired;
        public int CheckpointsPassed => save.checkpoints;
        public bool RepairHeld { get; set; }               // set by the UI hold control (or R key)
        public bool InRepairRange { get; private set; }
        public bool OnPad { get; private set; }
        public bool InDock { get; private set; }
        public bool Ready { get; private set; }
        public event Action<MissionDef, MissionStep> StepStarted, StepCompleted;
        public event Action<MissionDef> MissionCompleted;
        public event Action AllMissionsCompleted;

        MissionSave save = new MissionSave();
        float stepStartedAt, stillTime; bool started;
        Stage08B.MarsPrototype P => mission.hud.game.prototype;
        bool Reduced() => mission.Settings.reducedMotion;

        void Start()
        {
            // Re-resolve scene references if any went stale (seen once when entering Play Mode without a scene reload).
            if (checkpoints.Count == 0 || checkpoints.Any(c => !c)) checkpoints = FindObjectsByType<CheckpointState>(FindObjectsSortMode.None).OrderBy(c => c.order).ToList();
            if (!generator) generator = FindAnyObjectByType<GeneratorState>();
            if (!charging) charging = FindAnyObjectByType<ChargingPadState>();
            Load();
            if (generator) { generator.reduced = Reduced; generator.Init(particleTemplate); generator.SetRepaired(save.repaired); }
            if (charging) { charging.reduced = Reduced; charging.Init(particleTemplate); }
            foreach (var c in checkpoints) { c.reduced = Reduced; c.Init(particleTemplate); }
            Define();
            SyncCheckpointVisuals();
            Ready = true;
        }

        float Dist(Vector3 a, Vector3 b) => RoverSensorSuite.Planar(a, b);
        bool PlayerNear(Transform t, float r) => t && Dist(P.ActivePosition(), t.position) <= r;
        bool RoverNear(Transform t, float r) => t && P.driving && Dist(P.rover.transform.position, t.position) <= r;
        bool Objective(int i) { var o = mission.Objectives(); return i < o.Count && o[i].done; }

        void Define()
        {
            Missions.Clear();
            string site = mission.Designated ? mission.Designated.displayName : "the research site";
            string siteId = mission.Designated ? mission.Designated.stableId : "";
            var gen = generator ? generator.GetComponent<MissionStructure>() : null;

            // STORY: last night's dust storm knocked out base power. Life support runs on a draining oxygen reserve,
            // the rover is nearly flat and a second, bigger storm is coming. Survive the day.
            var m1 = new MissionDef { id = "m1.wakeup", title = "WAKE-UP CALL", brief = "Last night's dust storm knocked out base power. Life support is running on reserve oxygen. Get moving.", reward = "Rover ready to roll" };
            m1.steps.Add(new MissionStep { objective = "Grab your EVA kit at the crew camp", targetId = "fac.tents", guide = "Alarms are sounding across the base. Your EVA kit is at the crew camp. Collect it before heading out.", hint = "Use the MOVE joystick, hold RUN to go faster, and follow the orange light pillar.", done = () => PlayerNear(tentsAnchor, 6) });
            m1.steps.Add(new MissionStep { objective = "Run to the ARES-VI rover", targetId = "rover.ares6", guide = "The oxygen reserve is dropping. The rover is your only way to the generator. Get to it fast.", hint = "Watch BASE O2 at the top left. The map shows the full route.", done = () => P.driving || Dist(P.astronaut.transform.position, P.rover.transform.position) <= 5 });
            m1.steps.Add(new MissionStep { objective = "Board the rover", targetId = "rover.ares6", guide = "Climb in and take the controls.", hint = "Tap BOARD ROVER (or press E).", done = () => P.driving });
            Missions.Add(m1);

            var m2 = new MissionDef { id = "m2.power", title = "POWER UP THE ROVER", brief = "The storm drained the rover's battery. You need enough range to reach the generator and get back to shelter.", reward = "Rover fully charged" };
            m2.steps.Add(new MissionStep { objective = "Drive onto the charging pad", targetId = "fac.charging", guide = "The charging station is just east of you. Drive onto the glowing cyan pad.", hint = "Push ACCEL to move, steer with the joystick, and BRAKE to stop on the pad.", done = () => RoverNear(charging ? charging.pad : null, PadRadius) || save.charge >= 1 });
            m2.steps.Add(new MissionStep { objective = "Hold still until the battery is full", targetId = "fac.charging", guide = "Stay on the pad and keep still. Every second counts, so the oxygen reserve keeps dropping while you charge.", hint = "The rover battery is a game value (SIMULATED). Driving drains it slowly.", progress = () => save.charge, progressLabel = "CHARGE CYCLE • SIMULATED", done = () => save.charge >= 1 && Battery >= 99.5f });
            Missions.Add(m2);

            var m3 = new MissionDef { id = "m3.lifesupport", title = "RESTORE LIFE SUPPORT", brief = "The generator powers the domes' oxygen plant. Fix it before the oxygen reserve runs out, or the crew is in danger.", reward = "Life support back online" };
            m3.steps.Add(new MissionStep { objective = "Drive to the power generator", targetId = "fac.generator", guide = "Its warning beacons are flashing near the solar farm. Get there quickly.", hint = "Follow the orange pillar. NORMAL drive mode is the fastest.", done = () => PlayerNear(gen ? gen.Anchor : null, 14) || save.repaired });
            m3.steps.Add(new MissionStep { objective = "Exit the rover", targetId = "fac.generator", guide = "The fault panel can only be reached on foot. Stop and climb out.", hint = "Stop completely, then tap EXIT ROVER (or press E).", done = () => !P.driving || save.repaired });
            m3.steps.Add(new MissionStep { objective = "Reach the sparking fault panel", targetId = "fac.generator", guide = "The breaker panel at the west end is throwing sparks. That's the fault.", hint = "Get within about 2 m of the panel.", done = () => (!P.driving && PlayerNear(generator ? generator.console : null, PanelRadius)) || save.repaired });
            m3.steps.Add(new MissionStep { objective = "Hold REPAIR to restart the generator", targetId = "fac.generator", repair = true, guide = "Hold REPAIR to reset the breaker. When the bar is full, the oxygen plant restarts.", hint = "Letting go keeps your progress. Walking away pauses it.", progress = () => save.repaired ? 1 : generator ? generator.RepairProgress : 0, progressLabel = "REPAIR", done = () => save.repaired });
            Missions.Add(m3);

            var m4 = new MissionDef { id = "m4.supplysite", title = "SCOUT THE SUPPLY SITE", brief = "Food and spare parts arrive by lander tomorrow. Survey " + site + " so it can be recorded as the landing site.", reward = "Landing site recorded" };
            m4.steps.Add(new MissionStep { objective = "Board the rover", targetId = "rover.ares6", guide = "Life support is stable for now. Head back to the rover.", hint = "Use NAVIGATE TO ROVER if you lose it.", done = () => P.driving || Objective(3) && Objective(4) });
            m4.steps.Add(new MissionStep { objective = "Drive to " + site, targetId = siteId, guide = "Drive to " + site + " and stop within " + FirstMissionController.SiteRadius.ToString("0") + " m.", hint = "The station marker on the map turns from UNVISITED to DATA COLLECTED when you're done.", done = () => Objective(1) });
            m4.steps.Add(new MissionStep { objective = "Scan the landing zone with the mast camera", targetId = siteId, guide = "Tap SCIENCE, choose the MAST CONTEXT CAMERA and run it to record the landing zone.", hint = "Keep the rover still until the scan completes.", done = () => Objective(3) });
            m4.steps.Add(new MissionStep { objective = "Take a soil sample (within 6 m)", targetId = siteId, guide = "Drive within 6 m of the station and run the ARM SAMPLER. It records a ground sample for the site file.", hint = "Readings are SIMULATED and saved LOCAL ONLY. The AI analysis is NOT CONNECTED.", done = () => Objective(4) });
            Missions.Add(m4);

            var m5 = new MissionDef { id = "m5.beacons", title = "LIGHT THE STORM BEACONS", brief = "A second dust storm is coming. Drive through the three beacon gates to switch them on, so the crew can find the base when visibility drops.", reward = "Storm beacons lit" };
            for (int i = 0; i < checkpoints.Count; i++)
            {
                int k = i;
                m5.steps.Add(new MissionStep { objective = $"Switch on storm beacon {k + 1} of {checkpoints.Count}", targetId = "fac.checkpoint" + (k + 1), guide = k == 0 ? "Drive between the posts of the glowing gate to switch its beacon on. Watch the STORM timer." : $"Beacon {k} is online. Next: beacon {k + 1}.", hint = "Drive between the two posts.", progress = () => save.checkpoints / (float)Mathf.Max(1, checkpoints.Count), progressLabel = "BEACONS ONLINE", done = () => save.checkpoints > k });
            }
            Missions.Add(m5);

            var m6 = new MissionDef { id = "m6.shelter", title = "TAKE SHELTER", brief = "The storm is almost here. Get the rover into the dock before it hits.", reward = "You survived the storm" };
            m6.steps.Add(new MissionStep { objective = "Drive into the rover dock", targetId = "fac.dock", guide = "The covered dock is south-west of the charging station. Drive under the roof.", hint = "Go fast, then slow down at the entrance. PRECISION mode helps.", done = () => RoverNear(dockBay, DockRadius) });
            m6.steps.Add(new MissionStep { objective = "Stop inside the dock", targetId = "fac.dock", guide = "Bring the rover to a full stop inside the bay.", hint = "Hold BRAKE until the speed reads 0.0.", done = () => InDock && stillTime > 1.2f });
            Missions.Add(m6);
            if (save.mission >= Missions.Count) save.finished = true;
        }

        // ---------- survival rules (game rules, SIMULATED) ----------
        public const float O2Start = 40f, O2Drain = .12f, O2Refill = 2f, O2Retry = 30f, StormSeconds = 200f, StormRetry = 150f, StormDustAt = 100f, BatteryAfterStorm = 35f;
        public float BaseO2 => save.o2;
        public bool StormActive => save.stormStarted && !save.finished;
        public float StormEta => save.stormEta;
        public bool Failed { get; private set; }
        public string FailReason { get; private set; } = "";
        public event Action<string> MissionFailed;
        public event Action StormIncoming, Retried;

        public bool survivalPaused;   // test harness only (generic UI tests run before the mission chain)
        public void SetSurvivalForTests(float o2 = -1, float eta = -1) { if (o2 >= 0) save.o2 = o2; if (eta >= 0) save.stormEta = eta; }

        void Survival(float dt, bool paused)
        {
            if (paused || survivalPaused || save.finished || Failed) return;
            var p = P;
            // oxygen reserve: drains until the generator is repaired, then refills
            if (!save.repaired) save.o2 = Mathf.Max(0, save.o2 - O2Drain * dt);
            else save.o2 = Mathf.Min(100, save.o2 + O2Refill * dt);
            if (save.o2 <= 0 && !save.repaired) { Fail("LIFE SUPPORT FAILED", "The oxygen reserve ran out before the generator was restarted."); return; }
            // second storm: starts after the supply site is recorded, ends the day
            if (!save.stormStarted && Current != null && Current.id == "m5.beacons") { save.stormStarted = true; save.stormEta = StormSeconds; Save(); StormIncoming?.Invoke(); }
            if (save.stormStarted)
            {
                save.stormEta = Mathf.Max(0, save.stormEta - dt);
                if (save.stormEta <= StormDustAt && p.weather < 1) p.SetWeather(1);
                if (save.stormEta <= 0) { p.SetWeather(2); Fail("CAUGHT IN THE STORM", "The dust storm hit before the rover reached shelter."); }
            }
        }

        void Fail(string title, string why)
        {
            Failed = true; FailReason = title + "\n" + why;
            Save(); MissionFailed?.Invoke(FailReason);
        }

        // Retry the current mission: survival values are topped up a little, progress inside the mission is kept.
        public void Retry()
        {
            if (!Failed) return;
            Failed = false;
            if (save.o2 <= 0) save.o2 = O2Retry;
            if (save.stormStarted && save.stormEta <= 0) { save.stormEta = StormRetry; P.SetWeather(1); }
            Save(); Retried?.Invoke(); EnterStep();
        }

        void Update()
        {
            if (!Ready || !mission.Ready) return;
            var p = P; float dt = Time.deltaTime; bool paused = mission.hud.environment.paused;
            if (!started && ui && ui.Nav != null) { started = true; EnterStep(); }
            Survival(dt, paused);

            // rules that run regardless of the current step (structures keep their state)
            OnPad = charging && charging.pad && p.driving && Dist(p.rover.transform.position, charging.pad.position) <= PadRadius;
            InDock = dockBay && p.driving && Dist(p.rover.transform.position, dockBay.position) <= DockRadius;
            bool still = Mathf.Abs(p.speed) < .3f;
            stillTime = still && (InDock || OnPad) ? stillTime + dt : 0;
            var step = Step;
            bool chargeStep = step != null && step.progressLabel.StartsWith("CHARGE CYCLE");
            var game = mission.hud.game;
            if (charging)
            {
                // The pad charges the existing game battery (4 %/s) and runs a short charge cycle for the mission.
                charging.Charging = OnPad && still && !paused && (game.battery < 100 || (chargeStep && save.charge < 1));
                if (charging.Charging) { game.battery = Mathf.Min(100, game.battery + 4 * dt); if (chargeStep) save.charge = Mathf.Min(1, save.charge + dt / ChargeSeconds); }
                charging.Charge = game.battery / 100f;
            }

            InRepairRange = generator && !p.driving && !save.repaired && Dist(p.astronaut.transform.position, generator.console.position) <= PanelRadius;
            bool keyR = UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.rKey.isPressed && !mission.hud.game.testing;
            if (generator && step != null && step.repair && InRepairRange && (RepairHeld || keyR) && !paused)
            {
                generator.RepairProgress = Mathf.Min(1, generator.RepairProgress + dt / RepairSeconds);
                if (generator.RepairProgress >= 1 && !save.repaired) { save.repaired = true; generator.SetRepaired(true); Save(); }
            }

            // checkpoints pass only in order, only while driving the active mission step
            if (step != null && Current.id == "m5.beacons" && save.checkpoints < checkpoints.Count && p.driving)
            {
                var cp = checkpoints[save.checkpoints];
                if (Dist(p.rover.transform.position, cp.transform.position) <= GateRadius) { save.checkpoints++; SyncCheckpointVisuals(); Save(); }
            }

            if (step != null && !Failed && Time.unscaledTime - stepStartedAt > .5f && step.done()) CompleteStep();
        }

        void CompleteStep()
        {
            var m = Current; var s = Step;
            StepCompleted?.Invoke(m, s);
            save.step++;
            if (save.step >= m.steps.Count)
            {
                save.step = 0; save.mission++;
                MissionCompleted?.Invoke(m);
                if (save.mission >= Missions.Count) { save.finished = true; Save(); if (ui && ui.Nav != null) ui.Nav.Cancel(); P.SetWeather(2); AllMissionsCompleted?.Invoke(); return; }
            }
            Save();
            EnterStep();
        }

        void EnterStep()
        {
            stepStartedAt = Time.unscaledTime;
            SyncCheckpointVisuals();
            var s = Step; if (s == null || ui == null || ui.Registry == null) return;
            // the storm drained the rover overnight (once, when the power mission starts)
            if (Current.id == "m2.power" && save.step == 0 && !save.batteryDrained) { save.batteryDrained = true; mission.hud.game.battery = Mathf.Min(mission.hud.game.battery, BatteryAfterStorm); Save(); }
            var loc = ui.Registry.Locations.FirstOrDefault(l => l.id == s.targetId);
            if (loc != null && ui.Nav.Target != loc) ui.Nav.SetTarget(loc);
            StepStarted?.Invoke(Current, s);
        }

        void SyncCheckpointVisuals()
        {
            bool active = !save.finished && Current != null && Current.id == "m5.beacons";
            for (int i = 0; i < checkpoints.Count; i++)
                checkpoints[i].Set(i < save.checkpoints ? CheckpointState.Mode.Passed : active && i == save.checkpoints ? CheckpointState.Mode.Active : CheckpointState.Mode.Idle);
        }

        // ---------- locations for the map registry ----------
        public string StateOf(MissionStructure s)
        {
            switch (s.kind)
            {
                case StructureKind.Generator: return save.repaired ? "ONLINE • life support running • SIMULATED" : "FAULT • life support on reserve • SIMULATED";
                case StructureKind.ChargingStation: return $"Rover battery {Battery:0}% • SIMULATED" + (OnPad ? " • CHARGING" : "");
                case StructureKind.Checkpoint: var c = s.GetComponent<CheckpointState>(); return c ? (c.State == CheckpointState.Mode.Passed ? "BEACON ONLINE" : c.State == CheckpointState.Mode.Active ? "ACTIVE • drive through" : "BEACON OFF") : "";
                case StructureKind.Dock: return InDock ? "ROVER DOCKED" : "EMPTY";
                default: return "Crew camp";
            }
        }

        // ---------- persistence ----------
        void Load()
        {
            try { save = JsonUtility.FromJson<MissionSave>(PlayerPrefs.GetString(SaveKey, "{}")) ?? new MissionSave(); }
            catch { save = new MissionSave(); }
            save.charge = Mathf.Clamp01(save.charge); save.o2 = Mathf.Clamp(save.o2, 0, 100);
            if (save.o2 <= 0 && !save.repaired) save.o2 = O2Retry;          // never resume into an instant failure
            if (save.stormStarted && save.stormEta <= 0 && !save.finished) save.stormEta = StormRetry;
        }
        public void Save() { PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(save)); PlayerPrefs.Save(); }
        public void ResetForTests(int missionIndex = 0)
        {
            save = new MissionSave { mission = missionIndex };
            Failed = false; FailReason = "";
            if (missionIndex > 1) { save.charge = 1; save.batteryDrained = true; }
            if (missionIndex > 2) { save.repaired = true; save.o2 = 100; }
            if (generator) { generator.RepairProgress = save.repaired ? 1 : 0; generator.SetRepaired(save.repaired); }
            PlayerPrefs.DeleteKey(SaveKey);
            if (Missions.Count > 0 && save.mission >= Missions.Count) save.finished = true;
            EnterStep();
        }
        void OnApplicationPause(bool paused) { if (paused && Ready) Save(); }
    }
}
