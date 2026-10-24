using UnityEngine;
namespace AstroGateway.MarsEnvironment {
 public class ResearchTarget:MonoBehaviour {public string stableId,displayName;public bool TryScan(AstroGateway.NativeSlice.NativeSliceGame game){if(Vector3.Distance(game.prototype.ActivePosition(),transform.position)>3)return false;if(!game.Scan())return false;return true;}}
}
