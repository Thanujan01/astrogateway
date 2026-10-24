using System;using System.Reflection;using UnityEngine;using AstroGateway.MarsEnvironment;
namespace AstroGateway.CinematicHUD {
[DefaultExecutionOrder(100)]public class CinematicEnvironmentAdapter:EnvironmentPresentation {
 Action tick,late;
 Action Bind(string n)=>(Action)Delegate.CreateDelegate(typeof(Action),this,typeof(EnvironmentPresentation).GetMethod(n,BindingFlags.Instance|BindingFlags.NonPublic));
 void Start(){Bind("Start")();tick=Bind("Update");late=Bind("LateUpdate");}
 void Update(){tick?.Invoke();}void LateUpdate(){late?.Invoke();}void OnGUI(){}
}}
