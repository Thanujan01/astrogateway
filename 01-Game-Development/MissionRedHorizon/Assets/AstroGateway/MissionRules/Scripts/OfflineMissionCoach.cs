using System;
using System.Collections.Generic;
using System.Linq;

namespace AstroGateway.MissionRules
{
    // Offline advisory only. Never awards scores or changes the active mission.
    // Caller supplies chronological, completed, deduplicated reports for the CURRENT user.
    // This is an explainable rules model, not a model trained on human players or Mars observations.
    public sealed class OfflineMissionCoach
    {
        public sealed class Report
        {
            public string SessionId, RulesVersion;
            public int Difficulty;
            public double Score;
        }
        public sealed class Advice
        {
            public string MissionId, RulesVersion, Status;
            public int Difficulty;
            public bool Provisional = true;
            public readonly List<string> Rules = new List<string>();
        }
        readonly string version, mission;
        readonly double[] raise, lower;
        readonly int maximum, storm;
        public OfflineMissionCoach(MissionRulesConfig config)
        {
            version = config.Version;
            var recommendations = config.Section("recommendations");
            raise = ((List<object>)recommendations["raise_difficulty_after_scores"]).Select(MissionRulesConfig.Num).ToArray();
            lower = ((List<object>)recommendations["lower_difficulty_after_scores"]).Select(MissionRulesConfig.Num).ToArray();
            maximum = Convert.ToInt32(recommendations["max_difficulty"]);
            var first = MissionRulesConfig.Map(((List<object>)config.Section("missions")["canonical"])[0]);
            mission = (string)first["mission_id"];
            storm = Convert.ToInt32(first["storm_scenario_from_difficulty"]);
            if (raise.Length == 0 || lower.Length != raise.Length || maximum != 2 || storm < 0 || storm > maximum)
                throw new ArgumentException("Unsupported offline recommendation policy");
        }
        public Advice Recommend(IReadOnlyList<Report> history, int weatherAssessments)
        {
            var advice = new Advice { MissionId = mission, RulesVersion = version, Status = "offline-advisory" };
            if (history == null || weatherAssessments < 0 || history.Count > 10000)
                return Invalid(advice);
            var ids = new HashSet<string>();
            foreach (var r in history)
                if (r == null || string.IsNullOrWhiteSpace(r.SessionId) || !ids.Add(r.SessionId) ||
                    r.RulesVersion != version || r.Difficulty < 0 || r.Difficulty > maximum ||
                    double.IsNaN(r.Score) || double.IsInfinity(r.Score) || r.Score < 0 || r.Score > 100)
                    return Invalid(advice);
            if (history.Count == 0)
            {
                advice.Rules.Add("R1_NEW_TRAINEE");
                advice.Status = "no-completed-history";
                return advice;
            }
            var last = history[history.Count - 1];
            advice.Difficulty = last.Difficulty;
            bool up = history.Count >= raise.Length, down = history.Count >= lower.Length;
            for (int i = 0; i < raise.Length && history.Count >= raise.Length; i++)
            {
                var score = history[history.Count - raise.Length + i].Score;
                up &= score >= raise[i];
                down &= score < lower[i];
            }
            bool lowered = false;
            if (up && advice.Difficulty < maximum) { advice.Difficulty++; advice.Rules.Add("R2_RAISE_DIFFICULTY"); }
            else if (down && advice.Difficulty > 0) { advice.Difficulty--; lowered = true; advice.Rules.Add("R2_LOWER_DIFFICULTY"); }
            else advice.Rules.Add("R2_KEEP_DIFFICULTY");
            if (weatherAssessments == 0 && advice.Difficulty < storm && !lowered && last.Score >= 70)
            { advice.Difficulty = storm; advice.Rules.Add("R5_ASSESS_UNSEEN_SKILL"); }
            return advice;
        }
        static Advice Invalid(Advice advice)
        { advice.Status = "invalid-or-incompatible-history"; advice.Difficulty = 0; advice.Rules.Add("ABSTAIN"); return advice; }
    }
}
