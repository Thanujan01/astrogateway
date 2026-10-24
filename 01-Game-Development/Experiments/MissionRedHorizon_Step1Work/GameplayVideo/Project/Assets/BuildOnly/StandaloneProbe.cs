using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;

// Build-copy only (never in the main project). In the macOS player, when VU_PROBE_OUT is set: logs the
// post-processing state, saves real player screenshots, measures frame times on Medium/High/Ultra (runtime
// pipeline copy only, nothing persisted), captures the cinematic overview, then quits.
// v2 (HUD batch 2): adds CPU/GPU frame timing, draw calls / SetPass / batches, GC allocation per frame, memory,
// uGUI canvas timings, and a HUD hidden/full A/B at High to isolate UI rendering overhead.
public class StandaloneProbe : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        var dir = System.Environment.GetEnvironmentVariable("VU_PROBE_OUT");
        if (string.IsNullOrEmpty(dir)) return;
        var g = new GameObject("Standalone probe (build copy only)");
        DontDestroyOnLoad(g);
        g.AddComponent<StandaloneProbe>().dir = dir;
    }

    string dir;
    readonly List<string> lines = new List<string>();

    void Log(string s) { lines.Add(s); Debug.Log("[Probe] " + s); File.WriteAllLines(Path.Combine(dir, "standalone_probe.txt"), lines); }

    static ProfilerRecorder Rec(ProfilerCategory c, string name) => ProfilerRecorder.StartNew(c, name, 1);

    IEnumerator Measure(string label, float seconds)
    {
        var keys = new[] { "draw_calls", "setpass", "batches", "tris_k", "gc_alloc_B_per_frame", "ui_canvas_update_ms", "ui_overlay_render_ms" };
        var scales = new[] { 1, 1, 1, 1e-3, 1, 1e-6, 1e-6 };
        var recs = new[]
        {
            Rec(ProfilerCategory.Render, "Draw Calls Count"),
            Rec(ProfilerCategory.Render, "SetPass Calls Count"),
            Rec(ProfilerCategory.Render, "Batches Count"),
            Rec(ProfilerCategory.Render, "Triangles Count"),
            Rec(ProfilerCategory.Memory, "GC Allocated In Frame"),
            Rec(ProfilerCategory.Internal, "PostLateUpdate.PlayerUpdateCanvases"),
            Rec(ProfilerCategory.Render, "UGUI.Rendering.RenderOverlays"),
        };
        var sums = new double[recs.Length]; var counts = new int[recs.Length];
        var ms = new List<float>(); var cpu = new List<double>(); var gpu = new List<double>();
        var timings = new FrameTiming[1];
        float end = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < end)
        {
            yield return null;
            ms.Add(Time.unscaledDeltaTime * 1000);
            for (int i = 0; i < recs.Length; i++) if (recs[i].Valid && recs[i].Count > 0) { sums[i] += recs[i].LastValue * scales[i]; counts[i]++; }
            FrameTimingManager.CaptureFrameTimings();
            if (FrameTimingManager.GetLatestTimings(1, timings) > 0)
            {
                if (timings[0].cpuFrameTime > 0) cpu.Add(timings[0].cpuFrameTime);
                if (timings[0].gpuFrameTime > 0) gpu.Add(timings[0].gpuFrameTime);
            }
        }
        ms.Sort();
        float mean = ms.Average();
        string extra = string.Join(" ", keys.Select((k, i) => $"{k}={(counts[i] > 0 ? (sums[i] / counts[i]).ToString("0.###") : "n/a")}"));
        foreach (var r in recs) r.Dispose();
        Log($"{label}: frames={ms.Count} mean_ms={mean:F2} fps={1000 / mean:F1} p50_ms={ms[ms.Count / 2]:F2} p95_ms={ms[(int)(ms.Count * .95f)]:F2} p99_ms={ms[(int)(ms.Count * .99f)]:F2} max_ms={ms[ms.Count - 1]:F2} " +
            $"cpu_frame_ms={(cpu.Count > 0 ? cpu.Average().ToString("F2") : "n/a")} gpu_frame_ms={(gpu.Count > 0 ? gpu.Average().ToString("F2") : "n/a")} {extra} " +
            $"mem_alloc_MB={Profiler.GetTotalAllocatedMemoryLong() / 1048576.0:F0} mem_reserved_MB={Profiler.GetTotalReservedMemoryLong() / 1048576.0:F0} mono_MB={Profiler.GetMonoUsedSizeLong() / 1048576.0:F0} gfx_MB={Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576.0:F0} " +
            $"cap={Application.targetFrameRate} focused={Application.isFocused} screen={Screen.width}x{Screen.height}");
    }

    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(6);
        var hud = FindAnyObjectByType<AstroGatewayHUDController>();
        var cam = hud ? hud.game.prototype.follow : Camera.main;
        var data = cam ? cam.GetUniversalAdditionalCameraData() : null;
        var urp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        Log($"camera={cam?.name} renderPostProcessing={data?.renderPostProcessing} pipeline={urp?.name} hdr={urp?.supportsHDR} volumes={FindObjectsByType<Volume>(FindObjectsSortMode.None).Length} gpu={SystemInfo.graphicsDeviceName} api={SystemInfo.graphicsDeviceType} frameTimingStats={FrameTimingManager.IsFeatureEnabled()}");
        if (hud) { hud.settings.visibility = 0; hud.ApplySettings(); }
        int prior = hud ? hud.settings.quality : 1;
        string[] names = { "Low", "Medium", "High", "Ultra" };
        foreach (int q in new[] { 1, 2, 3 })
        {
            if (hud) hud.environment.ApplyQuality(q);
            yield return new WaitForSecondsRealtime(2);
            if (hud) Log("preset " + names[q] + ": " + hud.environment.Readback());
            yield return Measure("Standalone " + names[q] + " (astronaut idle at spawn, full HUD)", 8);
            ScreenCapture.CaptureScreenshot(Path.Combine(dir, $"standalone_{names[q]}.png"));
            yield return new WaitForSecondsRealtime(1);
        }
        if (hud)
        {
            // UI overhead A/B at High: same view, HUD hidden (profile 3) vs full; runtime only, never persisted.
            hud.environment.ApplyQuality(2);
            yield return new WaitForSecondsRealtime(2);
            hud.settings.visibility = 3; hud.ApplySettings();
            yield return new WaitForSecondsRealtime(1);
            yield return Measure("Standalone High, HUD hidden (UI A/B)", 8);
            hud.settings.visibility = 0; hud.ApplySettings();
            yield return new WaitForSecondsRealtime(1);
            yield return Measure("Standalone High, HUD full (UI A/B)", 8);
            hud.TogglePause();
            yield return new WaitForSecondsRealtime(1.5f);
            yield return Measure("Standalone High, pause card open", 6);
            ScreenCapture.CaptureScreenshot(Path.Combine(dir, "standalone_pause.png"));
            yield return new WaitForSecondsRealtime(1);
            hud.CloseModal();
            yield return new WaitForSecondsRealtime(1);
            hud.environment.ApplyQuality(3);
        }
        var coord = FindAnyObjectByType<ProductionModeCoordinator>();
        if (coord)
        {
            coord.ToggleCinematic();
            yield return new WaitForSecondsRealtime(4);
            yield return Measure("Standalone Ultra cinematic overview", 6);
            ScreenCapture.CaptureScreenshot(Path.Combine(dir, "standalone_cinematic.png"));
            yield return new WaitForSecondsRealtime(1);
            coord.ToggleCinematic();
        }
        if (hud) hud.environment.ApplyQuality(prior); // runtime copy only; settings were never persisted
        Log("done");
        Application.Quit();
    }
}
