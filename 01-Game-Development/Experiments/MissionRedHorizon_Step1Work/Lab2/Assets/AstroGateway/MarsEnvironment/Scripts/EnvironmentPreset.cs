using UnityEngine;
namespace AstroGateway.MarsEnvironment {
 [CreateAssetMenu(menuName="AstroGateway/Mars Lighting Preset")]public class EnvironmentPreset:ScriptableObject {public string label;public Vector3 sunEuler;public Color sunColor=new Color(1,.89f,.76f),sky=new Color(.3f,.23f,.19f),horizon=new Color(.64f,.43f,.3f);public float intensity=1.3f,fogEnd=400;}
}
