#if UNITY_EDITOR
using System;using System.IO;using System.Linq;using System.Collections.Generic;using UnityEngine;using UnityEditor;using Unity.Profiling;using Unity.Profiling.LowLevel.Unsafe;
// Explicitly requested, Editor-only observation. Never included in player builds.
[InitializeOnLoad]public static class Stage08C1EditorObserver {
 const string Root="/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp";
 static CounterProbe probe;static double started;static int first,last,gc0,gc1,gc2;static bool warm;static readonly List<double> intervals=new List<double>();static double previous;
 static Stage08C1EditorObserver(){EditorApplication.update+=Poll;AssemblyReloadEvents.beforeAssemblyReload+=Dispose;}
 static void Dispose(){probe?.Dispose();probe=null;}
 static void Poll(){
 if(probe==null){string req=Root+"/work/stage08c1/editor_profile.request";if(!File.Exists(req))return;File.Delete(req);if(!EditorApplication.isPlaying||EditorApplication.isPaused){File.WriteAllText(Root+"/outputs/Stage08C1/Evidence/Editor_Observer_Error.txt","Play Mode must be running and unpaused");return;}probe=new CounterProbe();started=EditorApplication.timeSinceStartup;warm=true;intervals.Clear();return;}
 if(!EditorApplication.isPlaying||EditorApplication.isPaused){Dispose();return;}
 double now=EditorApplication.timeSinceStartup;if(warm){if(now-started<5)return;warm=false;started=previous=now;first=last=Time.frameCount;gc0=GC.CollectionCount(0);gc1=GC.CollectionCount(1);gc2=GC.CollectionCount(2);probe.Reset();}
 if(Time.frameCount!=last){probe.Sample();intervals.Add(now-previous);previous=now;last=Time.frameCount;}
 if(now-started<25)return;
 var sorted=intervals.OrderBy(x=>x).ToArray();var row=new Row{scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene().path,device=SystemInfo.graphicsDeviceName,api=SystemInfo.graphicsDeviceType.ToString(),seconds=now-started,frames=last-first,fps=(last-first)/(now-started),sampled_intervals=sorted.Length,p95_observer_interval_ms=sorted.Length>0?sorted[(int)((sorted.Length-1)*.95)]*1000:0,counters=probe.Results(),gc0=GC.CollectionCount(0)-gc0,gc1=GC.CollectionCount(1)-gc1,gc2=GC.CollectionCount(2)-gc2,allocated_bytes=UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong(),reserved_bytes=UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong()};
 File.WriteAllText(Root+"/outputs/Stage08C1/Evidence/Editor_Performance.json",JsonUtility.ToJson(row,true));Dispose();
 }
 [Serializable]class Row{public string scene,device,api;public double seconds,fps,p95_observer_interval_ms;public int frames,sampled_intervals,gc0,gc1,gc2;public long allocated_bytes,reserved_bytes;public CounterResult[] counters;}
}
[Serializable]public class CounterResult {public string name,unit;public int samples;public double mean,max;}
public class CounterProbe:IDisposable {
 readonly List<ProfilerRecorder> recorders=new List<ProfilerRecorder>();readonly List<CounterResult> results=new List<CounterResult>();
 public CounterProbe(){var handles=new List<ProfilerRecorderHandle>();ProfilerRecorderHandle.GetAvailable(handles);var wanted=new[]{"Main Thread","Render Thread","CPU Main Thread Frame Time","CPU Render Thread Frame Time","GPU Frame Time","Batches Count","Draw Calls Count","SetPass Calls Count","Triangles Count","Vertices Count","GC Allocated In Frame","GC Used Memory","GC Reserved Memory","GC.Collect"};
 foreach(var name in wanted){var found=false;foreach(var h in handles){var d=ProfilerRecorderHandle.GetDescription(h);if(d.Name!=name)continue;recorders.Add(ProfilerRecorder.StartNew(d.Category,name,1));results.Add(new CounterResult{name=name,unit=d.UnitType.ToString()});found=true;break;}if(!found){recorders.Add(default);results.Add(new CounterResult{name=name,unit="unavailable"});}}
 }
 public void Reset(){foreach(var r in results){r.samples=0;r.mean=0;r.max=0;}}
 public void Sample(){for(int i=0;i<recorders.Count;i++){var p=recorders[i];if(!p.Valid||p.Count==0)continue;var r=results[i];double v=p.LastValue;r.samples++;r.mean+=(v-r.mean)/r.samples;r.max=Math.Max(r.max,v);}}
 public CounterResult[] Results(){return results.ToArray();}
 public void Dispose(){foreach(var p in recorders)p.Dispose();}
}
#endif
