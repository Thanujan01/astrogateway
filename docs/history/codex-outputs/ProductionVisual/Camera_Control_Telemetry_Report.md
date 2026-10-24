# Camera, controls and telemetry
ProductionCameraRig uses the existing follow Camera, existing yaw/pitch and original active actor. Smooth exponential damping, distinct astronaut/rover distances/FOV, short/long chase, scanner framing and cinematic overview are added. C / OVERVIEW toggles presentation without spawning an actor; simulation continues, movement is canonically locked. Optional cockpit view is not implemented.

Camera spherecasts use world layer0, radius .24m, then test the smoothed position against the same world sweep. Temporary wall validation detected obstruction and kept a .15m test sphere outside the wall. This is a controlled fixture, not a guarantee against every habitat/terrain angle, target-inside-collider or rapid transition.

PC W/RMB checks used real Input System Keyboard/Mouse events inside batch Play Mode. They are stronger than setting movement/yaw fields but are still automated, not hands-on OS keyboard/mouse verification. Existing independent pointer tests route multi-touch-style event data; physical multi-touch is untested. User-assisted diagonal/Shift and long terrain traversal remain manual checks.

HUD speed/battery/health and suit oxygen/power read existing simulated providers. Scan records are existing GUID/tick synthetic instrument data, not observations or authoritative mission witnesses. All event semantics remain provisional. Water/food/radiation/weather measurements/Sol are labelled unavailable. ORBIS/cloud upload remains unavailable. No fake Cloud Synced label.

New scene local snapshot namespace: AstroGateway_ProductionVisual_v1.json. Original files/namespaces are untouched; no automatic migration of saves. Existing partial save model retains its limitations. No backend/AI contract, Supabase, web or native Android code changed.
