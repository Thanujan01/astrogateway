# Graphics and FPS status

Four presets implemented in isolated Assets/MeetingGraphics.cs; main playable project untouched.

|Preset|Render scale|Shadow map|Distance m|MSAA|Mip limit|LOD bias|FPS cap|
|---|---:|---:|---:|---:|---:|---:|---:|
|Low|0.65|512|20|1|2|0.6|30|
|Balanced|0.8|1024|35|2|1|1|30|
|High|1|2048|50|4|0|1.5|60|
|Ultra Preview|1|4096|75|4|0|2|60|

Values were read back from Unity in the final isolated batch check. FPS caps are requested limits, not measured FPS. Texture mip limit affects eligible imported textures; it does not prove actual memory savings. Dust emission targets 2/5/12/20 particles per second with a 100-particle ceiling; interactive emission transitions remain untested. Runtime pipeline is cloned and settings restored on disable. Ultra is not an Android readiness claim.

Approved sandy gravel and rocks, existing afternoon sunlight/ambient haze and restrained grading reused in a separate preview. Four fresh 1280×800 Unity Editor Camera.Render captures are real renders, not native Play Mode screenshots or timing evidence. High capture visually reviewed: intact rover wheel/chassis silhouette and coherent ochre ground; no claim of cinematic quality.

NEW performance benchmark: UNTESTED. Physical device unavailable in ADB; Android FPS UNTESTED. Software emulator connected only. Historical Stage08C1 software FPS 4.27–4.34 at 2340×1080 is context, not a current benchmark. No performance improvement claimed.
