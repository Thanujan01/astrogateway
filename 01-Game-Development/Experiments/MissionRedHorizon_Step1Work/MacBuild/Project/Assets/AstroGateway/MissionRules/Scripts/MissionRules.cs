using System;
using System.Collections.Generic;
using System.Linq;
using AstroGateway.Integration;

// Client-side halves of the Step 4 rules, driven by the same mission_rules_v1.json as the backend.
// Isolated: no scene uses these types, and penalties/effects stay disabled (see config flags).
// Scoring, skills and recommendations are server-side only (backend/rules) so clients cannot award themselves scores.
namespace AstroGateway.MissionRules
{
    public class MissionRulesConfig
    {
        public readonly Dictionary<string, object> Raw;
        public MissionRulesConfig(string json) { Raw = (Dictionary<string, object>)MiniJson.Parse(json); }
        public string Version => (string)Raw["rules_version"];
        public Dictionary<string, object> Section(string name) => (Dictionary<string, object>)Raw[name];
        public static Dictionary<string, object> Map(object o) => (Dictionary<string, object>)o;
        public static double Num(object o) => Convert.ToDouble(o);
    }

    // ---------- mission identifiers ----------
    public struct ResolvedMission
    {
        public string canonicalId, originalId, buildVariant;
        public bool isAlias;
    }

    public class MissionCatalog
    {
        readonly HashSet<string> canonical = new HashSet<string>();
        readonly Dictionary<string, Dictionary<string, object>> aliases = new Dictionary<string, Dictionary<string, object>>();

        public MissionCatalog(MissionRulesConfig config)
        {
            var missions = config.Section("missions");
            foreach (var m in (List<object>)missions["canonical"]) canonical.Add((string)MissionRulesConfig.Map(m)["mission_id"]);
            foreach (var a in (List<object>)missions["aliases"]) { var map = MissionRulesConfig.Map(a); aliases[(string)map["alias"]] = map; }
        }

        // Never rewrites the stored id: originalId is returned unchanged next to the canonical id.
        public bool TryResolve(string missionId, out ResolvedMission result)
        {
            result = default;
            if (missionId == null) return false;
            if (canonical.Contains(missionId)) { result = new ResolvedMission { canonicalId = missionId, originalId = missionId }; return true; }
            if (!aliases.TryGetValue(missionId, out var a)) return false;
            result = new ResolvedMission { canonicalId = (string)a["canonical"], originalId = missionId, isAlias = true, buildVariant = a.TryGetValue("build_variant", out var v) ? (string)v : null };
            return true;
        }
    }

    // ---------- oxygen ----------
    // Implements the existing ILifeSupportModel contract (% of suit tank per real second).
    public class ConfiguredLifeSupportModel : ILifeSupportModel
    {
        readonly Dictionary<string, object> c;
        readonly string version;
        public ConfiguredLifeSupportModel(MissionRulesConfig config) { c = config.Section("life_support"); version = config.Version; }
        public string ModelVersion => version;
        public bool PenaltiesEnabled => (bool)c["penalties_enabled"];
        public double CapacityKg => MissionRulesConfig.Num(c["suit_o2_capacity_kg"]);

        public static string ActivityKey(Activity a) => a.ToString().ToLowerInvariant();
        public static string WeatherKey(DustLevel d) => d == DustLevel.Clear ? "clear" : d == DustLevel.LightDust ? "light_dust" : "dust_storm";

        public double RateKgPerHour(string activity, string weather) =>
            MissionRulesConfig.Num(MissionRulesConfig.Map(c["activity_rate_kg_per_h"])[activity]) * MissionRulesConfig.Num(MissionRulesConfig.Map(c["weather_workload_multiplier"])[weather])
            + MissionRulesConfig.Num(c["leak_rate_kg_per_h"]);

        public double PercentPerRealSecond(string activity, string weather) =>
            RateKgPerHour(activity, weather) * MissionRulesConfig.Num(c["time_compression"]) / 3600.0 / CapacityKg * 100.0;

        public float OxygenUsePerSecond(Activity activity, WeatherState weather) =>
            (float)PercentPerRealSecond(ActivityKey(activity), WeatherKey(weather != null ? weather.dust : DustLevel.Clear));

        public string Status(double fraction)
        {
            var t = MissionRulesConfig.Map(c["thresholds_fraction"]);
            if (fraction <= MissionRulesConfig.Num(t["critical"])) return "critical";
            if (fraction <= MissionRulesConfig.Num(t["warning"])) return "warning";
            if (fraction <= MissionRulesConfig.Num(t["caution"])) return "caution";
            return "nominal";
        }
    }

    // ---------- weather ----------
    public struct WeatherConditions
    {
        public string state, advisory;
        public double tau, visibilityM, solarPowerFactor, windMps, gustMps, temperatureC;
    }

    public class ConfiguredWeatherModel
    {
        readonly Dictionary<string, object> w;
        public ConfiguredWeatherModel(MissionRulesConfig config) { w = config.Section("weather"); }
        public bool EffectsEnabled => (bool)w["effects_enabled"];

        public string StateAt(int difficulty, double missionSeconds)
        {
            var steps = (List<object>)MissionRulesConfig.Map(w["scenarios"])[difficulty.ToString()];
            string current = (string)MissionRulesConfig.Map(steps[0])["state"];
            foreach (var s in steps.Select(MissionRulesConfig.Map))
                if (missionSeconds >= MissionRulesConfig.Num(s["start_s"])) current = (string)s["state"];
            return current;
        }

        public WeatherConditions ConditionsAt(int difficulty, double missionSeconds, double localHour = 12)
        {
            string state = StateAt(difficulty, missionSeconds);
            var s = MissionRulesConfig.Map(MissionRulesConfig.Map(w["states"])[state]);
            var d = MissionRulesConfig.Map(w["diurnal_temperature_c"]);
            double min = MissionRulesConfig.Num(d["min"]), max = MissionRulesConfig.Num(d["max"]);
            double phase = Math.Cos((localHour - MissionRulesConfig.Num(d["peak_hour"])) / 24.0 * 2 * Math.PI);
            double tau = MissionRulesConfig.Num(s["tau"]);
            return new WeatherConditions
            {
                state = state,
                tau = tau,
                windMps = MissionRulesConfig.Num(s["wind_mps"]),
                gustMps = MissionRulesConfig.Num(s["gust_mps"]),
                visibilityM = MissionRulesConfig.Num(s["visibility_m"]),
                temperatureC = Math.Round(min + (max - min) * (phase + 1) / 2 + MissionRulesConfig.Num(s["temp_offset_c"]), 3),
                solarPowerFactor = Math.Round(Math.Exp(-MissionRulesConfig.Num(w["solar_power_extinction_k"]) * tau), 6),
                advisory = (string)MissionRulesConfig.Map(w["advisory_by_state"])[state],
            };
        }

        // Ready for later wiring as IWeatherFeed; values are simulation, never measurements.
        public WeatherState ToWeatherState(WeatherConditions c) => new WeatherState
        {
            dust = c.state == "clear" ? DustLevel.Clear : c.state == "light_dust" ? DustLevel.LightDust : DustLevel.DustStorm,
            visibilityMeters = (float)c.visibilityM,
            windMetersPerSecond = (float)c.windMps,
            temperatureCelsius = (float)c.temperatureC,
            source = "simulation:" + (string)w["units"],
        };
    }
}
