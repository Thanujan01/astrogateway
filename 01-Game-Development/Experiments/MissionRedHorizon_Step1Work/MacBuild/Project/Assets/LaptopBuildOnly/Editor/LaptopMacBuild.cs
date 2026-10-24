using UnityEditor;using UnityEditor.Build.Reporting;using UnityEngine;
public static class LaptopMacBuild {
 public static void Build(){
  var o=new BuildPlayerOptions{scenes=new[]{"Assets/AstroGateway/ProductionVisual/Scenes/Mars_ProductionVisual_v1.unity"},locationPathName=System.Environment.GetEnvironmentVariable("MAC_BUILD_OUT")+"/AstroGateway_Dev.app",target=BuildTarget.StandaloneOSX,options=BuildOptions.Development};
  var r=BuildPipeline.BuildPlayer(o);var s=r.summary;
  Debug.Log($"[MacBuild] result={s.result} size={s.totalSize} time={s.totalTime} errors={s.totalErrors} warnings={s.totalWarnings} out={s.outputPath}");
  EditorApplication.Exit(s.result==BuildResult.Succeeded?0:1);}}
