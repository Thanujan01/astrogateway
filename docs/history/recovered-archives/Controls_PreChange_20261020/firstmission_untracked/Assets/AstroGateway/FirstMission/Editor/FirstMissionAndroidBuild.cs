using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Android development build of the First Mission scene. RUN ONLY ON AN ISOLATED COPY of the project:
// it switches the build target and changes Player settings (package id, landscape-only, IL2CPP ARM64),
// which must not happen in the main project without approval. Refuses to run in the main project folder.
// Env FM_APK (output path). Not signed for release, not uploaded anywhere.
namespace AstroGateway.FirstMission.EditorTools
{
    public static class FirstMissionAndroidBuild
    {
        public static void Build()
        {
            string root = Path.GetFullPath(".");
            if (root.TrimEnd('/').EndsWith("/Projects/MissionRedHorizon")) throw new Exception("Refusing to change build settings in the main project; use a copy.");
            string apk = Environment.GetEnvironmentVariable("FM_APK") ?? Path.Combine(root, "Build", "AstroGateway_FirstMission_dev.apk");
            Directory.CreateDirectory(Path.GetDirectoryName(apk));
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android)) throw new Exception("Android target switch failed");
            var android = NamedBuildTarget.Android;
            PlayerSettings.SetApplicationIdentifier(android, "com.astrogateway.firstmission.dev");
            PlayerSettings.productName = "AstroGateway First Mission (dev)";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false; PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true; PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.SetScriptingBackend(android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.renderOutsideSafeArea = true; // layout uses Screen.safeArea itself
            PlayerSettings.insecureHttpOption = InsecureHttpOption.NotAllowed;
            // The scene swaps in its own URP asset at runtime (MarsPrototype.previewPipeline, with SSAO). URP's build
            // stripper only sees the quality-level assets, so without this the SSAO resources are stripped and the
            // player renders black on Android (seen on the first device run). Copy project only.
            var sceneUrp = AssetDatabase.LoadAssetAtPath<UnityEngine.Rendering.RenderPipelineAsset>("Assets/AstroGateway/VisualUpgrade/Settings/VisualUpgrade_URP.asset") ?? throw new Exception("scene URP asset missing");
            for (int q = 0; q < QualitySettings.names.Length; q++) { QualitySettings.SetQualityLevel(q, false); QualitySettings.renderPipeline = sceneUrp; }
            QualitySettings.SetQualityLevel(0, false);
            var options = new BuildPlayerOptions
            {
                scenes = new[] { FirstMissionBuilder.Scene },
                locationPathName = apk,
                target = BuildTarget.Android,
                options = BuildOptions.Development,
            };
            EditorUserBuildSettings.buildAppBundle = false;
            var report = BuildPipeline.BuildPlayer(options);
            var s = report.summary;
            string text = $"Android development build — {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nresult {s.result}\noutput {apk}\nsize_bytes {s.totalSize}\nerrors {s.totalErrors} warnings {s.totalWarnings}\nduration {s.totalTime}\n" +
                          "scenes " + string.Join(",", options.scenes) + "\nIL2CPP ARM64, minSdk 26, landscape only, development build, unsigned for release.\n" +
                          string.Join("\n", report.steps.SelectMany(st => st.messages).Where(m => m.type == LogType.Error || m.type == LogType.Exception).Select(m => "ERROR " + m.content.Split('\n')[0]).Take(30));
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(apk), "Android_Build_Report.txt"), text);
            Debug.Log("[FirstMissionAndroid] " + text);
            if (Application.isBatchMode) EditorApplication.Exit(s.result == BuildResult.Succeeded ? 0 : 1);
        }
    }
}
