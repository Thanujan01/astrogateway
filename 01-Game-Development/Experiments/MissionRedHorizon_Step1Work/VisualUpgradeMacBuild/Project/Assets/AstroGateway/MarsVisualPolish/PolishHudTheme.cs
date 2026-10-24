using UnityEngine;
namespace AstroGateway.VisualPolish {
// Styles only the existing immediate-mode widgets; values and button handlers stay in their original owners.
[DefaultExecutionOrder(-300)] public class PolishHudTheme:MonoBehaviour {
 Texture2D panel,button;GUIStyle oldBox,oldButton,oldLabel;GUISkin shared;
 Texture2D Fill(Color c){var t=new Texture2D(1,1);t.SetPixel(0,0,c);t.Apply();return t;}
 void OnGUI(){if(!shared){shared=GUI.skin;oldBox=new GUIStyle(shared.box);oldButton=new GUIStyle(shared.button);oldLabel=new GUIStyle(shared.label);panel=Fill(new Color(.035f,.06f,.075f,.74f));button=Fill(new Color(.12f,.18f,.2f,.86f));}shared.box.normal.background=panel;shared.button.normal.background=button;shared.button.hover.background=button;shared.button.active.background=button;shared.label.normal.textColor=new Color(.91f,.93f,.92f);shared.button.normal.textColor=new Color(.91f,.93f,.92f);}
 void OnDestroy(){if(shared){shared.box=oldBox;shared.button=oldButton;shared.label=oldLabel;}if(panel)Destroy(panel);if(button)Destroy(button);}
}}
