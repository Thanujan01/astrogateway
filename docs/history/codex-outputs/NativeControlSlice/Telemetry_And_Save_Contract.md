# Telemetry and local data contract

ITelemetrySink.Record(id, source, value, unit, tick) rejects non-finite numeric values. TelemetryHistory retains at most 256 records and emits Updated events. Initial producers are rover battery/speed/health and astronaut oxygen. Field IDs are rover.battery, rover.speed, rover.health and astronaut.oxygen. Units are simulation_percent or m/s. Quality valid_simulated signifies valid local synthetic input, not authenticated scientific truth.

Sampling uses a one-second accumulator, not a 30-Sol physical simulation clock. Public Unity fields remain developer-tunable; client state is not a secure source of final scores. Existing Stage07 ingestion/event schemas were not replaced or fed these new fields automatically. Mapping requires a later explicit adapter and witness approval.

Separate local snapshot: AstroGateway_NativeSlice_v1.json under Application.persistentDataPath. Includes version, mission ID, avatar/rover positions, driving state, oxygen/power/battery/health, tick and up to 32 scans. No password, tokens, Supabase identity or cloud sync. Validate ranges, finite values, bounds, scan GUID uniqueness and simulated instrument/type/analysis before load. Atomic temporary-file replacement preserves the previous file on normal write completion. Corrupt input is rejected.

Scope limitations: inventory, astronaut health, rover orientation, original mission visited-target set, cloud identity and telemetry history are not persisted. Entering/exiting retains the same live astronaut object; this is not a full game-save implementation. Loading can fail if an exit or reboarding safety condition is unavailable; comprehensive failure-atomic recovery under changed obstacles is not yet proven. Do not use edited snapshots as trusted mission evidence.

Scans: sample ID, location in Unity meter coordinates, tick, sol placeholder=1, type simulated_surface_scan, instrument prototype.synthetic_scanner, unverified_simulated analysis, synthetic index [0,1]. This is no NASA dataset or real laboratory reading.

Export button writes NativeSliceExports/AstroGateway_Simulated_Scans.json, AstroGateway_Simulated_Scans.csv and AstroGateway_Simulated_Telemetry.json locally. No network or platform-share implementation. Player must deliberately share files externally; no automatic upload exists. Current UI is immediate-mode prototype UI, not a finalized event-driven production HUD.
