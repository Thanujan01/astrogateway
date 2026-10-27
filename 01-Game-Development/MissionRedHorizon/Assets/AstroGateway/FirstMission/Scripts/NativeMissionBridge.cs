using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using AstroGateway.FirstMission.Missions;

namespace AstroGateway.FirstMission
{
    // Local save namespace, NOT an authentication provider or a server-score claim.
    public static class NativeMissionContext
    {
        public static string Owner { get; private set; } = "";
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() { Owner = ""; }
        public static void Bind(string owner)
        {
            if (!Guid.TryParse(owner, out var id) || id == Guid.Empty) throw new ArgumentException("Invalid owner");
            Owner = id.ToString();
        }
        public static string PreferenceKey(string key) => string.IsNullOrEmpty(Owner) ? key : key + ".user." + Owner;
        public static string FilePath(string name)
        {
            if (Path.GetFileName(name) != name) throw new ArgumentException("Invalid save name");
            string root = string.IsNullOrEmpty(Owner) ? Application.persistentDataPath : Path.Combine(Application.persistentDataPath, "accounts", Owner);
            Directory.CreateDirectory(root);
            return Path.Combine(root, name);
        }
    }

    [UnityEngine.Scripting.Preserve]
    public sealed class NativeMissionBridge : MonoBehaviour
    {
        [Serializable] public class Descriptor
        {
            public int version; public string launchId, missionId, missionSessionId, userId; public long expiresAtMillis;
        }
        Descriptor current;
        GameObject[] roots;
        bool[] rootWasActive;
        MissionDirector director;
        bool expired;
        public static bool Valid(Descriptor d, long now)
        {
            return d != null && d.version == 1 && d.missionId == "mission-red-horizon-poc"
                && Guid.TryParse(d.launchId, out var launch) && launch != Guid.Empty
                && Guid.TryParse(d.missionSessionId, out var session) && session != Guid.Empty
                && Guid.TryParse(d.userId, out var user) && user != Guid.Empty
                && d.expiresAtMillis > now && d.expiresAtMillis - now <= 3600000;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!SceneManager.GetActiveScene().path.EndsWith("Mars_FirstMission_Missions.unity")) return;
            bool hosted = false;
            try {
                using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var klass = activity.Call<AndroidJavaObject>("getClass"))
                    hosted = klass.Call<string>("getName") == "com.sk2lb.astrogateway.poc.PocUnityActivity";
            } catch { }
            if (!hosted) return; // Standalone game remains a separately labelled offline build.
            var sceneRoots = SceneManager.GetActiveScene().GetRootGameObjects();
            var bridge = new GameObject("AstroGatewayBridge").AddComponent<NativeMissionBridge>();
            bridge.roots = sceneRoots;
            bridge.rootWasActive = new bool[sceneRoots.Length];
            for (int i=0; i<sceneRoots.Length; i++) { bridge.rootWasActive[i]=sceneRoots[i].activeSelf; sceneRoots[i].SetActive(false); } // No per-user save loads before the descriptor.
#endif
        }
        IEnumerator Start()
        {
            yield return null;
            CallNative("onUnityReady");
        }
        [UnityEngine.Scripting.Preserve]
        public void ReceiveMission(string json)
        {
            Descriptor next;
            try { if (json == null || json.Length > 2048) throw new ArgumentException(); next = JsonUtility.FromJson<Descriptor>(json); }
            catch { Send("ERROR", "Invalid mission context"); return; }
            if (!Valid(next, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())) { Send("ERROR", "Invalid or expired mission context"); return; }
            if (current != null && current.launchId == next.launchId) return;
            if (current != null) {
                // Warm relaunch/account switch gets fresh scene objects; no old user's runtime state survives.
                foreach (var root in roots) if (root) root.SetActive(false);
                SceneManager.LoadScene(SceneManager.GetActiveScene().path);
                return; // New bridge requests the new native descriptor before enabling the scene.
            }
            current = next;
            NativeMissionContext.Bind(next.userId);
            for (int i=0; i<roots.Length; i++) if (roots[i]) roots[i].SetActive(rootWasActive[i]);
            Send("ACK", "MISSIONS_CONTEXT_ACCEPTED: local gameplay; online scoring not connected");
            StartCoroutine(ObserveCompletion());
        }
        IEnumerator ObserveCompletion()
        {
            for (int frame = 0; frame < 1800; frame++) {
                director = FindAnyObjectByType<MissionDirector>();
                if (director && director.Ready) break;
                yield return null;
            }
            if (!director || !director.Ready) { Send("ERROR", "Mission initialization unavailable"); yield break; }
            director.AllMissionsCompleted += Completed;
        }
        void Completed() { Send("RESULT", "LOCAL_MISSIONS_COMPLETE: six local missions; no server-verified score awarded"); }
        void Update()
        {
            if (expired || current == null || DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() < current.expiresAtMillis) return;
            expired = true;
            foreach (var root in roots) if (root) root.SetActive(false);
            Send("ERROR", "Mission context expired; return to account dashboard");
        }
        void OnDestroy() { if (director) director.AllMissionsCompleted -= Completed; }
        void Send(string type, string message) { CallNative("onUnityMessage", type, current == null ? "" : current.launchId, message); }
        void CallNative(string method, params object[] args)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try {
                using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity")) activity.Call(method, args);
            } catch { Debug.LogWarning("Native mission host callback unavailable."); }
#endif
        }
    }
}
