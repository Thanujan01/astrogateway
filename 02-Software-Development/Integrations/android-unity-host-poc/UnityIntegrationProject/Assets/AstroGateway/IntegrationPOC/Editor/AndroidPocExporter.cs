using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AndroidPocExporter
{
    public static void Export()
    {
        const string scenePath = "Assets/AstroGateway/IntegrationPOC/UnityIntegration_Test.unity";
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var bridge = new GameObject("AstroGatewayBridge"); bridge.AddComponent<AstroGatewayPocBridge>();
        var camera = new GameObject("POC_Camera").AddComponent<Camera>();
        camera.transform.position = new Vector3(0, 2, -6); camera.transform.LookAt(new Vector3(0, 0.5f, 0));
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(0.11f, 0.15f, 0.19f); camera.tag = "MainCamera";
        var light = new GameObject("POC_KeyLight").AddComponent<Light>(); light.type = LightType.Directional; light.transform.rotation = Quaternion.Euler(40, -30, 0);
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube); cube.name = "POC_OneMeterCube"; cube.transform.position = new Vector3(0, 0.5f, 0);
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit")); material.color = new Color(0.76f, 0.44f, 0.22f);
        const string materialPath = "Assets/AstroGateway/IntegrationPOC/POC_Cube.mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(materialPath)) AssetDatabase.DeleteAsset(materialPath);
        AssetDatabase.CreateAsset(material, materialPath); cube.GetComponent<Renderer>().sharedMaterial = material;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);
        PlayerSettings.companyName = "SK2LB-AstroLab"; PlayerSettings.productName = "AstroGateway Integration POC";
        PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, "com.sk2lb.astrogateway.poc");
        PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.Activity;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)36;
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.useCustomKeystore = false;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        EditorUserBuildSettings.buildAppBundle = false;
        var exportPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../../unity-export"));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[]{scenePath}, locationPathName = exportPath, target = BuildTarget.Android, options = BuildOptions.Development | BuildOptions.AcceptExternalModificationsToPlayer });
        Debug.Log("ASTROGATEWAY_EXPORT_RESULT " + report.summary.result + " " + exportPath);
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Android POC export failed: " + report.summary.result);
    }
}
