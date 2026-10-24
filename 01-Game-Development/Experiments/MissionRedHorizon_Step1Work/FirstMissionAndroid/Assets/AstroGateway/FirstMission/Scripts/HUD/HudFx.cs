using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// High-end visual layer for the premium HUD: procedural sprites (soft shadow, glass gradient, radial glow,
// tick ring, capsule) and small animation components (press response, joystick feedback, pedal fill,
// sliding tab indicator, breathing glow). All mobile-friendly: static textures generated once, no blur
// pass, no per-frame allocation, honours the reduced-motion setting.
namespace AstroGateway.FirstMission.HUD
{
    public static class HudFx
    {
        static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();
        public static Func<bool> ReducedMotion = () => false;

        static Sprite Make(string key, int w, int h, Func<float, float, Color> f, Vector4 border = default)
        {
            if (cache.TryGetValue(key, out var s) && s) return s;
            var t = new Texture2D(w, h, TextureFormat.RGBA32, false) { filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp, name = "HudFx_" + key };
            var px = new Color32[w * h];
            for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) px[y * w + x] = f((x + .5f) / w, (y + .5f) / h);
            t.SetPixels32(px); t.Apply(false, true);
            s = Sprite.Create(t, new Rect(0, 0, w, h), new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect, border);
            cache[key] = s; return s;
        }

        static float RoundBox(float x, float y, float hw, float hh, float r) // signed distance, units = texture px
        {
            float qx = Mathf.Abs(x) - (hw - r), qy = Mathf.Abs(y) - (hh - r);
            return new Vector2(Mathf.Max(qx, 0), Mathf.Max(qy, 0)).magnitude + Mathf.Min(Mathf.Max(qx, qy), 0) - r;
        }

        // Soft drop shadow, 9-sliced (128 px texture, 40 px soft edge).
        public static Sprite Shadow => Make("shadow", 128, 128, (u, v) =>
        {
            float d = RoundBox((u - .5f) * 128, (v - .5f) * 128, 44, 44, 18);
            float a = Mathf.Clamp01(1 - (d + 4) / 20f); a = a * a * .55f;
            return new Color(0, 0, 0, a);
        }, new Vector4(60, 60, 60, 60));

        // Glass fill: vertical gradient (lighter top), subtle inner top highlight; tinted by Image.color.
        public static Sprite GlassFill => Make("glassfill", 64, 64, (u, v) =>
        {
            float d = RoundBox((u - .5f) * 64, (v - .5f) * 64, 31.5f, 31.5f, 14);
            float a = Mathf.Clamp01(.5f - d);
            float g = Mathf.Lerp(.78f, 1.0f, v);                        // darker bottom
            float hi = d > -3.5f && d < -1.5f && v > .6f ? .35f : 0;      // inner top rim highlight
            return new Color(g + hi, g + hi, g + hi, a);
        }, new Vector4(18, 18, 18, 18));

        // Aerospace corner brackets (outline only at the four corners).
        public static Sprite Corners => Make("corners", 64, 64, (u, v) =>
        {
            float x = u * 64, y = v * 64; float m = 3, len = 14, w = 2.2f;
            bool nearX = x < m + len || x > 64 - m - len, nearY = y < m + len || y > 64 - m - len;
            bool onV = (Mathf.Abs(x - m) < w || Mathf.Abs(x - (64 - m)) < w) && nearY;
            bool onH = (Mathf.Abs(y - m) < w || Mathf.Abs(y - (64 - m)) < w) && nearX;
            return new Color(1, 1, 1, onV || onH ? 1 : 0);
        }, new Vector4(24, 24, 24, 24));


        // ---- telemetry-glass kit (bottom HUD spec, 2026-10-21) ----
        public static readonly Color ControlFill = new Color(10 / 255f, 20 / 255f, 30 / 255f, .55f);       // rgba(10,20,30,.55)
        public static readonly Color ControlBorder = new Color(0, 210 / 255f, 1f, .30f);                    // 1px rgba(0,210,255,.3)
        public static readonly Color Amber = new Color(1f, 140 / 255f, 0f);                                 // #FF8C00

