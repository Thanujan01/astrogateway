using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AstroGateway.MarsEnvironment;
using AstroGateway.Stage08B;

// World-space navigation for the First Mission Explorer scene.
//  - WorldLocationRegistry: locations discovered from objects that really exist in the scene (never invented);
//    missing facilities are listed separately as design proposals with no position.
//  - MapCoordinateConverter: explicit scene (x,z metres, +Z = scene grid north) <-> map (0..1) conversion.
//  - TraversabilityGrid: 2 m grid built once at load from real colliders (ground raycast + obstacle overlap + slope).
//  - RoutePlanner: A* over that grid (8-neighbour, slope-weighted), smoothed with line-of-sight checks.
//  - NavigationTargetManager: current target, route status, re-routing (throttled), progress and arrival.
// Positions are Unity scene metres; nothing here is Mars GPS.
namespace AstroGateway.FirstMission.Nav
{
    public enum LocationCategory { Rover, Habitat, Airlock, Power, Communications, LifeSupport, Storage, ScienceStation, Proposal }
    public enum RouteStatus { Unavailable, Calculating, Available, Blocked, Unreachable, Arrived, None }

    public class MapLocation
    {
        public string id, name, icon, state = "", interaction = "", mission = "";
        public LocationCategory category;
        public Transform transform;            // live position source (null for proposals)
        public bool navigable = true, proposal;
        public float arrivalRadius = 4f;
        public Vector3 Position => transform ? transform.position : Vector3.zero;
        public string CategoryLabel => category switch
        {
            LocationCategory.Rover => "ROVER", LocationCategory.Habitat => "HABITAT MODULE", LocationCategory.Airlock => "AIRLOCK", LocationCategory.Power => "POWER SYSTEM",
            LocationCategory.Communications => "COMMUNICATIONS", LocationCategory.LifeSupport => "LIFE SUPPORT", LocationCategory.Storage => "STORAGE",
            LocationCategory.ScienceStation => "SCIENCE STATION", _ => "PROPOSAL (not in the world)",
        };
    }

    public static class MapCoordinateConverter
    {
        public const float HalfExtent = 125f;  // playable world clamp is +-119 m
        public static Vector2 ToMap(Vector3 world) => new Vector2((world.x + HalfExtent) / (HalfExtent * 2), (world.z + HalfExtent) / (HalfExtent * 2));
        public static Vector3 ToWorld(Vector2 map, float y = 0) => new Vector3(map.x * HalfExtent * 2 - HalfExtent, y, map.y * HalfExtent * 2 - HalfExtent);
        public static float Heading(Transform t) => Mathf.Repeat(t.eulerAngles.y, 360);   // 0 = scene north (+Z), clockwise
    }

    public class WorldLocationRegistry
    {
        public readonly List<MapLocation> Locations = new List<MapLocation>();
        public readonly List<MapLocation> Proposals = new List<MapLocation>();
        public MapLocation Rover { get; private set; }

        // Exact scene object names -> registry entries. Objects that are not found are simply not registered.
        static readonly (string obj, string id, string name, LocationCategory cat, string icon, string interaction)[] Known =
        {
            ("Crew Dome", "hab.crew", "Crew Dome", LocationCategory.Habitat, "dome", "Exterior only (interiors not implemented)"),
            ("Laboratory Dome", "hab.lab", "Laboratory Dome", LocationCategory.Habitat, "dome", "Exterior only (interiors not implemented)"),
            ("Life Support Dome", "hab.lifesupport", "Life Support Dome", LocationCategory.Habitat, "dome", "Exterior only (interiors not implemented)"),
            ("Crew Dome sealed airlock", "airlock.crew", "Crew Dome airlock", LocationCategory.Airlock, "door", "INSPECT (sealed)"),
            ("Laboratory Dome sealed airlock", "airlock.lab", "Laboratory airlock", LocationCategory.Airlock, "door", "INSPECT (sealed)"),
            ("Life Support Dome sealed airlock", "airlock.lifesupport", "Life Support airlock", LocationCategory.Airlock, "door", "INSPECT (sealed)"),
            ("Zone C — solar farm", "power.solarfarm", "Solar farm (main power)", LocationCategory.Power, "solar", "No interaction implemented"),
            ("Power distribution cabinet", "power.cabinet", "Power distribution cabinet", LocationCategory.Power, "bolt", "No interaction implemented"),
            ("Communications node mast", "comms.mast", "Communications mast", LocationCategory.Communications, "signal", "No interaction implemented"),
            ("Life support storage tank 0", "ls.tanks", "Life-support tanks", LocationCategory.LifeSupport, "tank", "No interaction implemented"),
            ("Storage crate 0", "store.crates", "Storage crates", LocationCategory.Storage, "crate", "No interaction implemented"),
        };

