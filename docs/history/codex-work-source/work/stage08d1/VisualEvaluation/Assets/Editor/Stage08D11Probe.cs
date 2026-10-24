using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
public static class Stage08D11Probe {
 public static void Validate(){
  var lines=new System.Collections.Generic.List<string>();
  for(int i=0;i<3;i++){
   var path="Assets/ApprovedAssets/Perseverance_LOD"+i+"_PNG.glb";var go=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(go==null)throw new Exception("Missing prefab "+path);
   var all=AssetDatabase.LoadAllAssetsAtPath(path);var meshes=all.OfType<Mesh>().ToArray();var materials=all.OfType<Material>().ToArray();var textures=all.OfType<Texture2D>().ToArray();
   long tris=0;foreach(var mesh in meshes)for(int s=0;s<mesh.subMeshCount;s++)tris+=mesh.GetIndexCount(s)/3;
   int nullSlots=go.GetComponentsInChildren<Renderer>(true).Sum(r=>r.sharedMaterials.Count(m=>m==null));if(nullSlots!=0)throw new Exception("Null material slots");
   lines.Add($"LOD{i}: prefab PASS; meshes={meshes.Length}; triangles={tris}; materials={materials.Length}; textures={textures.Length}; nullMaterialSlots={nullSlots}");
   foreach(var t in textures)lines.Add($"LOD{i} texture {t.name}: {t.width}x{t.height}, {t.format}");
   foreach(var m in materials){if(!m.shader.isSupported)throw new Exception("Unsupported shader "+m.shader.name);lines.Add($"LOD{i} material {m.name}: shader={m.shader.name}, queue={m.renderQueue}");}
  }
  var scene=EditorSceneManager.OpenScene("Assets/Evaluation/Stage08D1_Visual_Evaluation.unity");lines.Add("Saved scene reload PASS");
  var root=scene.GetRootGameObjects().First(o=>o.name.StartsWith("Approved Robotic"));var group=root.GetComponent<LODGroup>();if(group==null||group.GetLODs().Length!=3)throw new Exception("LOD Group invalid");foreach(var lod in group.GetLODs())if(lod.renderers.Length==0||lod.renderers.Any(r=>r==null))throw new Exception("LOD renderer missing");lines.Add("Rover 3 LOD renderer references PASS");
  var rs=root.GetComponentsInChildren<Renderer>();var b=rs[0].bounds;foreach(var r in rs)b.Encapsulate(r.bounds);var cam=UnityEngine.Object.FindFirstObjectByType<Camera>();cam.transform.position=b.center+new Vector3(5,3,-6);cam.transform.LookAt(b.center);EditorSceneManager.SaveScene(scene);File.WriteAllLines(Path.GetFullPath("../../../outputs/Stage08D11/Unity_Asset_Checks.txt"),lines);Debug.Log("STAGE08D11_IMPORT_SCENE_CHECKS_PASS");
 }
}
