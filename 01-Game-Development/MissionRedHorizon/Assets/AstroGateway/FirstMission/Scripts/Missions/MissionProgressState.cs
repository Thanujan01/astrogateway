using System;

namespace AstroGateway.FirstMission.Missions
{
    // Device-local progress only. These values never certify scores or skill levels.
    [Serializable]
    public sealed class MissionProgressState
    {
        public int mission, step;
        public float charge, repairProgress;
        public bool repaired;
        public int checkpoints;
        public bool finished;
        public float o2 = 40;
        public bool stormStarted, batteryDrained;
        public float stormEta = 200;
        public float activeSeconds;
        public int retries;
        public bool failed;
        public string failReason = "";

        // Older saves omit new fields and remain compatible. Invalid indices restart
        // local guidance rather than crashing or manufacturing mission completion.
        public MissionProgressState Normalize(int[] stepCounts, int checkpointCount)
        {
            if (stepCounts == null || stepCounts.Length == 0 || checkpointCount < 0)
                throw new ArgumentException("Mission catalog is required");
            foreach (int count in stepCounts)
                if (count <= 0) throw new ArgumentException("Every mission needs steps");
            if (mission < 0 || mission > stepCounts.Length) return new MissionProgressState();
            finished = mission == stepCounts.Length;
            step = finished || step < 0 || step >= stepCounts[mission] ? 0 : step;
            checkpoints = Math.Max(0, Math.Min(checkpoints, checkpointCount));
            charge = Bounded(charge, 0, 1, 0);
            repairProgress = repaired ? 1 : Bounded(repairProgress, 0, 1, 0);
            o2 = Bounded(o2, 0, 100, 40);
            stormEta = Bounded(stormEta, 0, 200, 200);
            activeSeconds = Bounded(activeSeconds, 0, 31536000, 0);
            retries = Math.Max(0, Math.Min(retries, 1000000));
            if (failReason == null || failReason.Length > 512) failReason = "Mission interrupted";
            if (!failed && o2 <= 0 && !repaired) o2 = 30;
            if (!failed && stormStarted && stormEta <= 0 && !finished) stormEta = 150;
            return this;
        }

        static float Bounded(float value, float min, float max, float fallback)
            => float.IsNaN(value) || float.IsInfinity(value) ? fallback : Math.Max(min, Math.Min(value, max));
    }
}
