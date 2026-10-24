# Graphics quality system

NOT IMPLEMENTED: stopped at the rendering gate before code or settings changes. Low, Balanced, High and Ultra Preview remain requested presets, not available features.

On resumption, use new scene-local runtime pipeline clones, coordinate with MarsPrototype.previewPipeline, and restore original settings on exit. Validate render scale, shadow resolution/distance, texture mip limits, LOD bias, camera AA, particle emission, atmospheric effects and frame cap individually. Weather currently controls fog and dust, so quality controls must compose with weather rather than overwrite mission state. Ultra is an Editor/capable-hardware preview. Preset values and performance claims require measured trials.
