using System;using System.Reflection;using UnityEngine;using AstroGateway.NativeSlice;
namespace AstroGateway.CinematicHUD {
// Isolated presentation/input adapter. All state, physics, boarding, scans and persistence remain inherited.
[DefaultExecutionOrder(-100)] public class CinematicGameAdapter:NativeSliceGame {
 Action originalTick;
 void Start(){Bind("Start")();originalTick=Bind("Update");}
 Action Bind(string name)=>(Action)Delegate.CreateDelegate(typeof(Action),this,typeof(NativeSliceGame).GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic));
 void Update(){bool prior=testing;testing=true;try{originalTick?.Invoke();}finally{testing=prior;}}
 void OnGUI(){} // Suppress only the inherited legacy presentation in this new scene.
}}
