using System;using System.Collections.Generic;using UnityEngine;
namespace AstroGateway.NativeSlice {
 [CreateAssetMenu(menuName="AstroGateway/Mission Registry")]public class MissionRegistry:ScriptableObject {public string missionId="mission.red_horizon.slice",displayName="Operation Red Horizon",rocket="AETHRION-01",lander="VELORA-X",habitat="ARESYN Habitat",rover="TERRAVEX-R1",robot="NEXBOT-S1",assistant="ORBIS Core";[TextArea]public string description="Provisional surface control and scientific scan slice";public string[] objectives={"Board rover","Drive and exit safely","Record a simulated scan"};[Range(0,2)]public int difficulty=0;public int provisionalReward=1;}
 public enum ControlMode {Astronaut,Rover,Menu,Robot,Cockpit}
 public struct NativeInput {public Vector2 movement,look;public bool run,brake,accelerate;}
 public class ControlModeManager:MonoBehaviour {public ControlMode Mode{get;private set;}public NativeInput Input{get;private set;}public event Action<ControlMode> Changed;public bool Switch(ControlMode mode){if(mode==ControlMode.Robot||mode==ControlMode.Cockpit)return false;Mode=mode;Input=default;Changed?.Invoke(mode);return true;}public void Submit(NativeInput input){Input=Mode==ControlMode.Menu?default:input;}}
 [Serializable]public class TelemetryField {public string id,source,unit,quality="valid_simulated";public float value;public long tick;}
 [Serializable]public class TelemetryExport {public int version=1;public TelemetryField[] fields;}
 public interface ITelemetrySink {void Record(string id,string source,float value,string unit,long tick);}
 public class TelemetryHistory:ITelemetrySink {public readonly List<TelemetryField> records=new List<TelemetryField>();public event Action<TelemetryField> Updated;public void Record(string id,string source,float value,string unit,long tick){if(float.IsNaN(value)||float.IsInfinity(value))return;var f=new TelemetryField{id=id,source=source,value=value,unit=unit,tick=tick};if(records.Count>=256)records.RemoveAt(0);records.Add(f);Updated?.Invoke(f);}}
 [Serializable]public class ScientificScan {public string sampleId,type="simulated_surface_scan",instrument="prototype.synthetic_scanner",analysis="unverified_simulated";public Vector3 location;public long tick;public int sol=1;public float syntheticIndex;}
 [Serializable]public class SliceSnapshot {public int version=1;public string missionId;public Vector3 astronaut,rover;public bool driving;public float oxygen,power,battery,health;public long tick;public List<ScientificScan> scans;}
}