        public void Discover(MarsPrototype p, IEnumerable<ResearchTarget> stations, Func<string, bool> collected)
        {
            Locations.Clear(); Proposals.Clear();
            Rover = new MapLocation { id = "rover.ares6", name = "ARES-VI rover", category = LocationCategory.Rover, icon = "rover", transform = p.rover.transform, interaction = "BOARD ROVER within 5 m", arrivalRadius = 5f, mission = "First mission vehicle" };
            Locations.Add(Rover);
            foreach (var s in stations.Where(s => s))
                Locations.Add(new MapLocation { id = s.stableId, name = s.displayName, category = LocationCategory.ScienceStation, icon = "science", transform = s.transform, interaction = "SCAN on foot (3 m) or rover instruments", arrivalRadius = 3f, mission = "Research site" });
            var all = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude);
            foreach (var k in Known)
            {
                var t = all.FirstOrDefault(x => x.name == k.obj);
                if (!t) continue;
                var pos = t;
                // Zone roots sit at the origin of their children; anchor them at their renderers' centre.
                if (k.cat == LocationCategory.Power && k.obj.StartsWith("Zone"))
                {
                    var rends = t.GetComponentsInChildren<Renderer>();
                    if (rends.Length > 0)
                    {
                        var b = rends[0].bounds; foreach (var r in rends) b.Encapsulate(r.bounds);
                        var anchor = new GameObject("Map anchor • " + k.name).transform; anchor.position = b.center; anchor.SetParent(t, true); pos = anchor;
                    }
                }
                Locations.Add(new MapLocation { id = k.id, name = k.name, category = k.cat, icon = k.icon, transform = pos, interaction = k.interaction, arrivalRadius = 8f, state = "Status not modelled" });
            }
            foreach (var (id, name) in new[] { ("prop.tents", "Research tents"), ("prop.generator", "Power generator"), ("prop.charging", "Rover charging station"), ("prop.repair", "Repair locations"), ("prop.dock", "Rover docking area"), ("prop.checkpoints", "Mission checkpoints") })
                Proposals.Add(new MapLocation { id = id, name = name, category = LocationCategory.Proposal, icon = "proposal", navigable = false, proposal = true, state = "DESIGN PROPOSAL • needs approval", interaction = "Not in the playable world" });
            Refresh(collected);
        }

