using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using AstroGateway.CinematicHUD;

// Read-only layout probe for planning mission structures: object positions, collider footprints, prefab sizes and a
// top-down render of the Explorer scene. Writes to env FM_OUT. Never saves the scene.
namespace AstroGateway.FirstMission.EditorTools
{
    public static class FirstMissionLayoutProbe
    {
        // Perspective close-ups of the mission structures (Missions scene). Edit-mode render, no play mode.
        public static void Views()
        {
            string outDir = System.Environment.GetEnvironmentVariable("FM_OUT") ?? "/tmp/fm_probe";
            Directory.CreateDirectory(outDir);
            EditorSceneManager.OpenScene(FirstMissionBuilder.MissionsScene);
            UnityEditor.ShaderUtil.allowAsyncCompilation = false;
            var cam = new GameObject("probe cam").AddComponent<Camera>(); cam.fieldOfView = 55; cam.farClipPlane = 600;
            var shots = new (string n, Vector3 eye, Vector3 at)[]
            {
                ("v_tents", new Vector3(-46, 4.5f, -96), new Vector3(-58, 1, -86)),
                ("v_charging", new Vector3(-4, 4, -104), new Vector3(6, .5f, -96)),
                ("v_generator", new Vector3(4, 7, -66), new Vector3(24, 2, -80)),
                ("v_generator_console", new Vector3(9, 2.4f, -74), new Vector3(16, 1, -77)),
                ("v_dock", new Vector3(-22, 6, -92), new Vector3(-21, 1.5f, -108)),
                ("v_checkpoint1", new Vector3(48, 3.5f, -44), new Vector3(60, 1.5f, -40)),
                ("v_overview", new Vector3(-30, 40, -130), new Vector3(10, 0, -80)),
            };
            var rt = new RenderTexture(1600, 900, 24); cam.targetTexture = rt;
            foreach (var s in shots)
            {
                cam.transform.position = s.eye; cam.transform.LookAt(s.at); cam.Render(); cam.Render();
                RenderTexture.active = rt; var tex = new Texture2D(1600, 900, TextureFormat.RGB24, false); tex.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); tex.Apply();
                File.WriteAllBytes(Path.Combine(outDir, s.n + ".png"), tex.EncodeToPNG());
            }
            Debug.Log("[FMProbe] views done " + outDir);
        }

