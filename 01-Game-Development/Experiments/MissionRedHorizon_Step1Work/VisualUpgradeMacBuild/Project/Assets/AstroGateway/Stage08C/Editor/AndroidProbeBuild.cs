#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
public static class AndroidProbeBuild {
 public static void Build(){
  if(!UnityEngine.Application.dataPath.Contains("/work/stage08b/PlayModeLab/"))throw new Exception("Probe build is restricted to the isolated rehearsal copy");
  PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android,"com.sk2lb.astrogateway.stage08cprobe");
  PlayerSettings.productName="AstroGateway Stage08C Probe";PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
  PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Android,"STAGE08C_PROFILE_HARNESS");
  PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
  var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/AstroGateway/Stage08B/MarsOutpost_Playable_Stage08B.unity"},locationPathName="/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage08C/AstroGateway_Stage08C_Probe.apk",target=BuildTarget.Android,options=BuildOptions.Development});
  if(result.summary.result!=BuildResult.Succeeded)throw new Exception("Stage08C isolated APK build failed: "+result.summary.result);
 }
}
#endif
