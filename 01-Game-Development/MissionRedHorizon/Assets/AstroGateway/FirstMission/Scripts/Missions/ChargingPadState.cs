using System.Collections.Generic;
using UnityEngine;

namespace AstroGateway.FirstMission.Missions
{
    // Charging pad: a soft cyan ring that pulses idle, brightens while charging and turns green when full.
    public class ChargingPadState : MonoBehaviour
    {
        public Transform pad;
        public float Charge { get; set; }     // 0..1 (SIMULATED mission charge)
        public bool Charging { get; set; }
        public System.Func<bool> reduced;
        LineRenderer ring, inner; Light lamp; Material ringMat;
        public void Init(Material lineTemplate)
        {
            ringMat = StructureFx.Glow(new Color(0, .86f, 1f, .8f), lineTemplate);
            ring = StructureFx.Ring(pad ? pad : transform, new Vector3(0, .12f, 0), 3.0f, .16f, ringMat);
            inner = StructureFx.Ring(pad ? pad : transform, new Vector3(0, .12f, 0), 1.9f, .08f, ringMat);
            var lg = new GameObject("Pad light"); lg.transform.SetParent(pad ? pad : transform, false); lg.transform.localPosition = new Vector3(0, 2.5f, 0);
            lamp = lg.AddComponent<Light>(); lamp.type = LightType.Point; lamp.range = 9; lamp.intensity = .9f; lamp.shadows = LightShadows.None; lamp.color = new Color(0, .86f, 1f);
        }
        void Update()
        {
            if (!ringMat) return;
            bool still = reduced != null && reduced();
            float pulse = still ? .8f : (Charging ? .65f + .35f * Mathf.Sin(Time.time * 9) : .55f + .25f * Mathf.Sin(Time.time * 2.2f));
            var c = Charge >= 1 ? new Color(.35f, 1f, .55f) : new Color(0, .86f, 1f);
            var cc = new Color(c.r, c.g, c.b, Mathf.Clamp01(pulse));
            if (ringMat.HasProperty("_BaseColor")) ringMat.SetColor("_BaseColor", cc); else ringMat.color = cc;
            if (lamp) { lamp.color = c; lamp.intensity = Charging ? 2.2f : .9f; }
            if (inner && !still) inner.transform.localRotation = Quaternion.Euler(90, 0, Time.time * (Charging ? 120 : 25));
        }
    }
}
