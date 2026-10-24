using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using AstroGateway.MarsEnvironment;
using AstroGateway.NativeSlice;

// Rover sensor simulator for the First Mission scene. Every value comes from the running Unity simulation
// (rover transform and controller speed, terrain geometry under the rover, the game's weather state, the
// research-station registry) using the methods named in sensor_catalog_v1.json. Sensors without a reviewed
// model (temperature, pressure, radiation, composition, spectra) are emitted as UNAVAILABLE with no value.
// Read-only towards gameplay: it never moves the rover or changes any game state.
namespace AstroGateway.FirstMission
{
    [DefaultExecutionOrder(650)]
    public class RoverSensorSuite : MonoBehaviour
    {
        public NativeSliceGame game;
        public ResearchTarget[] stations;
        public LayerMask groundMask = 1;
        public SensorCatalog Catalog { get; set; }
        public string LocalSessionId = Guid.NewGuid().ToString();
        public float DistanceTravelled { get; private set; }
        Vector3 lastPosition;
        bool hasLast;

        Transform Rover => game.prototype.rover.transform;

        void Update()
        {
            var p = Rover.position;
            if (hasLast && game.prototype.driving)
            {
                float d = Vector2.Distance(new Vector2(p.x, p.z), new Vector2(lastPosition.x, lastPosition.z));
                if (d < 5) DistanceTravelled += d; // ignore teleports (load / reset)
            }
            lastPosition = p; hasLast = true;
        }

        public void RestoreDistance(float metres) => DistanceTravelled = Mathf.Max(DistanceTravelled, metres);

        public float Heading => Mathf.Repeat(Rover.eulerAngles.y, 360);

        public ResearchTarget NearestStation(out float distance)
        {
            distance = float.MaxValue; ResearchTarget best = null;
            foreach (var s in stations)
            {
                if (!s) continue;
                float d = Planar(Rover.position, s.transform.position);
                if (d < distance) { distance = d; best = s; }
            }
            return best;
        }

        public static float Planar(Vector3 a, Vector3 b) => Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
        public static float Bearing(Vector3 from, Vector3 to) => Mathf.Repeat(Mathf.Atan2(to.x - from.x, to.z - from.z) * Mathf.Rad2Deg, 360);

        // Ground under the rover: slope from the surface normal, roughness as the RMS residual of a 3x3 grid of
        // heights (1 m spacing, a 2 m x 2 m patch) from the best-fit plane. Both are geometry of the game terrain.
        public bool Terrain(out float slopeDeg, out float roughnessM)
        {
            slopeDeg = roughnessM = 0;
            var c = Rover.position;
            if (!Physics.Raycast(c + Vector3.up * 4, Vector3.down, out var hit, 12, groundMask, QueryTriggerInteraction.Ignore)) return false;
            slopeDeg = Vector3.Angle(hit.normal, Vector3.up);
            var pts = new List<Vector3>();
            for (int x = -1; x <= 1; x++)
                for (int z = -1; z <= 1; z++)
                {
                    var o = c + Rover.right * x + Rover.forward * z;
                    if (Physics.Raycast(o + Vector3.up * 4, Vector3.down, out var h, 12, groundMask, QueryTriggerInteraction.Ignore)) pts.Add(h.point);
                }
            if (pts.Count < 6) return false;
            roughnessM = PlaneResidualRms(pts);
            return true;
        }

        // Least-squares plane y = a x + b z + d; returns the RMS of the vertical residuals.
        public static float PlaneResidualRms(IList<Vector3> pts)
        {
            double sx = 0, sz = 0, sy = 0, sxx = 0, szz = 0, sxz = 0, sxy = 0, szy = 0; int n = pts.Count;
            var mean = new Vector3(pts.Average(p => p.x), pts.Average(p => p.y), pts.Average(p => p.z));
            foreach (var q in pts)
            {
                double x = q.x - mean.x, z = q.z - mean.z, y = q.y - mean.y;
                sx += x; sz += z; sy += y; sxx += x * x; szz += z * z; sxz += x * z; sxy += x * y; szy += z * y;
            }
            double det = sxx * szz - sxz * sxz;
            double a = 0, b = 0;
            if (Math.Abs(det) > 1e-9) { a = (sxy * szz - szy * sxz) / det; b = (szy * sxx - sxy * sxz) / det; }
            double sum = 0;
            foreach (var q in pts) { double r = (q.y - mean.y) - (a * (q.x - mean.x) + b * (q.z - mean.z)); sum += r * r; }
            return (float)Math.Sqrt(sum / n);
        }

