using UnityEngine;
namespace AstroGateway.CinematicHUD {
[CreateAssetMenu(menuName="AstroGateway/Cinematic HUD Theme")]public class HUDTheme:ScriptableObject {
 public Color panel=new Color(.09f,.106f,.125f,.74f),text=new Color(.96f,.97f,.98f),secondary=new Color(.68f,.72f,.76f),orange=new Color(1,.60f,.18f),oxygen=new Color(.286f,.725f,.91f),critical=new Color(1,.34f,.34f);
 public Sprite rounded,circle,map,heading;public Font font;[Range(.55f,.85f)]public float opacity=.74f;
}}
