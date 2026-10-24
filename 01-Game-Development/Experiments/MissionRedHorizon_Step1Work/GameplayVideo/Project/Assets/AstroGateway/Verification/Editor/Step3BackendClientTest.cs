using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using AstroGateway.Integration;

// Step 3: drives HttpMissionSessionService against the LOCAL mock FastAPI server
// (AstroGatewayAndroid/backend/tests/local_mock_server.py). Batch mode, edit mode, no scene changes.
// Lab copies only: plain HTTP to 127.0.0.1 is allowed for the run and the setting is restored afterwards.
// Env: STEP3_URL (default http://127.0.0.1:8766), STEP3_OUT (results folder).
namespace AstroGateway.Verification
{
    public static class Step3BackendClientTest
    {
        const string FixtureUser = "11111111-1111-4111-8111-111111111111";
        const string FixtureIssuer = "https://fixture.supabase.co/auth/v1";
        static string Url => Environment.GetEnvironmentVariable("STEP3_URL") ?? "http://127.0.0.1:8766";
        static string Out => Environment.GetEnvironmentVariable("STEP3_OUT") ?? Path.Combine(Path.GetTempPath(), "Step3Client");
        static readonly List<string> lines = new List<string>();
        static int pass, fail;
        static IEnumerator routine;
        static Func<bool> waitFor;
        static double deadline, started;
        static InsecureHttpOption priorHttp;

        public static void Run()
        {
            priorHttp = PlayerSettings.insecureHttpOption;
            PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
            started = EditorApplication.timeSinceStartup;
            routine = Tests();
            EditorApplication.update += Tick;
        }

        static void Tick()
        {
            try
            {
                if (EditorApplication.timeSinceStartup - started > 300) throw new Exception("Client test timeout");
                if (waitFor != null)
                {
                    if (!waitFor() && EditorApplication.timeSinceStartup < deadline) return;
                    if (!waitFor()) Line("WARN  wait timed out");
                    waitFor = null;
                }
                if (!routine.MoveNext()) { Finish(); return; }
                if (routine.Current is Func<bool> f) { waitFor = f; deadline = EditorApplication.timeSinceStartup + 20; }
            }
            catch (Exception e) { Line("EXCEPTION: " + e); fail++; Finish(); }
        }

        static void Line(string s) { lines.Add(s); Debug.Log("[Step3] " + s); }
        static void Check(string n, bool ok, string d = "") { if (ok) pass++; else fail++; Line((ok ? "PASS  " : "FAIL  ") + n + (string.IsNullOrEmpty(d) ? "" : "  [" + d + "]")); }

