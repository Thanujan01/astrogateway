# Visual defect register — Mars_VisualUpgrade_v1

Evidence: real Game View captures at 1512×982 (`Docs/VisualUpgrade/Captures/After_Final/`) and the scene inventory. Severity: H misleading/unreadable · M cluttered/inconsistent · L polish.
Risk: Low = scene-only Visual Upgrade script · Med = scene/material in the Visual Upgrade folder · High = shared script, assets, renderer/URP, architecture.

| ID | Sev | Area | Defect | Evidence | Fix | Risk | Batch | Status |
|---|---|---|---|---|---|---|---|---|
| UI-01 | H | Objectives | "ORBIS upload: unavailable" clipped at right edge | 06_gameplay_hud | best-fit text | Low | 1 | VERIFIED UPGRADED (batch 1) |
| UI-02 | M | Resources | NOT CONNECTED resources draw an empty bar (reads as 0%) | 06 | dim track to 18% | Low | 1 | VERIFIED UPGRADED (batch 1) |
| UI-03 | M | Prompt pill | Idle "No interaction in range" always on; duplicates "No scientific samples recorded" | 06, 07 | actionable lines only, hide when empty | Low | 1 | VERIFIED UPGRADED (batch 1) |
| UI-04 | M | Bottom centre | Pill / speed gauge / telemetry different widths | 06, 07 | one column .36–.64 | Low | 1 | VERIFIED UPGRADED (batch 1) |
| UI-05 | M | Recent scan | Footer wraps to 2 lines at ~11 px | 06 | re-lay out card footer (widening it shrank OPEN REPORT to 9 px in the HUD audit, so reverted) | Low | 2 | VERIFIED UPGRADED (batch 2) |
| UI-06 | M | Pause card | Controls reference ~12 px in empty card | 09 | larger area, 17 px max, line spacing | Low | 1 | VERIFIED UPGRADED (batch 1) |
| UI-07 | M | Settings | Touch-only settings shown on desktop | 10 | hide when touch hidden | Low | 2 | VERIFIED UPGRADED (batch 2) |
| UI-08 | M | Settings | Touch-sized sliders/buttons; GRAPHICS duplicates preset row; tiny note | 10 | desktop settings layout | High (controls built by shared controller) | approval | Open |
| UI-09 | M | Rover HUD | Speed shown twice | 07 | drop speed from telemetry line | Low | 2 | VERIFIED UPGRADED (batch 2) |
| UI-10 | L | Mode label | Floating text without backing | 06 | small backing pill | Low | 2 | Open |
| UI-11 | L | Side buttons | II / SET / MAP / BAG cryptic | 06 | PAUSE / SETTINGS / MAP / ITEMS or icons | Low | 2 | VERIFIED UPGRADED (batch 2) |
| UI-12 | L | Right column | Empty gap; empty RECENT SCAN card before first scan | 06 | compact card until first scan | Low | 2 | Open |
| UI-13 | L | Resources | "99.6 / 100" and "99.6%" repeat | 06 | single line | Low | 2 | Open |
| UI-14 | L | Modals | HUD at full brightness behind modal | 09, 10 | fade HUD while modal open | Low | 2 | VERIFIED UPGRADED (batch 2) |
| UI-15 | L | Dev UI | CONTROL TUNE developer window offered to players | 06 | hide behind dev flag | Med | 2 | Open (ask) |
| UI-16 | known | Minimap | Square map inside round mask | 06 | mask/texture fix | High (shared) | approval | Open |
| UI-17 | gap | QA | Automated HUD audit ran at 1796×1582, not 1512×982/1920/2560 | Playtest_Final/HUD_Audit.txt | multi-resolution audit in harness | Med (editor tool) | 2 | VERIFIED UPGRADED (batch 2) |
| 3D-01 | M | Comms + rover dish | Harsh black/white striped artifact on dish | 06, 07, 09 | diagnose (texture vs shadow acne), then material/bias fix | Med | 3 | Open |
| 3D-02 | H | Astronaut | Procedural placeholder (capsule limbs, no gloves, no rig) | 02, 06 | design-team rigged EVA asset | High | blocked | BLOCKED on asset |
| 3D-03 | H | Rover | Box chassis/primitive placeholder | 03, 07 | final rover model | High | blocked | BLOCKED on asset |
| 3D-04 | M | Habitat | Flat-shaded dome panels, little surface detail | 04 | normal/roughness variation, decals | Med | 3 | Open |
| 3D-05 | L | Far terrain | Massifs low detail, single material | 01 | tint/silhouette variety | Med | 3 | Open |
| 3D-06 | L | Terrain | Repetition at low camera angles | 06 | macro variation | Med | 3 | Open |
| 3D-07 | L | Stations | Only a status light gives scan feedback | 05 | scan-state colours, label plate, pulse | Med | 3 | Open |
| UI-18 | H | Branding | "LOCAL TRAINING • PROVISIONAL" cut off at 1920×1080 and 2560×1440 | Batch2/Before | best-fit | Low | 2 | VERIFIED UPGRADED (batch 2) |
| UI-19 | M | Prompt pill | Said "[K] SCAN" during a scan | Batch2 After_1 04 | hide while scanning | Low | 2 | VERIFIED UPGRADED (batch 2) |
| PF-01 | gap | Performance | Draw calls, SetPass, texture memory, GC not measured | report | Profiler capture in Mac player | Low | 2 | Open |
