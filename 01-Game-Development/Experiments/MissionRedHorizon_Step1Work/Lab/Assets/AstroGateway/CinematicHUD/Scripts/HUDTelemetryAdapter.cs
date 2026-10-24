using System;using UnityEngine;using AstroGateway.NativeSlice;
namespace AstroGateway.CinematicHUD {
public struct ResourceTelemetrySnapshot {public string id,label,unit,status;public float current,max;public bool available;public float Fraction=>available&&max>0?Mathf.Clamp01(current/max):0;}
public interface IResourceTelemetryProvider {ResourceTelemetrySnapshot Read(string id);}
public class HUDTelemetryAdapter:MonoBehaviour,IResourceTelemetryProvider {
 public NativeSliceGame game;public event Action Changed;
 public void Bind(){game.telemetry.Updated+=OnField;game.modes.Changed+=OnMode;}
 void OnField(TelemetryField f){Changed?.Invoke();}void OnMode(ControlMode m){Changed?.Invoke();}
 public ResourceTelemetrySnapshot Read(string id){bool has=game&&game.prototype&&(id=="oxygen"||id=="power");float v=has?(id=="oxygen"?game.prototype.oxygen:game.prototype.power):0;return new ResourceTelemetrySnapshot{id=id,label=id.ToUpperInvariant(),unit=has?"simulation %":"",current=v,max=has?100:0,available=has,status=!has?"NOT CONNECTED":v<=15?"CRITICAL":v<=30?"LOW":"NOMINAL"};}
 void OnDestroy(){if(game!=null){game.telemetry.Updated-=OnField;if(game.modes)game.modes.Changed-=OnMode;}}
}}
