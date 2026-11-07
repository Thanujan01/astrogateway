using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PrototypeBuild
{
    const string Scene = "Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Missions.unity";
    public static void Play()
    {
        EditorSceneManager.OpenScene(Scene);
        EditorApplication.isPlaying = true;
    }
    public static void BuildMac()
    {
        string output = Environment.GetEnvironmentVariable("ASTROGATEWAY_PROTOTYPE_OUTPUT");
        if (string.IsNullOrEmpty(output) || !Path.IsPathRooted(output) || !output.EndsWith(".app"))
            throw new Exception("Set ASTROGATEWAY_PROTOTYPE_OUTPUT to an absolute .app output path");
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        PlayerSettings.companyName = "SK2LB-AstroLab";
        PlayerSettings.productName = "AstroGateway Mission Red Horizon";
        PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Standalone, "com.sk2lb.astrogateway.prototype");
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
        PlayerSettings.defaultIsNativeResolution = false;
        PlayerSettings.defaultScreenWidth = 1600; PlayerSettings.defaultScreenHeight = 900;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] { Scene }, locationPathName = output,
            target = BuildTarget.StandaloneOSX, options = BuildOptions.Development });
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Prototype Mac build failed");
        Debug.Log("ASTROGATEWAY_PROTOTYPE_MAC_BUILD PASS " + output);
    }
}
