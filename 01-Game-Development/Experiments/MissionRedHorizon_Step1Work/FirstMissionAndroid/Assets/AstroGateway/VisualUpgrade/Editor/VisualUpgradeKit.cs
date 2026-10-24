using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace AstroGateway.VisualUpgrade.EditorTools
{
    // Small helpers shared by the Visual Upgrade builder steps: material assets under VisualUpgrade/Materials and
    // collider-free primitive parts. Every part is presentation only.
    public static class VisualUpgradeKit
    {
        const string F = VisualUpgradeBuilder.F;
        static readonly Dictionary<string, Material> cache = new Dictionary<string, Material>();
        static readonly Dictionary<PrimitiveType, Mesh> meshes = new Dictionary<PrimitiveType, Mesh>();

        public static void Reset() { cache.Clear(); }

        public static Material Mat(string name, Color color, float smooth = .3f, float metal = 0, Color? emission = null, float alpha = 1)
        {
            if (cache.TryGetValue(name, out var existing)) return existing;
            var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "VU " + name, enableInstancing = true };
            m.SetColor("_BaseColor", new Color(color.r, color.g, color.b, alpha));
            m.SetFloat("_Smoothness", smooth);
            m.SetFloat("_Metallic", metal);
            if (emission.HasValue)
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", emission.Value);
                m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive; // URP keeps _EMISSION only when flagged emissive
            }
            if (alpha < 1)
            {
                m.SetFloat("_Surface", 1); m.SetFloat("_Blend", 0);
                m.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha); m.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                m.SetFloat("_ZWrite", 0); m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                m.SetOverrideTag("RenderType", "Transparent"); m.renderQueue = (int)RenderQueue.Transparent;
            }
            string path = F + "/Materials/VU_" + name.Replace(" ", "_").Replace("/", "_") + ".mat";
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(m, path);
            m = AssetDatabase.LoadAssetAtPath<Material>(path);
            Keywords(m);
            cache[name] = m;
            return m;
        }

        // URP derives keywords (_EMISSION, transparency, alpha clip) from properties in its material inspector;
        // materials made from script must run the same setup or the keywords are dropped on import.
        public static void Keywords(Material m)
        {
            if (m.shader.name == "Universal Render Pipeline/Lit") BaseShaderGUI.SetMaterialKeywords(m, UnityEditor.Rendering.Universal.ShaderGUI.LitGUI.SetMaterialKeywords);
            else if (m.shader.name.Contains("Particles")) BaseShaderGUI.SetMaterialKeywords(m, null, UnityEditor.Rendering.Universal.ShaderGUI.ParticleGUI.SetMaterialKeywords);
            EditorUtility.SetDirty(m);
        }

        public static Mesh Primitive(PrimitiveType type)
        {
            if (meshes.TryGetValue(type, out var m) && m) return m;
            var g = GameObject.CreatePrimitive(type);
            m = g.GetComponent<MeshFilter>().sharedMesh;
            Object.DestroyImmediate(g);
            return meshes[type] = m;
        }

        public static GameObject Part(string name, Transform parent, PrimitiveType type, Vector3 pos, Vector3 euler, Vector3 scale, Material mat, bool shadows = true)
            => Part(name, parent, Primitive(type), pos, euler, scale, mat, shadows);

        public static GameObject Part(string name, Transform parent, Mesh mesh, Vector3 pos, Vector3 euler, Vector3 scale, Material mat, bool shadows = true)
        {
            var g = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
            g.transform.SetParent(parent, false);
            g.transform.localPosition = pos; g.transform.localEulerAngles = euler; g.transform.localScale = scale;
            g.GetComponent<MeshFilter>().sharedMesh = mesh;
            var r = g.GetComponent<MeshRenderer>();
            r.sharedMaterial = mat;
            r.shadowCastingMode = shadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            return g;
        }

        public static Transform Group(string name, Transform parent, Vector3 pos = default, Vector3 euler = default)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false); t.localPosition = pos; t.localEulerAngles = euler;
            return t;
        }

        public static Mesh SaveMesh(Mesh mesh, string name)
        {
            string path = F + "/Meshes/" + name + ".asset";
            AssetDatabase.DeleteAsset(path);
            mesh.name = name;
            AssetDatabase.CreateAsset(mesh, path);
            return AssetDatabase.LoadAssetAtPath<Mesh>(path);
        }

        // Parabolic dish (open bowl) for antennas.
        public static Mesh Dish(int rings = 8, int segments = 24, float depth = .28f)
        {
            var v = new List<Vector3>(); var t = new List<int>();
            v.Add(Vector3.zero);
            for (int r = 1; r <= rings; r++)
            {
                float rad = r / (float)rings * .5f;
                for (int s = 0; s < segments; s++)
                {
                    float a = s / (float)segments * Mathf.PI * 2;
                    v.Add(new Vector3(Mathf.Cos(a) * rad, rad * rad / .25f * depth, Mathf.Sin(a) * rad));
                }
            }
            for (int s = 0; s < segments; s++) { t.Add(0); t.Add(1 + (s + 1) % segments); t.Add(1 + s); }
            for (int r = 1; r < rings; r++)
                for (int s = 0; s < segments; s++)
                {
                    int a = 1 + (r - 1) * segments + s, b = 1 + (r - 1) * segments + (s + 1) % segments, c = a + segments, d = b + segments;
                    t.Add(a); t.Add(b); t.Add(c); t.Add(b); t.Add(d); t.Add(c);
                }
            // Back faces so the dish reads from behind. Batch 3 fix: the back faces get their own vertices with
            // flipped normals. Re-using the front vertices made RecalculateNormals average front and back normals to
            // ~zero, which rendered as black/white stripes and shadow acne on every dish.
            var front = new Mesh { vertices = v.ToArray(), triangles = t.ToArray() };
            front.RecalculateNormals();
            var fn = front.normals;
            int vc = v.Count, n = t.Count;
            var verts = new List<Vector3>(v); verts.AddRange(v);
            var norms = new List<Vector3>(fn); foreach (var x in fn) norms.Add(-x);
            for (int i = 0; i < n; i += 3) { t.Add(t[i] + vc); t.Add(t[i + 2] + vc); t.Add(t[i + 1] + vc); }
            Object.DestroyImmediate(front);
            var m = new Mesh { name = "Parabolic dish (two-sided)" };
            m.SetVertices(verts); m.SetNormals(norms); m.SetTriangles(t, 0);
            m.RecalculateBounds();
            return m;
        }

        // Simple procedural emblem texture (no text, no real-world insignia): orange ring, cyan core, white chevron.
        public static Texture2D Emblem()
        {
            const int s = 128;
            var tex = new Texture2D(s, s, TextureFormat.RGBA32, true);
            var px = new Color[s * s];
            for (int y = 0; y < s; y++)
                for (int x = 0; x < s; x++)
                {
                    float dx = (x + .5f) / s * 2 - 1, dy = (y + .5f) / s * 2 - 1, d = Mathf.Sqrt(dx * dx + dy * dy);
                    Color c = new Color(0, 0, 0, 0);
                    if (d < .98f) c = new Color(.06f, .09f, .13f, 1);
                    if (d > .80f && d < .96f) c = new Color(1f, .45f, .12f, 1);
                    if (d < .42f) c = new Color(.25f, .85f, .95f, 1);
                    if (Mathf.Abs(dx) < .55f && Mathf.Abs(dy - (.25f - Mathf.Abs(dx) * .6f)) < .07f) c = new Color(.95f, .95f, .92f, 1);
                    px[y * s + x] = c;
                }
            tex.SetPixels(px); tex.Apply();
            string path = F + "/Textures/VisualUpgradeEmblem.png";
            System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
            var ti = (TextureImporter)AssetImporter.GetAtPath(path); ti.alphaIsTransparency = true; ti.wrapMode = TextureWrapMode.Clamp; ti.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }
}
