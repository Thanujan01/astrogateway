using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;

namespace AstroGateway.VisualUpgrade.EditorTools
{
    // Visual system 1 — Mars environment, lighting and post-processing for Mars_VisualUpgrade_v1.
    // Procedural, clearly temporary art built only from textures/meshes already in the project. Never touches the
    // playable terrain collider, gameplay objects or shared assets: every material/asset is created under
    // Assets/AstroGateway/VisualUpgrade, and new geometry has no colliders.
    public static class VisualUpgradeEnvironment
    {
        const string F = VisualUpgradeBuilder.F;
        const string Tex = "Assets/AstroGateway/MarsEnvironment/Textures/";
        public static readonly List<string> Notes = new List<string>();

        public static void Apply(AstroGatewayHUDController hud, ProductionModeCoordinator modes, GameObject adapters)
        {
            Notes.Clear();
            rock07 = rock09 = null;
            Directory.CreateDirectory(F + "/Materials"); Directory.CreateDirectory(F + "/Meshes"); Directory.CreateDirectory(F + "/Textures");
            var world = GameObject.Find("Mars Research Base — 250m playable world").transform;
            var playable = GameObject.Find("Traversable terrain");
            var playableCollider = playable.GetComponent<MeshCollider>();

            var root = new GameObject("Visual Upgrade v1 — environment (temporary art, no colliders)").transform;
            root.SetParent(world, false);

            // Ground: one material for the playable mesh (visual only; collider untouched) and the far terrain.
            var ground = TerrainMaterial();
            playable.GetComponent<Renderer>().sharedMaterial = ground;
            var plain = GameObject.Find("Distant dust plain");
            if (plain) plain.GetComponent<Renderer>().enabled = false;
            var far = FarTerrain(playableCollider, ground, root);
            Scatter(playableCollider, root);
            RockMaterials();

            // Sky, sun presets, pipeline, renderer (SSAO), post-processing, reflections, haze.
            var env = hud.environment;
            var sky = Save(new Material(Shader.Find("AstroGateway/VisualUpgrade/MarsSky")) { name = "VisualUpgradeMarsSky" }, "Materials/VisualUpgradeMarsSky.mat");
            env.sky = sky; RenderSettings.skybox = sky;
            Presets(env.lighting);
            env.sun.shadowStrength = .92f;
            env.sun.shadowNormalBias = .25f;
            var urp = hud.game.prototype.previewPipeline;
            Pipeline(urp);
            var volume = Object.FindAnyObjectByType<Volume>();
            PostProcessing(volume.sharedProfile);
            var probe = Probe(root);
            var haze = Haze(root);

            var atmosphere = adapters.AddComponent<VisualUpgradeAtmosphere>();
            atmosphere.hud = hud; atmosphere.modes = modes; atmosphere.volume = volume; atmosphere.haze = haze; atmosphere.probe = probe; atmosphere.farTerrain = far.transform;
            Notes.Add("Environment applied: terrain shader, far terrain, scatter, sky, presets, URP copy, SSAO, post, probe, haze");
        }

