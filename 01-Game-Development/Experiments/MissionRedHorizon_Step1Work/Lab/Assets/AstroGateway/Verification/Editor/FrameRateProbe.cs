using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using AstroGateway.CinematicHUD;
using AstroGateway.NativeSlice;

// Lab-only diagnostic: frame rate in open ground versus pushing into the Laboratory Dome.
namespace AstroGateway.Verification
{
    [InitializeOnLoad]
    public static class FrameRateProbe
    {
        static double t0, nextLog; static int f0, phase; static AstroGatewayHUDController hud; static StreamWriter log;
        static FrameRateProbe() { EditorApplication.update += Tick; }
        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/AstroGateway/ProductionVisual/Scenes/Mars_ProductionVisual_v1.unity");
            SessionState.SetBool("FrameProbe", true);
            Application.runInBackground = true;
            EditorApplication.isPlaying = true;
        }
        static void Tick()
        {
            if (!Application.isBatchMode || !EditorApplication.isPlaying) return;
            if (log == null)
            {
                if (!SessionState.GetBool("FrameProbe", false)) return;
                hud = Object.FindAnyObjectByType<AstroGatewayHUDController>();
                if (!hud || !hud.game || !hud.game.initialized) return;
                log = new StreamWriter(System.Environment.GetEnvironmentVariable("PROBE_OUT"));
                hud.game.testing = true;
                t0 = nextLog = EditorApplication.timeSinceStartup; f0 = Time.frameCount;
            }
            double t = EditorApplication.timeSinceStartup - t0;
            var p = hud.game.prototype;
            if (phase == 0 && t > 20)
            {
                var dome = GameObject.Find("Laboratory Dome").GetComponent<Collider>().bounds;
                Vector3 dir = new Vector3(1, 0, 0);
                p.astronaut.enabled = false; p.astronaut.transform.position = dome.center + dir * (dome.extents.x + 2) + Vector3.down * dome.center.y; p.astronaut.enabled = true;
                p.yaw = -90; hud.game.modes.Submit(new NativeInput { movement = Vector2.up, run = true });
                phase = 1; log.WriteLine("--- pushing into dome");
                foreach (var c in GameObject.Find("Laboratory Dome").GetComponentsInChildren<Collider>()) log.WriteLine("collider " + c.GetType().Name + " " + c.name + (c is MeshCollider mc && mc.sharedMesh ? " tris " + mc.sharedMesh.triangles.Length / 3 + " convex " + mc.convex + " mesh " + mc.sharedMesh.name : ""));
                log.WriteLine("cc skin " + p.astronaut.skinWidth + " step " + p.astronaut.stepOffset + " slope " + p.astronaut.slopeLimit);
            }
            if (EditorApplication.timeSinceStartup >= nextLog)
            {
                nextLog += 5;
                log.WriteLine($"t {t:F0}s frames {Time.frameCount - f0} elapsed {p.elapsed:F2} pos {p.astronaut.transform.position}");
                log.Flush();
            }
            if (t > 60 || (phase == 1 && Time.frameCount - f0 > 600)) { log.Close(); EditorApplication.Exit(0); }
        }
    }
}