        static string B64(string s) => Convert.ToBase64String(Encoding.UTF8.GetBytes(s)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        // Synthetic fixture token: only the local mock Auth transport accepts it. Not a real credential.
        static string FixtureToken(string sessionId) =>
            B64("{\"alg\":\"ES256\"}") + "." + B64("{\"iss\":\"" + FixtureIssuer + "\",\"aud\":\"authenticated\",\"role\":\"authenticated\",\"sub\":\"" + FixtureUser +
            "\",\"session_id\":\"" + sessionId + "\",\"exp\":" + (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 600) + "}") + ".fixture-not-a-signature";

        static long rawCode; static string rawText;
        static Func<bool> Raw(string method, string path, string token)
        {
            rawCode = -1;
            var req = new UnityWebRequest(Url + path, method) { downloadHandler = new DownloadHandlerBuffer() };
            req.SetRequestHeader("Authorization", "Bearer " + token);
            req.SendWebRequest().completed += _ => { rawText = req.downloadHandler.text; rawCode = req.responseCode; req.Dispose(); };
            return () => rawCode != -1;
        }

        static MissionCommand Cmd(string kind, string payload = "{}") => new MissionCommand { kind = kind, payloadJson = payload };

        static IEnumerator Tests()
        {
            Line("Backend: " + Url + " (local mock Auth + in-memory repository)");
            bool rejected = false;
            try { new HttpMissionSessionService("http://example.com", null); } catch (ArgumentException) { rejected = true; }
            Check("Plain HTTP is refused for non-local hosts", rejected);

            // 1. No token: stays offline, queues locally.
            var identity = new ShellIdentityProvider();
            var client = new HttpMissionSessionService(Url, identity);
            client.Start("mission.red_horizon.production_visual.v1", 1, null);
            yield return (Func<bool>)(() => client.Session.status != "starting");
            Check("Without sign-in the client stays offline", !client.Connected && client.LastError == "sign-in required" && !identity.Current.authenticated, client.ConnectionLabel);
            client.Send(Cmd(CommandKinds.BoardRover), null);
            client.Send(Cmd(CommandKinds.ExitRover), null);
            Check("Offline commands are queued, not sent", client.Pending.Count == 2);

            // 2. Signed-in session: offline queue replays in order.
            string authSession = Guid.NewGuid().ToString();
            string token = FixtureToken(authSession);
            identity.SetSession(FixtureUser, "Fixture trainee", token);
            Check("Shell identity is not trusted before the server accepts it", !identity.Current.authenticated);
            client.Start("mission.red_horizon.production_visual.v1", 1, null);
            yield return (Func<bool>)(() => client.Session.status != "starting" && client.Pending.Count == 0);
            Check("Server session created and marked authoritative", client.Connected && client.Session.authoritative && Guid.TryParse(client.Session.sessionId, out _), client.ConnectionLabel);
            Check("Identity verified only after the server accepted it", identity.Current.authenticated && identity.Current.source == "supabase");
            Check("Offline queue replayed in order", client.Pending.Count == 0 && client.Session.revision == 2, "revision " + client.Session.revision);

            // 3. Typical gameplay commands (same JSON the GameplayEventBridge produces).
            client.Send(Cmd(CommandKinds.ScanTarget, "{\"sample_id\":\"" + Guid.NewGuid() + "\",\"tick\":12,\"analysis\":\"unverified_simulated\"}"), null);
            client.Send(Cmd(CommandKinds.WeatherChanged, "{\"dust\":2}"), null);
            client.Send(Cmd(CommandKinds.Pause), null);
            client.Send(Cmd(CommandKinds.Resume), null);
            yield return (Func<bool>)(() => client.Pending.Count == 0);
            Check("Scan, weather, pause and resume accepted", client.Session.revision == 6, "revision " + client.Session.revision);

            yield return Raw("GET", "/v1/mission-sessions/" + client.Session.sessionId, token);
            Check("Server state matches the client revision", rawCode == 200 && rawText.Contains("\"revision\":6") && rawText.Contains("\"paused\":false"), rawCode + " " + rawText);
            yield return Raw("GET", "/v1/reports/" + client.Session.sessionId, token);
            Check("Report has evidence counts and no score", rawCode == 200 && rawText.Contains("\"simulated_scans\":1") && rawText.Contains("\"rover_board_exit_cycles\":1") && rawText.Contains("\"score\":null") && rawText.Contains("pending_approval"), rawText);
            yield return Raw("GET", "/v1/recommendations", token);
            Check("Recommendation is the labelled placeholder", rawCode == 200 && rawText.Contains("pending_approval"), rawText);

            // 4. Validation and conflicts.
            client.Send(Cmd(CommandKinds.WeatherChanged, "{\"dust\":7}"), null);
            client.Send(Cmd(CommandKinds.BoardRover), null);
            yield return (Func<bool>)(() => client.Pending.Count == 0);
            Check("Invalid command is rejected by the server and dropped; queue continues", client.Session.revision == 7 && client.LastError.Contains("422"), client.LastError);
            bool refused = false; client.Send(Cmd(CommandKinds.Complete), (ok, _) => refused = !ok);
            Check("Completion is not sent as a command", refused && client.Pending.Count == 0);
            client.Session.revision = 0;   // Simulate a stale client.
            client.Send(Cmd(CommandKinds.ExitRover), null);
            yield return (Func<bool>)(() => client.Pending.Count == 0);
            Check("Revision conflict resynchronises from the server", client.Session.revision == 8, "revision " + client.Session.revision);

            // 5. Other identities and revoked sessions.
            yield return Raw("GET", "/v1/mission-sessions/" + client.Session.sessionId, FixtureToken(Guid.NewGuid().ToString()));
            Check("Same user, different Auth session can still read state", rawCode == 200, rawCode.ToString());
            yield return Raw("GET", "/v1/mission-sessions/" + client.Session.sessionId, "not-a-token");
            Check("Malformed token is refused", rawCode == 401, rawCode.ToString());
            yield return Raw("POST", "/v1/auth-session/revoke", token);
            Check("Session revoked on the server", rawCode == 200, rawCode.ToString());
            client.Send(Cmd(CommandKinds.Pause), null);
            yield return (Func<bool>)(() => !client.Connected);
            Check("After revocation the client disconnects and keeps the command queued", !client.Connected && client.Pending.Count == 1 && client.LastError == "sign-in required", client.ConnectionLabel);
        }

        static void Finish()
        {
            EditorApplication.update -= Tick;
            PlayerSettings.insecureHttpOption = priorHttp;
            Directory.CreateDirectory(Out);
            var all = new List<string> { $"Step 3 Unity client vs local backend — {DateTime.Now:yyyy-MM-dd HH:mm:ss}", $"PASS {pass}  FAIL {fail}", "" };
            all.AddRange(lines);
            File.WriteAllLines(Path.Combine(Out, "Step3_Client_Results.txt"), all);
            if (Application.isBatchMode) EditorApplication.Exit(fail == 0 ? 0 : 1);
        }
    }
}
