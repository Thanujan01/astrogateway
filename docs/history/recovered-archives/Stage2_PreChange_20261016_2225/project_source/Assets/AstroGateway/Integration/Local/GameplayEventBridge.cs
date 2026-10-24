using UnityEngine;
using AstroGateway.NativeSlice;

// Observes the existing gameplay (read-only) and turns state changes into semantic mission commands.
// It does not modify NativeSliceGame, MarsPrototype or the HUD. Add it to a scene only when the backend work is approved.
namespace AstroGateway.Integration
{
    [DefaultExecutionOrder(900)]
    public class GameplayEventBridge : MonoBehaviour
    {
        public NativeSliceGame game;
        public int commandsSent;
        bool wasDriving, wasPaused;
        int lastScans;
        PrototypeWeatherFeed weather;

        void Start()
        {
            if (!game) game = FindAnyObjectByType<NativeSliceGame>();
            if (!game) { enabled = false; return; }
            weather = new PrototypeWeatherFeed(game.prototype);
            IntegrationServices.Weather = weather;
            weather.Changed += w => Send(CommandKinds.WeatherChanged, "{\"dust\":" + (int)w.dust + "}");
            string missionId = game.registry ? game.registry.missionId : "mission.red_horizon.slice";
            IntegrationServices.Sessions.Start(missionId, 1, null);
            wasDriving = game.prototype.driving;
            lastScans = game.scans.Count;
        }

        void LateUpdate()
        {
            if (!game || !game.initialized) return;
            weather.Poll();
            bool driving = game.prototype.driving;
            if (driving != wasDriving) { Send(driving ? CommandKinds.BoardRover : CommandKinds.ExitRover, "{}"); wasDriving = driving; }
            while (lastScans < game.scans.Count)
            {
                var s = game.scans[lastScans++];
                Send(CommandKinds.ScanTarget, JsonUtility.ToJson(new ScanPayload { sampleId = s.sampleId, tick = s.tick, analysis = s.analysis }));
            }
            if (lastScans > game.scans.Count) lastScans = game.scans.Count;   // A local load replaced the scan list.
            bool paused = !game.enabled;
            if (paused != wasPaused) { Send(paused ? CommandKinds.Pause : CommandKinds.Resume, "{}"); wasPaused = paused; }
        }

        void Send(string kind, string payload)
        {
            IntegrationServices.Sessions.Send(new MissionCommand { kind = kind, payloadJson = payload, clientElapsedSeconds = game.prototype.elapsed }, null);
            commandsSent++;
        }

        void OnDestroy()
        {
            if (IntegrationServices.Weather == weather) IntegrationServices.Weather = null;
        }

        [System.Serializable]
        class ScanPayload { public string sampleId; public long tick; public string analysis; }
    }
}
