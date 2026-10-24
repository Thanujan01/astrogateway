using System;
using System.Collections.Generic;
using UnityEngine;

// Client-side seams for the planned central backend (FastAPI + Supabase, see the Architecture outputs 02/03).
// Nothing here opens a network connection. The default implementations in LocalServices.cs are offline
// and clearly provisional, so existing scenes keep working while real services are built behind these interfaces.
namespace AstroGateway.Integration
{
    // ---------- Unified user accounts ----------
    [Serializable]
    public class UserIdentity
    {
        public string userId = "";          // Supabase auth.users.id (UUID) once signed in; empty offline.
        public string displayName = "Local trainee";
        public bool authenticated;          // True only when a verified backend session exists.
        public string source = "offline";   // "offline" or "supabase".
    }

    public interface IIdentityProvider
    {
        UserIdentity Current { get; }
        event Action<UserIdentity> Changed;
        // Access token for the backend broker. Never persisted or logged by the game. Null when offline.
        string AccessToken { get; }
    }

    // ---------- Mission sessions and semantic commands ----------
    // Mirrors POST /v1/mission-sessions/{id}/commands: the client sends semantic actions, never scores or resource totals.
    public static class CommandKinds
    {
        public const string BoardRover = "board_rover";
        public const string ExitRover = "exit_rover";
        public const string ScanTarget = "scan_target";
        public const string WeatherChanged = "weather_changed";
        public const string Pause = "pause";
        public const string Resume = "resume";
        public const string Complete = "complete";
    }

    [Serializable]
    public class MissionCommand
    {
        public string commandId = Guid.NewGuid().ToString();  // Idempotency key.
        public long expectedRevision;
        public string kind;
        public string payloadJson = "{}";                      // Bounded, validated server-side.
        public float clientElapsedSeconds;                     // Informational only; server uses its own UTC time.

        // Proposed body for POST /v1/mission-sessions/{id}/commands, snake_case like the existing FastAPI models.
        // payloadJson must already be a JSON object.
        public string ToBrokerJson() =>
            "{\"command_id\":\"" + commandId + "\",\"expected_revision\":" + expectedRevision + ",\"kind\":\"" + kind + "\",\"payload\":" + (string.IsNullOrEmpty(payloadJson) ? "{}" : payloadJson) + "}";
    }

    [Serializable]
    public class MissionSessionState
    {
        public string sessionId = "";
        public string missionId = "";
        public int missionVersion = 1;
        public long revision;
        public string status = "none";    // none / active / paused / completed / abandoned
        public bool authoritative;        // True only when the state came from the backend.
    }

    public interface IMissionSessionService
    {
        MissionSessionState Session { get; }
        bool Connected { get; }            // True only when a real backend acknowledged the session.
        string ConnectionLabel { get; }
        void Start(string missionId, int version, Action<MissionSessionState> done);
        void Send(MissionCommand command, Action<bool, string> done);
        IReadOnlyList<MissionCommand> Pending { get; }   // Commands not yet acknowledged (offline queue).
    }

    // ---------- Mission scoring and skill assessment ----------
    [Serializable]
    public class CriterionScore { public string criterionId; public float score; public string evidence; }

    [Serializable]
    public class SkillEvidence { public string skillId; public int count; public string note; }

    [Serializable]
    public class MissionReport
    {
        public string sessionId;
        public float score = float.NaN;     // 0–100 from the server rubric. NaN = not scored (no approved rubric yet).
        public string rubricVersion = "pending-approval";
        public List<CriterionScore> criteria = new List<CriterionScore>();
        public List<SkillEvidence> skillEvidence = new List<SkillEvidence>();
        public bool authoritative;
        public bool Scored => !float.IsNaN(score);
    }

    // Skill assessment: evidence is gathered locally, levels are only ever computed by the backend.
    public interface IMissionAssessment
    {
        // Backend computes the real report on /complete; the local version only previews it.
        void RequestReport(string sessionId, Action<MissionReport> done);
    }

    // ---------- Weather ----------
    public enum DustLevel { Clear = 0, LightDust = 1, DustStorm = 2 }

    [Serializable]
    public class WeatherState
    {
        public DustLevel dust;
        public float visibilityMeters = 280;
        public float windMetersPerSecond = float.NaN;   // NaN = not connected.
        public float temperatureCelsius = float.NaN;    // NaN = not connected.
        public string source = "local_simulated";
    }

    public interface IWeatherFeed
    {
        WeatherState Current { get; }
        event Action<WeatherState> Changed;
    }

    // ---------- Oxygen / life support ----------
    public enum Activity { Idle, Walking, Running, Driving, Scanning }

    public interface ILifeSupportModel
    {
        // Percent of suit oxygen used per simulated second for the given activity and weather.
        float OxygenUsePerSecond(Activity activity, WeatherState weather);
        string ModelVersion { get; }
    }

    // ---------- Mission recommendations ----------
    [Serializable]
    public class MissionRecommendation
    {
        public string missionId;
        public int difficulty;
        public string reason;
        public bool authoritative;
    }

    public interface IRecommendationService
    {
        void Recommend(UserIdentity user, Action<MissionRecommendation> done);
    }
}