        // Rounded rect / pill with a 1-px hairline border option (radius in px of a 64 px texture).
        public static Sprite Round(int radius) => Make($"round{radius}", 64, 64, (u, v) =>
        {
            float d = RoundBox((u - .5f) * 64, (v - .5f) * 64, 31.5f, 31.5f, radius);
            return new Color(1, 1, 1, Mathf.Clamp01(.5f - d));
        }, new Vector4(radius + 1, radius + 1, radius + 1, radius + 1));
        public static Sprite Hairline(int radius) => Make($"hair{radius}", 64, 64, (u, v) =>
        {
            float d = RoundBox((u - .5f) * 64, (v - .5f) * 64, 31.5f, 31.5f, radius);
            return new Color(1, 1, 1, Mathf.Clamp01(.5f - d) * Mathf.Clamp01(d + 1.5f));
        }, new Vector4(radius + 1, radius + 1, radius + 1, radius + 1));
        public static Sprite Circle => Make("circle", 256, 256, (u, v) => { float r = new Vector2(u - .5f, v - .5f).magnitude * 256; return new Color(1, 1, 1, Mathf.Clamp01(127.5f - r)); });
        public static Sprite CircleHairline => Make("circlehair", 256, 256, (u, v) => { float r = new Vector2(u - .5f, v - .5f).magnitude * 256; return new Color(1, 1, 1, Mathf.Clamp01(127.5f - r) * Mathf.Clamp01(r - 125.5f)); });

        // Subtle crosshair: 4 short ticks at the rim + 4 at the centre, hairline.
        public static Sprite Crosshair => Make("crosshair", 256, 256, (u, v) =>
        {
            float x = (u - .5f) * 256, y = (v - .5f) * 256, a = 0;
            bool vert = Mathf.Abs(x) < .9f, hor = Mathf.Abs(y) < .9f;
            float ay = Mathf.Abs(y), ax = Mathf.Abs(x);
            if (vert && ((ay > 104 && ay < 122) || (ay > 10 && ay < 22))) a = 1;
            if (hor && ((ax > 104 && ax < 122) || (ax > 10 && ax < 22))) a = 1;
            return new Color(1, 1, 1, a);
        });

        // Semicircle arc (180°, from 9 o'clock over the top to 3 o'clock), thickness as fraction of radius.
        public static Sprite Arc(float inner, float outer) => Make($"arc{inner}_{outer}", 256, 256, (u, v) =>
        {
            var p = new Vector2(u - .5f, v - .5f) * 2; float r = p.magnitude;
            float a = Mathf.Clamp01((r - inner) * 128) * Mathf.Clamp01((outer - r) * 128) * Mathf.Clamp01(p.y * 128 + .5f);
            return new Color(1, 1, 1, a);
        });
        // Arc ticks along the semicircle (n ticks, every 'major' longer).
        public static Sprite ArcTicks(int n, int major) => Make($"arcticks{n}_{major}", 256, 256, (u, v) =>
        {
            var p = new Vector2(u - .5f, v - .5f) * 2; float r = p.magnitude; if (p.y < -.01f) return new Color(1, 1, 1, 0);
            float ang = Mathf.Atan2(p.y, -p.x) * Mathf.Rad2Deg; // 0 at 9 o'clock, 180 at 3 o'clock
            float step = 180f / (n - 1); float k = Mathf.Round(ang / step); float da = Mathf.Abs(ang - k * step) * Mathf.Deg2Rad * r * 128;
            bool maj = ((int)k) % major == 0; float r0 = maj ? .74f : .79f;
            return new Color(1, 1, 1, Mathf.Clamp01(1.2f - da) * Mathf.Clamp01((r - r0) * 128) * Mathf.Clamp01((.84f - r) * 128) * (maj ? .9f : .45f));
        });

        public static Sprite Glow => Make("glow", 128, 128, (u, v) =>
        {
            float r = new Vector2(u - .5f, v - .5f).magnitude * 2; float a = Mathf.Clamp01(1 - r); a = a * a * a;
            return new Color(1, 1, 1, a);
        });

        public static Sprite RadialDisc => Make("raddisc", 128, 128, (u, v) =>
        {
            float r = new Vector2(u - .5f, v - .5f).magnitude * 2; float a = Mathf.Clamp01((1 - r) * 64);
            float g = Mathf.Lerp(1f, .55f, r);                       // brighter centre
            return new Color(g, g, g, a);
        });

        public static Sprite Ring(float inner, float outer) => Make($"ring{inner}_{outer}", 128, 128, (u, v) =>
        {
            float r = new Vector2(u - .5f, v - .5f).magnitude * 2;
            float a = Mathf.Clamp01((r - inner) * 64) * Mathf.Clamp01((outer - r) * 64);
            return new Color(1, 1, 1, a);
        });

        // Tick ring (n ticks, every 'major'th longer) for gauges and the joystick bezel.
        public static Sprite Ticks(int n, int major, float startDeg = 0, float sweep = 360) => Make($"ticks{n}_{major}_{startDeg}_{sweep}", 256, 256, (u, v) =>
        {
            var p = new Vector2(u - .5f, v - .5f) * 2; float r = p.magnitude;
            float ang = Mathf.Repeat(Mathf.Atan2(p.x, p.y) * Mathf.Rad2Deg - startDeg, 360);
            if (ang > sweep + .01f) return new Color(1, 1, 1, 0);
            float step = sweep / (sweep >= 360 ? n : n - 1); float k = Mathf.Round(ang / step); float da = Mathf.Abs(ang - k * step) * Mathf.Deg2Rad * r * 128;
            bool isMajor = ((int)k) % major == 0;
            float r0 = isMajor ? .80f : .87f;
            float a = Mathf.Clamp01(1.4f - da) * Mathf.Clamp01((r - r0) * 128) * Mathf.Clamp01((.97f - r) * 128);
            return new Color(1, 1, 1, a * (isMajor ? 1 : .6f));
        });

