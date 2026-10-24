using UnityEngine;using UnityEngine.InputSystem;using AstroGateway.NativeSlice;using AstroGateway.CinematicHUD;
namespace AstroGateway.ProductionVisual {
 public enum ProductionMode {Astronaut,Rover,Scanner,Menu,Cinematic,FutureRobot}
 [DefaultExecutionOrder(420)]public class ProductionModeCoordinator:MonoBehaviour {
 public AstroGatewayHUDController hud;public ProductionCameraRig cameraRig;public string notice="";public bool cinematic;public ProductionMode Current=>hud.environment.paused?ProductionMode.Menu:hud.scanning?ProductionMode.Scanner:cinematic?ProductionMode.Cinematic:hud.game.modes.Mode==ControlMode.Menu?ProductionMode.Menu:hud.game.prototype.driving?ProductionMode.Rover:ProductionMode.Astronaut;
 public bool Request(ProductionMode mode){if(mode==ProductionMode.FutureRobot){notice="Robot control is not implemented";return false;}if(mode==ProductionMode.Cinematic)return ToggleCinematic();if(cinematic)LeaveCinematic();if(mode==ProductionMode.Menu){if(!hud.environment.paused)hud.TogglePause();return true;}if(mode==ProductionMode.Scanner){hud.BeginScan();return hud.scanning;}if(hud.environment.paused||hud.scanning)return false;bool drive=hud.game.prototype.driving;if(mode==ProductionMode.Rover&&!drive||mode==ProductionMode.Astronaut&&drive){hud.Interact();return hud.game.prototype.driving==(mode==ProductionMode.Rover);}return true;}
 public bool ToggleCinematic(){if(cinematic){LeaveCinematic();return true;}if(hud.environment.paused||hud.scanning)return false;hud.ClearInput();cinematic=true;hud.game.modes.Switch(ControlMode.Menu);notice="Cinematic view • simulation clock continues • C to return";return true;}
 public void LeaveCinematic(){cinematic=false;hud.ClearInput();if(!hud.environment.paused)hud.game.modes.Switch(hud.game.prototype.driving?ControlMode.Rover:ControlMode.Astronaut);notice="Gameplay camera restored";}
 void Update(){if(cinematic&&(hud.environment.paused||hud.scanning))LeaveCinematic();if(!hud.game.testing&&Keyboard.current?.cKey.wasPressedThisFrame==true)ToggleCinematic();}
 void OnDisable(){if(cinematic&&hud)LeaveCinematic();}
 }
}
