using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using AstroGateway.Stage08B;
namespace AstroGateway.Stage08C {
// Presentation/input only. No accounts, network, trusted scores or persistent telemetry.
public sealed class MobileControls : MonoBehaviour {
 public MarsPrototype prototype;
 public bool preview;
 public Vector2 Move {get;private set;}
 public Vector2 Look {get;private set;}
 public bool Run {get;private set;}
 public bool Brake {get;private set;}
 public bool Accelerate {get;private set;}
 public bool Visible=>Application.platform==RuntimePlatform.Android||preview;
 public Rect safe,joystick,run,interaction,throttle,brake;
 public float scale;
 enum Region {Move,Camera,Run,Interaction,Throttle,Brake}
 readonly Dictionary<int,Region> owners=new Dictionary<int,Region>();
 bool enhancedEnabled; RectTransform safeHud; ScreenOrientation priorOrientation; bool changedOrientation;
 public void Initialize(MarsPrototype p){prototype=p;
  UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();enhancedEnabled=true;
  if(Application.platform==RuntimePlatform.Android){priorOrientation=Screen.orientation;Screen.orientation=ScreenOrientation.LandscapeLeft;changedOrientation=true;}
  if(p.hud){var g=new GameObject("Stage08C_SafeArea",typeof(RectTransform));safeHud=g.GetComponent<RectTransform>();safeHud.SetParent(p.hud.transform,false);safeHud.offsetMin=safeHud.offsetMax=Vector2.zero;
   var panel=p.status.transform.parent as RectTransform;panel.SetParent(safeHud,false);
  }
 }
 public void Layout(Rect bottomOriginSafe,int width,int height){
  safe=new Rect(bottomOriginSafe.x,height-bottomOriginSafe.yMax,bottomOriginSafe.width,bottomOriginSafe.height);
  scale=Mathf.Max(.1f,Mathf.Min(safe.width/1280f,safe.height/720f));float size=112*scale,gap=16*scale;
  joystick=new Rect(safe.x+gap,safe.yMax-200*scale-gap,200*scale,200*scale);
  interaction=new Rect(safe.xMax-size-gap,safe.yMax-size-gap,size,size);
  run=new Rect(interaction.x-size-gap,interaction.y,size,size);
  throttle=new Rect(interaction.x,interaction.y-size-gap,size,size);
  brake=new Rect(run.x,throttle.y,size,size);
 }
 public void Frame(){Look=Vector2.zero;Layout(Screen.safeArea,Screen.width,Screen.height);
  if(safeHud){var a=Screen.safeArea;safeHud.anchorMin=new Vector2(a.x/Screen.width,a.y/Screen.height);safeHud.anchorMax=new Vector2(a.xMax/Screen.width,a.yMax/Screen.height);safeHud.offsetMin=safeHud.offsetMax=Vector2.zero;}
  if(!Visible||prototype.testControl){Clear();return;}
  if(Touchscreen.current==null){Clear();return;}
  foreach(var t in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches){var phase=t.phase;if(phase==UnityEngine.InputSystem.TouchPhase.None)continue;
   var pos=t.screenPosition;var delta=t.delta;Finger(t.touchId,new Vector2(pos.x,Screen.height-pos.y),new Vector2(delta.x,-delta.y),phase==UnityEngine.InputSystem.TouchPhase.Began,phase==UnityEngine.InputSystem.TouchPhase.Ended||phase==UnityEngine.InputSystem.TouchPhase.Canceled);
  }
 }
 public void Finger(int id,Vector2 point,Vector2 delta,bool began,bool ended){
  if(ended){if(owners.TryGetValue(id,out var released)&&released==Region.Move)Move=Vector2.zero;owners.Remove(id);Flags();return;}
  if(began&&!owners.ContainsKey(id)&&safe.Contains(point)){
   Region? region=joystick.Contains(point)?Region.Move:interaction.Contains(point)?Region.Interaction:run.Contains(point)?Region.Run:throttle.Contains(point)?Region.Throttle:brake.Contains(point)?Region.Brake:point.x>safe.center.x&&point.y>safe.y+210*scale?(Region?)Region.Camera:null;
   if(region.HasValue&&!owners.ContainsValue(region.Value)){owners[id]=region.Value;if(region==Region.Interaction)prototype.Interact();}
  }
  if(!owners.TryGetValue(id,out var own))return;
  if(own==Region.Move){var v=(point-joystick.center)/(joystick.width*.4f);v.y=-v.y;Move=v.magnitude<.12f?Vector2.zero:Vector2.ClampMagnitude(v,1);}
  if(own==Region.Camera)Look+=new Vector2(delta.x,-delta.y)/Mathf.Max(scale,.1f);
  Flags();
 }
 void Flags(){Run=owners.ContainsValue(Region.Run);Brake=owners.ContainsValue(Region.Brake);Accelerate=owners.ContainsValue(Region.Throttle);}
 public void Clear(){owners.Clear();Move=Look=Vector2.zero;Run=Brake=Accelerate=false;}
 void OnApplicationFocus(bool focus){if(!focus)Clear();}
 void OnApplicationPause(bool paused){if(paused)Clear();}
 void OnDisable(){Clear();if(enhancedEnabled){UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Disable();enhancedEnabled=false;}if(changedOrientation)Screen.orientation=priorOrientation;}
 GUIStyle label;
 void OnGUI(){if(!Visible||prototype==null)return;Layout(Screen.safeArea,Screen.width,Screen.height);
  if(label==null)label=new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter,wordWrap=true};label.fontSize=Mathf.Max(14,Mathf.RoundToInt(23*scale));label.normal.textColor=new Color(.85f,.93f,.95f);
  Box(joystick,"MOVE\nsteer / reverse");var knob=new Rect(joystick.center.x+Move.x*joystick.width*.4f-22*scale,joystick.center.y-Move.y*joystick.height*.4f-22*scale,44*scale,44*scale);GUI.color=new Color(.2f,.7f,.75f,.8f);GUI.DrawTexture(knob,Texture2D.whiteTexture);GUI.color=Color.white;
  Box(run,prototype.driving?"STEER\njoystick":"RUN\nhold");Box(interaction,prototype.driving?"EXIT\nstop first":"ENTER\nnear rover");Box(throttle,"DRIVE\nhold");Box(brake,"BRAKE\nhold");GUI.Label(new Rect(safe.center.x,safe.y+205*scale,safe.width*.45f,55*scale),"Drag right side to orbit",label);
 }
 void Box(Rect rect,string text){GUI.color=new Color(.035f,.085f,.11f,.78f);GUI.DrawTexture(rect,Texture2D.whiteTexture);GUI.color=Color.white;GUI.Label(rect,text,label);}
}
}
