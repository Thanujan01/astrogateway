using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using AstroGateway.FirstMission.Missions;
using AstroGateway.FirstMission.HUD;

namespace AstroGateway.FirstMission.EditorTools
{
    // Actual scene reload/save/replay smoke test under a fresh synthetic owner.
    // Never changes the player's account namespace or global control settings.
    [InitializeOnLoad]
    public static class PrototypeRunValidation
    {
        const string Flag = "PrototypeRunValidation.Active", OwnerKey = "PrototypeRunValidation.Owner";
        static IEnumerator routine;
        static double start, waitUntil;
        static readonly List<string> results = new List<string>();
        static int pass;
        static string Out => Environment.GetEnvironmentVariable("AG_PROTOTYPE_OUT");
        static PrototypeRunValidation() { EditorApplication.update += Tick; }
        public static void Run()
        {
            if (string.IsNullOrEmpty(Out) || !Path.IsPathRooted(Out)) throw new Exception("Set AG_PROTOTYPE_OUT to an external evidence directory");
            Directory.CreateDirectory(Out);
            SessionState.SetString(OwnerKey, Guid.NewGuid().ToString());
            SessionState.SetBool(Flag, true);
            EditorSceneManager.OpenScene("Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Missions.unity");
            Application.runInBackground = true;
            EditorApplication.isPlaying = true;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void BindTestOwner()
        {
            if (SessionState.GetBool(Flag, false)) NativeMissionContext.Bind(SessionState.GetString(OwnerKey, ""));
        }
        static MissionDirector Director() => UnityEngine.Object.FindAnyObjectByType<MissionDirector>();
        static void Tick()
        {
            if (!SessionState.GetBool(Flag, false) || !EditorApplication.isPlaying) return;
            try
            {
                // Keep known Editor Search errors visible in the log while allowing
                // this lifecycle-only test to continue after Editor Error Pause.
                if (EditorApplication.isPaused) EditorApplication.isPaused = false;
                if (routine == null) { start = EditorApplication.timeSinceStartup; routine = Tests(); }
                if (EditorApplication.timeSinceStartup - start > 150) throw new Exception("Prototype run validation timed out");
                if (EditorApplication.timeSinceStartup < waitUntil) return;
                if (!routine.MoveNext()) { Finish(true); return; }
                if (routine.Current is float seconds) waitUntil = EditorApplication.timeSinceStartup + seconds;
            }
            catch (Exception e) { results.Add("FAIL " + e); Finish(false); }
        }
        static void Check(bool ok, string name)
        {
            if (!ok) throw new Exception(name);
            pass++; results.Add("PASS " + name); Debug.Log("[PrototypeRun] PASS " + name);
        }
        static void Place(CharacterController actor, Vector3 position)
        {
            actor.enabled = false; actor.transform.position = position; actor.enabled = true; Physics.SyncTransforms();
        }
        static IEnumerator Tests()
        {
            MissionDirector d;
            while (!(d = Director()) || !d.Ready || !d.ui || !d.ui.Safe) yield return null;
            var m = d.mission; var p = m.hud.game.prototype;
            Check(NativeMissionContext.Owner == SessionState.GetString(OwnerKey, ""), "Synthetic owner bound before save load");
            Check(d.Run && !d.Run.Restarting, "Run lifecycle installed in actual Missions scene");
            string settings = PlayerPrefs.GetString(FirstMissionController.SettingsKey, "");
            d.ResetForTests(2);
            d.CaptureLocalProgress().repairProgress = .45f; d.generator.RepairProgress = .45f;
            m.hud.game.battery = 63; p.oxygen = 81; p.power = 76; p.yaw = 47; p.pitch = 22;
            var position = p.astronaut.transform.position + Vector3.right * 2;
            Place(p.astronaut, position);
            var readings = m.sensors.Observe(m.Catalog.Instrument("rover.terrain_profiler"));
            Check(readings.Count > 0 && m.Store.Add(readings, out _), "Actual simulated sensor records collected for checkpoint");
            int records = m.Store.Records.Count;
            Check(d.Run.SaveNow(), "Atomic complete-run checkpoint saved");
            var old = d;
            SceneManager.LoadScene(SceneManager.GetActiveScene().path);
            yield return .4f;
            while (!(d = Director()) || !d.Ready || ReferenceEquals(d, old) || !d.ui || !d.ui.Safe) yield return null;
            m = d.mission; p = m.hud.game.prototype;
            Check(d.MissionIndex == 2 && Mathf.Abs(d.generator.RepairProgress - .45f) < .001f, "Scene reload restores mission and partial repair together");
            Check(RoverSensorSuite.Planar(position, p.astronaut.transform.position) < .1f, "Scene reload restores player position");
            Check(Mathf.Abs(m.hud.game.battery - 63) < .1f && Mathf.Abs(p.oxygen - 81) < .1f, "Scene reload restores simulated resources");
            Check(m.Store.Records.Count == records, "Scene reload restores observations without duplication");
            Check(Mathf.Abs(p.yaw - 47) < .1f, "Scene reload restores camera heading");
            d.survivalPaused = false; d.SetSurvivalForTests(o2: .001f);
            while (!d.Failed) yield return null;
            Check(d.Run.SaveNow(), "Failed state checkpoint saved");
            old = d; SceneManager.LoadScene(SceneManager.GetActiveScene().path); yield return .4f;
            while (!(d = Director()) || !d.Ready || ReferenceEquals(d, old) || !d.ui || !d.ui.Safe) yield return null;
            Check(d.Failed, "Failure survives scene reload until explicit retry");
            d.Retry(); Check(!d.Failed && d.RetryCount == 1, "Explicit retry resumes with recorded retry count");
            var ui = d.ui as MissionsHUD;
            d.mission.hud.TogglePause(); yield return .2f;
            ui.NewRunButton.onClick.Invoke(); yield return .1f;
            Check(ui.NewRunCard.activeInHierarchy, "New run requires visible in-game confirmation");
            ui.CancelNewRunButton.onClick.Invoke(); yield return .1f;
            Check(!ui.NewRunCard.activeSelf && d.MissionIndex == 2, "Cancel keeps current run");
            ui.NewRunButton.onClick.Invoke(); yield return .1f;
            old = d; ui.ConfirmNewRunButton.onClick.Invoke(); yield return .4f;
            while (!(d = Director()) || !d.Ready || ReferenceEquals(d, old) || !d.ui || !d.ui.Safe) yield return null;
            Check(d.MissionIndex == 0 && d.StepIndex == 0 && !d.Repaired && !d.Failed, "Confirmed replay starts clean first mission");
            Check(d.mission.Store.Records.Count == 0, "Replay does not reuse old science completion evidence");
            Check(PlayerPrefs.GetString(FirstMissionController.SettingsKey, "") == settings, "Replay preserves device settings");
            string archives = NativeMissionContext.FilePath("run-archives");
            Check(Directory.Exists(archives) && Directory.GetFiles(archives, MissionRunSession.CheckpointName, SearchOption.AllDirectories).Length == 1, "Previous run archived before replay");
        }
        static void Finish(bool ok)
        {
            SessionState.SetBool(Flag, false);
            File.WriteAllLines(Path.Combine(Out, "prototype-run-results.txt"), results);
            Debug.Log("PROTOTYPE_RUN_VALIDATION " + (ok ? "PASS " : "FAIL ") + pass);
            EditorApplication.isPlaying = false;
            EditorApplication.delayCall += () => EditorApplication.Exit(ok ? 0 : 1);
        }
    }
}