        public string Traversability(float slope, float rough) =>
            slope >= Catalog.HazardSlope || rough >= Catalog.HazardRough ? "hazard" : slope >= Catalog.CautionSlope || rough >= Catalog.CautionRough ? "caution" : "nominal";

        static readonly string[] Dust = { "clear", "light_dust", "dust_storm" };

        // Checks whether an observation is allowed right now. Returns null when it is, else a player-facing reason.
        public string CanObserve(InstrumentDef inst, out ResearchTarget station, out float range)
        {
            station = NearestStation(out range);
            var p = game.prototype;
            if (inst == null) return "Select an instrument";
            if (!inst.available) return "UNAVAILABLE • " + inst.unavailableReason;
            if (!p.driving) return "Board the rover to use its instruments";
            if (inst.requiresStopped && Mathf.Abs(p.speed) > .2f) return "Stop the rover first";
            if (inst.maxRange > 0 && (!station || range > inst.maxRange))
                return station ? $"Out of range • {station.displayName} is {range:0.0} m away (max {inst.maxRange:0} m)" : "No research station registered";
            return null;
        }

        // Produces one observation's records. Caller must have checked CanObserve.
        public List<SensorRecord> Observe(InstrumentDef inst)
        {
            var station = NearestStation(out float range);
            bool ranged = inst.maxRange > 0 && station && range <= inst.maxRange;
            string obs = Guid.NewGuid().ToString(), now = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            var p = game.prototype;
            bool terrainOk = Terrain(out float slope, out float rough);
            var list = new List<SensorRecord>();
            foreach (var id in inst.sensors)
            {
                var def = Catalog.Sensors[id];
                var r = new SensorRecord
                {
                    eventId = Guid.NewGuid().ToString(), observationId = obs, localSessionId = LocalSessionId,
                    sensorId = id, sensorType = def.type, instrumentId = inst.id, unit = def.unit, method = def.method,
                    simTimeS = p.elapsed, simTick = game.tick, capturedAtUtc = now, location = Rover.position,
                    provenance = Provenance.Simulated, stationId = ranged ? station.stableId : "",
                };
                switch (id)
                {
                    case "nav.position": break;
                    case "nav.heading": Num(r, Heading); break;
                    case "nav.speed": Num(r, Mathf.Abs(p.speed)); break;
                    case "nav.distance_travelled": Num(r, DistanceTravelled); break;
                    case "terrain.slope": if (terrainOk) Num(r, slope); else Unavailable(r, "no_ground_hit"); break;
                    case "terrain.roughness": if (terrainOk) Num(r, rough); else Unavailable(r, "no_ground_hit"); break;
                    case "terrain.traversability": if (terrainOk) r.valueText = Traversability(slope, rough); else Unavailable(r, "no_ground_hit"); break;
                    case "env.dust_condition": r.valueText = Dust[Mathf.Clamp(p.weather, 0, 2)]; break;
                    case "sci.station_id": if (ranged) r.valueText = station.stableId; else Unavailable(r, "no_station_in_range"); break;
                    case "sci.target_range": if (ranged) Num(r, range); else Unavailable(r, "no_station_in_range"); break;
                    case "sci.target_bearing": if (ranged) Num(r, Bearing(Rover.position, station.transform.position)); else Unavailable(r, "no_station_in_range"); break;
                    case "sci.sample_id": if (ranged) r.valueText = Guid.NewGuid().ToString(); else Unavailable(r, "no_station_in_range"); break;
                    default: Unavailable(r, def.method); break; // not-modelled sensors
                }
                if (!def.implemented) Unavailable(r, "not_modeled");
                r.analysisState = r.provenance == Provenance.Unavailable ? AnalysisState.Unavailable : AnalysisState.NotConnected;
                list.Add(r);
            }
            return list;
        }

        static void Num(SensorRecord r, float v) { r.hasValue = true; r.value = Math.Round(v, 4); }
        static void Unavailable(SensorRecord r, string method) { r.provenance = Provenance.Unavailable; r.hasValue = false; r.valueText = ""; r.method = method; }
    }
}
