using System;
using System.Collections.Generic;
using UnityEngine;

// Small line icons drawn once at runtime into 64x64 alpha textures (no external art, no icon font, works on
// every player). White glyphs; the UI tints them with the design-token colours. Also provides a rounded-panel
// sprite with a 1-px inner border for the glass panels.
namespace AstroGateway.FirstMission.HUD
{
    public static class HudIcons
    {
        const int S = 64;
        static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

        public static Sprite Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            if (cache.TryGetValue(id, out var sp) && sp) return sp;
            var px = new float[S * S];
            Draw(id, px);
            var tex = new Texture2D(S, S, TextureFormat.RGBA32, false) { name = "HudIcon_" + id, filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
            var cols = new Color32[S * S];
            for (int i = 0; i < cols.Length; i++) cols[i] = new Color32(255, 255, 255, (byte)(Mathf.Clamp01(px[i]) * 255));
            tex.SetPixels32(cols); tex.Apply(false, true);
            sp = Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(.5f, .5f), 100);
            cache[id] = sp;
            return sp;
        }

        // Rounded rectangle (radius r px of 64) with optional border-only rendering, 9-slice ready.
        public static Sprite Panel(int radius = 14, bool borderOnly = false, float border = 2f)
        {
            string key = $"panel_{radius}_{borderOnly}_{border}";
            if (cache.TryGetValue(key, out var sp) && sp) return sp;
            var tex = new Texture2D(S, S, TextureFormat.RGBA32, false) { filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
            var cols = new Color32[S * S];
            for (int y = 0; y < S; y++)
                for (int x = 0; x < S; x++)
                {
                    float d = RoundRectDist(x + .5f, y + .5f, S / 2f - .5f, S / 2f - .5f, radius);
                    float a = Mathf.Clamp01(.5f - d);
                    if (borderOnly) a = Mathf.Min(a, Mathf.Clamp01(d + border + .5f));
                    cols[y * S + x] = new Color32(255, 255, 255, (byte)(a * 255));
                }
            tex.SetPixels32(cols); tex.Apply(false, true);
            sp = Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect, new Vector4(radius + 2, radius + 2, radius + 2, radius + 2));
            cache[key] = sp;
            return sp;
        }

        static float RoundRectDist(float x, float y, float hx, float hy, float r)
        {
            float qx = Mathf.Abs(x - S / 2f) - (hx - r), qy = Mathf.Abs(y - S / 2f) - (hy - r);
            return new Vector2(Mathf.Max(qx, 0), Mathf.Max(qy, 0)).magnitude + Mathf.Min(Mathf.Max(qx, qy), 0) - r;
        }

        // ---- primitive painters (anti-aliased by distance) ----
        static void Plot(float[] px, Func<float, float, float> dist, float width)
        {
            for (int y = 0; y < S; y++)
                for (int x = 0; x < S; x++)
                {
                    float d = dist(x + .5f, y + .5f);
                    float a = Mathf.Clamp01(width / 2f + .5f - Mathf.Abs(d));
                    int i = y * S + x; px[i] = Mathf.Max(px[i], a);
                }
        }
        static void Fill(float[] px, Func<float, float, float> inside)
        {
            for (int y = 0; y < S; y++)
                for (int x = 0; x < S; x++) { float a = Mathf.Clamp01(.5f - inside(x + .5f, y + .5f)); int i = y * S + x; px[i] = Mathf.Max(px[i], a); }
        }
        static float Seg(float x, float y, float ax, float ay, float bx, float by)
        {
            var p = new Vector2(x - ax, y - ay); var b = new Vector2(bx - ax, by - ay);
            float t = Mathf.Clamp01(Vector2.Dot(p, b) / Mathf.Max(.0001f, b.sqrMagnitude));
            return (p - b * t).magnitude;
        }
        static void Line(float[] px, float ax, float ay, float bx, float by, float w = 4) => Plot(px, (x, y) => Seg(x, y, ax, ay, bx, by), w);
        static void Ring(float[] px, float cx, float cy, float r, float w = 4) => Plot(px, (x, y) => new Vector2(x - cx, y - cy).magnitude - r, w);
        static void Disc(float[] px, float cx, float cy, float r) => Fill(px, (x, y) => new Vector2(x - cx, y - cy).magnitude - r);
        static void Box(float[] px, float x0, float y0, float x1, float y1, float w = 4)
        { Line(px, x0, y0, x1, y0, w); Line(px, x1, y0, x1, y1, w); Line(px, x1, y1, x0, y1, w); Line(px, x0, y1, x0, y0, w); }
        static void Solid(float[] px, float x0, float y0, float x1, float y1) => Fill(px, (x, y) => Mathf.Max(Mathf.Max(x0 - x, x - x1), Mathf.Max(y0 - y, y - y1)));
        static void Arc(float[] px, float cx, float cy, float r, float a0, float a1, float w = 4) =>
            Plot(px, (x, y) => { float ang = Mathf.Atan2(y - cy, x - cx) * Mathf.Rad2Deg; ang = Mathf.Repeat(ang, 360); bool inArc = a0 <= a1 ? ang >= a0 && ang <= a1 : ang >= a0 || ang <= a1; return inArc ? new Vector2(x - cx, y - cy).magnitude - r : 99; }, w);

