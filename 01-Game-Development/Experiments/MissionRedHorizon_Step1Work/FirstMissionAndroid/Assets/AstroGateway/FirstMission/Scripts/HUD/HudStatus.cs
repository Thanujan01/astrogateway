using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Premium HUD (v2) data layer: design tokens, the status-reading model and the rover status provider.
// Presentation (HudStatusWidget, PremiumHUD) only reads StatusReading values; it never talks to the
// sensor store transport or the backend directly. Every reading names its provenance so the UI can say
// SIMULATED / LOCAL / NOT AVAILABLE / NOT CONNECTED instead of showing an invented number.
namespace AstroGateway.FirstMission.HUD
{
    public static class HudTokens
    {
        static Color Hex(string h) { ColorUtility.TryParseHtmlString(h, out var c); return c; }
        public static readonly Color Background = Hex("#101820"), Panel = Hex("#14212B"), Cyan = Hex("#00D9FF"), Orange = Hex("#FF8A32"),
            Green = Hex("#54D88A"), Amber = Hex("#FFBE45"), Red = Hex("#FF5A5F"), Text = Hex("#F2F6FA"), TextDim = Hex("#9AA8B4");
        public static Color PanelGlass(float a = .72f) => new Color(Panel.r, Panel.g, Panel.b, a);
        public static Color Edge(float a = .45f) => new Color(Cyan.r, Cyan.g, Cyan.b, a);
        public const int Gap = 12, Pad = 16, Radius = 14;
        public const int FontTitle = 26, FontBody = 20, FontSmall = 16, FontValue = 30, FontHero = 64;
        public const float Fast = .15f, Medium = .3f;
        public static Color ForLevel(StatusLevel l) => l switch
        {
            StatusLevel.Normal => Green, StatusLevel.Info => Cyan, StatusLevel.Warning => Amber, StatusLevel.Critical => Red, _ => TextDim,
        };
    }

    public enum StatusLevel { Unknown, Info, Normal, Warning, Critical }

    // Where a value comes from. Shown as a small tag next to every value.
    public static class Source
    {
        public const string GameState = "GAME STATE";       // live Unity simulation state (transforms, controller speed)
        public const string Simulated = "SIMULATED";         // game-rule value, not a real measurement
        public const string Local = "LOCAL";                 // stored on this device
        public const string NotAvailable = "NOT AVAILABLE";  // no model / no source exists
        public const string NotConnected = "NOT CONNECTED";  // a remote source that is not connected
    }

    public struct StatusReading
    {
        public string label, text, unit, source, icon, detail;
        public float? value;          // numeric value when one exists (drives bars and smooth transitions)
        public float? fill;           // 0..1 bar fill, when meaningful
        public StatusLevel level;
        public float timestamp;       // Time.unscaledTime when read
        public bool Available => source != Source.NotAvailable && source != Source.NotConnected;
        public string Accessible => $"{label}: {(Available ? text + (string.IsNullOrEmpty(unit) ? "" : " " + unit) : source)}, source {source}" + (level == StatusLevel.Warning ? ", warning" : level == StatusLevel.Critical ? ", critical" : "");
    }

    public interface IStatusSource { string Id { get; } StatusReading Read(); }

    public class FuncStatusSource : IStatusSource
    {
        readonly Func<StatusReading> read;
        public string Id { get; }
        public FuncStatusSource(string id, Func<StatusReading> read) { Id = id; this.read = read; }
        public StatusReading Read() { var r = read(); r.timestamp = Time.unscaledTime; return r; }
    }

    // The ten rover status categories requested for the HUD, each bound to its real source.
    public class RoverStatusProvider
    {
        public readonly List<IStatusSource> Sources = new List<IStatusSource>();
        readonly FirstMissionController m;
        readonly FirstMissionControlLayer control;
        float lastYaw, yawRate;

