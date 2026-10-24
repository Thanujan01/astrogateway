using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public static class Stage08D1Builder {
 const string A="Assets/ApprovedAssets/";
 static string Out=>Path.GetFullPath("../../../outputs/Stage08D11");
 static List<string> checks=new List<string>();
 static Camera cam;static GameObject rover;static GameObject original;
 [MenuItem("AstroGateway/Stage08D1/Build Evaluation")]
 public static void Build(){
  Directory.CreateDirectory(Out+"/Screenshots"); Directory.CreateDirectory("Assets/Evaluation");
  AssetDatabase.Refresh();
  var rp=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/Mobile_RPAsset.asset");
  GraphicsSettings.defaultRenderPipeline=rp;QualitySettings.renderPipeline=rp;
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  RenderSettings.ambientMode=AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.44f,.36f,.29f);RenderSettings.ambientEquatorColor=new Color(.28f,.23f,.19f);RenderSettings.ambientGroundColor=new Color(.14f,.11f,.09f);
  RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogColor=new Color(.57f,.42f,.30f);RenderSettings.fogDensity=.0045f;
  RenderSettings.skybox=null;var sun=new GameObject("Afternoon Sun").AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=2f;sun.color=new Color(1,.92f,.82f);sun.transform.rotation=Quaternion.Euler(42,-38,0);sun.shadows=LightShadows.Soft;RenderSettings.sun=sun;
  var ground=GameObject.CreatePrimitive(PrimitiveType.Plane);ground.name="Approved Sandy Gravel — 60m Surface";ground.transform.localScale=new Vector3(6,1,6);ground.GetComponent<Renderer>().sharedMaterial=Ground();
  rover=Group("Perseverance");rover.name="Approved Robotic Perseverance — LOD Group";FitGround(rover);
  original=Load("Perseverance_Original_PNG");original.name="Source Fidelity Reference — Rover";FitGround(original);original.SetActive(false);
  var stones=new GameObject("Approved Small Stones");var random=new System.Random(83);
  for(int i=0;i<48;i++){var stone=Group(i%2==0?"rock_07":"rock_09");stone.transform.SetParent(stones.transform);float x=(float)(random.NextDouble()*22-11),z=(float)(random.NextDouble()*22-11);if(Mathf.Abs(x)<2&&Mathf.Abs(z)<3)x+=4;FitGround(stone);stone.transform.position+=new Vector3(x,0,z);stone.transform.rotation=Quaternion.Euler(0,(float)random.NextDouble()*360,0);stone.transform.localScale=Vector3.one*(float)(.8+random.NextDouble()*.6);}
  var volume=new GameObject("Global — restrained grading").AddComponent<Volume>();volume.isGlobal=true;var profile=ScriptableObject.CreateInstance<VolumeProfile>();var color=profile.Add<ColorAdjustments>();color.saturation.Override(-8);color.contrast.Override(5);var tone=profile.Add<Tonemapping>();tone.mode.Override(TonemappingMode.Neutral);if(AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Evaluation/MarsVolume.asset")==null)AssetDatabase.CreateAsset(profile,"Assets/Evaluation/MarsVolume.asset");else profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Evaluation/MarsVolume.asset");volume.sharedProfile=profile;
  cam=new GameObject("Evaluation Camera").AddComponent<Camera>();cam.tag="MainCamera";cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=RenderSettings.fogColor;cam.fieldOfView=42;cam.nearClipPlane=.05f;cam.farClipPlane=160;cam.allowHDR=false;cam.GetUniversalAdditionalCameraData().renderPostProcessing=true;
  var b=BoundsOf(rover);checks.Add("Rover world bounds "+b);cam.transform.position=b.center+new Vector3(5,3,-6);cam.transform.LookAt(b.center);
  foreach(var r in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None)){foreach(var m in r.sharedMaterials){if(m==null||m.shader==null||!m.shader.isSupported)throw new Exception("Invalid material: "+r.name); } }
  checks.Add("PASS material slots nonnull and shaders supported");checks.Add("Pipeline "+GraphicsSettings.currentRenderPipeline.name);checks.Add("LOD Groups "+UnityEngine.Object.FindObjectsByType<LODGroup>(FindObjectsSortMode.None).Length);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),"Assets/Evaluation/Stage08D1_Visual_Evaluation.unity");AssetDatabase.SaveAssets();
  Capture("Composition",b.center+new Vector3(8,5,-10),b.center);Capture("Rover_LOD0",b.center+new Vector3(4,2.4f,-4.5f),b.center);rover.SetActive(false);original.SetActive(true);Capture("Rover_Original",b.center+new Vector3(4,2.4f,-4.5f),b.center);original.SetActive(false);rover.SetActive(true);
  Capture("Rover_LOD1",b.center+new Vector3(4,2.4f,-4.5f),b.center,1);Capture("Rover_LOD2",b.center+new Vector3(4,2.4f,-4.5f),b.center,2);
  Capture("Rover_Rear",b.center+new Vector3(-4,2.5f,5),b.center);Capture("Rock_Ground",new Vector3(4,.6f,-.8f),new Vector3(4,.1f,1));
  CompareRock("rock_07",new Vector3(-2,0,0));CompareRock("rock_09",new Vector3(2,0,0));
  File.WriteAllLines(Out+"/Scene_Checks.txt",checks);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());Debug.Log("STAGE08D1_BUILD_PASS");
 }
 static GameObject Load(string name){var asset=AssetDatabase.LoadAssetAtPath<GameObject>(A+name+(name.StartsWith("Perseverance_LOD")?"_PNG":"")+".glb");if(asset==null)throw new Exception("Import missing "+name);return (GameObject)PrefabUtility.InstantiatePrefab(asset);}
 static GameObject Group(string name){var root=new GameObject(name);var group=root.AddComponent<LODGroup>();var lods=new LOD[3];for(int i=0;i<3;i++){var o=Load(name+"_LOD"+i);o.transform.SetParent(root.transform,false);lods[i]=new LOD(new[]{.5f,.2f,.035f}[i],o.GetComponentsInChildren<Renderer>());group.SetLODs(lods.Take(i+1).ToArray());}group.SetLODs(lods);group.RecalculateBounds();return root;}
 static Bounds BoundsOf(GameObject o){var rs=o.GetComponentsInChildren<Renderer>();var b=rs[0].bounds;foreach(var r in rs)b.Encapsulate(r.bounds);return b;}
 static void FitGround(GameObject o){var b=BoundsOf(o);o.transform.position+=new Vector3(-b.center.x,-b.min.y,-b.center.z);}
 static Material Ground(){string p=A+"Ground/";foreach(var f in Directory.GetFiles(p,"*.png")){var importer=(TextureImporter)AssetImporter.GetAtPath(f);importer.isReadable=true;importer.sRGBTexture=!f.Contains("nor_gl")&&!f.Contains("Rough")&&!f.Contains("AO");if(f.Contains("nor_gl"))importer.textureType=TextureImporterType.NormalMap;importer.SaveAndReimport();}
  var material=new Material(Shader.Find("Universal Render Pipeline/Lit"));var diffuse=AssetDatabase.LoadAssetAtPath<Texture2D>(p+"sandy_gravel_Diffuse.png");material.SetTexture("_BaseMap",diffuse);material.SetColor("_BaseColor",new Color(.7f,.56f,.42f));material.SetTextureScale("_BaseMap",new Vector2(30,30));material.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>(p+"sandy_gravel_nor_gl.png"));material.EnableKeyword("_NORMALMAP");material.SetTextureScale("_BumpMap",new Vector2(30,30));
  var rough=AssetDatabase.LoadAssetAtPath<Texture2D>(p+"sandy_gravel_Rough.png");var px=rough.GetPixels();for(int i=0;i<px.Length;i++)px[i]=new Color(0,0,0,1-px[i].r);var mask=new Texture2D(rough.width,rough.height,TextureFormat.RGBA32,false,true);mask.SetPixels(px);mask.Apply();File.WriteAllBytes(p+"Generated_Smoothness.png",mask.EncodeToPNG());AssetDatabase.ImportAsset(p+"Generated_Smoothness.png");var ti=(TextureImporter)AssetImporter.GetAtPath(p+"Generated_Smoothness.png");ti.sRGBTexture=false;ti.SaveAndReimport();material.SetTexture("_MetallicGlossMap",AssetDatabase.LoadAssetAtPath<Texture2D>(p+"Generated_Smoothness.png"));material.EnableKeyword("_METALLICSPECGLOSSMAP");material.SetTextureScale("_MetallicGlossMap",new Vector2(30,30));material.SetTexture("_OcclusionMap",AssetDatabase.LoadAssetAtPath<Texture2D>(p+"sandy_gravel_AO.png"));material.SetTextureScale("_OcclusionMap",new Vector2(30,30));var existing=AssetDatabase.LoadAssetAtPath<Material>("Assets/Evaluation/Ground.mat");if(existing==null)AssetDatabase.CreateAsset(material,"Assets/Evaluation/Ground.mat");else {EditorUtility.CopySerialized(material,existing);material=existing;}return material;
 }
 static void Capture(string name,Vector3 position,Vector3 target,int lod=0){cam.transform.position=position;cam.transform.LookAt(target);foreach(var g in UnityEngine.Object.FindObjectsByType<LODGroup>(FindObjectsSortMode.None))g.ForceLOD(lod);var rt=new RenderTexture(1600,1000,24,RenderTextureFormat.ARGB32);cam.targetTexture=rt;cam.Render();RenderTexture.active=rt;var tex=new Texture2D(1600,1000,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1600,1000),0,0);tex.Apply();File.WriteAllBytes(Out+"/Screenshots/"+name+".png",tex.EncodeToPNG());cam.targetTexture=null;RenderTexture.active=null;UnityEngine.Object.DestroyImmediate(tex);UnityEngine.Object.DestroyImmediate(rt);foreach(var g in UnityEngine.Object.FindObjectsByType<LODGroup>(FindObjectsSortMode.None))g.ForceLOD(-1);checks.Add("Captured "+name);}
 static void CompareRock(string n,Vector3 pos){var a=Load(n+"_Original");FitGround(a);a.transform.position+=pos;var b=BoundsOf(a);Capture(n+"_Original",b.center+new Vector3(.35f,.22f,-.4f),b.center);a.SetActive(false);var c=Load(n+"_LOD0");FitGround(c);c.transform.position+=pos;Capture(n+"_LOD0",b.center+new Vector3(.35f,.22f,-.4f),b.center);UnityEngine.Object.DestroyImmediate(a);UnityEngine.Object.DestroyImmediate(c);}
}