        // ---------------- materials ----------------
        static T Save<T>(T asset, string relative) where T : Object
        {
            string path = F + "/" + relative;
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(asset, path);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        static Texture2D T(string path) => AssetDatabase.LoadAssetAtPath<Texture2D>(path) ?? throw new System.Exception("Missing texture " + path);

        static Material TerrainMaterial()
        {
            var m = new Material(Shader.Find("AstroGateway/VisualUpgrade/MarsTerrain")) { name = "VisualUpgrade Mars ground", enableInstancing = true };
            m.SetTexture("_BaseMap", T(Tex + "sandy_gravel_Diffuse.png"));
            m.SetTexture("_BumpMap", T(Tex + "sandy_gravel_nor_gl.png"));
            m.SetTexture("_RockMap", T(Tex + "rock_07_Diffuse.png"));
            m.SetTexture("_RockNormal", T(Tex + "rock_07_nor_gl.png"));
            m.SetTexture("_MacroMap", T("Assets/AstroGateway/Environment/Materials/Regolith_Macro_Pathwear_1024.png"));
            m.SetColor("_Tint", new Color(.80f, .55f, .38f)); m.SetColor("_DarkTint", new Color(.58f, .38f, .26f)); m.SetColor("_RockTint", new Color(.55f, .42f, .33f));
            m.SetFloat("_TileNear", 1.8f); m.SetFloat("_NormalStrength", 1.35f); m.SetFloat("_Smoothness", .14f);
            return Save(m, "Materials/VisualUpgradeMarsGround.mat");
        }

        static Material rock07, rock09;
        static void RockMaterials()
        {
            // Copies of the inherited scanned-rock materials with Mars-plausible roughness (originals untouched).
            foreach (var r in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
            {
                if (!r.transform.parent || !(r.transform.parent.name.StartsWith("Approved rock_") || r.name.StartsWith("Distant rocky massif"))) continue;
                r.sharedMaterials = r.sharedMaterials.Select(m => m && m.name == "Inherited_0" ? (rock07 ?? (rock07 = RockCopy(m, "Rock07"))) : m && m.name == "Inherited_1" ? (rock09 ?? (rock09 = RockCopy(m, "Rock09"))) : m).ToArray();
            }
            foreach (var r in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None).Where(r => r.name.StartsWith("Distant rocky massif")))
                r.sharedMaterials = r.sharedMaterials.Select(m => m && m.name == "Inherited_1" ? (rock09 ?? (rock09 = RockCopy(m, "Rock09"))) : m).ToArray();
        }

        static Material RockCopy(Material original, string name)
        {
            var m = new Material(original) { name = "VisualUpgrade " + name, enableInstancing = true };
            m.SetColor("_BaseColor", name == "Rock07" ? new Color(.66f, .47f, .35f) : new Color(.58f, .43f, .32f));
            m.SetFloat("_Smoothness", .55f); // metallic-smoothness map is the inverted roughness; .55 keeps rocks matte
            return Save(m, "Materials/VisualUpgrade" + name + ".mat");
        }

        // ---------------- far terrain: dunes and craters outside the playable square ----------------
        static readonly Vector4[] Craters = { new Vector4(300, -250, 85, 13), new Vector4(-380, -140, 60, 9), new Vector4(-210, -360, 45, 7), new Vector4(420, 120, 70, 10), new Vector4(-460, 300, 110, 15) };

        static GameObject FarTerrain(MeshCollider playable, Material mat, Transform parent)
        {
            const int n = 201; const float size = 1200f;
            float step = size / (n - 1);
            var verts = new Vector3[n * n];
            for (int z = 0; z < n; z++)
                for (int x = 0; x < n; x++)
                {
                    var p = new Vector2(-size / 2 + x * step, -size / 2 + z * step);
                    verts[z * n + x] = new Vector3(p.x, Height(p, playable), p.y);
                }
            var tris = new int[(n - 1) * (n - 1) * 6];
            int t = 0;
            for (int z = 0; z < n - 1; z++)
                for (int x = 0; x < n - 1; x++)
                {
                    int i = z * n + x;
                    tris[t++] = i; tris[t++] = i + n; tris[t++] = i + 1;
                    tris[t++] = i + 1; tris[t++] = i + n; tris[t++] = i + n + 1;
                }
            var mesh = new Mesh { name = "VisualUpgradeFarTerrain", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32, vertices = verts, triangles = tris };
            mesh.RecalculateNormals(); mesh.RecalculateBounds(); mesh.RecalculateTangents();
            mesh = Save(mesh, "Meshes/VisualUpgradeFarTerrain.asset");
            var g = new GameObject("Far terrain — dunes and craters (visual only)", typeof(MeshFilter), typeof(MeshRenderer));
            g.transform.SetParent(parent, false);
            g.GetComponent<MeshFilter>().sharedMesh = mesh;
            var r = g.GetComponent<MeshRenderer>(); r.sharedMaterial = mat; r.shadowCastingMode = ShadowCastingMode.On;
            GameObjectUtility.SetStaticEditorFlags(g, StaticEditorFlags.BatchingStatic);
            Notes.Add($"Far terrain {n}x{n} vertices, {step:F1} m spacing, {Craters.Length} craters");
            return g;
        }

        static float Sample(MeshCollider c, Vector2 p)
        {
            var q = new Vector2(Mathf.Clamp(p.x, -124.5f, 124.5f), Mathf.Clamp(p.y, -124.5f, 124.5f));
            return c.Raycast(new Ray(new Vector3(q.x, 200, q.y), Vector3.down), out var hit, 400) ? hit.point.y : 0;
        }

        static float Height(Vector2 p, MeshCollider playable)
        {
            float edge = Mathf.Max(Mathf.Abs(p.x), Mathf.Abs(p.y));
            float baseH = Sample(playable, p);
            if (edge < 112) return baseH - .8f;               // hidden under the playable terrain
            if (edge <= 127) return baseH - .04f;             // meets the playable edge
            float t = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(127, 330, edge));
            float swell = (Mathf.PerlinNoise(p.x * .0021f + 3.1f, p.y * .0021f + 7.4f) - .35f) * 26 * t;
            float duneAmp = (2.5f + 6 * Mathf.PerlinNoise(p.x * .006f + 1.3f, p.y * .006f + 2.9f)) * t;
            float phase = Vector2.Dot(p, new Vector2(.82f, .57f)) / 42f + Mathf.PerlinNoise(p.x * .004f, p.y * .004f) * 5;
            float s = phase - Mathf.Floor(phase);
            float profile = s < .72f ? s / .72f : (1 - s) / .28f;   // gentle windward, steep lee face
            float dunes = Mathf.Pow(Mathf.SmoothStep(0, 1, profile), 1.4f) * duneAmp;
            float crater = 0;
            foreach (var c in Craters)
            {
                float d = Vector2.Distance(p, new Vector2(c.x, c.y)), r = c.z, depth = c.w;
                if (d < r) crater -= depth * (1 - (d / r) * (d / r));
                crater += depth * .45f * Mathf.Exp(-Mathf.Pow((d - r) / (.2f * r), 2));
            }
            return Mathf.Lerp(baseH - .04f, baseH * .35f, t) + swell + dunes + crater * t;
        }

