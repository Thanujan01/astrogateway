using System.Collections.Generic;
using UnityEngine;

// Mission structures (Mars_FirstMission_Missions only). The geometry is placed in the scene by
// FirstMissionBuilder.BuildMissions; these components mark each structure for the map registry and drive its
// runtime state visuals (lights, sparks, pad glow). Gameplay values here are game rules (SIMULATED), not telemetry.
namespace AstroGateway.FirstMission.Missions
{
    public enum StructureKind { Generator, ChargingStation, Tents, Dock, Checkpoint }

    public class MissionStructure : MonoBehaviour
    {
        public string id, displayName, icon = "pin", interaction = "";
        public StructureKind kind;
        public Transform anchor;            // navigation / arrival point (defaults to this transform)
        public float arrivalRadius = 6f;
        public string state = "";
        public Transform Anchor => anchor ? anchor : transform;
        public static readonly List<MissionStructure> All = new List<MissionStructure>();
        void OnEnable() { if (!All.Contains(this)) All.Add(this); }
        void OnDisable() { All.Remove(this); }
    }

    // Shared runtime materials for structure lights (copied from a transparent particle material when given so
    // they blend in URP; otherwise an unlit colour).
    public static class StructureFx
    {
        static Shader unlit;
        public static Material Glow(Color c, Material template = null)
        {
            Material m;
            if (template) m = new Material(template);
            else { unlit = unlit ? unlit : Shader.Find("Universal Render Pipeline/Unlit"); m = new Material(unlit); }
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c); else m.color = c;
            return m;
        }
        public static void SetColor(Renderer r, Color c)
        {
            if (!r) return;
            var m = r.sharedMaterial; if (!m) return;
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c); else m.color = c;
        }
        public static GameObject Lamp(Transform parent, Vector3 local, float size, Material mat, string name)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere); go.name = name;
            Object.Destroy(go.GetComponent<Collider>());
            go.transform.SetParent(parent, false); go.transform.localPosition = local; go.transform.localScale = Vector3.one * size;
            var r = go.GetComponent<Renderer>(); r.sharedMaterial = mat; r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; r.receiveShadows = false;
            return go;
        }
        public static LineRenderer Ring(Transform parent, Vector3 local, float radius, float width, Material mat, int seg = 48)
        {
            var go = new GameObject("Glow ring"); go.transform.SetParent(parent, false); go.transform.localPosition = local;
            var lr = go.AddComponent<LineRenderer>(); lr.useWorldSpace = false; lr.loop = true; lr.positionCount = seg; lr.widthMultiplier = width;
            lr.sharedMaterial = mat; lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; lr.receiveShadows = false; lr.alignment = LineAlignment.TransformZ;
            go.transform.localRotation = Quaternion.Euler(90, 0, 0);
            for (int i = 0; i < seg; i++) { float a = i / (float)seg * Mathf.PI * 2; lr.SetPosition(i, new Vector3(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius, 0)); }
            return lr;
        }
    }
}
