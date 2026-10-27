using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class MissionsAndroidBuild
{
    public const string Scene = "Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Missions.unity";
    static void Configure()
    {
        if (!File.Exists(Scene)) throw new Exception("Latest Missions scene missing");
        PlayerSettings.companyName = "SK2LB-AstroLab";
        PlayerSettings.productName = "AstroGateway Mission Red Horizon";
        PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, "com.sk2lb.astrogateway");
        PlayerSettings.bundleVersion = "0.3.0"; PlayerSettings.Android.bundleVersionCode = 3;
        PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.Activity;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)36;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.useCustomKeystore = false; // Development only. Release signing is operator-managed.
        EditorUserBuildSettings.buildAppBundle = false;
    }
    public static void ExportHost()
    {
        Configure(); EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        string target = Environment.GetEnvironmentVariable("ASTROGATEWAY_UNITY_EXPORT");
        if (string.IsNullOrEmpty(target) || !Path.IsPathRooted(target)) throw new Exception("Set absolute ASTROGATEWAY_UNITY_EXPORT in a staging workspace");
        Build(target, BuildOptions.AcceptExternalModificationsToPlayer);
    }
    public static void BuildApk()
    {
        Configure(); EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        string target = Environment.GetEnvironmentVariable("ASTROGATEWAY_APK_OUTPUT");
        if (string.IsNullOrEmpty(target) || !Path.IsPathRooted(target)) throw new Exception("Set absolute ASTROGATEWAY_APK_OUTPUT in a staging workspace");
        Build(target, BuildOptions.None);
    }
    static void Build(string target, BuildOptions options)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(target));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] { Scene }, locationPathName = target, target = BuildTarget.Android, options = options | BuildOptions.Development });
        Debug.Log("ASTROGATEWAY_MISSIONS_BUILD " + report.summary.result);
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Missions Android build failed");
        if (EditorUserBuildSettings.exportAsGoogleAndroidProject)
            File.WriteAllText(Path.Combine(target, "astrogateway-missions-export.json"), "{\"schema_version\":1,\"scene\":\"Mars_FirstMission_Missions\",\"application_id\":\"com.sk2lb.astrogateway\",\"version\":\"0.3.0\"}");
    }
}
