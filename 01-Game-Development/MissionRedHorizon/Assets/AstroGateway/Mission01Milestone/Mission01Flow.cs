using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
namespace AstroGateway.Mission01 {
// Local onboarding only. This configuration is not an authenticated profile or authoritative mission result.
public class Mission01Flow : MonoBehaviour {
 public GameObject earthPreview,facility,player;public Camera menuCamera,followCamera;public CharacterController controller;public Transform boarding;public Renderer suit;
 public enum Screen {Menu,Setup,Briefing,Launch,Complete,Profile,Training,Settings}
 public Screen screen;public string playerName="Trainee";public int suitColor;public string notice="";public bool paused;
 public int diagnosticTicks;public float diagnosticSeconds; public float yaw,pitch=20;float gravity,fade;string testPath;
 [Serializable] public class Save {public int version=1;public string name;public int color;public bool boarded;}
 string SavePath=>string.IsNullOrEmpty(testPath)?Path.Combine(Application.persistentDataPath,"AstroGateway_Mission01_Local.json"):testPath;
 public void UseTestSave(string path){testPath=path;}
 public bool HasContinue {get {Save s;return Read(out s);}}
 public bool initialized;
 void Start(){suit.sharedMaterial=Instantiate(suit.sharedMaterial);var audio=earthPreview.AddComponent<AudioSource>();var clip=AudioClip.Create("Procedural ambient tone — original",144000,1,24000,false);var samples=new float[144000];for(int i=0;i<samples.Length;i++){float t=i/24000f;samples[i]=.012f*(Mathf.Sin(2*Mathf.PI*80*t)+.5f*Mathf.Sin(2*Mathf.PI*120*t));}clip.SetData(samples,0);audio.clip=clip;audio.loop=true;audio.volume=.25f;audio.Play();SetScreen(Screen.Menu);initialized=true;}
 public void SetScreen(Screen value){screen=value;paused=false;fade=0;bool world=value==Screen.Launch||value==Screen.Complete;facility.SetActive(world);player.SetActive(world);earthPreview.SetActive(!world);menuCamera.enabled=!world;followCamera.enabled=world;Cursor.lockState=CursorLockMode.None;Cursor.visible=true;}
 public void NewMission(){SetScreen(Screen.Setup);}
 public bool ConfirmSetup(){playerName=(playerName??"").Trim();if(playerName.Length<2||playerName.Length>32||Array.Exists(playerName.ToCharArray(),char.IsControl)){notice="Use 2–32 characters without control characters.";return false;}notice="";suit.sharedMaterial.color=new[]{new Color(.83f,.87f,.89f),new Color(.75f,.39f,.2f),new Color(.28f,.45f,.59f)}[Mathf.Clamp(suitColor,0,2)];SetScreen(Screen.Briefing);return true;}
 public void BeginLaunch(){controller.enabled=false;player.transform.position=new Vector3(0,0,-16);controller.enabled=true;gravity=0;SetScreen(Screen.Launch);Persist(false);}
 public bool TryBoard(){if(screen!=Screen.Launch||paused||Vector3.Distance(player.transform.position,boarding.position)>2.8f)return false;SetScreen(Screen.Complete);Persist(true);return true;}
 public bool Continue(){Save s;if(!Read(out s)){notice="No valid local checkpoint is available.";return false;}playerName=s.name;suitColor=s.color;ConfirmSetup();BeginLaunch();if(s.boarded)SetScreen(Screen.Complete);return true;}
 bool Read(out Save s){s=null;try{if(!File.Exists(SavePath)||new FileInfo(SavePath).Length>4096)return false;s=JsonUtility.FromJson<Save>(File.ReadAllText(SavePath));return s!=null&&s.version==1&&!string.IsNullOrWhiteSpace(s.name)&&s.name.Trim().Length>=2&&s.name.Length<=32&&!Array.Exists(s.name.ToCharArray(),char.IsControl)&&s.color>=0&&s.color<=2;}catch{return false;}}
 void Persist(bool boarded){try{Directory.CreateDirectory(Path.GetDirectoryName(SavePath));var temp=SavePath+".tmp";File.WriteAllText(temp,JsonUtility.ToJson(new Save{name=playerName,color=suitColor,boarded=boarded}));if(File.Exists(SavePath))File.Replace(temp,SavePath,null);else File.Move(temp,SavePath);}catch{notice="Local save unavailable. Current session remains playable.";}}
 void Update(){diagnosticTicks++;diagnosticSeconds+=Time.deltaTime;fade=Mathf.Min(1,fade+Time.unscaledDeltaTime*3);if(earthPreview.activeSelf)earthPreview.transform.Rotate(0,Time.deltaTime*3,0);if(screen!=Screen.Launch)return;var k=Keyboard.current;var m=Mouse.current;if(k!=null&&k.escapeKey.wasPressedThisFrame){paused=!paused;Cursor.lockState=CursorLockMode.None;Cursor.visible=true;}if(paused)return;
 if(k!=null&&!k.leftCtrlKey.isPressed&&!k.leftMetaKey.isPressed){var v=Vector2.ClampMagnitude(new Vector2((k.dKey.isPressed?1:0)-(k.aKey.isPressed?1:0),(k.wKey.isPressed?1:0)-(k.sKey.isPressed?1:0)),1);var move=Quaternion.Euler(0,yaw,0)*new Vector3(v.x,0,v.y);if(controller.isGrounded&&gravity<0)gravity=-2;if(controller.isGrounded&&k.spaceKey.wasPressedThisFrame)gravity=5;gravity-=9.81f*Time.deltaTime;controller.Move((move*(k.leftShiftKey.isPressed?6:3.5f)+Vector3.up*gravity)*Time.deltaTime);if(move.sqrMagnitude>.01f)player.transform.rotation=Quaternion.LookRotation(move);if(k.eKey.wasPressedThisFrame)TryBoard();}
 if(m!=null&&m.rightButton.isPressed){var look=m.delta.ReadValue();yaw+=look.x*.12f;pitch=Mathf.Clamp(pitch-look.y*.12f,5,55);}
 }
 void LateUpdate(){if(!followCamera.enabled)return;var target=player.transform.position+Vector3.up*1.3f;var offset=Quaternion.Euler(pitch,yaw,0)*new Vector3(0,0,-4.5f);var desired=target+offset;if(Physics.SphereCast(target,.2f,offset.normalized,out var hit,offset.magnitude,1<<0,QueryTriggerInteraction.Ignore))desired=target+offset.normalized*Mathf.Max(.5f,hit.distance-.2f);followCamera.transform.position=desired;followCamera.transform.LookAt(target);}
 GUISkin theme;
 Texture2D Solid(Color c){var t=new Texture2D(2,2);t.SetPixels(new[]{c,c,c,c});t.Apply();return t;}
 void OnGUI(){if(!theme){theme=Instantiate(GUI.skin);theme.label.fontSize=18;theme.label.wordWrap=true;theme.label.normal.textColor=new Color(.86f,.9f,.94f);theme.button.fontSize=18;theme.button.fixedHeight=40;theme.button.normal.background=Solid(new Color(.12f,.19f,.26f));theme.button.normal.textColor=Color.white;theme.button.hover.background=Solid(new Color(.25f,.35f,.43f));theme.box.normal.background=Solid(new Color(.035f,.06f,.09f,.94f));theme.textField.fontSize=18;theme.textField.fixedHeight=38;}GUI.skin=theme;float scale=Mathf.Clamp(UnityEngine.Screen.width/1100f,.65f,1.8f);GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,Vector3.one*scale);GUI.color=new Color(1,1,1,fade);GUILayout.BeginArea(new Rect(30,30,420,610),GUI.skin.box);GUILayout.Label("ASTROGATEWAY  /  OPERATION RED HORIZON");GUILayout.Label("LOCAL FIRST-MILESTONE PROTOTYPE");GUILayout.Space(18);
 switch(screen){case Screen.Menu:GUILayout.Label("Earth is the beginning. Mars is the horizon.");if(GUILayout.Button("New Mission",GUILayout.Height(42)))NewMission();GUI.enabled=HasContinue;if(GUILayout.Button("Continue Mission",GUILayout.Height(42)))Continue();GUI.enabled=true;if(GUILayout.Button("Player Profile"))SetScreen(Screen.Profile);if(GUILayout.Button("Training"))SetScreen(Screen.Training);if(GUILayout.Button("Settings"))SetScreen(Screen.Settings);if(GUILayout.Button("Exit Game")){
#if UNITY_EDITOR
 UnityEditor.EditorApplication.isPlaying=false;
#else
 Application.Quit();
#endif
}break;
 case Screen.Setup:GUILayout.Label("Local mission callsign — not your account profile");playerName=GUILayout.TextField(playerName,32);GUILayout.Label("Suit color (one prototype appearance)");suitColor=GUILayout.SelectionGrid(suitColor,new[]{"Polar","Ochre","Blue"},3);if(GUILayout.Button("Confirm and Continue",GUILayout.Height(42)))ConfirmSetup();if(GUILayout.Button("Back"))SetScreen(Screen.Menu);break;
 case Screen.Briefing:GUILayout.Label("MISSION 01 — OPERATION RED HORIZON");GUILayout.Label("Join the Mars Exploration Program. First, reach the boarding console and confirm readiness.");GUILayout.Space(12);GUILayout.Label("Equipment: EVA suit, oxygen supply, radio.\nHazards: launch exclusion zones, system faults.\nSurvival: verify oxygen and power before launch.\nReward: local boarding checkpoint (provisional).\nLater travel and landing are not implemented.");if(GUILayout.Button("Enter Earth Launch Site",GUILayout.Height(42)))BeginLaunch();if(GUILayout.Button("Back"))SetScreen(Screen.Setup);break;
 case Screen.Launch:GUILayout.Label("OBJECTIVE 01: Reach rocket boarding console");GUILayout.Label("Distance: "+Vector3.Distance(player.transform.position,boarding.position).ToString("F1")+" m");GUILayout.Label("WASD walk · Shift run · Space jump\nRMB orbit · E confirm boarding · Esc pause");GUILayout.Label("Oxygen 100% / Power 100% — briefing values, not a survival simulation");if(Vector3.Distance(player.transform.position,boarding.position)<2.8f)GUILayout.Label("[E] Confirm readiness at boarding console");if(paused){GUILayout.Label("PAUSED");if(GUILayout.Button("Resume"))paused=false;if(GUILayout.Button("Main menu"))SetScreen(Screen.Menu);}break;
 case Screen.Complete:GUILayout.Label("FIRST OBJECTIVE COMPLETE");GUILayout.Label("Boarding readiness confirmed. Local checkpoint saved.\nRocket flight, space travel and Mars landing are future milestones.");if(GUILayout.Button("Return to Main Menu"))SetScreen(Screen.Menu);break;
 case Screen.Profile:GUILayout.Label("Local callsign: "+playerName+"\nAccount identity remains owned by the existing Web/Android host. This screen does not sign in or alter Supabase profiles.");if(GUILayout.Button("Back"))SetScreen(Screen.Menu);break;
 case Screen.Training:GUILayout.Label("Practice: move, orbit camera, approach console and press E. No trained AI or authoritative score is used here.");if(GUILayout.Button("Back"))SetScreen(Screen.Menu);break;
 case Screen.Settings:GUILayout.Label("Frame cap (requested, not measured)");if(GUILayout.Button("30 FPS cap"))Application.targetFrameRate=30;if(GUILayout.Button("60 FPS cap"))Application.targetFrameRate=60;if(GUILayout.Button("Back"))SetScreen(Screen.Menu);break;}
 GUILayout.Label(notice);GUILayout.EndArea();GUI.color=Color.white;}
 }
}
