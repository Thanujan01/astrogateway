using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using AstroGateway.CinematicHUD;
using AstroGateway.NativeSlice;
using AstroGateway.ProductionVisual;

// Builds the isolated First Mission scene from Mars_VisualUpgrade_v1 (only read, never saved) and the
// ARES-VI First Mission prefab variant (the showroom prefab is only read). Batch-safe.
// Entry: AstroGateway.FirstMission.EditorTools.FirstMissionBuilder.Build  (report: env FM_OUT, default Docs/FirstMission)
namespace AstroGateway.FirstMission.EditorTools
{
    public static class FirstMissionBuilder
    {
        public const string Source = "Assets/AstroGateway/VisualUpgrade/Scenes/Mars_VisualUpgrade_v1.unity";
        public const string F = "Assets/AstroGateway/FirstMission";
        public const string Scene = F + "/Scenes/Mars_FirstMission_v1.unity";
        public const string AresBase = "Assets/AstroGateway/VehicleShowroom/ARES_VI/Upgraded/ARES_VI_Upgraded.prefab";
        public const string AresVariant = F + "/Prefabs/ARES_VI_FirstMission.prefab";
        const string Anim = "Assets/AstroGateway/VehicleShowroom/ARES_VI/Upgraded/Animation/";
        public const float AresScale = .9f; // ARES wheelbase 2.36 m x track 2.52 m -> close to TERRAVEX 2.10 x 2.20 m
        static string Out => Environment.GetEnvironmentVariable("FM_OUT") ?? Path.GetFullPath("Docs/FirstMission");

        public static void Build()
        {
            var notes = new List<string> { "First Mission build " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "source (read only): " + Source };
            var variant = BuildVariant(notes);

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Scene)) AssetDatabase.DeleteAsset(Scene); // our own scene only
            if (!AssetDatabase.CopyAsset(Source, Scene)) throw new Exception("Scene copy failed");
            EditorSceneManager.OpenScene(Scene);
            var hud = UnityEngine.Object.FindAnyObjectByType<AstroGatewayHUDController>();
            var modes = UnityEngine.Object.FindAnyObjectByType<ProductionModeCoordinator>();
            var legacy = UnityEngine.Object.FindAnyObjectByType<ProductionCameraRig>();
            if (!hud || !modes || !legacy) throw new Exception("Visual Upgrade dependencies missing");
            var p = hud.game.prototype;

            // Scene-local mission registry with the approved canonical id (the Visual Upgrade registry is untouched).
            var registry = UnityEngine.Object.Instantiate(hud.game.registry);
            string regPath = F + "/Settings/FirstMissionRegistry.asset";
            AssetDatabase.DeleteAsset(regPath);
            registry.name = "FirstMissionRegistry";
            registry.missionId = FirstMissionIds.MissionId;
            registry.objectives = new[] { "Board the research rover", "Drive to the research site", "Run rover observations", "Store observations for upload" };
            AssetDatabase.CreateAsset(registry, regPath);
            hud.game.registry = AssetDatabase.LoadAssetAtPath<MissionRegistry>(regPath);
            notes.Add("registry: " + regPath + " mission_id=" + registry.missionId + " (Visual Upgrade keeps mission.red_horizon.visual_upgrade.v1)");

            // ARES-VI visuals on the TERRAVEX rover: hide TERRAVEX renderers (objects, colliders and wheel
            // transforms stay, so MarsPrototype and VisualUpgradeRoverPresentation keep working unchanged).
            int hidden = 0;
            foreach (var r in p.roverVisual.GetComponentsInChildren<Renderer>(true)) { if (r.enabled) hidden++; r.enabled = false; }
            var ares = (GameObject)PrefabUtility.InstantiatePrefab(variant, p.rover.transform);
            ares.name = "ARES-VI visual (First Mission variant, visual only)";
            ares.transform.localPosition = Vector3.zero; ares.transform.localRotation = Quaternion.identity; ares.transform.localScale = Vector3.one * AresScale;
            notes.Add($"TERRAVEX renderers hidden: {hidden}; ARES-VI variant instance under {p.rover.name} scale {AresScale}");
            var cc = p.rover;
            notes.Add($"gameplay collider unchanged: CharacterController radius {cc.radius} height {cc.height} center {cc.center} layer {cc.gameObject.layer}");
            var bounds = new Bounds(ares.transform.position, Vector3.zero);
            foreach (var r in ares.GetComponentsInChildren<Renderer>()) bounds.Encapsulate(r.bounds);
            notes.Add($"ARES visual bounds size {bounds.size} min.y {bounds.min.y - p.rover.transform.position.y:0.000} (relative to rover origin)");