        // ---------------- pebble scatter (combined per 40 m chunk, no colliders) ----------------
        static void Scatter(MeshCollider playable, Transform parent)
        {
            var meshes = new[] { AssetDatabase.LoadAssetAtPath<Mesh>("Assets/AstroGateway/MarsEnvironment/Models/rock_07_LOD2.asset"), AssetDatabase.LoadAssetAtPath<Mesh>("Assets/AstroGateway/MarsEnvironment/Models/rock_09_LOD2.asset") };
            var mats = new[] { rock07 ??= RockCopy(AssetDatabase.LoadAssetAtPath<Material>("Assets/AstroGateway/ProductionVisual/Materials/Inherited_0.mat"), "Rock07"), rock09 ??= RockCopy(AssetDatabase.LoadAssetAtPath<Material>("Assets/AstroGateway/ProductionVisual/Materials/Inherited_1.mat"), "Rock09") };
            float meshSize = meshes.Max(m => m.bounds.size.magnitude);
            var rng = new System.Random(4242);
            var chunks = new Dictionary<Vector2Int, List<CombineInstance>[]>();
            int placed = 0, skipped = 0;
            Physics.SyncTransforms();
            for (int i = 0; i < 1400 && placed < 760; i++)
            {
                var p = new Vector2((float)rng.NextDouble() * 236 - 118, (float)rng.NextDouble() * 236 - 118);
                // clusters: bias some pebbles toward random existing positions for natural grouping
                if (rng.NextDouble() < .45 && placed > 10) { p += new Vector2((float)rng.NextDouble() * 6 - 3, (float)rng.NextDouble() * 6 - 3); }
                if (!playable.Raycast(new Ray(new Vector3(p.x, 200, p.y), Vector3.down), out var hit, 400)) { skipped++; continue; }
                if (Physics.OverlapSphere(hit.point + Vector3.up * .5f, 2.2f, ~0, QueryTriggerInteraction.Ignore).Any(c => c != playable && !(c is TerrainCollider))) { skipped++; continue; }
                float size = Mathf.Lerp(.07f, .38f, Mathf.Pow((float)rng.NextDouble(), 2.2f));
                int type = rng.Next(2);
                float scale = size / (meshSize * .5f);
                var rot = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler((float)rng.NextDouble() * 20 - 10, (float)rng.NextDouble() * 360, (float)rng.NextDouble() * 20 - 10);
                var pos = hit.point - hit.normal * size * .25f;
                var key = new Vector2Int(Mathf.FloorToInt((p.x + 120) / 40), Mathf.FloorToInt((p.y + 120) / 40));
                if (!chunks.TryGetValue(key, out var lists)) chunks[key] = lists = new[] { new List<CombineInstance>(), new List<CombineInstance>() };
                lists[type].Add(new CombineInstance { mesh = meshes[type], transform = Matrix4x4.TRS(pos, rot, Vector3.one * scale) });
                placed++;
            }
            var holder = new GameObject("Surface pebble scatter (combined, no colliders)").transform;
            holder.SetParent(parent, false);
            foreach (var kv in chunks)
            {
                var parts = new List<CombineInstance>(); var used = new List<Material>();
                for (int k = 0; k < 2; k++)
                {
                    if (kv.Value[k].Count == 0) continue;
                    var sub = new Mesh { indexFormat = IndexFormat.UInt32 };
                    sub.CombineMeshes(kv.Value[k].ToArray(), true, true);
                    parts.Add(new CombineInstance { mesh = sub, transform = Matrix4x4.identity });
                    used.Add(mats[k]);
                }
                var mesh = new Mesh { name = $"VisualUpgradeScatter_{kv.Key.x}_{kv.Key.y}", indexFormat = IndexFormat.UInt32 };
                mesh.CombineMeshes(parts.ToArray(), false, false);
                mesh.RecalculateBounds();
                foreach (var part in parts) Object.DestroyImmediate(part.mesh);
                mesh = Save(mesh, $"Meshes/{mesh.name}.asset");
                var g = new GameObject(mesh.name, typeof(MeshFilter), typeof(MeshRenderer));
                g.transform.SetParent(holder, false);
                g.GetComponent<MeshFilter>().sharedMesh = mesh;
                var r = g.GetComponent<MeshRenderer>(); r.sharedMaterials = used.ToArray(); r.shadowCastingMode = ShadowCastingMode.On;
                var lod = g.AddComponent<LODGroup>();
                lod.SetLODs(new[] { new LOD(.06f, new Renderer[] { r }) });
                lod.RecalculateBounds();
            }
            Notes.Add($"Scatter: {placed} pebbles in {chunks.Count} combined chunks (skipped {skipped} near structures)");
        }

