using UnityEngine;
namespace AstroGateway.CinematicHUD {
// Temporary QA-only component, added by the editor harness, never saved in the scene/prefab.
[DefaultExecutionOrder(450)]public class HUDLayoutProbe:MonoBehaviour {
 public AstroGatewayHUDController hud;public bool request,complete;public Vector2 viewport;public Rect safe;public bool[] inside=new bool[10];public readonly string[] names={"Mission branding","Survival resources","Weather and clock","Mission objectives","Circular minimap panel","Analog movement joystick","Scan","Context interact","Rover telemetry","Recent scientific scan"};
 void LateUpdate(){if(!request)return;request=false;Canvas.ForceUpdateCanvases();viewport=new Vector2(Screen.width,Screen.height);safe=hud.simulateSafeArea?hud.simulatedSafeArea:Screen.safeArea;for(int i=0;i<names.Length;i++){var r=hud.safe.Find(names[i]) as RectTransform;var corners=new Vector3[4];if(r)r.GetWorldCorners(corners);bool ok=r;foreach(var c in corners)ok&=c.x>=safe.xMin-2&&c.x<=safe.xMax+2&&c.y>=safe.yMin-2&&c.y<=safe.yMax+2;inside[i]=ok;}complete=true;}
}}
