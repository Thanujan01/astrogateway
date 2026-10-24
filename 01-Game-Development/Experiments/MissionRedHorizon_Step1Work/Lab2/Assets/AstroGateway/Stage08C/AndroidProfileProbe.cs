#if UNITY_ANDROID && STAGE08C_PROFILE_HARNESS && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using AstroGateway.Stage08B;
// Compiled only into the isolated profiling APK, never enabled in normal gameplay.
public sealed class AndroidProfileProbe:MonoBehaviour {
 MarsPrototype p;float begin;int stage;bool measuring;readonly List<float> frames=new List<float>();long peak;int particlePeak;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){new GameObject("Stage08C_IsolatedProfilingHarness").AddComponent<AndroidProfileProbe>();}
 IEnumerator Start(){yield return new WaitForSecondsRealtime(5);p=FindAnyObjectByType<MarsPrototype>();Application.targetFrameRate=60;Begin(0);}
 void Begin(int n){stage=n;p.SetWeather(n==0?0:n==1?1:2);if(n==3)p.dust.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);frames.Clear();peak=0;particlePeak=0;begin=Time.realtimeSinceStartup;measuring=true;}
 void Update(){if(!measuring||!p)return;float dt=Time.unscaledDeltaTime;if(dt>0)frames.Add(dt);peak=Math.Max(peak,Profiler.GetTotalAllocatedMemoryLong());particlePeak=Math.Max(particlePeak,p.dust.particleCount);
  if(Time.realtimeSinceStartup-begin<25)return;measuring=false;float seconds=Time.realtimeSinceStartup-begin;var sorted=frames.OrderBy(x=>x).ToArray();int unsupported=FindObjectsByType<Renderer>(FindObjectsSortMode.None).Count(r=>r.sharedMaterials.Any(m=>m&&(!m.shader||!m.shader.isSupported)));
  string json=JsonUtility.ToJson(new Row{stage=stage,label=new[]{"clear","light_dust","storm","storm_particles_disabled"}[stage],frames=frames.Count,seconds=seconds,average_fps=frames.Count/seconds,p95_frame_ms=sorted[(int)((sorted.Length-1)*.95f)]*1000,max_frame_ms=sorted.Last()*1000,peak_unity_allocated_bytes=peak,peak_particles=particlePeak,unsupported_material_renderers=unsupported,width=Screen.width,height=Screen.height});
  string dir=Application.persistentDataPath;File.WriteAllText(Path.Combine(dir,"stage08c_"+stage+".json"),json);ScreenCapture.CaptureScreenshot("stage08c_"+stage+".png");Debug.Log("STAGE08C_PROFILE "+json);if(stage<3)StartCoroutine(Next());else Debug.Log("STAGE08C_PROFILE_COMPLETE");
 }
 IEnumerator Next(){yield return new WaitForSecondsRealtime(2);Begin(stage+1);}
 [Serializable]class Row {public int stage,frames,peak_particles,unsupported_material_renderers,width,height;public string label;public float seconds,average_fps,p95_frame_ms,max_frame_ms;public long peak_unity_allocated_bytes;}
}
#endif