        // ---------------- lighting presets ----------------
        static void Presets(AstroGateway.MarsEnvironment.EnvironmentPreset[] presets)
        {
            foreach (var p in presets)
            {
                switch (p.label)
                {
                    case "Morning": p.sunEuler = new Vector3(22, -118, 0); p.sunColor = new Color(1, .84f, .70f); p.intensity = 1.35f; p.sky = new Color(.40f, .30f, .23f); p.horizon = new Color(.78f, .58f, .42f); p.fogEnd = 360; break;
                    case "Midday": p.sunEuler = new Vector3(58, -35, 0); p.sunColor = new Color(1, .93f, .84f); p.intensity = 1.55f; p.sky = new Color(.46f, .34f, .25f); p.horizon = new Color(.82f, .64f, .47f); p.fogEnd = 420; break;
                    case "Sunset": p.sunEuler = new Vector3(24, -58, 0); p.sunColor = new Color(1, .80f, .60f); p.intensity = 1.45f; p.sky = new Color(.34f, .25f, .20f); p.horizon = new Color(.74f, .52f, .37f); p.fogEnd = 340; break;
                    default: p.sunEuler = new Vector3(32, -45, 0); p.sunColor = new Color(.95f, .76f, .58f); p.intensity = .95f; p.sky = new Color(.48f, .36f, .27f); p.horizon = new Color(.70f, .52f, .38f); p.fogEnd = 220; break;
                }
                EditorUtility.SetDirty(p);
            }
        }

