using System;
using System.Collections;
using UnityEngine;

public sealed class AstroGatewayPocBridge : MonoBehaviour
{
    [Serializable] private class Descriptor { public int version; public string launchId; public string missionId; }
    private string display = "Waiting for native mission descriptor";
    private IEnumerator Start()
    {
        yield return null;
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
            activity.Call("onUnityReady");
#endif
    }
    public void ReceiveMission(string json)
    {
        try
        {
            var mission = JsonUtility.FromJson<Descriptor>(json);
            if (mission == null || mission.version != 1 || string.IsNullOrEmpty(mission.launchId) || mission.missionId != "mission-red-horizon-poc")
                throw new ArgumentException("Invalid POC descriptor");
            display = "Native mission received: " + mission.missionId + "\nLaunch: " + mission.launchId;
            Debug.Log("AstroGatewayPOC UNITY_RECEIVED " + mission.launchId);
            Send("ACK", mission.launchId, "Unity acknowledged " + mission.missionId);
            Send("RESULT", mission.launchId, "POC_RESULT_OK: native mission identifier received and Unity response sent");
        }
        catch (Exception error) { display = "Descriptor error: " + error.Message; Debug.LogError(display); }
    }
    private void Send(string type, string launchId, string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
            activity.Call("onUnityMessage", type, launchId, message);
#endif
    }
    private void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Max(20, Screen.height / 30), wordWrap = true };
        GUI.Label(new Rect(24, 24, Screen.width - 48, 160), "AstroGateway — Unity integration scene\n" + display, style);
    }
}
