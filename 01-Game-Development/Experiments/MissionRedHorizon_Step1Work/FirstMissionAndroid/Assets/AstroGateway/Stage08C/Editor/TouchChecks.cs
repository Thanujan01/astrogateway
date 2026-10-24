#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using AstroGateway.Stage08B;
using AstroGateway.Stage08C;
using System.Collections.Generic;
using System.IO;
public static class TouchChecks {
 public static void Run(MarsPrototype p){var checks=new List<string>();var m=p.mobile;m.preview=true;p.testControl=false;
  System.Action<string,bool> check=(n,ok)=>checks.Add(n+": "+(ok?"PASS":"FAIL"));
  m.Layout(new Rect(80,20,1120,660),1280,720);check("safe area bottom-to-top conversion",m.safe.y==40&&m.safe.x==80);
  check("buttons inside safe area",m.safe.Contains(m.interaction.min)&&m.safe.Contains(m.interaction.max-Vector2.one)&&m.safe.Contains(m.joystick.min));
  var pt=m.joystick.center+new Vector2(300,-300);m.Finger(1,m.joystick.center,Vector2.zero,true,false);m.Finger(1,pt,Vector2.zero,false,false);check("diagonal joystick normalized",Mathf.Abs(m.Move.magnitude-1)<.001f&&m.Move.y>0);
  var prior=m.Move;m.Finger(2,m.joystick.center,Vector2.zero,true,false);check("second finger cannot steal joystick",m.Move==prior);
  m.Finger(3,m.run.center,Vector2.zero,true,false);check("run and movement simultaneous",m.Run&&m.Move.magnitude>.9f);
  m.Finger(3,m.run.center,Vector2.zero,false,true);check("release run preserves movement",!m.Run&&m.Move.magnitude>.9f);
  var camera=new Vector2(m.safe.xMax-40,m.safe.y+250*m.scale);m.Finger(4,camera,new Vector2(20,10),true,false);check("camera independent finger",m.Look.x>0&&m.Look.y<0&&m.Move.magnitude>.9f);
  m.Finger(5,m.throttle.center,Vector2.zero,true,false);m.Finger(6,m.brake.center,Vector2.zero,true,false);check("throttle and brake distinct",m.Accelerate&&m.Brake);
  m.Clear();check("cancellation clears held state",m.Move==Vector2.zero&&m.Look==Vector2.zero&&!m.Run&&!m.Brake&&!m.Accelerate);
  m.Layout(new Rect(0,0,800,360),800,360);check("small landscape layout",m.safe.Contains(m.joystick.min)&&m.safe.Contains(m.brake.min));
  m.Layout(Screen.safeArea,Screen.width,Screen.height);
  var screen=InputSystem.AddDevice<Touchscreen>();var start=m.joystick.center;var screenPos=new Vector2(start.x,Screen.height-start.y);
  InputSystem.QueueStateEvent(screen,new TouchState{touchId=11,phase=UnityEngine.InputSystem.TouchPhase.Began,position=screenPos});InputSystem.Update();m.Frame();
  InputSystem.QueueStateEvent(screen,new TouchState{touchId=11,phase=UnityEngine.InputSystem.TouchPhase.Moved,position=screenPos+new Vector2(45,45)});InputSystem.Update();m.Frame();check("EnhancedTouch virtual-device delivery",m.Move.x>0&&m.Move.y>0);
  InputSystem.QueueStateEvent(screen,new TouchState{touchId=11,phase=UnityEngine.InputSystem.TouchPhase.Canceled,position=screenPos});InputSystem.Update();m.Frame();check("EnhancedTouch cancel delivered",m.Move==Vector2.zero);InputSystem.RemoveDevice(screen);m.Clear();
  File.WriteAllText("/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage08C/Evidence/Touch_Unit_Checks.txt",string.Join("\n",checks));
 }
}
#endif