        public void Refresh(Func<string, bool> collected)
        {
            foreach (var l in Locations.Where(l => l.category == LocationCategory.ScienceStation))
                l.state = collected(l.id) ? "DATA COLLECTED" : "UNVISITED";
        }
    }

    // ---------- traversability grid ----------
    public class TraversabilityGrid
    {
        public const float Cell = 2f;
        public readonly int N = Mathf.CeilToInt(MapCoordinateConverter.HalfExtent * 2 / Cell);
        public bool Ready { get; private set; }
        public float Progress { get; private set; }
        public byte[] cost;        // 0 = blocked, 1 normal, 3 caution slope, 255 = no ground
        public float[] slope;
        public int Blocked { get; private set; }
        public int Hazard { get; private set; }
        readonly Collider[] buffer = new Collider[8];
        readonly Func<Collider, bool> ignore;
        public float maxSlope = 30f, cautionSlope = 18f;

        // Ground = the large terrain colliders (the 250 m playable surface); everything smaller is an obstacle.
        static bool IsGround(Collider c) => c is TerrainCollider || c.bounds.size.x > 150 || c.bounds.size.z > 150;

        public TraversabilityGrid(Func<Collider, bool> ignore) { this.ignore = ignore; cost = new byte[N * N]; slope = new float[N * N]; }

        public Vector3 CellCentre(int x, int z) => new Vector3(-MapCoordinateConverter.HalfExtent + (x + .5f) * Cell, 0, -MapCoordinateConverter.HalfExtent + (z + .5f) * Cell);
        public bool ToCell(Vector3 w, out int x, out int z)
        {
            x = Mathf.FloorToInt((w.x + MapCoordinateConverter.HalfExtent) / Cell); z = Mathf.FloorToInt((w.z + MapCoordinateConverter.HalfExtent) / Cell);
            return x >= 0 && z >= 0 && x < N && z < N;
        }
        public byte Cost(int x, int z) => cost[z * N + x];

        // Built over several frames (about 1500 cells per frame) so loading never hitches.
        public IEnumerator Build()
        {
            Ready = false; Blocked = Hazard = 0;
            int total = N * N, done = 0;
            for (int z = 0; z < N; z++)
                for (int x = 0; x < N; x++)
                {
                    var c = CellCentre(x, z); int i = z * N + x;
                    if (Mathf.Abs(c.x) > 119 || Mathf.Abs(c.z) > 119) { cost[i] = 0; Blocked++; }
                    else if (!Physics.Raycast(c + Vector3.up * 80, Vector3.down, out var hit, 160, 1, QueryTriggerInteraction.Ignore)) cost[i] = 255;
                    else
                    {
                        float s = Vector3.Angle(hit.normal, Vector3.up); slope[i] = s;
                        // Anything solid between 0.5 m and 2.3 m above the ground (other than the ground collider itself) blocks the cell.
                        var g = hit.point; bool blocked = false;
                        int n = Physics.OverlapCapsuleNonAlloc(g + Vector3.up * .55f, g + Vector3.up * 2.3f, .85f, buffer, 1, QueryTriggerInteraction.Ignore);
                        for (int k = 0; k < n; k++) if (buffer[k] != hit.collider && !ignore(buffer[k]) && !IsGround(buffer[k])) { blocked = true; break; }
                        if (hit.collider && !ignore(hit.collider) && !IsGround(hit.collider)) blocked = true; // the first thing below is a structure or rock, not the ground
                        if (blocked || s > maxSlope) { cost[i] = 0; Blocked++; }
                        else if (s > cautionSlope) { cost[i] = 3; Hazard++; }
                        else cost[i] = 1;
                    }
                    if (++done % 1500 == 0) { Progress = done / (float)total; yield return null; }
                }
            Progress = 1; Ready = true;
        }
    }

    public class RouteResult
    {
        public RouteStatus status;
        public List<Vector3> points = new List<Vector3>();
        public float length;
        public string message = "";
        public int expanded;
    }

    public static class RoutePlanner
    {
        static readonly int[] DX = { 1, -1, 0, 0, 1, 1, -1, -1 }, DZ = { 0, 0, 1, -1, 1, -1, 1, -1 };

        public static RouteResult Plan(TraversabilityGrid g, Vector3 from, Vector3 to, float goalRadius)
        {
            var r = new RouteResult();
            if (g == null || !g.Ready) { r.status = RouteStatus.Unavailable; r.message = "Navigation grid still loading"; return r; }
            if (!g.ToCell(from, out int sx, out int sz) || !g.ToCell(to, out int tx, out int tz)) { r.status = RouteStatus.Unreachable; r.message = "Outside the mapped area"; return r; }
            if (!Nearest(g, ref sx, ref sz, 4)) { r.status = RouteStatus.Blocked; r.message = "Start position is blocked"; return r; }
            int gx = tx, gz = tz;
            bool goalOk = Nearest(g, ref gx, ref gz, Mathf.CeilToInt(goalRadius / TraversabilityGrid.Cell) + 6);
            if (!goalOk) { r.status = RouteStatus.Unreachable; r.message = "No walkable ground near the target"; return r; }
            int N = g.N, start = sz * N + sx, goal = gz * N + gx;
            var gScore = new float[N * N]; for (int i = 0; i < gScore.Length; i++) gScore[i] = float.MaxValue;
            var came = new int[N * N]; var closed = new bool[N * N];
            var open = new SortedSet<(float f, int i)>(); gScore[start] = 0; open.Add((H(sx, sz, gx, gz), start)); came[start] = -1;
            int expanded = 0;
            while (open.Count > 0 && expanded < 40000)
            {
                var cur = open.Min; open.Remove(cur); int ci = cur.i;
                if (closed[ci]) continue; closed[ci] = true; expanded++;
                if (ci == goal) break;
                int cx = ci % N, cz = ci / N;
                for (int d = 0; d < 8; d++)
                {
                    int nx = cx + DX[d], nz = cz + DZ[d]; if (nx < 0 || nz < 0 || nx >= N || nz >= N) continue;
                    int ni = nz * N + nx; byte c = g.cost[ni]; if (c == 0 || c == 255 || closed[ni]) continue;
                    if (d >= 4 && (g.cost[cz * N + nx] == 0 || g.cost[nz * N + cx] == 0)) continue; // no corner cutting
                    float step = (d >= 4 ? 1.4142f : 1f) * c;
                    float ng = gScore[ci] + step;
                    if (ng < gScore[ni]) { gScore[ni] = ng; came[ni] = ci; open.Add((ng + H(nx, nz, gx, gz), ni)); }
                }
            }
            r.expanded = expanded;
            if (!closed[goal]) { r.status = RouteStatus.Unreachable; r.message = "No traversable route found"; return r; }
            var cells = new List<int>(); for (int i = goal; i != -1; i = came[i]) cells.Add(i); cells.Reverse();
            var pts = cells.Select(i => g.CellCentre(i % N, i / N)).ToList();
            pts[0] = new Vector3(from.x, 0, from.z);
            // Line-of-sight smoothing over the grid (only skips points when every cell in between is walkable).
            var smooth = new List<Vector3> { pts[0] }; int a = 0;
            while (a < pts.Count - 1)
            {
                int b = pts.Count - 1;
                while (b > a + 1 && !Clear(g, pts[a], pts[b])) b--;
                smooth.Add(pts[b]); a = b;
            }
            for (int i = 0; i < smooth.Count; i++) { var p = smooth[i]; if (Physics.Raycast(p + Vector3.up * 80, Vector3.down, out var hit, 160, 1, QueryTriggerInteraction.Ignore)) p.y = hit.point.y; smooth[i] = p; }
            r.points = smooth; r.length = Length(smooth); r.status = RouteStatus.Available; r.message = $"Route {r.length:0} m";
            return r;
        }

        public static float Length(IList<Vector3> pts) { float l = 0; for (int i = 1; i < pts.Count; i++) l += Vector2.Distance(new Vector2(pts[i - 1].x, pts[i - 1].z), new Vector2(pts[i].x, pts[i].z)); return l; }
        static float H(int x, int z, int gx, int gz) { int dx = Mathf.Abs(x - gx), dz = Mathf.Abs(z - gz); return (dx + dz) + (1.4142f - 2) * Mathf.Min(dx, dz); }

        static bool Nearest(TraversabilityGrid g, ref int x, ref int z, int radius)
        {
            if (Walk(g, x, z)) return true;
            for (int r = 1; r <= radius; r++)
                for (int dz = -r; dz <= r; dz++)
                    for (int dx = -r; dx <= r; dx++)
                    {
                        if (Mathf.Abs(dx) != r && Mathf.Abs(dz) != r) continue;
                        if (Walk(g, x + dx, z + dz)) { x += dx; z += dz; return true; }
                    }
            return false;
        }
        static bool Walk(TraversabilityGrid g, int x, int z) => x >= 0 && z >= 0 && x < g.N && z < g.N && g.cost[z * g.N + x] != 0 && g.cost[z * g.N + x] != 255;

        static bool Clear(TraversabilityGrid g, Vector3 a, Vector3 b)
        {
            float d = Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z)); int steps = Mathf.CeilToInt(d / (TraversabilityGrid.Cell * .5f));
            for (int i = 0; i <= steps; i++)
            {
                var p = Vector3.Lerp(a, b, i / (float)Mathf.Max(1, steps));
                if (!g.ToCell(p, out int x, out int z) || g.cost[z * g.N + x] != 1) return false;   // smoothing only across normal-cost cells
            }
            return true;
        }
    }

    // Current navigation target, route, re-routing and arrival. Route re-planning is throttled (at most once a
    // second, only when the player drifts > 4 m off the route or the target moved > 3 m).
    public class NavigationTargetManager
    {
        public MapLocation Target { get; private set; }
        public RouteResult Route { get; private set; } = new RouteResult { status = RouteStatus.None };
        public float InitialLength { get; private set; }
        public float Remaining { get; private set; }
        public float Progress => InitialLength > .1f ? Mathf.Clamp01(1 - Remaining / InitialLength) : 0;
        public bool Arrived { get; private set; }
        public int Replans { get; private set; }
        public event Action<MapLocation> ArrivedAt;
        readonly TraversabilityGrid grid;
        readonly Func<Vector3> player;
        float nextPlan; Vector3 lastTargetPos;

        public NavigationTargetManager(TraversabilityGrid grid, Func<Vector3> player) { this.grid = grid; this.player = player; }

        public RouteResult Preview(MapLocation l) => l == null || !l.navigable || !l.transform ? new RouteResult { status = RouteStatus.Unreachable, message = "Not a navigable location" } : RoutePlanner.Plan(grid, player(), l.Position, l.arrivalRadius);

        public void SetTarget(MapLocation l)
        {
            Target = l; Arrived = false;
            if (l == null) { Route = new RouteResult { status = RouteStatus.None }; return; }
            Route = new RouteResult { status = RouteStatus.Calculating, message = "Calculating route…" };
            nextPlan = 0; Replans = 0; InitialLength = 0;
        }
        public void Cancel() => SetTarget(null);

        public void Tick(float now)
        {
            if (Target == null) return;
            var p = player(); var t = Target.Position;
            float direct = Vector2.Distance(new Vector2(p.x, p.z), new Vector2(t.x, t.z));
            if (direct <= Target.arrivalRadius)
            {
                if (!Arrived) { Arrived = true; Route.status = RouteStatus.Arrived; Route.message = "Arrived"; Remaining = 0; ArrivedAt?.Invoke(Target); }
                return;
            }
            Arrived = false;
            bool needPlan = Route.status == RouteStatus.Calculating || Route.status == RouteStatus.Unavailable || Route.status == RouteStatus.Arrived
                || (Route.status == RouteStatus.Available && (OffRoute(p) > 4f || Vector3.Distance(lastTargetPos, t) > 3f));
            if (needPlan && now >= nextPlan)
            {
                nextPlan = now + 1f;
                Route = RoutePlanner.Plan(grid, p, t, Target.arrivalRadius); Replans++; lastTargetPos = t;
                if (Route.status == RouteStatus.Available && InitialLength <= 0) InitialLength = Route.length;
            }
            Remaining = Route.status == RouteStatus.Available ? RemainingAlong(p) : direct;
            if (InitialLength <= 0) InitialLength = Remaining;
        }

        float OffRoute(Vector3 p)
        {
            var pts = Route.points; if (pts.Count < 2) return 0;
            float best = float.MaxValue;
            for (int i = 1; i < pts.Count; i++) best = Mathf.Min(best, SegDist(new Vector2(p.x, p.z), new Vector2(pts[i - 1].x, pts[i - 1].z), new Vector2(pts[i].x, pts[i].z)));
            return best;
        }
        float RemainingAlong(Vector3 p)
        {
            var pts = Route.points; if (pts.Count < 2) return 0;
            int seg = 1; float best = float.MaxValue;
            for (int i = 1; i < pts.Count; i++) { float d = SegDist(new Vector2(p.x, p.z), new Vector2(pts[i - 1].x, pts[i - 1].z), new Vector2(pts[i].x, pts[i].z)); if (d < best) { best = d; seg = i; } }
            float rem = Vector2.Distance(new Vector2(p.x, p.z), new Vector2(pts[seg].x, pts[seg].z));
            for (int i = seg + 1; i < pts.Count; i++) rem += Vector2.Distance(new Vector2(pts[i - 1].x, pts[i - 1].z), new Vector2(pts[i].x, pts[i].z));
            return rem;
        }
        static float SegDist(Vector2 p, Vector2 a, Vector2 b) { var ab = b - a; float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / Mathf.Max(.0001f, ab.sqrMagnitude)); return (a + ab * t - p).magnitude; }

        public string StatusLabel => Target == null ? "No destination" : Route.status switch
        {
            RouteStatus.Available => $"ROUTE AVAILABLE • {Remaining:0} m • {Progress * 100:0}%",
            RouteStatus.Calculating => "ROUTE CALCULATING…",
            RouteStatus.Blocked => "ROUTE BLOCKED • " + Route.message,
            RouteStatus.Unreachable => "TARGET UNREACHABLE • " + Route.message,
            RouteStatus.Unavailable => "NAVIGATION UNAVAILABLE • " + Route.message,
            RouteStatus.Arrived => "ARRIVED",
            _ => "",
        };
    }
}
