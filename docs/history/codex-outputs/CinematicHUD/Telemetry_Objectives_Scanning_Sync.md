# Data integrity and synchronization contract

| Widget | Actual provider | Meaning |
|---|---|---|
| Oxygen | MarsPrototype.oxygen | Existing local simulated percent |
| Power | MarsPrototype.power | Suit power, existing local simulated percent |
| Water / food / radiation | None | NOT CONNECTED; no invented values |
| Rover battery | NativeSliceGame.battery | Existing simulated battery |
| Rover speed | MarsPrototype.speed while driving | Actual existing controller movement speed in m/s; zero when not driving |
| Rover condition | NativeSliceGame.health | Existing simulated condition |
| Weather | MarsPrototype.WeatherName | Actual clear/light dust/storm state |
| Temperature / wind / Sol | None | Explicitly not connected / not implemented |
| Sim elapsed | MarsPrototype.elapsed | Local simulation elapsed time, not Martian local solar time |
| Mission name | MissionRegistry.displayName | Existing configured name; full canonical mission ID remains in the registry asset |
| Survey objective | MarsPrototype.objective | Original actual proximity progress / provisional completion string |
| Research stations | Existing ScientificScan locations + ResearchTarget transforms | Unique stations with successful saved scan records; repeated scans do not add duplicate station progress |
| Scientific results | NativeSliceGame.scans | Type, tick, synthetic index and simulated status; no fabricated chemical percentages |
| Inventory | NativeSliceGame.inventory | Actual list; explicit empty/not-implemented collection state |

The resource contract supports current/max/percentage, status and thresholds: low <=30%, critical <=15% for the implemented consumable meters. Animated fills are visual smoothing only; numerical readings remain direct source values. Missing meters remain unavailable rather than zero-percent live readings.

SCAN requires an existing target within the original 3m range, astronaut mode and no open menu. Its 0.8s progress is a UI presentation delay, not a scientific measurement model. During the delay, the existing ControlModeManager is set to Menu to lock input, without stopping the resource clock. Completion revalidates range, restores Astronaut mode and invokes ResearchTarget.TryScan → NativeSliceGame.Scan. Cancel restores the mode without recording. The current provider is proximity-based; no new line-of-sight/chemistry system is claimed. Colored contextual text identifies the current target; no object outline shader was added.

Existing E-based equipment scanning can still create source records. The HUD detects scan-count changes and updates its UI/export status; objective counters are derived from actual records, not button clicks. Scans remain local, simulated and provisional; no Stage07 authoritative witness added.

## Local status only

Initial: LOCAL ONLY — no cloud connection. A new scan: Pending Export — local only. Save succeeds: Saved Locally. Load: Loaded Locally. Export: Exported Locally. Exceptions: Failed local action. No code path sets Synced or claims ORBIS upload. Retry is the same actual save/load/export action. No remote destination or credentials are configured.

Save/load/export use the original data model and validation. The HUD scene uses AstroGateway_CinematicHUD_v1.json and a separate mission identifier, preserving earlier snapshot files. Existing snapshots are not automatically migrated into this new namespace. Original partial-snapshot limitations remain (not full orientation/inventory/witness persistence).
