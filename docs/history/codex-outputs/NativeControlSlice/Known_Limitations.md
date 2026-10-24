# Known limitations

- Existing primitive astronaut/rover/environment retained; no new finished licensed models or production animations.
- Seat attachment is instantaneous; no boarding cinematic, cockpit view or robot feed. Existing chase camera reused.
- Mode enum includes future names but Robot/Cockpit explicitly unavailable. Menu blocks input only; it is not a pause of the survival simulation.
- Touch ownership tests are synthetic, not device fingers. Main-Editor human controls/camera and physical Android remain untested.
- Inspector controls configure handedness/size/sensitivity/opacity; no finalized player preferences screen or saved preferences.
- Native HUD is immediate-mode prototype UI. Actual screenshot exposed overlap; fixed by hiding the old HUD only at runtime in the new scene and displaying resources/objective in the new panel.
- Full 30-Sol clock/resources/replenishment/failure/recovery, robotics/tuning/upgrades, ORBIS and native share APIs not implemented.
- Local save covers the listed slice state, not original target-completion/inventory/orientation/full-survival state. Some changed-world/load safety failures may partially alter state; do not claim transactional full-game persistence.
- Health is an initialized simulated value with no damage model. Science is synthetic index data; sol is placeholder 1.
- Authenticated identity remains in existing host. No new Unity login, authenticated handoff, backend transmission or authoritative result.
- QuickSearch exception remains in Editor logs. No performance benchmarks or packaged builds.