        static void Draw(string id, float[] px)
        {
            switch (id)
            {
                case "battery": Box(px, 10, 18, 50, 46); Solid(px, 50, 26, 56, 38); Solid(px, 16, 24, 34, 40); break;
                case "thermo": Line(px, 32, 52, 32, 26, 8); Ring(px, 32, 18, 9, 4); Disc(px, 32, 18, 5); Line(px, 40, 48, 46, 48, 3); Line(px, 40, 40, 46, 40, 3); break;
                case "signal": Solid(px, 10, 12, 16, 22); Solid(px, 22, 12, 28, 32); Solid(px, 34, 12, 40, 42); Solid(px, 46, 12, 52, 52); break;
                case "nosignal": Solid(px, 10, 12, 16, 22); Solid(px, 22, 12, 28, 32); Line(px, 36, 30, 54, 48, 4); Line(px, 54, 30, 36, 48, 4); break;
                case "database": Box(px, 14, 12, 50, 52, 4); Line(px, 14, 26, 50, 26, 3); Line(px, 14, 39, 50, 39, 3); break;
                case "gauge": Arc(px, 32, 26, 22, 0, 180, 5); Line(px, 32, 26, 46, 40, 4); Disc(px, 32, 26, 4); break;
                case "wheel": Ring(px, 32, 32, 22, 5); Disc(px, 32, 32, 5); Line(px, 32, 32, 32, 10, 4); Line(px, 32, 32, 13, 43, 4); Line(px, 32, 32, 51, 43, 4); break;
                case "mode": Ring(px, 32, 32, 12, 5); for (int i = 0; i < 8; i++) { float a = i * 45 * Mathf.Deg2Rad; Line(px, 32 + Mathf.Cos(a) * 16, 32 + Mathf.Sin(a) * 16, 32 + Mathf.Cos(a) * 24, 32 + Mathf.Sin(a) * 24, 6); } break;
                case "radar": Arc(px, 32, 14, 40, 50, 130, 4); Arc(px, 32, 14, 26, 50, 130, 4); Arc(px, 32, 14, 12, 50, 130, 4); Disc(px, 32, 14, 4); break;
                case "flag": Line(px, 18, 8, 18, 56, 4); Fill(px, (x, y) => TriInside(x, y, 18, 56, 18, 32, 50, 44) ? -1 : 1); break;
                case "cloud": Disc(px, 24, 28, 11); Disc(px, 38, 32, 14); Disc(px, 48, 26, 9); Solid(px, 16, 18, 54, 27); break;
                case "compass": Ring(px, 32, 32, 24, 4); Fill(px, (x, y) => TriInside(x, y, 32, 54, 25, 32, 39, 32) ? -1 : 1); break;
                case "pin": Ring(px, 32, 40, 12, 5); Line(px, 32, 28, 32, 8, 5); break;
                case "pause": Solid(px, 18, 14, 27, 50); Solid(px, 37, 14, 46, 50); break;
                case "camera": Box(px, 10, 16, 54, 46, 4); Ring(px, 32, 31, 9, 4); Solid(px, 22, 46, 34, 52); break;
                case "eye": Arc(px, 32, 18, 26, 30, 150, 4); Arc(px, 32, 46, 26, 210, 330, 4); Disc(px, 32, 32, 7); break;
                case "terrain": Line(px, 6, 16, 24, 40, 4); Line(px, 24, 40, 34, 28, 4); Line(px, 34, 28, 58, 52, 4); Line(px, 6, 12, 58, 12, 3); break;
                case "clock": Ring(px, 32, 32, 22, 4); Line(px, 32, 32, 32, 46, 4); Line(px, 32, 32, 42, 26, 4); break;
                case "scan": Ring(px, 32, 32, 22, 3); Ring(px, 32, 32, 12, 3); Line(px, 32, 4, 32, 60, 3); Line(px, 4, 32, 60, 32, 3); break;
                case "target": Ring(px, 32, 32, 18, 4); Disc(px, 32, 32, 5); Line(px, 32, 4, 32, 16, 4); Line(px, 32, 48, 32, 60, 4); Line(px, 4, 32, 16, 32, 4); Line(px, 48, 32, 60, 32, 4); break;
                case "ai": Box(px, 16, 16, 48, 48, 4); Solid(px, 26, 26, 38, 38); for (int i = 0; i < 3; i++) { float o = 22 + i * 10; Line(px, o, 4, o, 14, 3); Line(px, o, 50, o, 60, 3); Line(px, 4, o, 14, o, 3); Line(px, 50, o, 60, o, 3); } break;
                case "warning": Fill(px, (x, y) => TriInside(x, y, 32, 56, 6, 10, 58, 10) ? -1 : 1); break;
                case "disc": Disc(px, 32, 32, 30); break;
                case "ringfull": Ring(px, 32, 32, 28, 5); break;
                case "accelarrow": Line(px, 32, 10, 32, 50, 6); Line(px, 32, 54, 16, 36, 6); Line(px, 32, 54, 48, 36, 6); break;
                case "brakebars": Solid(px, 14, 20, 24, 44); Solid(px, 28, 20, 36, 44); Solid(px, 40, 20, 50, 44); break;
                case "cam2": Box(px, 10, 18, 54, 46, 3); Ring(px, 32, 32, 9, 3); Line(px, 22, 46, 28, 52, 3); Line(px, 28, 52, 36, 52, 3); Line(px, 36, 52, 42, 46, 3); break;
                case "eye2": Arc(px, 32, 14, 26, 42, 138, 3); Arc(px, 32, 50, 26, 222, 318, 3); Ring(px, 32, 32, 7, 3); break;
                case "recenter2": Ring(px, 32, 32, 18, 3); Line(px, 32, 4, 32, 14, 3); Line(px, 32, 50, 32, 60, 3); Line(px, 4, 32, 14, 32, 3); Line(px, 50, 32, 60, 32, 3); Disc(px, 32, 32, 3.5f); break;
                case "exit2": Box(px, 10, 12, 34, 52, 3); Line(px, 26, 32, 56, 32, 3); Line(px, 56, 32, 46, 42, 3); Line(px, 56, 32, 46, 22, 3); break;
                case "scan2": Ring(px, 32, 32, 22, 3); Ring(px, 32, 32, 11, 3); Line(px, 32, 32, 50, 46, 3); break;
                case "target2": Ring(px, 32, 32, 16, 3); Line(px, 32, 4, 32, 18, 3); Line(px, 32, 46, 32, 60, 3); Line(px, 4, 32, 18, 32, 3); Line(px, 46, 32, 60, 32, 3); break;
                case "map": Line(px, 8, 14, 24, 8, 3); Line(px, 24, 8, 40, 14, 3); Line(px, 40, 14, 56, 8, 3); Line(px, 56, 8, 56, 50, 3); Line(px, 56, 50, 40, 56, 3); Line(px, 40, 56, 24, 50, 3); Line(px, 24, 50, 8, 56, 3); Line(px, 8, 56, 8, 14, 3); Line(px, 24, 8, 24, 50, 3); Line(px, 40, 14, 40, 56, 3); break;
                case "dots": Disc(px, 14, 32, 5); Disc(px, 32, 32, 5); Disc(px, 50, 32, 5); break;
                case "close": Line(px, 16, 16, 48, 48, 4); Line(px, 48, 16, 16, 48, 4); break;
                case "rover": Box(px, 12, 26, 52, 42, 3); Ring(px, 18, 18, 6, 3); Ring(px, 32, 18, 6, 3); Ring(px, 46, 18, 6, 3); Line(px, 40, 42, 40, 54, 3); Line(px, 40, 54, 48, 54, 3); break;
                case "astronaut": Ring(px, 32, 44, 10, 3); Box(px, 22, 10, 42, 32, 3); break;
                case "dome": Arc(px, 32, 18, 22, 0, 180, 3); Line(px, 8, 18, 56, 18, 3); Line(px, 28, 18, 28, 28, 3); Line(px, 36, 18, 36, 28, 3); break;
                case "door": Box(px, 20, 10, 44, 54, 3); Disc(px, 38, 32, 3); break;
                case "solar": Box(px, 10, 24, 54, 50, 3); Line(px, 32, 24, 32, 50, 2); Line(px, 10, 37, 54, 37, 2); Line(px, 32, 24, 32, 8, 3); break;
                case "bolt": Line(px, 36, 56, 22, 30, 4); Line(px, 22, 30, 42, 32, 4); Line(px, 42, 32, 28, 8, 4); break;
                case "tank": Box(px, 20, 10, 44, 54, 3); Line(px, 20, 22, 44, 22, 2); Line(px, 20, 42, 44, 42, 2); break;
                case "crate": Box(px, 12, 14, 52, 50, 3); Line(px, 12, 14, 52, 50, 2); Line(px, 52, 14, 12, 50, 2); break;
                case "science": Line(px, 26, 54, 26, 36, 3); Line(px, 38, 54, 38, 36, 3); Line(px, 26, 36, 14, 12, 3); Line(px, 38, 36, 50, 12, 3); Line(px, 14, 12, 50, 12, 3); break;
                case "proposal": Ring(px, 32, 32, 20, 2); Line(px, 32, 22, 32, 36, 3); Disc(px, 32, 43, 2.5f); break;
                case "chevron": Line(px, 22, 14, 42, 32, 6); Line(px, 42, 32, 22, 50, 6); break;
                case "check": Line(px, 14, 32, 27, 18, 6); Line(px, 27, 18, 52, 46, 6); break;
                case "tent": Line(px, 8, 12, 32, 50, 4); Line(px, 32, 50, 56, 12, 4); Line(px, 8, 12, 56, 12, 4); Line(px, 32, 12, 32, 30, 3); break;
                case "wrench": Line(px, 18, 14, 40, 36, 7); Ring(px, 44, 42, 9, 5); break;
                case "gate": Line(px, 14, 8, 14, 56, 5); Line(px, 50, 8, 50, 56, 5); Line(px, 14, 44, 50, 44, 3); break;
                default: Ring(px, 32, 32, 20, 4); break;
            }
            if (id == "warning") { for (int i = 0; i < px.Length; i++) { int x = i % S, y = i / S; if (Seg(x + .5f, y + .5f, 32, 42, 32, 24) < 3 || new Vector2(x + .5f - 32, y + .5f - 17).magnitude < 3) px[i] = 0; } }
        }

        static bool TriInside(float x, float y, float ax, float ay, float bx, float by, float cx, float cy)
        {
            float d1 = (x - bx) * (ay - by) - (ax - bx) * (y - by), d2 = (x - cx) * (by - cy) - (bx - cx) * (y - cy), d3 = (x - ax) * (cy - ay) - (cx - ax) * (y - ay);
            bool neg = d1 < 0 || d2 < 0 || d3 < 0, pos = d1 > 0 || d2 > 0 || d3 > 0;
            return !(neg && pos);
        }
    }
}
