using System;
using System.Collections.Generic;
using UnityEngine;
using AstroGateway.Stage08B;

// Offline defaults for the integration contracts. They reproduce today's local, simulated behaviour and
// never claim to be authoritative. Real backend clients replace them through IntegrationServices.
namespace AstroGateway.Integration
{
    public static class IntegrationServices
    {
        public static IIdentityProvider Identity = new OfflineIdentityProvider();
        public static IMissionSessionService Sessions = new LocalMissionSessionService();
        public static IMissionAssessment Assessment = new LocalProvisionalAssessment();
        public static ILifeSupportModel LifeSupport = new PrototypeLifeSupportModel();
        public static IRecommendationService Recommendations = new FixedRecommendationService();
        // Weather is scene-bound; GameplayEventBridge installs a PrototypeWeatherFeed when it starts.
        public static IWeatherFeed Weather;

        public static void ResetToOffline()
        {
            Identity = new OfflineIdentityProvider();
            Sessions = new LocalMissionSessionService();
            Assessment = new LocalProvisionalAssessment();
            LifeSupport = new PrototypeLifeSupportModel();
            Recommendations = new FixedRecommendationService();
            Weather = null;
        }
    }

    public class OfflineIdentityProvider : IIdentityProvider
    {
        public UserIdentity Current { get; } = new UserIdentity();
        public string AccessToken => null;
        public event Action<UserIdentity> Changed { add { } remove { } }
    }

    // Memory-only queue. Commands stay pending until a backend client acknowledges them.
    public class LocalMissionSessionService : IMissionSessionService
    {
        const int MaxPending = 256;
        readonly List<MissionCommand> pending = new List<MissionCommand>();
        public MissionSessionState Session { get; private set; } = new MissionSessionState();
        public bool Connected => false;
        public string ConnectionLabel => "Backend not connected • " + pending.Count + " commands queued on this device";
        public IReadOnlyList<MissionCommand> Pending => pending;

        public void Start(string missionId, int version, Action<MissionSessionState> done)
        {
            Session = new MissionSessionState { sessionId = Guid.NewGuid().ToString(), missionId = missionId, missionVersion = version, status = "active" };
            pending.Clear();
            done?.Invoke(Session);
        }

        public void Send(MissionCommand command, Action<bool, string> done)
        {
            if (command == null || string.IsNullOrEmpty(command.kind)) { done?.Invoke(false, "invalid command"); return; }
            if (Session.status != "active" && command.kind != CommandKinds.Resume) { done?.Invoke(false, "no active session"); return; }
            if (pending.Count >= MaxPending) { done?.Invoke(false, "offline queue full"); return; }
            command.expectedRevision = Session.revision;
            pending.Add(command);
            Session.revision++;
            if (command.kind == CommandKinds.Pause) Session.status = "paused";
            else if (command.kind == CommandKinds.Resume) Session.status = "active";
            else if (command.kind == CommandKinds.Complete) Session.status = "completed";
            done?.Invoke(true, "queued locally");
        }
    }

    // Gathers evidence counts only. No score is produced until the backend rubric is approved.
    public class LocalProvisionalAssessment : IMissionAssessment
    {
        public void RequestReport(string sessionId, Action<MissionReport> done)
        {
            var report = new MissionReport { sessionId = sessionId };
            int scans = 0, boards = 0, exits = 0, weather = 0;
            foreach (var c in IntegrationServices.Sessions.Pending)
            {
                if (c.kind == CommandKinds.ScanTarget) scans++;
                else if (c.kind == CommandKinds.BoardRover) boards++;
                else if (c.kind == CommandKinds.ExitRover) exits++;
                else if (c.kind == CommandKinds.WeatherChanged) weather++;
            }
            report.skillEvidence.Add(new SkillEvidence { skillId = "observation.simulated_scans", count = scans, note = "simulated scans recorded" });
            report.skillEvidence.Add(new SkillEvidence { skillId = "equipment.rover_operation", count = Mathf.Min(boards, exits), note = "completed board/exit cycles" });
            report.skillEvidence.Add(new SkillEvidence { skillId = "environment.weather_events", count = weather, note = "weather changes experienced" });
            done?.Invoke(report);
        }
    }

    // Same rates MarsPrototype uses today (0.015 %/s, 0.035 %/s in a dust storm), so swapping it in changes nothing.
    public class PrototypeLifeSupportModel : ILifeSupportModel
    {
        public string ModelVersion => "prototype-stage08b";
        public float OxygenUsePerSecond(Activity activity, WeatherState weather) => weather != null && weather.dust == DustLevel.DustStorm ? 0.035f : 0.015f;
    }

    public class PrototypeWeatherFeed : IWeatherFeed
    {
        readonly MarsPrototype prototype;
        int last = -1;
        public WeatherState Current { get; private set; } = new WeatherState();
        public event Action<WeatherState> Changed;
        public PrototypeWeatherFeed(MarsPrototype prototype) { this.prototype = prototype; Poll(); }

        public void Poll()
        {
            if (!prototype || prototype.weather == last) return;
            last = prototype.weather;
            Current = new WeatherState { dust = (DustLevel)Mathf.Clamp(last, 0, 2), visibilityMeters = RenderSettings.fogEndDistance };
            Changed?.Invoke(Current);
        }
    }

    public class FixedRecommendationService : IRecommendationService
    {
        public void Recommend(UserIdentity user, Action<MissionRecommendation> done) =>
            done?.Invoke(new MissionRecommendation { missionId = "mission.red_horizon.slice", difficulty = 0, reason = "Offline default: first surface mission", authoritative = false });
    }
}
