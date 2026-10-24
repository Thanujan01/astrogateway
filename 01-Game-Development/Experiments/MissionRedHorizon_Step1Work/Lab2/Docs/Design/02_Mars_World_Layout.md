**AstroGateway: Mission Red Horizon · SK2LB-AstroLab · Stage 02 · 8 October 2026**

Status: proposed blueprint for approval. Documentation only; no Unity implementation authorized by this stage.

Evidence labels: **F** = verified source statement; **E** = proposed engineering concept; **G** = fictional gameplay/art/production assumption. Source IDs resolve in [06_References_And_Attribution.md](06_References_And_Attribution.md). All unlabelled design specifications below are **G**, not NASA measurements or engineering certification.

# Mars world layout

## Coordinate contract

Playable bounds: **250 m × 250 m**, X = −125…125 m, Z = −125…125 m. Unity 1 unit = 1 m; +Z is map north, +X east, +Y up. Origin is station survey datum, not a georeferenced Mars coordinate. Ground Y = 0 m around the station. Zone dimensions are axis-aligned reserved footprints including work aprons, not all building footprints. All positions and terrain heights are proposed.

| ID / zone | Center (X,Z), m | Reserved width X × depth Z | Elevation | Purpose / interaction point (X,Z) |
|---|---|---|---|---|
| H Habitat / airlock | (−65,−60) | 36 × 28 m | 0 | Spawn (−45,−60), return terminal (−48,−60) |
| G Greenhouse research module | (−65,−10) | 30 × 22 m | 0…1 | Closed-loop monitor (−47,−10); exterior access only |
| P Power / storage | (25,−45) | 24 × 24 m | 0 | Load-priority console (13,−45) |
| S Solar field | (75,−55) | 48 × 40 m | 0 | Inspection/cleaning rail (53,−55) |
| R Rover bay | (−15,−95) | 32 × 20 m | 0 | Static rover readiness panel (−15,−82) |
| C Science staging | (−5,20) | 24 × 20 m | 1 | Sample logging desk (−5,10) |
| D Delta study terrace | (0,85) | 70 × 40 m | 4…8 | Sample target (0,75), observation point (15,70) |
| B Dark-rock comparison site | (75,35) | 30 × 26 m | 1…2 | Comparison target (65,35) |

The solar reservation is a teaching demonstrator, not a verified power system sized to sustain a crew. The base is staged away from the sampling terrace, but real contamination-control distances require mission-specific assessment.

## Top-down diagram

Diagram is schematic; the coordinate table is authoritative. North is up. Dots represent circulation, not cables.

```text
                 NORTH +Z
      X=-125         X=0           X=125
Z=125 +--------------------------------+
      |     distant ledge silhouette   |
      |          [D DELTA]             |
      |              :                 |
      |          [C SCIENCE]....[B ROCK]|
      |              :          :      |
      | [G GREENHOUSE]:          :      |
      |      :       :          :      |
      | [H HAB]......[P POWER]..[S SOLAR]|
      |   spawn      :                 |
      |         [R ROVER]              |
Z=-125+--------------------------------+
           EAST +X ->
```

## Navigation and mission circulation

Primary learning route: spawn → power → solar → power → science staging → delta → habitat. Waypoints: (−45,−60), (0,−60), (13,−45), (53,−55), (13,−45), (0,0), (−5,10), (0,55), (0,75), (−35,40), (−45,0), (−45,−60). Approximate polyline length 421 m; interaction time makes the full mission 8–12 minutes. Walk speed proposal is 2.5 m/s, so uninterrupted movement is roughly 2.8 minutes. Validate in greybox rather than treating this estimate as final pacing.

Secondary loop: C → B → S → P → C, approximately 260 m, introduces material comparison and offers a clear return without a dead end. Greenhouse and rover are optional 30–60 second exterior stops; keep them out of mandatory scoring in the first slice.

Main paths 4 m clear, equipment aisles 2 m minimum, airlock entry 2 m clear. Reserve 5 m around consoles for camera orbit and accessibility. Low marker posts every 15–20 m on the return corridor. Cable conduits remain protected and outside foot lanes; no mandatory stepping over cables. Informal paths are subtle disturbed dust, not asphalt highways.

## Sightlines and landmarks

From spawn, reveal power mast and delta silhouette before prompting movement. Keep the H→P view corridor clear below 1.2 m; solar rows sit to its east. From D, habitat roof and mast remain visible even when most ground detail is hidden. Habitat mast at (−65,−60) is proposed 8 m high; avoid bright beacons flooding the landscape. Greenhouse is a rounded low module, power has a compact vertical service mast, and delta has horizontal layering: distinct silhouettes without arbitrary decoration.

Use small rock clusters to frame turns; never place a large rock in the H–P corridor or at a console backing space. During dust, posts and the map carry navigation once distant landmarks disappear.

## Terrain elevation strategy

Flat working pad within X −90…105, Z −110…0; local undulations ≤0.5 m. Northward rise from Y 1 at C to Y 4 at Z 55, with a broad approach ramp ≤8°; terrace working surface around Y 4. Layered non-traversable faces reach Y 8–12 near Z 105–120. No sheer drop beside a required interaction. East comparison site uses shallow relief and sparse sand pockets.

Small ripples are material/normal detail first; mesh relief only where silhouettes or collision justify it. Main walking slopes ≤12°, optional slopes ≤20°, no jump-required route. Transition out of bounds with broken ledges, instrument warning, and a clearly marked training limit. A decorative horizon ring outside the playable bounds may be proposed later; do not enlarge the traversable world.

**F:** Jezero contains a delta and a diverse geologic record [S02, S06, S13]. **G:** The nearby compressed exposures and 250 m training arena are compositional conveniences; no exact local stratigraphic section, DEM, or surveyed slope is claimed.

## Greybox review tests

Walk all routes at intended eye/camera heights. Check footprints do not overlap, required consoles are accessible, no critical path leaves bounds, and the farthest required target has a readable return cue. Capture spawn, power, and delta viewpoints in both profiles. A first-time tester should reach power without opening the map and find habitat during dust using the return markers. Revise coordinates here before approving detailed art.
