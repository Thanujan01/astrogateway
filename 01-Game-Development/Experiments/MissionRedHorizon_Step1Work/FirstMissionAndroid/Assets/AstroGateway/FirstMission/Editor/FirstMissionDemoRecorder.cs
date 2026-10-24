using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using AstroGateway.FirstMission.HUD;
using AstroGateway.FirstMission.Missions;

// Gameplay video recorder for the Missions scene (graphical Editor Play Mode). An autopilot plays the whole mission
// chain through the normal HUD controls (joystick, pedals, buttons, hold-to-repair); it walks and drives along the
// real A* routes, it never teleports. Frames are captured at a fixed 30 fps game clock (Time.captureFramerate) as
// JPEGs into env FM_FRAMES; encode them with Step1Work/FirstMission/encode_video.swift. Player data is isolated and
// restored; the scene is never saved. Env FM_QUIT=1 exits the Editor when done.
namespace AstroGateway.FirstMission.EditorTools
{
    [InitializeOnLoad]
    public static class FirstMissionDemoRecorder
    {
        const string Flag = "FirstMissionDemoRecorder.Run";
        public static string Frames => Environment.GetEnvironmentVariable("FM_FRAMES") ?? Path.GetFullPath("Docs/FirstMission/Video/frames");
        static FirstMissionDemoRecorder() { EditorApplication.update += Tick; }

        public static void Run()
        {
            EditorSceneManager.OpenScene(FirstMissionBuilder.MissionsScene);
            try { PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.GameView); } catch { }
            try { PlayModeWindow.SetCustomRenderingResolution(1920, 1080, "AstroGateway video 1080p"); } catch { }
            ShaderUtil.allowAsyncCompilation = false;
            SessionState.SetBool(Flag, true);
            Application.runInBackground = true;
            EditorApplication.isPlaying = true;
        }

        static void Tick()
        {
            if (!EditorApplication.isPlaying || !SessionState.GetBool(Flag, false)) return;
            var mission = UnityEngine.Object.FindAnyObjectByType<FirstMissionController>();
            var ui = UnityEngine.Object.FindAnyObjectByType<MissionsHUD>();
            var dir = UnityEngine.Object.FindAnyObjectByType<MissionDirector>();
            if (!mission || !mission.Ready || !ui || !ui.Safe || !dir || !dir.Ready || !mission.hud.game.initialized) return;
            SessionState.SetBool(Flag, false);
            var go = new GameObject("Demo autopilot (recording)");
            go.AddComponent<DemoAutopilot>().Begin(mission, ui, dir);
        }