        public static void Run()
        {
            string outDir = System.Environment.GetEnvironmentVariable("FM_OUT") ?? "/tmp/fm_probe";
            Directory.CreateDirectory(outDir);
            EditorSceneManager.OpenScene(FirstMissionBuilder.ExplorerScene);
            var sb = new StringBuilder();
            var hud = Object.FindAnyObjectByType<AstroGatewayHUDController>();
            var p = hud.game.prototype;
            sb.AppendLine($"rover {p.rover.transform.position} yaw {p.rover.transform.eulerAngles.y}");
            sb.AppendLine($"astronaut {p.astronaut.transform.position} yaw {p.astronaut.transform.eulerAngles.y}");
            foreach (var n in new[] { "Crew Dome", "Laboratory Dome", "Life Support Dome", "Crew Dome sealed airlock", "Laboratory Dome sealed airlock", "Life Support Dome sealed airlock", "Zone C — solar farm", "Power distribution cabinet", "Communications node mast", "Life support storage tank 0", "Storage crate 0" })
            {
                var t = Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude).FirstOrDefault(x => x.name == n);
                if (!t) { sb.AppendLine("MISSING " + n); continue; }
                var rs = t.GetComponentsInChildren<Renderer>(); var b = rs.Length > 0 ? rs[0].bounds : new Bounds(t.position, Vector3.zero); foreach (var r in rs) b.Encapsulate(r.bounds);
                sb.AppendLine($"{n}: pos {t.position} bounds c {b.center} size {b.size}");
            }
            foreach (var t in hud.environment.targets) if (t) sb.AppendLine($"station {t.stableId} {t.displayName} {t.transform.position}");
            sb.AppendLine("\nColliders (non-trigger, footprint > 1.5 m, excluding ground):");
            foreach (var c in Object.FindObjectsByType<Collider>(FindObjectsInactive.Exclude).Where(c => !c.isTrigger).OrderByDescending(c => c.bounds.size.x * c.bounds.size.z))
            {
                var b = c.bounds; if (b.size.x > 150 || b.size.z > 150) { sb.AppendLine($"GROUND {c.name} {b.center} {b.size}"); continue; }
                if (b.size.x * b.size.z < 2.25f) continue;
                sb.AppendLine($"{c.GetType().Name} '{c.name}' parent '{(c.transform.parent ? c.transform.parent.name : "")}' c {b.center} s {b.size}");
            }
            int small = Object.FindObjectsByType<Collider>(FindObjectsInactive.Exclude).Count(c => !c.isTrigger && c.bounds.size.x * c.bounds.size.z < 2.25f);
            sb.AppendLine("small colliders: " + small);
            sb.AppendLine("\nPrefabs:");
            foreach (var path in new[] { "Assets/AstroGateway/Environment/Prefabs/P_PowerStation.prefab", "Assets/AstroGateway/Environment/Prefabs/R_RoverBay.prefab", "Assets/AstroGateway/Environment/Prefabs/C_ScienceStaging.prefab", "Assets/AstroGateway/Environment/Prefabs/S_SolarArray.prefab", "Assets/AstroGateway/Mission01Milestone/Prefabs/Boarding_Console.prefab" })
            {
                var go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                go.transform.position = new Vector3(0, -500, 0);
                var rs = go.GetComponentsInChildren<Renderer>(); var b = rs[0].bounds; foreach (var r in rs) b.Encapsulate(r.bounds);
                sb.AppendLine($"{Path.GetFileName(path)} size {b.size} centre offset {b.center - go.transform.position} renderers {rs.Length} colliders {go.GetComponentsInChildren<Collider>().Length} mats {string.Join(",", rs.SelectMany(r => r.sharedMaterials).Where(m => m).Select(m => m.name).Distinct())}");
                foreach (Transform ch in go.transform) sb.AppendLine($"   child {ch.name} local {ch.localPosition} scale {ch.localScale}");
                Object.DestroyImmediate(go);
            }
            // ground heights on a coarse grid
            sb.AppendLine("\nGround height samples (x,z:y):");
            for (int z = -100; z <= 100; z += 20) { var line = new StringBuilder(); for (int x = -100; x <= 100; x += 20) { float y = Physics.Raycast(new Vector3(x, 200, z), Vector3.down, out var h, 400, ~0, QueryTriggerInteraction.Ignore) ? h.point.y : float.NaN; line.Append($"{y,6:0.0}"); } sb.AppendLine($"z{z,4}: {line}"); }
            File.WriteAllText(Path.Combine(outDir, "layout.txt"), sb.ToString());

            // top-down render with 10 m grid reference
            var cam = new GameObject("probe cam").AddComponent<Camera>();
            cam.orthographic = true; cam.orthographicSize = 125; cam.transform.SetPositionAndRotation(new Vector3(0, 400, 0), Quaternion.Euler(90, 0, 0));
            cam.farClipPlane = 800; cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = Color.black;
            var rt = new RenderTexture(2048, 2048, 24); cam.targetTexture = rt; RenderSettings.fog = false; cam.Render();
            RenderTexture.active = rt; var tex = new Texture2D(2048, 2048, TextureFormat.RGB24, false); tex.ReadPixels(new Rect(0, 0, 2048, 2048), 0, 0);
            for (int i = -120; i <= 120; i += 10) { int px = Mathf.RoundToInt((i + 125) / 250f * 2047); var col = i == 0 ? Color.cyan : new Color(1, 1, 1, .4f); for (int k = 0; k < 2048; k += (i % 50 == 0 ? 1 : 4)) { tex.SetPixel(px, k, col); tex.SetPixel(k, px, col); } }
            tex.Apply(); File.WriteAllBytes(Path.Combine(outDir, "topdown.png"), tex.EncodeToPNG());
            Debug.Log("[FMProbe] done " + outDir);
        }
    }
}
