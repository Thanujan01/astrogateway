using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;
namespace AstroGateway.CinematicHUD {
public class HUDPointerControl:MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler,IEndDragHandler {
 public string role;public AstroGatewayHUDController hud;public RectTransform knob;public bool floating;public float deadZone=.12f;public int owner=int.MinValue;Vector2 anchor;
 public void OnPointerDown(PointerEventData e){if(e.button!=PointerEventData.InputButton.Left)return;if(owner!=int.MinValue||!hud.InputAllowed)return;owner=e.pointerId;RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform,e.position,e.pressEventCamera,out var p);anchor=floating?p:Vector2.zero;if(role=="move")Move(p);else if(role=="run")hud.runHeld=true;else if(role=="brake")hud.brakeHeld=true;}
 public void OnDrag(PointerEventData e){if(owner!=e.pointerId||!hud.InputAllowed)return;if(role=="move"){RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform,e.position,e.pressEventCamera,out var p);Move(p);}if(role=="camera")hud.lookDelta+=new Vector2(e.delta.x,e.delta.y)*hud.settings.sensitivity;}
 void Move(Vector2 p){float radius=((RectTransform)transform).rect.width*.38f;var v=(p-anchor)/Mathf.Max(1,radius);v=v.magnitude<deadZone?Vector2.zero:Vector2.ClampMagnitude(v*hud.settings.sensitivity,1);hud.movement=v;if(knob)knob.anchoredPosition=anchor+v*radius;}
 public void OnPointerUp(PointerEventData e){if(e.button!=PointerEventData.InputButton.Left)return;if(owner==e.pointerId)Clear();}public void OnEndDrag(PointerEventData e){OnPointerUp(e);}
 public void Clear(){owner=int.MinValue;if(role=="move"){hud.movement=Vector2.zero;if(knob)knob.anchoredPosition=Vector2.zero;}if(role=="run")hud.runHeld=false;if(role=="brake")hud.brakeHeld=false;}
 void OnDisable(){if(hud)Clear();}void OnApplicationFocus(bool f){if(!f)Clear();}void OnApplicationPause(bool p){if(p)Clear();}
}}