            var systems = new GameObject("First Mission systems");
            var sensors = systems.AddComponent<RoverSensorSuite>();
            sensors.game = hud.game; sensors.stations = hud.environment.targets;
            var pres = systems.AddComponent<AresRoverPresentation>();
            pres.prototype = p; pres.ares = ares.transform;
            Transform Find(string path) => ares.transform.Find(path) ?? throw new Exception("ARES part missing: " + path);
            pres.wheels = new[] { Find("Chassis/Rocker_L/Wheel_FL"), Find("Chassis/Rocker_L/Bogie_L/Wheel_ML"), Find("Chassis/Rocker_L/Bogie_L/Wheel_RL"),
                                  Find("Chassis/Rocker_R/Wheel_FR"), Find("Chassis/Rocker_R/Bogie_R/Wheel_MR"), Find("Chassis/Rocker_R/Bogie_R/Wheel_RR") };
            pres.rockerL = Find("Chassis/Rocker_L"); pres.rockerR = Find("Chassis/Rocker_R");
            pres.bogieL = Find("Chassis/Rocker_L/Bogie_L"); pres.bogieR = Find("Chassis/Rocker_R/Bogie_R");
            pres.armDeploy = AssetDatabase.LoadAssetAtPath<AnimationClip>(Anim + "ARES_ArmDeploy.anim");
            pres.mastScan = AssetDatabase.LoadAssetAtPath<AnimationClip>(Anim + "ARES_ChemCamScan.anim");
            pres.wheelRadius = .38f;
            if (!pres.armDeploy || !pres.mastScan) throw new Exception("ARES clips missing");

            var mission = systems.AddComponent<FirstMissionController>();
            mission.hud = hud; mission.modes = modes; mission.legacyRig = legacy; mission.sensors = sensors; mission.ares = pres;
            mission.catalogJson = AssetDatabase.LoadAssetAtPath<TextAsset>(F + "/Config/sensor_catalog_v1.json");
            if (!mission.catalogJson) throw new Exception("sensor catalog missing");
            // Designated research site: the nearest station at least 25 m from the rover's start, so the mission has a drive leg.
            var start = p.rover.transform.position;
            var ordered = hud.environment.targets.Where(t => t).OrderBy(t => RoverSensorSuite.Planar(t.transform.position, start)).ToList();
            foreach (var t in ordered) notes.Add($"station {t.stableId} '{t.displayName}' at {t.transform.position} distance {RoverSensorSuite.Planar(t.transform.position, start):0.0} m");
            var site = ordered.FirstOrDefault(t => RoverSensorSuite.Planar(t.transform.position, start) >= 25) ?? ordered.First();
            mission.designatedStationId = site.stableId;
            notes.Add("designated research site: " + site.stableId + " " + site.displayName);
            var hudGo = new GameObject("First Mission HUD");
            hudGo.transform.SetParent(systems.transform, false);
            hudGo.AddComponent<FirstMissionHUD>().mission = mission;

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
            notes.Insert(0, "PASS");
            Directory.CreateDirectory(Out);
            File.WriteAllText(Path.Combine(Out, "Build.txt"), string.Join("\n", notes) + "\n");
            Debug.Log("[FirstMission] " + string.Join(" | ", notes));
        }

        // Prefab variant of the showroom ARES-VI: Animator off (no auto-playing showroom clips), hover plumes
        // removed, hover modules inert. Base prefab is not modified.
        static GameObject BuildVariant(List<string> notes)
        {
            var basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AresBase) ?? throw new Exception("ARES-VI upgraded prefab missing");
            Directory.CreateDirectory(Path.GetDirectoryName(AresVariant));
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
            try
            {
                foreach (var a in inst.GetComponentsInChildren<Animator>(true)) a.enabled = false;
                int plumes = 0;
                foreach (var t in inst.GetComponentsInChildren<Transform>(true)) if (t.name.EndsWith("_Plume")) { t.gameObject.SetActive(false); plumes++; }
                inst.name = "ARES_VI_FirstMission";
                var saved = PrefabUtility.SaveAsPrefabAsset(inst, AresVariant, out bool ok);
                if (!ok || !saved) throw new Exception("variant save failed");
                notes.Add($"variant: {AresVariant} (base {AresBase}); Animator disabled; {plumes} hover plumes inactive; is variant: {PrefabUtility.GetPrefabAssetType(saved) == PrefabAssetType.Variant}");
                return saved;
            }
            finally { UnityEngine.Object.DestroyImmediate(inst); }
        }
    }
}
