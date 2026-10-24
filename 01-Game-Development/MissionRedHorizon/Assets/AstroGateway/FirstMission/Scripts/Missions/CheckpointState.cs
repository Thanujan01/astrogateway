using System.Collections.Generic;
using UnityEngine;

namespace AstroGateway.FirstMission.Missions
{
    // Checkpoint gate: idle (dim), ACTIVE (orange pulsing light curtain between the posts), PASSED (green).
    public class CheckpointState : MonoBehaviour
    {
        public int order;
        public Transform postA, postB;
        public enum Mode { Idle, Active, Passed }
        public Mode State { get; private set; } = Mode.Idle;
        public System.Func<bool> reduced;
        LineRenderer curtain; Material mat; readonly List<Renderer> lamps = new List<Renderer>(); Material lampMat;
        public void Init(Material lineTemplate)
        {
            mat = StructureFx.Glow(new Color(1f, .55f, .15f, .5f), lineTemplate);
            lampMat = StructureFx.Glow(new Color(.6f, .6f, .6f, 1));
            var go = new GameObject("Light curtain"); go.transform.SetParent(transform, false);
            curtain = go.AddComponent<LineRenderer>(); curtain.useWorldSpace = true; curtain.positionCount = 2; curtain.sharedMaterial = mat;
            curtain.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; curtain.receiveShadows = false; curtain.widthMultiplier = 2.6f; curtain.alignment = LineAlignment.View;
            if (postA && postB) foreach (var p in new[] { postA, postB }) lamps.Add(StructureFx.Lamp(p, new Vector3(0, .55f, 0), .7f, lampMat, "Gate lamp").GetComponent<Renderer>());
            Set(Mode.Idle);
        }
        public void Set(Mode m) { State = m; if (curtain) curtain.enabled = m == Mode.Active; }
        void Update()
        {
            if (!postA || !postB || !mat) return;
            bool still = reduced != null && reduced();
            var a = postA.position; var b = postB.position; float y = Mathf.Min(a.y, b.y);
            curtain.SetPosition(0, new Vector3(a.x, y + 1.4f, a.z)); curtain.SetPosition(1, new Vector3(b.x, y + 1.4f, b.z));
            float pulse = still ? .45f : .3f + .25f * (.5f + .5f * Mathf.Sin(Time.time * 4));
            var mc = new Color(1f, .55f, .15f, pulse); if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", mc); else mat.color = mc;
            var lc = State == Mode.Passed ? new Color(.35f, 1f, .55f) : State == Mode.Active ? new Color(1f, .55f, .15f) : new Color(.45f, .5f, .55f);
            if (lampMat.HasProperty("_BaseColor")) lampMat.SetColor("_BaseColor", lc); else lampMat.color = lc;
        }
    }
}
