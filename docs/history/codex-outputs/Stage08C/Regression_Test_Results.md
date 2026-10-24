# Regression and preservation — 2026-10-10

## Final local results
PASS:20/20 Stage08B controlled Play Mode assertions and12/12 touch input/layout checks in the isolated source-matched lab. Test-window Console0 errors/0 warnings. These are not hands-on results. Full Editor logs retain the known Unity QuickSearch startup indexing exception; an initial Editor-only UIOrientation namespace compile error was corrected and its log retained as Resolved_Compile_Trial.log.

The final-source wall-clock regression initially passed19/20, failing run displacement while run speed passed. Screenshot stalls consume wall time while controller movement caps Time.deltaTime at0.05 s. The lab-only test clock now measures MarsPrototype.elapsed simulation time rather than wall time. The same displacement thresholds then passed; the failing trial is retained in Evidence/Timing_Trial/. The original main-project Stage08BBuilder source was not edited. Other lab harness differences redirect output paths and invoke TouchChecks before the legacy checks.

```
walk displacement: PASS
walk speed: PASS
grounded: PASS
walk animator: PASS
run speed: PASS
run displacement: PASS
camera yaw: PASS
enter rover: PASS
rover acceleration: PASS
rover terrain travel: PASS
rover grounded: PASS
exit while moving rejected: PASS
rover steering: PASS
rover braking: PASS
exit rover: PASS
light dust: PASS
storm particles bounded: PASS
storm visibility: PASS
HUD resources: PASS
clear transition: PASS
```

Telemetry_Compatibility.txt: two actual synthetic QA batches (8 and19 events) passed Stage07A Pydantic Batch parsing and interval/resource validation. Events remain memory-only and provisional in normal gameplay. QA export is explicit; no transmission, trusted scores, player accounts or Stage07C witness claims.

Runtime_Source_Provenance.json: main and lab MarsPrototype, MobileControls and AndroidProfileProbe hashes match; records final APK checksum. Android harness is compiled only under UNITY_ANDROID + STAGE08C_PROFILE_HARNESS + !UNITY_EDITOR. Normal builds do not install the probe. AndroidProbeBuild refuses execution outside the isolated lab before changing any PlayerSettings.

Preservation_Check.json compares pre-stage Assets/Packages/ProjectSettings/Docs hashes. The only intentional existing-file modification is Stage08B/MarsPrototype.cs. Stage08B development scene and all earlier scenes/materials/assets/settings remain unchanged. Four new C# files under Assets/AstroGateway/Stage08C: MobileControls.cs, AndroidProfileProbe.cs, Editor/TouchChecks.cs, Editor/AndroidProbeBuild.cs; Unity-generated metadata accompanies them. Existing Android/React/backend/Supabase sources untouched. Original AVD copied into work/stage08c/EmulatorAVD and run read-only; no account creation or remote migration.

## Missing regressions
Full post-change desktop physical/manual controls, all three target traversal, wall/slope/camera/boundary exploration, Android host bridge/authentication regression and physical-device gesture/thermal tests are not completed. The standalone test APK is not evidence of the existing host application's lifecycle integration.
