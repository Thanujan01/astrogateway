# Separate FSR warning investigation

No renderer or project settings were changed to suppress the inherited warning.

The installed URP 17.6.0 implementation, `Runtime/UniversalRenderPipeline.cs`, lines 2528–2618, resolves Automatic upscaling to point or linear sampling. Non-integer scaling uses linear sampling. The unchanged Web profile `Assets/Settings/Mobile_RPAsset.asset` has render scale 0.8 and upscaling selection 0 (Automatic); its default resolution scaling is non-integer. This supports linear upscaling, not a claim that FSR is the active effect.

Stage 03 observed an unsupported/stripped hidden FSR shader diagnostic in the browser. URP includes FSR resources among pipeline shaders even when automatic selection resolves to another filter. An unsupported shader diagnostic alone does not prove FSR ran or caused an image failure. The exact shader-inclusion/stripping cause is not fully isolated. Browser rendering is the practical smoke check; see the final browser log for whether it recurs in this build.

A later scoped diagnostic could inspect the player shader stripping report and explicit filter variants. It is not necessary to disable render graph, add shader variants, change the pipeline asset, or install packages just to remove this warning.

Final Stage 04 browser run reproduced the same warning for `Hidden/Universal Render Pipeline/Edge Adaptive Spatial Upsampling` in `Blit FSR Upscaling`, followed by “PostProcessing render passes will not execute.” The scene rendered correctly in the browser with post-processing disabled on its cameras. No browser errors were observed in the captured log. This is a non-blocking warning for this configuration, not a guarantee for future post-processing changes.