        // Vertical capsule for pedals (9-sliced).
        public static Sprite Capsule => Make("capsule", 64, 128, (u, v) =>
        {
            float d = RoundBox((u - .5f) * 64, (v - .5f) * 128, 31.5f, 63.5f, 28);
            float a = Mathf.Clamp01(.5f - d); float g = Mathf.Lerp(.75f, 1f, v);
            return new Color(g, g, g, a);
        }, new Vector4(30, 30, 30, 30));

        public static Sprite CapsuleEdge => Make("capsuleedge", 64, 128, (u, v) =>
        {
            float d = RoundBox((u - .5f) * 64, (v - .5f) * 128, 31.5f, 63.5f, 28);
            float a = Mathf.Clamp01(.5f - d) * Mathf.Clamp01(d + 3.5f);
            return new Color(1, 1, 1, a);
        }, new Vector4(30, 30, 30, 30));

        // Four direction chevrons around a joystick.
        public static Sprite DirectionMarks => Make("dirmarks", 128, 128, (u, v) =>
        {
            var p = new Vector2(u - .5f, v - .5f) * 2; float best = 0;
            for (int i = 0; i < 4; i++)
            {
                float a = i * 90 * Mathf.Deg2Rad; var dir = new Vector2(Mathf.Sin(a), Mathf.Cos(a)); var side = new Vector2(dir.y, -dir.x);
                float along = Vector2.Dot(p, dir), across = Mathf.Abs(Vector2.Dot(p, side));
                // chevron: along ~ .84 - |across|*0.6, thickness
                float d = Mathf.Abs(along - (.86f - across * .7f)) * 128;
                if (across < .13f && along > .6f) best = Mathf.Max(best, Mathf.Clamp01(2.2f - d));
            }
            return new Color(1, 1, 1, best);
        });
    }


    // Glass pedal: keeps the 55 % dark fill (HoldControl would brighten it), hairline border that lights up
    // with a soft outer glow while pressed, and a slim level bar (throttle / brake).
    public class HudGlassPedalFx : MonoBehaviour
    {
        public HoldControl hold; public Image body, border, glow, level; public Func<float> value; public Color accent;
        float shownLevel, lit;
        void LateUpdate()
        {
            bool held = hold && hold.Held; float dt = Time.unscaledDeltaTime;
            lit = Mathf.MoveTowards(lit, held ? 1 : 0, dt / (HudFx.ReducedMotion() ? .01f : .1f));
            if (body) body.color = Color.Lerp(HudFx.ControlFill, new Color(accent.r * .18f, accent.g * .18f, accent.b * .18f, .62f), lit);
            if (border) border.color = new Color(accent.r, accent.g, accent.b, Mathf.Lerp(.30f, 1f, lit));
            if (glow) glow.color = new Color(accent.r, accent.g, accent.b, lit * .28f);
            float target = value != null ? value() : held ? 1 : 0;
            shownLevel = Mathf.Lerp(shownLevel, target, 1 - Mathf.Exp(-16 * dt));
            if (level) { level.rectTransform.anchorMax = new Vector2(level.rectTransform.anchorMax.x, Mathf.Lerp(.08f, .92f, shownLevel)); level.color = new Color(accent.r, accent.g, accent.b, .35f + .65f * shownLevel); }
        }
    }

    // Segmented D | R pill: the active gear gets a subtle cyan (D) or amber (R) background.
    public class HudGearPillFx : MonoBehaviour
    {
        public Toggle toggle; public Image dSeg, rSeg; public Text dText, rText;
        void LateUpdate()
        {
            bool r = toggle && toggle.isOn; var c = new Color(0, 210 / 255f, 1f); var a = HudFx.Amber;
            dSeg.color = new Color(c.r, c.g, c.b, r ? 0 : .22f); rSeg.color = new Color(a.r, a.g, a.b, r ? .26f : 0);
            dText.color = r ? new Color(1, 1, 1, .45f) : c; rText.color = r ? a : new Color(1, 1, 1, .45f);
        }
    }

    // Minimal joystick feedback: hairline ring brightens slightly while held; no glow.
    public class HudMinimalStickFx : MonoBehaviour
    {
        public VirtualStick stick; public Image ring;
        void LateUpdate()
        {
            bool active = stick && stick.Value.sqrMagnitude > .0001f;
            if (ring) ring.color = new Color(0, 210 / 255f, 1f, active ? .7f : .35f);
        }
    }

    // Press response: scale-in, glow flash, brightness. Works with Button, Toggle and HoldControl targets.
    public class HudPressFx : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public Image glow; public float pressedScale = .94f;
        float press, flash; bool down; Vector3 baseScale = Vector3.one; bool init;
        public void OnPointerDown(PointerEventData e) { down = true; flash = 1; }
        public void OnPointerUp(PointerEventData e) { down = false; }
        public void OnPointerExit(PointerEventData e) { down = false; }
        void OnEnable() { down = false; press = 0; flash = 0; }
        void LateUpdate()
        {
            if (!init || press <= 0) { baseScale = transform.localScale; init = true; }  // follow the button-size setting
            float dt = Time.unscaledDeltaTime; bool reduced = HudFx.ReducedMotion();
            press = Mathf.MoveTowards(press, down ? 1 : 0, dt / .07f);
            flash = Mathf.MoveTowards(flash, 0, dt / .35f);
            float s = reduced ? 1 : Mathf.Lerp(1, pressedScale, press);
            transform.localScale = new Vector3(baseScale.x * s, baseScale.y * s, 1);
            if (glow) { var c = glow.color; c.a = Mathf.Max(down ? .45f : 0, flash * .6f); glow.color = c; }
        }
        public void Rebase() { init = false; }
    }

    // Joystick feedback: bezel brightens while held, a direction glow follows the stick.
    public class HudStickFx : MonoBehaviour
    {
        public VirtualStick stick; public Image bezel, direction, knobGlow; public Transform pivot; public Color accent;
        float level;
        void LateUpdate()
        {
            var v = stick ? stick.Value : Vector2.zero; float m = v.magnitude;
            level = Mathf.MoveTowards(level, m > .01f ? 1 : 0, Time.unscaledDeltaTime / .12f);
            if (bezel) bezel.color = new Color(accent.r, accent.g, accent.b, Mathf.Lerp(.35f, .85f, level));
            if (direction)
            {
                direction.color = new Color(accent.r, accent.g, accent.b, m * .55f);
                if (m > .01f && pivot) pivot.localEulerAngles = new Vector3(0, 0, -Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg);
            }
            if (knobGlow) knobGlow.color = new Color(accent.r, accent.g, accent.b, Mathf.Lerp(.25f, .7f, level));
        }
    }

    // Pedal: fill height follows a value (throttle 0..1 or held state), edge glows while pressed.
    public class HudPedalFx : MonoBehaviour
    {
        public HoldControl hold; public Image fill, edge, glow; public Func<float> value; public Color accent;
        float shown;
        void LateUpdate()
        {
            float target = value != null ? value() : hold && hold.Held ? 1 : 0;
            shown = Mathf.Lerp(shown, target, 1 - Mathf.Exp(-16 * Time.unscaledDeltaTime));
            if (fill) { fill.rectTransform.anchorMax = new Vector2(1, Mathf.Max(.0f, shown)); fill.color = new Color(accent.r, accent.g, accent.b, .55f); }
            bool held = hold && hold.Held;
            if (edge) edge.color = new Color(accent.r, accent.g, accent.b, held ? 1f : .55f);
            if (glow) glow.color = new Color(accent.r, accent.g, accent.b, held ? .35f : 0);
        }
    }

    // Sliding pill behind the selected tab.
    public class HudTabIndicator : MonoBehaviour
    {
        public FirstMissionHUD ui; public RectTransform pill; public Dictionary<FirstMissionHUD.Tab, RectTransform> tabs = new Dictionary<FirstMissionHUD.Tab, RectTransform>();
        void LateUpdate()
        {
            if (!ui || !pill || !tabs.TryGetValue(ui.Current, out var t)) return;
            var target = (Vector2)pill.parent.InverseTransformPoint(t.TransformPoint(t.rect.center));
            float k = HudFx.ReducedMotion() ? 1 : 1 - Mathf.Exp(-18 * Time.unscaledDeltaTime);
            pill.anchoredPosition = Vector2.Lerp(pill.anchoredPosition, target, k);   // pill anchored at the parent's centre pivot
            pill.sizeDelta = Vector2.Lerp(pill.sizeDelta, t.rect.size, k);
        }
    }

    // Slow breathing alpha for accent elements (status dots, active objective). Off with reduced motion.
    public class HudBreathe : MonoBehaviour
    {
        public Graphic target; public float min = .45f, max = 1f, period = 2.4f;
        void LateUpdate()
        {
            if (!target) return; var c = target.color;
            c.a = HudFx.ReducedMotion() ? max : Mathf.Lerp(min, max, .5f + .5f * Mathf.Sin(Time.unscaledTime * Mathf.PI * 2 / period));
            target.color = c;
        }
    }
}
