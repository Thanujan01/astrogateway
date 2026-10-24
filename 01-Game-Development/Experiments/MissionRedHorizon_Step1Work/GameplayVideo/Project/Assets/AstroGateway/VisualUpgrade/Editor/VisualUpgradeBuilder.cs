using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;

namespace AstroGateway.VisualUpgrade.EditorTools
{
    // Builds the isolated Visual Upgrade scene from Mars_ProductionVisual_v1 (which is only read, never saved).
    // Every asset this scene changes is a copy under Assets/AstroGateway/VisualUpgrade, so the original scene,
    // its materials, volume profile, URP preview asset and lighting presets stay byte-identical.
    // Entry points (run with -executeMethod): Build, Diagnose.
    public static class VisualUpgradeBuilder
    {
        public const string Source = "Assets/AstroGateway/ProductionVisual/Scenes/Mars_ProductionVisual_v1.unity";
        public const string F = "Assets/AstroGateway/VisualUpgrade";
        public const string Scene = F + "/Scenes/Mars_VisualUpgrade_v1.unity";
        static string Out => Environment.GetEnvironmentVariable("VU_OUT") ?? Path.GetFullPath("Docs/VisualUpgrade");

        public static void Build()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Scene)) AssetDatabase.DeleteAsset(Scene); // our own scene only
            if (!AssetDatabase.CopyAsset(Source, Scene)) throw new Exception("Scene copy failed");
            EditorSceneManager.OpenScene(Scene);
            var hud = UnityEngine.Object.FindAnyObjectByType<AstroGatewayHUDController>();
            var modes = UnityEngine.Object.FindAnyObjectByType<ProductionModeCoordinator>();
            var legacy = UnityEngine.Object.FindAnyObjectByType<ProductionCameraRig>();
            var art = UnityEngine.Object.FindAnyObjectByType<ProductionPresentation>();
            if (!hud || !modes || !legacy || !art) throw new Exception("Production Visual dependencies missing");

            // Scene-local copies of shared assets this upgrade will tune.
            var p = hud.game.prototype;
            p.previewPipeline = Copy(p.previewPipeline, "Settings/VisualUpgrade_URP.asset");
            var env = hud.environment;
            env.sky = Copy(env.sky, "Materials/VisualUpgradeSky.mat");
            RenderSettings.skybox = env.sky;
            for (int i = 0; i < env.lighting.Length; i++) env.lighting[i] = Copy(env.lighting[i], "Settings/Lighting_" + i + ".asset");
            hud.theme = Copy(hud.theme, "Settings/VisualUpgradeHUDTheme.asset");
            // HUD palette for this scene: dark translucent navy panels, cyan and orange accents.
            hud.theme.panel = new Color(.015f, .035f, .055f, .64f); hud.theme.text = new Color(.90f, .94f, .96f);
            hud.theme.secondary = new Color(.58f, .68f, .74f); hud.theme.orange = new Color(1f, .56f, .16f);
            hud.theme.oxygen = new Color(.30f, .86f, .96f); hud.theme.critical = new Color(1f, .33f, .30f);
            EditorUtility.SetDirty(hud.theme);
            foreach (var volume in UnityEngine.Object.FindObjectsByType<Volume>(FindObjectsSortMode.None))
            {
                var profile = CopyProfile(volume.sharedProfile, "Settings/VisualUpgradeVolume.asset");
                volume.sharedProfile = profile;
                // Standalone fix (no project setting changed): with "strip unused post-processing" on, URP strips the
                // DoF and Panini shaders unless some VolumeProfile under Assets has them with IsActive() true, and the
                // player then skips ALL post-processing. Both components are stored with a non-zero setting so the
                // stripper keeps the shaders, but disabled (component.active = false) so nothing renders by default.
                var dof = Add<DepthOfField>(profile);
                dof.mode.Override(DepthOfFieldMode.Gaussian);
                dof.gaussianStart.Override(170f); dof.gaussianEnd.Override(420f); dof.gaussianMaxRadius.Override(1f);
                dof.active = false; // enabled at runtime for the cinematic overview only
                var panini = Add<PaniniProjection>(profile);
                panini.distance.Override(0.01f);
                panini.active = false;
                EditorUtility.SetDirty(profile);
            }

            // Camera: keep the original rig (state owner for buttons/tests) but let the fixed rig drive the camera.
            legacy.enabled = false;
            var root = legacy.gameObject;
            var rig = root.AddComponent<VisualUpgradeCameraRig>();
            rig.hud = hud; rig.modes = modes; rig.legacy = legacy;
            var fixes = root.AddComponent<VisualUpgradeHUDFixes>();
            fixes.hud = hud; fixes.presentation = art;
            var style = root.AddComponent<VisualUpgradeHUDStyle>();
            style.hud = hud; style.modes = modes; style.presentation = art;

            // Visual systems, one at a time (each is a separate builder step).
            VisualUpgradeKit.Reset();
            VisualUpgradeEnvironment.Apply(hud, modes, root);
            VisualUpgradeCharacters.Apply(hud, root);
            VisualUpgradeProps.Apply(hud, root);

            hud.game.registry = Copy(hud.game.registry, "Settings/VisualUpgradeMissionRegistry.asset");
            hud.game.registry.missionId = "mission.red_horizon.visual_upgrade.v1";
            EditorUtility.SetDirty(hud.game.registry);

            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
            Report("Build.txt", "PASS scene built " + DateTime.Now + "\n" + Scene + "\n" + string.Join("\n", VisualUpgradeEnvironment.Notes.Concat(VisualUpgradeCharacters.Notes).Concat(VisualUpgradeProps.Notes)));
        }

        static T Copy<T>(T original, string relative) where T : UnityEngine.Object
        {
            string path = F + "/" + relative;
            AssetDatabase.DeleteAsset(path);
            var copy = UnityEngine.Object.Instantiate(original);
            copy.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(copy, path);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        static T Add<T>(VolumeProfile profile) where T : VolumeComponent
        {
            if (profile.TryGet<T>(out var existing)) return existing;
            var c = profile.Add<T>();
            c.name = typeof(T).Name;
            AssetDatabase.AddObjectToAsset(c, profile);
            return c;
        }

        static VolumeProfile CopyProfile(VolumeProfile original, string relative)
        {
            string path = F + "/" + relative;
            AssetDatabase.DeleteAsset(path);
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, path);
            foreach (var c in original.components)
            {
                var copy = UnityEngine.Object.Instantiate(c);
                copy.name = c.name;
                profile.components.Add(copy);
                AssetDatabase.AddObjectToAsset(copy, profile);
            }
            EditorUtility.SetDirty(profile);
            return profile;
        }

        // Read-only scene report used to track down the white sky line and camera-vs-airlock geometry.
        public static void Diagnose()
        {
            EditorSceneManager.OpenScene(File.Exists(Scene) ? Scene : Source);
            var lines = new List<string> { "Scene " + EditorSceneManager.GetActiveScene().path };
            foreach (var r in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
            {
                var s = r.bounds.size;
                float max = Mathf.Max(s.x, s.y, s.z), min = Mathf.Min(s.x, s.y, s.z);
                string kind = r.GetType().Name;
                bool special = r is LineRenderer || r is TrailRenderer || r is ParticleSystemRenderer;
                if (special || max > 20)
                    lines.Add($"{kind} | {Hier(r.transform)} | enabled={r.enabled} active={r.gameObject.activeInHierarchy} size={s} center={r.bounds.center} mat={string.Join(",", r.sharedMaterials.Select(m => m ? m.name + "/" + m.shader.name : "null"))}"
                        + (r is ParticleSystemRenderer pr ? $" mode={pr.renderMode} lengthScale={pr.lengthScale} velScale={pr.velocityScale}" : ""));
            }
            foreach (var ps in UnityEngine.Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None))
            {
                var m = ps.main;
                lines.Add($"Particles | {Hier(ps.transform)} | space={m.simulationSpace} speed={m.startSpeed.constant} size={m.startSize.constant} life={m.startLifetime.constant} max={m.maxParticles} shape={ps.shape.shapeType} scale={ps.shape.scale}");
            }
            foreach (var t in UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t => t.name.Contains("airlock")))
            {
                var c = t.GetComponent<Collider>();
                lines.Add($"Airlock | {Hier(t)} | pos={t.position} scale={t.lossyScale} collider={(c ? c.GetType().Name + " " + c.bounds.size : "none")} layer={t.gameObject.layer}");
            }
            Report("Scene_Diagnostics.txt", string.Join("\n", lines));
        }

        // Read-only inventory of what the visual systems will work with.
        public static void Inventory()
        {
            EditorSceneManager.OpenScene(Scene);
            var lines = new List<string>();
            var hud = UnityEngine.Object.FindAnyObjectByType<AstroGatewayHUDController>();
            var p = hud.game.prototype;
            lines.Add("== Roots");
            foreach (var r in EditorSceneManager.GetActiveScene().GetRootGameObjects()) lines.Add(r.name + " children=" + r.transform.childCount);
            void Tree(string title, Transform root, int depth = 3)
            {
                lines.Add("== " + title);
                void Walk(Transform t, int d)
                {
                    var mf = t.GetComponent<MeshFilter>(); var mr = t.GetComponent<Renderer>(); var col = t.GetComponent<Collider>();
                    lines.Add(new string(' ', d * 2) + t.name + $" pos={t.localPosition} rot={t.localEulerAngles} scale={t.localScale}"
                        + (mf && mf.sharedMesh ? $" mesh={mf.sharedMesh.name}({mf.sharedMesh.vertexCount}v)" : "")
                        + (mr ? $" mat={string.Join(",", mr.sharedMaterials.Select(m => m ? m.name : "null"))} on={mr.enabled}" : "")
                        + (col ? " col=" + col.GetType().Name : "") + " layer=" + t.gameObject.layer
                        + " comps=" + string.Join(",", t.GetComponents<Component>().Select(c => c ? c.GetType().Name : "missing").Where(n => n != "Transform" && n != "MeshFilter" && n != "MeshRenderer")));
                    if (d < depth) foreach (Transform c in t) Walk(c, d + 1);
                }
                Walk(root, 0);
            }
            Tree("Astronaut", p.astronaut.transform, 4);
            Tree("Rover", p.rover.transform, 3);
            foreach (var t in hud.environment.targets) if (t) Tree("Research target " + t.displayName, t.transform, 2);
            var world = GameObject.Find("Mars Research Base — 250m playable world");
            if (world) Tree("World", world.transform, 1);
            var exploration = GameObject.Find("02_LocalProvisionalExploration");
            if (exploration) Tree("Exploration", exploration.transform, 1);
            lines.Add("== Materials");
            foreach (var m in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None).SelectMany(r => r.sharedMaterials).Where(m => m).Distinct())
                lines.Add($"{m.name} | {m.shader.name} | base={(m.HasProperty("_BaseMap") && m.GetTexture("_BaseMap") ? m.GetTexture("_BaseMap").name + " tile=" + m.GetTextureScale("_BaseMap") : "-")} normal={(m.HasProperty("_BumpMap") && m.GetTexture("_BumpMap") ? m.GetTexture("_BumpMap").name : "-")} color={(m.HasProperty("_BaseColor") ? m.GetColor("_BaseColor").ToString() : "-")} smooth={(m.HasProperty("_Smoothness") ? m.GetFloat("_Smoothness") : -1)} metal={(m.HasProperty("_Metallic") ? m.GetFloat("_Metallic") : -1)} | {AssetDatabase.GetAssetPath(m)}");
            lines.Add("== Lights");
            foreach (var l in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None)) lines.Add($"{l.name} {l.type} int={l.intensity} color={l.color} shadows={l.shadows} rot={l.transform.eulerAngles}");
            lines.Add("== Cameras");
            foreach (var c in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None)) lines.Add($"{c.name} enabled={c.enabled} fov={c.fieldOfView} near={c.nearClipPlane} far={c.farClipPlane} pos={c.transform.position}");
            var urp = p.previewPipeline;
            lines.Add($"== URP {urp.name} hdr={urp.supportsHDR} msaa={urp.msaaSampleCount} shadowDist={urp.shadowDistance} cascades={urp.shadowCascadeCount} softShadows={urp.supportsSoftShadows} depthTex={urp.supportsCameraDepthTexture} opaqueTex={urp.supportsCameraOpaqueTexture} renderer={AssetDatabase.GetAssetPath(urp.rendererDataList[0])} features={string.Join(",", ((UniversalRendererData)urp.rendererDataList[0]).rendererFeatures.Select(f => f ? f.name : "null"))}");
            lines.Add("== Volume");
            foreach (var v in UnityEngine.Object.FindObjectsByType<Volume>(FindObjectsSortMode.None)) foreach (var c in v.sharedProfile.components) lines.Add($"{c.GetType().Name} active={c.active}");
            lines.Add($"== Render settings fog={RenderSettings.fog} {RenderSettings.fogMode} sky={RenderSettings.skybox?.name}/{RenderSettings.skybox?.shader.name} ambient={RenderSettings.ambientMode}");
            int tris = UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None).Where(f => f.sharedMesh && f.GetComponent<Renderer>() && f.GetComponent<Renderer>().enabled).Sum(f => (int)f.sharedMesh.GetIndexCount(0) / 3);
            lines.Add($"== Totals renderers={UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None).Length} approx_triangles_submesh0={tris}");
            Report("Scene_Inventory.txt", string.Join("\n", lines));
        }

        static string Hier(Transform t) => t.parent ? Hier(t.parent) + "/" + t.name : t.name;

        public static void Report(string file, string text)
        {
            Directory.CreateDirectory(Out);
            File.WriteAllText(System.IO.Path.Combine(Out, file), text + "\n");
        }
    }
}