        // ---------------- pipeline and renderer ----------------
        static void Pipeline(UniversalRenderPipelineAsset urp)
        {
            urp.supportsCameraDepthTexture = true;
            urp.shadowCascadeCount = 2;
            urp.supportsHDR = true;
            var so = new SerializedObject(urp);
            so.FindProperty("m_AdditionalLightsRenderingMode").intValue = 1; // per pixel (rover headlights)
            so.FindProperty("m_AdditionalLightsPerObjectLimit").intValue = 4;
            so.FindProperty("m_SoftShadowsSupported").boolValue = true;
            so.ApplyModifiedPropertiesWithoutUndo();
            var source = (UniversalRendererData)urp.rendererDataList[0];
            var data = Save(Object.Instantiate(source), "Settings/VisualUpgrade_Renderer.asset");
            var ssao = ScriptableObject.CreateInstance<ScreenSpaceAmbientOcclusion>();
            ssao.name = "Visual Upgrade SSAO";
            AssetDatabase.AddObjectToAsset(ssao, data);
            var s = new SerializedObject(ssao);
            s.FindProperty("m_Settings.Intensity").floatValue = 1.1f;
            s.FindProperty("m_Settings.Radius").floatValue = .45f;
            s.FindProperty("m_Settings.DirectLightingStrength").floatValue = .2f;
            s.FindProperty("m_Settings.Downsample").boolValue = true;
            s.FindProperty("m_Settings.Falloff").floatValue = 60f;
            s.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(ssao, out string _, out long localId);
            var d = new SerializedObject(data);
            var list = d.FindProperty("m_RendererFeatures"); list.arraySize++; list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = ssao;
            var map = d.FindProperty("m_RendererFeatureMap"); map.arraySize++; map.GetArrayElementAtIndex(map.arraySize - 1).longValue = localId;
            d.ApplyModifiedPropertiesWithoutUndo();
            var u = new SerializedObject(urp);
            var rd = u.FindProperty("m_RendererDataList"); rd.GetArrayElementAtIndex(0).objectReferenceValue = data;
            u.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(data); EditorUtility.SetDirty(urp);
            Notes.Add("URP copy: depth texture, soft shadows, HDR (High/Ultra), per-pixel lights; renderer copy with half-res SSAO");
        }

        // ---------------- post-processing ----------------
        static TC Get<TC>(VolumeProfile profile) where TC : VolumeComponent
        {
            if (profile.TryGet<TC>(out var c)) return c;
            c = profile.Add<TC>(); c.name = typeof(TC).Name; AssetDatabase.AddObjectToAsset(c, profile); return c;
        }

        static void PostProcessing(VolumeProfile profile)
        {
            var tone = Get<Tonemapping>(profile); tone.mode.Override(TonemappingMode.ACES);
            var color = Get<ColorAdjustments>(profile);
            color.postExposure.Override(.35f); color.contrast.Override(14); color.saturation.Override(-4); color.colorFilter.Override(new Color(1, .975f, .94f));
            var white = Get<WhiteBalance>(profile); white.temperature.Override(5); white.tint.Override(2);
            var bloom = Get<Bloom>(profile); bloom.threshold.Override(1.05f); bloom.intensity.Override(.35f); bloom.scatter.Override(.62f); bloom.tint.Override(new Color(1, .86f, .72f));
            var vignette = Get<Vignette>(profile); vignette.intensity.Override(.24f); vignette.smoothness.Override(.42f); vignette.color.Override(new Color(.08f, .04f, .02f));
            var smh = Get<ShadowsMidtonesHighlights>(profile); smh.shadows.Override(new Vector4(.96f, .98f, 1.04f, 0)); smh.highlights.Override(new Vector4(1.03f, 1, .95f, 0));
            EditorUtility.SetDirty(profile);
            Notes.Add("Post: ACES, exposure/contrast grade, white balance, bloom, vignette, split toning; DoF for cinematic only");
        }

