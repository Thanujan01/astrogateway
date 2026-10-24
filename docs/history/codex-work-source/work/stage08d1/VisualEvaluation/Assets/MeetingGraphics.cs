using UnityEngine;
using UnityEngine.Rendering.Universal;
public class MeetingGraphics:MonoBehaviour {
 public UniversalRenderPipelineAsset source;
 UniversalRenderPipelineAsset clone; UnityEngine.Rendering.RenderPipelineAsset prior;
 int priorMips;float priorLod;int priorCap; public int preset=1; public string[] names={"Low","Balanced","High","Ultra Preview"};
 public void Apply(int i){preset=Mathf.Clamp(i,0,3);if(!clone)return;clone.renderScale=new[]{.65f,.8f,1f,1f}[preset];clone.shadowDistance=new[]{20f,35f,50f,75f}[preset];clone.mainLightShadowmapResolution=new[]{512,1024,2048,4096}[preset];clone.msaaSampleCount=new[]{1,2,4,4}[preset];QualitySettings.globalTextureMipmapLimit=new[]{2,1,0,0}[preset];QualitySettings.lodBias=new[]{.6f,1f,1.5f,2f}[preset];Application.targetFrameRate=new[]{30,30,60,60}[preset];foreach(var p in FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None)){var e=p.emission;e.rateOverTime=new[]{2f,5f,12f,20f}[preset];} }
 void OnEnable(){InitializeForValidation();}
 public void InitializeForValidation(){if(!source||clone)return;prior=QualitySettings.renderPipeline;priorMips=QualitySettings.globalTextureMipmapLimit;priorLod=QualitySettings.lodBias;priorCap=Application.targetFrameRate;clone=Instantiate(source);QualitySettings.renderPipeline=clone;Apply(preset);}
 void OnDisable(){QualitySettings.renderPipeline=prior;QualitySettings.globalTextureMipmapLimit=priorMips;QualitySettings.lodBias=priorLod;Application.targetFrameRate=priorCap;if(clone){if(Application.isPlaying)Destroy(clone);else DestroyImmediate(clone);}}
 void OnGUI(){GUILayout.BeginArea(new Rect(15,15,270,125),GUI.skin.box);GUILayout.Label("ISOLATED VISUAL PREVIEW — not gameplay");for(int i=0;i<4;i++)if(GUILayout.Button(names[i]))Apply(i);GUILayout.EndArea();}
}