        public static void Done(string summary)
        {
            Debug.Log("[FMVideo] DONE " + summary);
            File.WriteAllText(Path.Combine(Frames, "..", "recording.txt"), summary);
            if (Environment.GetEnvironmentVariable("FM_QUIT") == "1") EditorApplication.delayCall += () => { EditorApplication.isPlaying = false; EditorApplication.delayCall += () => EditorApplication.Exit(0); };
            else EditorApplication.isPlaying = false;
        }
    }

    public class DemoAutopilot : MonoBehaviour
    {
        FirstMissionController mission; MissionsHUD ui; MissionDirector d; Stage08B.MarsPrototype p;
        int frame; bool recording; string priorSettings, priorProgress, priorMissions;
        readonly List<InputDevice> disabled = new List<InputDevice>();
        readonly List<string> log = new List<string>();
        float startReal;

        public void Begin(FirstMissionController m, MissionsHUD h, MissionDirector dd)
        {
            mission = m; ui = h; d = dd; p = m.hud.game.prototype;
            priorSettings = PlayerPrefs.GetString(FirstMissionController.SettingsKey, null);
            priorProgress = PlayerPrefs.GetString(FirstMissionController.ProgressKey, null);
            priorMissions = PlayerPrefs.GetString(MissionDirector.SaveKey, null);
            var store = Path.Combine(FirstMissionDemoRecorder.Frames, "..", "DemoStore");
            if (Directory.Exists(store)) Directory.Delete(store, true);
            mission.UseStoreForTests(store); mission.ResetProgressForTests(); mission.ResetSettingsForTests();
            d.ResetForTests(0);
            mission.hud.game.battery = 72;
            foreach (var dev in InputSystem.devices.ToList()) if (dev.enabled && (dev is Keyboard || dev is Pointer || dev is Gamepad)) { InputSystem.DisableDevice(dev); disabled.Add(dev); }
            if (Directory.Exists(FirstMissionDemoRecorder.Frames)) Directory.Delete(FirstMissionDemoRecorder.Frames, true);
            Directory.CreateDirectory(FirstMissionDemoRecorder.Frames);
            Time.captureFramerate = 30; startReal = Time.realtimeSinceStartup;
            ui.Cinematics.ReplayIntro();   // the video opens with the cinematic fly-in
            recording = true;
            StartCoroutine(Capture());
            StartCoroutine(Script());
        }

        IEnumerator Capture()
        {
            var eof = new WaitForEndOfFrame();
            while (recording)
            {
                yield return eof;
                var tex = ScreenCapture.CaptureScreenshotAsTexture();
                File.WriteAllBytes(Path.Combine(FirstMissionDemoRecorder.Frames, $"f{frame:00000}.jpg"), tex.EncodeToJPG(88));
                Destroy(tex);
                if (frame == 60) Note($"clock check: 60 frames -> game {Time.time:F2} s, unscaled {Time.unscaledTime:F2} s, real {Time.realtimeSinceStartup - startReal:F1} s");
                frame++;
            }
        }

        void Note(string s) { log.Add($"[{frame / 30f,6:F1}s] {s}"); Debug.Log("[FMVideo] " + s); }

        // ---------- control helpers (HUD widgets only) ----------
        static float Yaw(Vector3 a, Vector3 b) => Mathf.Atan2(b.x - a.x, b.z - a.z) * Mathf.Rad2Deg;
        static float Flat(Vector3 a, Vector3 b) => Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
        IEnumerator Wait(float s) { float t = Time.time + s; while (Time.time < t) yield return null; }

        List<Vector3> Route(Vector3 from, Vector3 to)
        {
            var r = Nav.RoutePlanner.Plan(ui.Grid, from, to, 1.5f);
            var pts = r.status == Nav.RouteStatus.Available && r.points.Count > 1 ? new List<Vector3>(r.points) : new List<Vector3> { from };
            pts.Add(to);
            return pts;
        }

        IEnumerator WalkTo(Vector3 target, float stop, float timeout = 40)
        {
            var pts = Route(p.astronaut.transform.position, target); int i = 1; float end = Time.time + timeout;
            while (Time.time < end)
            {
                var me = p.astronaut.transform.position;
                if (Flat(me, target) <= stop) break;
                while (i < pts.Count - 1 && Flat(me, pts[i]) < 1.6f) i++;
                p.yaw = Mathf.MoveTowardsAngle(p.yaw, Yaw(me, pts[i]), 240 * Time.deltaTime);
                ui.MoveStick.SetForTest(new Vector2(0, 1)); ui.Run.SetForTest(Flat(me, target) > 7);
                yield return null;
            }
            ui.MoveStick.SetForTest(Vector2.zero); ui.Run.SetForTest(false);
            yield return Wait(.3f);
        }

        IEnumerator DriveTo(Vector3 target, float stop, bool direct = false, float slowWithin = 9, float timeout = 60)
        {
            ui.Select(FirstMissionHUD.Tab.Drive);
            var pts = direct ? new List<Vector3> { p.rover.transform.position, target } : Route(p.rover.transform.position, target);
            int i = 1; float end = Time.time + timeout, stuck = 0;
            while (Time.time < end)
            {
                var me = p.rover.transform.position; float dist = Flat(me, target);
                if (dist <= stop) break;
                while (i < pts.Count - 1 && Flat(me, pts[i]) < 4f) i++;
                float ang = Mathf.DeltaAngle(p.rover.transform.eulerAngles.y, Yaw(me, pts[i]));
                ui.SteerStick.SetForTest(new Vector2(Mathf.Clamp(ang / 30f, -1, 1), 0));
                bool slow = dist < slowWithin || Mathf.Abs(ang) > 70;
                float cap = dist < slowWithin ? Mathf.Lerp(.35f, 1f, Mathf.InverseLerp(stop, slowWithin, dist)) : 1;
                ui.Accelerate.SetForTest(!slow || Mathf.Abs(p.speed) < 3.8f * cap);
                ui.Brake.SetForTest(false);
                stuck = Mathf.Abs(p.speed) < .1f ? stuck + Time.deltaTime : 0;
                if (stuck > 2.5f) { Note("unstick reverse"); ui.Accelerate.SetForTest(false); ui.SteerStick.SetForTest(Vector2.zero); yield return Reverse(1.6f); stuck = 0; }
                yield return null;
            }
            ui.Accelerate.SetForTest(false); ui.SteerStick.SetForTest(Vector2.zero); ui.Brake.SetForTest(true);
            float until = Time.time + 4; while (Mathf.Abs(p.speed) > .05f && Time.time < until) yield return null;
            ui.Brake.SetForTest(false);
        }

        IEnumerator Reverse(float s)
        {
            var rev = ui.GetComponentsInChildren<UnityEngine.UI.Button>(true).FirstOrDefault(b => b.name.Contains("Gear R") || b.name == "Reverse" || b.name.Contains("Segment R"));
            if (rev) rev.onClick.Invoke();
            ui.Accelerate.SetForTest(true); yield return Wait(s); ui.Accelerate.SetForTest(false);
            var fwd = ui.GetComponentsInChildren<UnityEngine.UI.Button>(true).FirstOrDefault(b => b.name.Contains("Gear D") || b.name == "Drive gear" || b.name.Contains("Segment D"));
            if (fwd) fwd.onClick.Invoke();
        }

        IEnumerator Board()
        {
            yield return WalkTo(p.rover.transform.position, 3.6f);
            yield return Wait(.6f);
            if (ui.InteractButton.gameObject.activeInHierarchy) ui.InteractButton.onClick.Invoke(); else p.Interact();
            float until = Time.time + 3; while (!p.driving && Time.time < until) yield return null;
            yield return Wait(.8f);
        }

        IEnumerator Exit()
        {
            ui.Brake.SetForTest(true); float until = Time.time + 4; while (Mathf.Abs(p.speed) > .05f && Time.time < until) yield return null; ui.Brake.SetForTest(false);
            ui.ExitButton.onClick.Invoke(); until = Time.time + 3; while (p.driving && Time.time < until) yield return null;
            yield return Wait(.8f);
        }

        IEnumerator WaitMission(int index, float timeout) { float end = Time.time + timeout; while ((d.MissionIndex < index && !d.AllComplete) && Time.time < end) yield return null; }

        // ---------- the run ----------
        IEnumerator Script()
        {
            Note("start: intro fly-in");
            yield return Wait(7.5f);   // intro + mission 1 title card
            Note("M1 walk to the tents"); yield return Wait(2.5f);
            yield return WalkTo(d.tentsAnchor.position, 2.5f);
            yield return Wait(2.5f);
            Note("M1 walk to the rover and board");
            yield return Board();
            yield return WaitMission(1, 5); yield return Wait(6.5f);

            Note("M2 drive onto the charging pad");
            yield return DriveTo(d.charging.pad.position, .9f, false, 10);
            yield return WaitMission(2, 20);
            yield return Wait(6.5f);

            Note("M3 drive to the generator");
            var gen = d.generator.GetComponent<MissionStructure>().Anchor.position;
            yield return DriveTo(gen, 6f);
            yield return Wait(1.2f);
            yield return Exit();
            var stand = d.generator.console.position + new Vector3(-1.6f, 0, 0);
            yield return WalkTo(stand, .5f);
            p.yaw = 90; yield return Wait(1.5f);
            Note("M3 hold REPAIR");
            ui.RepairHold.SetForTest(true);
            float until = Time.time + 8; while (!d.Repaired && Time.time < until) yield return null;
            ui.RepairHold.SetForTest(false);
            p.yaw = 60; yield return Wait(3f);
            yield return WaitMission(3, 4); yield return Wait(5f);

            Note("M4 science survey");
            yield return Board();
            var site = mission.Designated.transform.position;
            yield return DriveTo(site, 4.8f, false, 10);
            yield return Wait(1f);
            ui.Select(FirstMissionHUD.Tab.Science); yield return Wait(1.2f);
            var ctx = mission.Catalog.Instrument("rover.context_camera");
            mission.BeginObservation(ctx); until = Time.time + 6; while (mission.Observing != null && Time.time < until) yield return null;
            yield return Wait(2.5f);
            var arm = mission.Catalog.Instrument("rover.arm_sampler");
            if (!mission.BeginObservation(arm)) Note("arm refused: " + mission.LastFeedback);
            until = Time.time + 12; while (mission.Observing != null && Time.time < until) yield return null;
            yield return Wait(2.5f);
            ui.Select(FirstMissionHUD.Tab.Drive);
            yield return WaitMission(4, 6); yield return Wait(6f);

            Note("M5 checkpoint run");
            foreach (var cp in d.checkpoints)
            {
                var t = cp.transform; var approach = t.position - t.forward * 9; var exit = t.position + t.forward * 7;
                yield return DriveTo(approach, 3f, false, 4);
                yield return DriveTo(exit, 2f, true, 1);
                yield return Wait(1.2f);
            }
            yield return WaitMission(5, 4); yield return Wait(6f);

            Note("M6 return to dock");
            var bay = d.dockBay.position;
            yield return DriveTo(bay + new Vector3(0, 0, 11), 2.5f, false, 6);
            ui.GetComponent<ExplorerHUD>().ApplyProfile(DriveProfile.Precision);
            yield return DriveTo(bay, .8f, true, 8);
            ui.GetComponent<ExplorerHUD>().ApplyProfile(DriveProfile.Normal);
            until = Time.time + 6; while (!d.AllComplete && Time.time < until) yield return null;
            yield return Wait(3.5f);
            // outro: a slow crane shot rising out of the dock over the base
            Note("outro crane shot");
            var rig = mission.legacyRig; var rp = p.rover.transform.position;
            for (float t = 0; t < 9f; t += Time.deltaTime)
            {
                float e = Mathf.SmoothStep(0, 1, t / 9f);
                var from = rp + new Vector3(0, 3.2f, 14f); var to = rp + Quaternion.Euler(0, 130, 0) * new Vector3(0, 0, 48) + Vector3.up * 34;
                rig.captureOverride = true; rig.capturePosition = Vector3.Lerp(from, to, e) + Vector3.up * Mathf.Sin(e * Mathf.PI) * 6;
                rig.captureTarget = Vector3.Lerp(rp + Vector3.up, new Vector3(5, 0, -85), e);
                yield return null;
            }
            yield return Wait(1.5f);
            rig.captureOverride = false;
            Note($"end: all complete {d.AllComplete}, mission {d.MissionIndex}");
            Finish();
        }

        void Finish()
        {
            recording = false; Time.captureFramerate = 0;
            foreach (var dev in disabled) if (dev != null && dev.added) InputSystem.EnableDevice(dev);
            if (priorSettings == null) PlayerPrefs.DeleteKey(FirstMissionController.SettingsKey); else PlayerPrefs.SetString(FirstMissionController.SettingsKey, priorSettings);
            if (priorProgress == null) PlayerPrefs.DeleteKey(FirstMissionController.ProgressKey); else PlayerPrefs.SetString(FirstMissionController.ProgressKey, priorProgress);
            if (priorMissions == null) PlayerPrefs.DeleteKey(MissionDirector.SaveKey); else PlayerPrefs.SetString(MissionDirector.SaveKey, priorMissions);
            PlayerPrefs.Save();
            FirstMissionDemoRecorder.Done($"frames {frame} ({frame / 30f:F1} s at 30 fps), all missions complete: {d.AllComplete}\n" + string.Join("\n", log));
        }
    }
}
