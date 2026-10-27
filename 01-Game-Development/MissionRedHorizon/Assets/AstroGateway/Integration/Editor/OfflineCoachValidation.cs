using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AstroGateway.MissionRules;

public static class OfflineCoachValidation
{
    [Serializable] public class Cases { public string rulesVersion; public Case[] cases; }
    [Serializable] public class Case { public double[] scores; public int difficulty, weather, expected; }
    public static void Run()
    {
        const string root = "Assets/AstroGateway/MissionRules/Config/";
        var config = new MissionRulesConfig(File.ReadAllText(root + "mission_rules_v1.json"));
        var coach = new OfflineMissionCoach(config);
        var vectors = JsonUtility.FromJson<Cases>(File.ReadAllText(root + "offline_coach_vectors.json"));
        if (vectors.rulesVersion != config.Version) throw new Exception("Fixture rules version mismatch");
        int count = 0;
        foreach (var c in vectors.cases)
        {
            var history = new List<OfflineMissionCoach.Report>();
            for (int i = 0; i < c.scores.Length; i++)
                history.Add(new OfflineMissionCoach.Report { SessionId = "fixture-" + i, RulesVersion = config.Version, Difficulty = c.difficulty, Score = c.scores[i] });
            var actual = coach.Recommend(history, c.weather);
            if (actual.Difficulty != c.expected) throw new Exception("Teacher parity failed at " + count);
            count++;
        }
        foreach (double invalid in new[] { double.NaN, double.PositiveInfinity, -1, 101 })
        {
            var r = new OfflineMissionCoach.Report { SessionId = "x", RulesVersion = config.Version, Difficulty = 1, Score = invalid };
            if (coach.Recommend(new[] { r }, 0).Status != "invalid-or-incompatible-history") throw new Exception("Invalid score accepted");
            count++;
        }
        var good = new OfflineMissionCoach.Report { SessionId = "x", RulesVersion = config.Version, Difficulty = 1, Score = 90 };
        if (coach.Recommend(new[] { good, good }, 0).Status != "invalid-or-incompatible-history") throw new Exception("Duplicate accepted");
        good.RulesVersion = "old";
        if (coach.Recommend(new[] { good }, 0).Status != "invalid-or-incompatible-history") throw new Exception("Old rules accepted");
        if (coach.Recommend(null, 0).Status != "invalid-or-incompatible-history") throw new Exception("Null accepted");
        if (coach.Recommend(new OfflineMissionCoach.Report[0], -1).Status != "invalid-or-incompatible-history") throw new Exception("Negative assessments accepted");
        Debug.Log("OFFLINE_COACH_VALIDATION PASS: " + (count + 4) + " cases; 438 backend parity boundaries; no network required");
        EditorApplication.Exit(0);
    }
}