        public RoverStatusProvider(FirstMissionController mission, FirstMissionControlLayer control)
        {
            m = mission; this.control = control;
            var g = m.hud.game; var p = g.prototype;
            Add("power", () =>
            {
                float b = g.battery;
                return new StatusReading { label = "POWER", value = b, fill = b / 100f, text = b.ToString("0"), unit = "%", source = Source.Simulated, icon = "battery",
                    level = b < 15 ? StatusLevel.Critical : b < 30 ? StatusLevel.Warning : StatusLevel.Normal, detail = "Game battery model (drains while driving); not a real power system" };
            });
            Add("temperature", () => new StatusReading { label = "TEMP", text = "N/A", source = Source.NotAvailable, icon = "thermo", level = StatusLevel.Unknown, detail = "No vehicle thermal model exists" });
            Add("comms", () =>
            {
                bool online = m.Sync.Uploader.Online;
                return new StatusReading { label = "LINK", text = online ? "ONLINE" : "OFFLINE", source = online ? Source.GameState : Source.NotConnected, icon = "signal",
                    level = online ? StatusLevel.Normal : StatusLevel.Warning, detail = m.Sync.Uploader.Label };
            });
            Add("storage", () =>
            {
                int n = m.Store.Records.Count, max = m.Store.MaxRecords;
                return new StatusReading { label = "STORAGE", value = n, fill = n / (float)max, text = $"{n}/{max}", unit = "records", source = Source.Local, icon = "database",
                    level = n >= max ? StatusLevel.Critical : n > max * .8f ? StatusLevel.Warning : StatusLevel.Normal, detail = "Sensor records on this device" };
            });
            Add("speed", () =>
            {
                float kmh = Mathf.Abs(p.speed) * 3.6f;
                return new StatusReading { label = "SPEED", value = kmh, fill = Mathf.Abs(p.speed) / 3.8f, text = kmh.ToString("0.0"), unit = "km/h", source = Source.GameState, icon = "gauge",
                    level = p.driving ? StatusLevel.Info : StatusLevel.Unknown, detail = "MarsPrototype controller speed (max 13.7 km/h forward)" };
            });
            Add("steering", () =>
            {
                float steer = p.driving ? Mathf.Clamp(p.testMove.x, -1, 1) : 0;
                return new StatusReading { label = "STEER", value = steer, fill = (steer + 1) / 2, text = steer > .05f ? "RIGHT" : steer < -.05f ? "LEFT" : "CENTRE", source = Source.GameState, icon = "wheel",
                    level = p.driving ? StatusLevel.Info : StatusLevel.Unknown, detail = $"Steering input {steer:+0.00;-0.00}; yaw rate {yawRate:0} °/s" };
            });
            Add("mode", () => new StatusReading { label = "MODE", text = ModeName(control ? control.State : ControlState.Astronaut), source = Source.GameState, icon = "mode", level = StatusLevel.Info,
                detail = p.driving ? (p.speed < -.05f ? "Reversing" : p.speed > .05f ? "Driving forward" : "Stopped") : "On foot" });
            Add("sensors", () =>
            {
                int avail = m.Catalog.Instruments.Count(i => i.available), total = m.Catalog.Instruments.Count;
                return new StatusReading { label = "SENSORS", value = avail, fill = avail / (float)total, text = $"{avail}/{total}", unit = "available", source = Source.Simulated, icon = "radar",
                    level = avail == total ? StatusLevel.Normal : StatusLevel.Warning, detail = string.Join(", ", m.Catalog.Instruments.Where(i => !i.available).Select(i => i.name + " unavailable")) };
            });
            Add("mission", () =>
            {
                var o = m.Objectives().Where(x => !x.info).ToList(); int done = o.Count(x => x.done);
                return new StatusReading { label = "MISSION", value = done, fill = done / (float)o.Count, text = $"{done}/{o.Count}", unit = "objectives", source = Source.Local, icon = "flag",
                    level = done == o.Count ? StatusLevel.Normal : StatusLevel.Info, detail = "Local guidance, not scored" };
            });
            Add("sync", () =>
            {
                var s = SyncSummary(m);
                return new StatusReading { label = "SYNC", text = s.label, value = s.pending, source = m.Sync.Uploader.Online ? Source.GameState : Source.NotConnected, icon = "cloud",
                    level = s.failed > 0 ? StatusLevel.Critical : m.Sync.Uploader.Online ? StatusLevel.Normal : StatusLevel.Warning, detail = m.Sync.Status };
            });
        }

        void Add(string id, Func<StatusReading> f) => Sources.Add(new FuncStatusSource(id, f));
        public IStatusSource Get(string id) => Sources.First(s => s.Id == id);

        public void Tick(float dt)
        {
            var p = m.hud.game.prototype;
            float yaw = p.rover.transform.eulerAngles.y;
            if (dt > 0) yawRate = Mathf.Lerp(yawRate, Mathf.DeltaAngle(lastYaw, yaw) / dt, .2f);
            lastYaw = yaw;
        }

        public static string ModeName(ControlState s) => s switch
        {
            ControlState.RoverDrive => "ROVER DRIVE", ControlState.RoverScience => "ROVER SCIENCE", ControlState.DataReview => "DATA REVIEW",
            ControlState.PauseMenu => "PAUSED", _ => "ASTRONAUT",
        };

        // The five sync states shown in the HUD, mapped from the queue's detailed upload states.
        public static string SyncLabel(SensorRecord r, bool online) => r.uploadState switch
        {
            UploadState.Uploaded => "SYNCED",
            UploadState.Uploading => "SYNCING",
            UploadState.Failed => "FAILED",
            UploadState.Rejected => "FAILED",
            _ => online ? "QUEUED" : "LOCAL ONLY",
        };

        public static (string label, int pending, int failed, int synced) SyncSummary(FirstMissionController m)
        {
            bool online = m.Sync.Uploader.Online;
            int synced = 0, failed = 0, syncing = 0, pending = 0;
            foreach (var r in m.Store.Records)
            {
                var l = SyncLabel(r, online);
                if (l == "SYNCED") synced++; else if (l == "FAILED") failed++; else if (l == "SYNCING") syncing++; else pending++;
            }
            string label = failed > 0 ? "FAILED" : syncing > 0 ? "SYNCING" : pending == 0 && synced > 0 ? "SYNCED" : online ? (pending > 0 ? "QUEUED" : "IDLE") : "LOCAL ONLY";
            return (label, pending + syncing, failed, synced);
        }
    }
}
