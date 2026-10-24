using UnityEngine;using AstroGateway.MarsEnvironment;
namespace AstroGateway.VisualPolish {
[DefaultExecutionOrder(220)] public class VisualPolishPresentation:MonoBehaviour {
 public EnvironmentPresentation environment; public int preset=1;
 void Start(){environment.game.testSave=System.IO.Path.Combine(Application.persistentDataPath,"AstroGateway_MarsVisualPolish_v2.json");environment.ApplyQuality(preset);}
 void LateUpdate(){environment.game.prototype.follow.fieldOfView=environment.game.prototype.driving?61:55;}
 void OnGUI(){var r=Screen.safeArea;float w=Mathf.Min(440,r.width*.54f);var box=new Rect(r.center.x-w/2,Screen.height-r.yMin-52,w,46);GUI.backgroundColor=new Color(.13f,.18f,.2f,.85f);GUILayout.BeginArea(box,GUI.skin.box);GUILayout.BeginHorizontal();foreach(var n in new[]{"Low","Medium","High","Desktop"}){int i=System.Array.IndexOf(new[]{"Low","Medium","High","Desktop"},n);if(GUILayout.Button((preset==i?"● ":"")+n,GUILayout.Height(30))){preset=i;environment.ApplyQuality(i);}}GUILayout.EndHorizontal();GUILayout.EndArea();}
}}
