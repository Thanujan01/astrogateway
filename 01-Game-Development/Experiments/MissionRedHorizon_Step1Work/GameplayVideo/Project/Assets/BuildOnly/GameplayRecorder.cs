using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;
using AstroGateway.Stage08B;

// Video-copy only (never in the main project). In the macOS player, when VU_RECORD_OUT is set: drives a short
// scripted play session through the real keyboard input path (virtual Input System keyboard) and saves every
// rendered frame (ScreenCapture, real player output) at a fixed 30 fps game clock, plus captions.txt marking
// each segment, then quits. Nothing is saved to the game or its settings.
public class GameplayRecorder : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        var dir = System.Environment.GetEnvironmentVariable("VU_RECORD_OUT");
        if (string.IsNullOrEmpty(dir)) return;
        Directory.CreateDirectory(Path.Combine(dir, "frames"));
        var g = new GameObject("Gameplay recorder (video copy only)");
        DontDestroyOnLoad(g);
        g.AddComponent<GameplayRecorder>().dir = dir;
    }

    const int Fps = 30;
    string dir;
    bool recording;
    int frame;
    readonly List<string> captions = new List<string>();
    readonly List<string> log = new List<string>();
    Keyboard keyboard;
    readonly HashSet<Key> held = new HashSet<Key>();
    MarsPrototype p;

    void Log(string s) { log.Add(s); Debug.Log("[Recorder] " + s); File.WriteAllLines(Path.Combine(dir, "recorder_log.txt"), log); }
    void Caption(string s) { captions.Add(frame + "|" + s); File.WriteAllLines(Path.Combine(dir, "captions.txt"), captions); Log("frame " + frame + ": " + s); }

    void Apply() { InputSystem.QueueStateEvent(keyboard, held.Count == 0 ? new KeyboardState() : new KeyboardState(held.ToArray())); }
    void Hold(params Key[] keys) { foreach (var k in keys) held.Add(k); Apply(); }
    void Release(params Key[] keys) { if (keys.Length == 0) held.Clear(); else foreach (var k in keys) held.Remove(k); Apply(); }
    IEnumerator Frames(float seconds) { int n = Mathf.RoundToInt(seconds * Fps); for (int i = 0; i < n; i++) yield return null; }
    IEnumerator Tap(Key key) { Hold(key); yield return Frames(0.1f); Release(key); yield return Frames(0.1f); }

    bool Own(Collider c) => c == p.astronaut || c == p.rover || c.transform.IsChildOf(p.rover.transform) || c.transform.IsChildOf(p.astronaut.transform);
    float GroundY(Vector3 pos)
    {
        var hits = Physics.RaycastAll(pos + Vector3.up * 30, Vector3.down, 80, 1 << 0, QueryTriggerInteraction.Ignore);
        var valid = hits.Where(h => !Own(h.collider)).OrderBy(h => h.distance).ToArray();
        return valid.Length > 0 ? valid[0].point.y : float.NaN;
    }
    void Teleport(CharacterController c, Vector3 pos, float yaw)
    {
        float g = GroundY(pos); if (!float.IsNaN(g)) pos.y = g + 0.05f;
        c.enabled = false; c.transform.SetPositionAndRotation(pos, Quaternion.Euler(0, yaw, 0)); c.enabled = true;
        Physics.SyncTransforms();
    }
    Vector3 OpenSpot(Vector3 hint, float clear)
    {
        for (int ring = 0; ring < 12; ring++)
            for (int i = 0; i < 8; i++)
            {
                float a = i * 45 * Mathf.Deg2Rad;
                Vector3 c = hint + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * ring * 6;
                if (Mathf.Abs(c.x) > 105 || Mathf.Abs(c.z) > 105) continue;
                float g = GroundY(c); if (float.IsNaN(g)) continue; c.y = g;
                if (Physics.OverlapSphere(c + Vector3.up * 1.2f, clear, 1 << 0, QueryTriggerInteraction.Ignore).All(x => Own(x) || x is TerrainCollider || x.bounds.size.y < 0.3f || x.bounds.size.x > 200)) return c;
            }
        return hint;
    }
    static float YawTo(Vector3 from, Vector3 to) { var d = to - from; return Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg; }

    IEnumerator CaptureLoop()
    {
        var eof = new WaitForEndOfFrame();
        while (true)
        {
            yield return eof;
            if (!recording) continue;
            var tex = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(dir, "frames", $"f{frame:D5}.jpg"), tex.EncodeToJPG(92));
            Destroy(tex);
            frame++;
        }
    }

    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(6);
        var hud = FindAnyObjectByType<AstroGatewayHUDController>();
        var modes = FindAnyObjectByType<ProductionModeCoordinator>();
        p = hud.game.prototype;
        Log($"screen={Screen.width}x{Screen.height} gpu={SystemInfo.graphicsDeviceName} scene ready, targets={hud.environment.targets.Count(t => t)}");

        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        keyboard = InputSystem.AddDevice<Keyboard>("Recorder keyboard");
        keyboard.MakeCurrent();
        hud.game.testing = false;
        hud.settings.visibility = 0; hud.ApplySettings();   // full HUD for this run only, not persisted
        hud.environment.ApplyQuality(3);                     // Ultra, runtime only
        yield return new WaitForSecondsRealtime(2);

        Time.captureFramerate = Fps;
        StartCoroutine(CaptureLoop());
        recording = true;

        var cc = p.astronaut; var rover = p.rover;
        Caption("Mars research base: astronaut at spawn");
        yield return Frames(2.5f);

        Caption("On foot: walking and running across the base");
        Hold(Key.W); yield return Frames(2.5f);
        Hold(Key.LeftShift); yield return Frames(2.5f);
        Release(); yield return Frames(0.5f);

        Caption("Boarding the rover");
        var rSpot = OpenSpot(new Vector3(-30, 0, -10), 9f);
        Teleport(rover, rSpot, 0);
        Teleport(cc, rover.transform.position + rover.transform.right * 3.5f, 0);
        p.yaw = YawTo(cc.transform.position, rover.transform.position) - 35f;
        yield return Frames(1.5f);
        yield return Tap(Key.E);
        yield return Frames(1f);
        Log("driving=" + p.driving);

        Caption("Driving the rover");
        p.yaw = rover.transform.eulerAngles.y;
        Hold(Key.W); yield return Frames(4f);
        Hold(Key.D); yield return Frames(2.5f); Release(Key.D);
        yield return Frames(2f);
        Release(); Hold(Key.Space); yield return Frames(2f); Release();
        p.speed = 0; yield return Frames(0.3f);
        yield return Tap(Key.E);
        yield return Frames(0.5f);
        Log("after exit driving=" + p.driving);
        if (p.driving) { p.speed = 0; yield return Tap(Key.E); }

        var target = hud.environment.targets.FirstOrDefault(t => t);
        if (target)
        {
            Caption("Science: scanning " + target.displayName);
            Vector3 tp = target.transform.position;
            Teleport(cc, tp - Vector3.forward * 2f, 0);
            p.yaw = YawTo(cc.transform.position, tp);
            yield return Frames(0.8f);
            yield return Tap(Key.K);
            yield return Frames(3f);
            Log("scan result: " + hud.lastResult);
        }

        Caption("Weather: dust storm rolls in");
        yield return Tap(Key.Digit3);
        yield return Frames(3.5f);
        yield return Tap(Key.Digit1);
        yield return Frames(0.5f);

        if (modes)
        {
            Caption("Cinematic overview of the base");
            yield return Tap(Key.C);
            yield return Frames(6f);
        }

        recording = false;
        yield return new WaitForEndOfFrame();
        Time.captureFramerate = 0;
        Log("done frames=" + frame);
        File.WriteAllText(Path.Combine(dir, "done.txt"), frame.ToString());
        Application.Quit();
    }
}