        // ---------------- reflections and haze ----------------
        static ReflectionProbe Probe(Transform parent)
        {
            var g = new GameObject("Base reflection probe (realtime, refreshed on start)");
            g.transform.SetParent(parent, false);
            g.transform.position = new Vector3(-35, 6, -40);
            var p = g.AddComponent<ReflectionProbe>();
            p.mode = ReflectionProbeMode.Realtime;
            p.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
            p.timeSlicingMode = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
            p.size = new Vector3(260, 60, 260);
            p.resolution = 128;
            p.importance = 1;
            p.intensity = .9f;
            return p;
        }

        static ParticleSystem Haze(Transform parent)
        {
            var texture = SoftTexture();
            var mat = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit")) { name = "VisualUpgrade distant dust haze" };
            mat.SetFloat("_Surface", 1); mat.SetFloat("_Blend", 0);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha); mat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha); mat.SetFloat("_ZWrite", 0);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)RenderQueue.Transparent;
            mat.SetTexture("_BaseMap", texture);
            mat.SetColor("_BaseColor", new Color(.86f, .64f, .46f, .09f));
            mat = Save(mat, "Materials/VisualUpgradeHaze.mat");
            VisualUpgradeKit.Keywords(mat);
            var g = new GameObject("Distant dust haze (follows player, presentation only)", typeof(ParticleSystem));
            g.transform.SetParent(parent, false);
            var ps = g.GetComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main; main.loop = true; main.startLifetime = 24; main.startSpeed = .6f; main.startSize = new ParticleSystem.MinMaxCurve(35, 70);
            main.maxParticles = 48; main.simulationSpace = ParticleSystemSimulationSpace.World; main.startColor = new Color(1, 1, 1, 1); main.playOnAwake = true;
            main.startRotation = new ParticleSystem.MinMaxCurve(0, Mathf.PI * 2);
            var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Box; shape.scale = new Vector3(320, 14, 320); shape.position = new Vector3(0, 9, 0);
            var emission = ps.emission; emission.rateOverTime = 1.5f;
            var col = ps.colorOverLifetime; col.enabled = true;
            var grad = new Gradient(); grad.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) }, new[] { new GradientAlphaKey(0, 0), new GradientAlphaKey(1, .3f), new GradientAlphaKey(1, .7f), new GradientAlphaKey(0, 1) });
            col.color = grad;
            var vel = ps.velocityOverLifetime; vel.enabled = true; vel.space = ParticleSystemSimulationSpace.World; vel.x = new ParticleSystem.MinMaxCurve(.8f, 1.6f); vel.y = new ParticleSystem.MinMaxCurve(0, 0); vel.z = new ParticleSystem.MinMaxCurve(.3f, .9f);
            var r = g.GetComponent<ParticleSystemRenderer>(); r.sharedMaterial = mat; r.renderMode = ParticleSystemRenderMode.Billboard; r.sortMode = ParticleSystemSortMode.Distance;
            r.shadowCastingMode = ShadowCastingMode.Off; r.receiveShadows = false; r.maxParticleSize = 4;
            return ps;
        }

        static Texture2D SoftTexture()
        {
            const int s = 128;
            var t = new Texture2D(s, s, TextureFormat.RGBA32, false);
            var px = new Color[s * s];
            for (int y = 0; y < s; y++)
                for (int x = 0; x < s; x++)
                {
                    float dx = (x + .5f) / s * 2 - 1, dy = (y + .5f) / s * 2 - 1;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float n = Mathf.PerlinNoise(x * .06f, y * .06f) * .5f + Mathf.PerlinNoise(x * .15f + 9, y * .15f + 3) * .5f;
                    float a = Mathf.Clamp01(1 - d) ; a = a * a * (3 - 2 * a) * Mathf.Lerp(.55f, 1f, n);
                    px[y * s + x] = new Color(1, 1, 1, a);
                }
            t.SetPixels(px); t.Apply();
            string path = F + "/Textures/VisualUpgradeSoftDust.png";
            File.WriteAllBytes(path, t.EncodeToPNG());
            Object.DestroyImmediate(t);
            AssetDatabase.ImportAsset(path);
            var ti = (TextureImporter)AssetImporter.GetAtPath(path);
            ti.alphaIsTransparency = true; ti.wrapMode = TextureWrapMode.Clamp; ti.mipmapEnabled = true; ti.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }
}
