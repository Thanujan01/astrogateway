using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Client for the planned FastAPI mission endpoints (see Docs/Backend_Mission_Endpoints_Plan.md).
// Commands are queued locally and sent one at a time in order; Connected becomes true only after the
// server created the session. Not used by any scene yet: swap it in through IntegrationServices once approved.
namespace AstroGateway.Integration
{
    public class HttpMissionSessionService : IMissionSessionService
    {
        [Serializable] class SessionRequest { public string launch_id; public string mission_id; }
        [Serializable] class SessionResponse { public string id; public string mission_id; public string status; }
        [Serializable] class CommandAck { public string session_id; public string command_id; public string kind; public long revision; public bool duplicate; }
        [Serializable] class StateResponse { public string id; public long revision; public bool paused; public string status; }

        const int MaxPending = 256;
        readonly string baseUrl;
        readonly IIdentityProvider identity;
        readonly string serverMissionId;
        readonly List<MissionCommand> pending = new List<MissionCommand>();
        bool sending, resynced;

        public MissionSessionState Session { get; private set; } = new MissionSessionState();
        public IReadOnlyList<MissionCommand> Pending => pending;
        public bool Connected { get; private set; }
        public string LastError { get; private set; } = "";
        public string ConnectionLabel => Connected ? "Backend connected • " + pending.Count + " pending" : "Backend not connected" + (string.IsNullOrEmpty(LastError) ? "" : " • " + LastError) + " • " + pending.Count + " queued";

        // serverMissionId maps the Unity registry id to the backend's published mission id (decision pending).
        public HttpMissionSessionService(string baseUrl, IIdentityProvider identity, string serverMissionId = "mission-red-horizon-poc")
        {
            var uri = new Uri(baseUrl);
            bool local = uri.IsLoopback;
            if (uri.Scheme != "https" && !(local && uri.Scheme == "http")) throw new ArgumentException("Backend URL must use HTTPS (plain HTTP only for localhost testing).");
            this.baseUrl = baseUrl.TrimEnd('/');
            this.identity = identity;
            this.serverMissionId = serverMissionId;
        }

        public void Start(string missionId, int version, Action<MissionSessionState> done)
        {
            Session = new MissionSessionState { missionId = missionId, missionVersion = version, status = "starting" };
            var body = JsonUtility.ToJson(new SessionRequest { launch_id = Guid.NewGuid().ToString(), mission_id = serverMissionId });
            Request("POST", "/v1/mission-sessions", body, (code, text) =>
            {
                if (code == 201)
                {
                    var s = JsonUtility.FromJson<SessionResponse>(text);
                    Session = new MissionSessionState { sessionId = s.id, missionId = missionId, missionVersion = version, status = "active", authoritative = true };
                    Connected = true; LastError = "";
                    (identity as ShellIdentityProvider)?.MarkVerified();
                    Flush();
                }
                else { Session.status = "offline"; Fail(code); }
                done?.Invoke(Session);
            });
        }

        public void Send(MissionCommand command, Action<bool, string> done)
        {
            if (command == null || string.IsNullOrEmpty(command.kind)) { done?.Invoke(false, "invalid command"); return; }
            if (command.kind == CommandKinds.Complete) { done?.Invoke(false, "completion uses the /complete endpoint"); return; }
            if (pending.Count >= MaxPending) { done?.Invoke(false, "queue full"); return; }
            pending.Add(command);
            done?.Invoke(true, Connected ? "queued for upload" : "queued offline");
            Flush();
        }

        // Sends queued commands in order, one request at a time. Safe to call repeatedly (e.g. after reconnecting).
        public void Flush()
        {
            if (sending || !Connected || pending.Count == 0) return;
            var c = pending[0];
            c.expectedRevision = Session.revision;
            sending = true;
            Request("POST", "/v1/mission-sessions/" + Session.sessionId + "/commands", c.ToBrokerJson(), (code, text) =>
            {
                sending = false;
                if (code == 200)
                {
                    var ack = JsonUtility.FromJson<CommandAck>(text);
                    if (ack.command_id != c.commandId) { LastError = "acknowledgement mismatch"; Connected = false; return; }
                    Session.revision = ack.revision;
                    pending.RemoveAt(0);
                    resynced = false;
                    Flush();
                }
                else if (code == 409 && !resynced)
                {
                    // Revision moved (another device or a lost response): read the canonical revision once and retry.
                    resynced = true;
                    Request("GET", "/v1/mission-sessions/" + Session.sessionId, null, (sc, st) =>
                    {
                        if (sc == 200) { Session.revision = JsonUtility.FromJson<StateResponse>(st).revision; Flush(); }
                        else Fail(sc);
                    });
                }
                else if (code == 422 || code == 409)
                {
                    LastError = "server rejected " + c.kind + " (" + code + ")";
                    pending.RemoveAt(0);   // Never retried with altered content.
                    resynced = false;
                    Flush();
                }
                else Fail(code);
            });
        }

        void Fail(long code)
        {
            LastError = code == 0 ? "offline" : code == 401 ? "sign-in required" : code == 403 ? "account not allowed" : code == 429 ? "rate limited" : "server error " + code;
            if (code == 401 || code == 403 || code == 404) Connected = false;
        }

        void Request(string method, string path, string json, Action<long, string> done)
        {
            string token = identity?.AccessToken;
            if (string.IsNullOrEmpty(token)) { done(401, ""); return; }
            var req = new UnityWebRequest(baseUrl + path, method) { downloadHandler = new DownloadHandlerBuffer(), timeout = 15 };
            if (json != null) { req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json)); req.SetRequestHeader("Content-Type", "application/json"); }
            req.SetRequestHeader("Authorization", "Bearer " + token);
            req.SendWebRequest().completed += _ =>
            {
                long code = req.result == UnityWebRequest.Result.ConnectionError ? 0 : req.responseCode;
                string text = req.downloadHandler.text;
                req.Dispose();
                done(code, text);
            };
        }
    }

    // Receives the signed-in session from the Android/web shell. Keeps the token in memory only; never logs or saves it.
    public class ShellIdentityProvider : IIdentityProvider
    {
        string token;
        public UserIdentity Current { get; private set; } = new UserIdentity();
        public string AccessToken => token;
        public event Action<UserIdentity> Changed;

        public void SetSession(string userId, string displayName, string accessToken)
        {
            token = accessToken;
            // Not "authenticated" until the backend has accepted the token.
            Current = new UserIdentity { userId = userId, displayName = displayName, authenticated = false, source = "shell (unverified)" };
            Changed?.Invoke(Current);
        }

        public void MarkVerified()
        {
            if (string.IsNullOrEmpty(token)) return;
            Current = new UserIdentity { userId = Current.userId, displayName = Current.displayName, authenticated = true, source = "supabase" };
            Changed?.Invoke(Current);
        }

        public void Clear()
        {
            token = null;
            Current = new UserIdentity();
            Changed?.Invoke(Current);
        }
    }
}
