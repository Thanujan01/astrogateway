using UnityEditor;using UnityEditor.Build.Reporting;using UnityEngine;
// Build-copy only (never in the main project). Builds Mars_VisualUpgrade_v1 as a macOS development player.
public static class VisualUpgradeMacBuild {
 public static void Build(){
  PlayerSettings.productName="AstroGateway_VisualUpgradeBuild";PlayerSettings.runInBackground=true;// build copy only: lets the probe finish if focus moves away
  PlayerSettings.enableFrameTimingStats=true;// build copy only: CPU/GPU frame times for the probe
  var o=new BuildPlayerOptions{scenes=new[]{"Assets/AstroGateway/VisualUpgrade/Scenes/Mars_VisualUpgrade_v1.unity"},locationPathName=System.Environment.GetEnvironmentVariable("MAC_BUILD_OUT")+"/AstroGateway_VisualUpgrade.app",target=BuildTarget.StandaloneOSX,options=BuildOptions.Development};
  var r=BuildPipeline.BuildPlayer(o);var s=r.summary;
  Debug.Log($"[MacBuild] result={s.result} size={s.totalSize} time={s.totalTime} errors={s.totalErrors} warnings={s.totalWarnings} out={s.outputPath}");
  EditorApplication.Exit(s.result==BuildResult.Succeeded?0:1);}}
