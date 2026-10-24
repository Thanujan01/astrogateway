#if UNITY_EDITOR
using System;using System.IO;using UnityEditor;using UnityEngine;using UnityEngine.InputSystem;using AstroGateway.Stage08B;
// Passive observation only: never sets controls, transforms or gameplay state.
[InitializeOnLoad]public static class Stage08C1ManualObserver {
 const string Root="/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp";static double next;
 static Stage08C1ManualObserver(){EditorApplication.update+=Poll;}
 static void Poll(){if(!File.Exists(Root+"/work/stage08c1/manual_observe.request")||!EditorApplication.isPlaying||EditorApplication.isPaused||EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.1;var p=UnityEngine.Object.FindAnyObjectByType<MarsPrototype>();if(!p)return;var k=Keyboard.current;string keys="";if(k!=null){if(k.wKey.isPressed)keys+="W";if(k.aKey.isPressed)keys+="A";if(k.sKey.isPressed)keys+="S";if(k.dKey.isPressed)keys+="D";if(k.leftShiftKey.isPressed||k.rightShiftKey.isPressed)keys+="Shift";if(k.eKey.isPressed)keys+="E";if(k.spaceKey.isPressed)keys+="Space";}
 var r=new Row{time=Time.realtimeSinceStartup,position=p.driving?p.rover.transform.position:p.astronaut.transform.position,keys=keys,speed=p.speed,driving=p.driving,yaw=p.yaw,pitch=p.pitch,weather=p.weather,testControl=p.testControl,objective=p.objective};string s=JsonUtility.ToJson(r);File.AppendAllText(Root+"/outputs/Stage08C1/Evidence/Manual_Input_Trace.jsonl",s+"\n");File.WriteAllText(Root+"/work/stage08c1/current_manual_state.json",s);}
 [Serializable]class Row{public float time,speed,yaw,pitch;public Vector3 position;public string keys,objective;public bool driving,testControl;public int weather;}
}
#endif
