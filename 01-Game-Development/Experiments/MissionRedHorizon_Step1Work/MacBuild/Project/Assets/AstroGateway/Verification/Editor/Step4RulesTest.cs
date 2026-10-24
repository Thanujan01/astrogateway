using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using AstroGateway.Integration;
using AstroGateway.MissionRules;
using AstroGateway.NativeSlice;

// Step 4: checks the Unity rule models against the backend's shared test vectors, resolves every mission
// registry in the project through the alias map, and confirms nothing in any scene or prefab uses the new rules.
// Edit mode, batch only, read-only. Env: STEP4_OUT (results), STEP4_BACKEND_RULES (optional backend config path).
namespace AstroGateway.Verification
{
    public static class Step4RulesTest
    {
        const string Folder = "Assets/AstroGateway/MissionRules";
        static readonly List<string> lines = new List<string>();
        static int pass, fail;

        static void Check(string n, bool ok, string d = "") { if (ok) pass++; else fail++; lines.Add((ok ? "PASS  " : "FAIL  ") + n + (string.IsNullOrEmpty(d) ? "" : "  [" + d + "]")); }

        public static void Run()
        {
            try { Tests(); } catch (Exception e) { fail++; lines.Add("EXCEPTION: " + e); }
            string outDir = Environment.GetEnvironmentVariable("STEP4_OUT") ?? Path.Combine(Path.GetTempPath(), "Step4Rules");
            Directory.CreateDirectory(outDir);
            File.WriteAllLines(Path.Combine(outDir, "Step4_Unity_Rules_Results.txt"),
                new[] { $"Step 4 Unity rule models — {DateTime.Now:yyyy-MM-dd HH:mm:ss}", $"PASS {pass}  FAIL {fail}", "" }.Concat(lines));
            if (Application.isBatchMode) EditorApplication.Exit(fail == 0 ? 0 : 1);
        }

        static void Tests()
        {
            string json = File.ReadAllText(Folder + "/Config/mission_rules_v1.json");
            var config = new MissionRulesConfig(json);
            var life = new ConfiguredLifeSupportModel(config);
            var weather = new ConfiguredWeatherModel(config);
            Check("Config parses and is labelled proposed", config.Version.EndsWith("proposed") && ((string)config.Raw["status"]).Contains("PROPOSED"), config.Version);
            Check("Penalties and weather effects are disabled", !life.PenaltiesEnabled && !weather.EffectsEnabled);

            string backend = Environment.GetEnvironmentVariable("STEP4_BACKEND_RULES");
            if (!string.IsNullOrEmpty(backend)) Check("Unity config is identical to the backend config", File.ReadAllText(backend) == json);

            var vectors = (Dictionary<string, object>)MiniJson.Parse(File.ReadAllText(Folder + "/Config/test_vectors_v1.json"));
            Check("Vectors were generated from the same rules version", (string)vectors["rules_version"] == config.Version);
            int oxygenOk = 0, oxygenAll = 0;
            foreach (var kv in (Dictionary<string, object>)vectors["oxygen_percent_per_real_second"])
            {
                var parts = kv.Key.Split('|');
                oxygenAll++;
                if (Math.Abs(life.PercentPerRealSecond(parts[0], parts[1]) - Convert.ToDouble(kv.Value)) < 1e-9) oxygenOk++;
                else lines.Add("  mismatch oxygen " + kv.Key);
            }
            Check("Oxygen rates match backend vectors (activity × weather)", oxygenAll == 15 && oxygenOk == oxygenAll, oxygenOk + "/" + oxygenAll);
            int weatherOk = 0, weatherAll = 0;
            foreach (Dictionary<string, object> v in (List<object>)vectors["weather"])
            {
                weatherAll++;
                var c = weather.ConditionsAt(Convert.ToInt32(v["difficulty"]), Convert.ToDouble(v["t"]));
                bool ok = c.state == (string)v["state"] && c.advisory == (string)v["advisory"] && Math.Abs(c.tau - Convert.ToDouble(v["tau"])) < 1e-9
                          && Math.Abs(c.visibilityM - Convert.ToDouble(v["visibility_m"])) < 1e-9 && Math.Abs(c.solarPowerFactor - Convert.ToDouble(v["solar_power_factor"])) < 1e-9;
                if (ok) weatherOk++; else lines.Add($"  mismatch weather d{v["difficulty"]} t{v["t"]}");
            }
            Check("Weather scenario matches backend vectors", weatherAll == 18 && weatherOk == weatherAll, weatherOk + "/" + weatherAll);

            var storm = new WeatherState { dust = DustLevel.DustStorm };
            Check("ILifeSupportModel contract: storm walking uses more O2 than clear idle",
                life.OxygenUsePerSecond(Activity.Walking, storm) > life.OxygenUsePerSecond(Activity.Idle, new WeatherState()));
            var legacy = new PrototypeLifeSupportModel();
            Check("Existing default model is unchanged (0.015 / 0.035 %/s)",
                Mathf.Approximately(legacy.OxygenUsePerSecond(Activity.Walking, new WeatherState()), 0.015f) && Mathf.Approximately(legacy.OxygenUsePerSecond(Activity.Walking, storm), 0.035f));
            var ws = weather.ToWeatherState(weather.ConditionsAt(2, 400));
            Check("Weather feed values are labelled simulation", ws.dust == DustLevel.DustStorm && ws.source.StartsWith("simulation"));

            // Every mission registry asset in the project resolves to the canonical id without being changed.
            var catalog = new MissionCatalog(config);
            var registries = AssetDatabase.FindAssets("t:MissionRegistry").Select(AssetDatabase.GUIDToAssetPath).ToList();
            int resolved = 0;
            foreach (var path in registries)
            {
                var reg = AssetDatabase.LoadAssetAtPath<MissionRegistry>(path);
                if (catalog.TryResolve(reg.missionId, out var r) && r.canonicalId == "ag.mars.red-horizon.surface-ops" && r.originalId == reg.missionId) resolved++;
                else lines.Add("  unresolved registry " + path + " = " + reg.missionId);
            }
            Check("Every MissionRegistry asset resolves via the alias map", registries.Count >= 5 && resolved == registries.Count, resolved + "/" + registries.Count);
            Check("Backend legacy id resolves; unknown id does not",
                catalog.TryResolve("mission-red-horizon-poc", out var poc) && poc.isAlias && !catalog.TryResolve("mission.unknown", out _));

            // Isolation: no scene or prefab references the new rule scripts.
            var guids = Directory.GetFiles(Folder + "/Scripts", "*.cs.meta").Select(m => File.ReadAllLines(m).First(l => l.StartsWith("guid:")).Substring(6).Trim()).ToList();
            var users = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories).Concat(Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories))
                .Where(f => { var t = File.ReadAllText(f); return guids.Any(t.Contains); }).ToList();
            Check("No scene or prefab uses the new rules (not enabled)", guids.Count == 2 && users.Count == 0, string.Join(", ", users));
        }
    }
}
