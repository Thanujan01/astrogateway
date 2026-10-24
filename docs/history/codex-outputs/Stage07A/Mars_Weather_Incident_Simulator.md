# Mars conditions and incident simulator
Rule07a.1 uses isolated random.Random(uint32 seed); same seed/difficulty/rule/runtime gives identical JSON. Store seed+rule version and pin runtime for cross-release replay. No global RNG, current clock, external weather feed or trained forecast model. No mission state controlled by ARIA language output.

**Observed/general science:** Mars is cold with a thin atmosphere; suspended dust affects its appearance; NASA describes broad planetary temperature extremes around−153…20°C. This is not a Jezero seasonal series or a scenario probability distribution. [NASA Mars facts](https://science.nasa.gov/mars/facts/).

Dust accumulating on solar arrays reduces incoming sunlight; missions have observed cleaning events. [NASA rover solar-panel dust](https://science.nasa.gov/photojournal/solar-panel-dust-accumulation-and-cleanings/). No numerical simulator coefficient is inferred from that observation.

**Synthetic game parameters:** difficulty index d=0/1/2; temperature uniform(−85,−20)−3d°C; dust index uniform(.05,.35)+.18d; visibility=300(1−dust)^2m; solar fraction=max(.1,1−.85dust); slope uniform(0,8)+4d degrees; rover limit=1.5(1−.4dust)/(1+.2d)m/s; route risk=min(1,.5dust+.15d). Dust index is a dimensionless game knob, not measured optical depth. Rover limit is a game rule, not rover operational speed data. No Mars pressure/radiation/EVA life-support physics is claimed.

Incident probability=.15+.15d, then uniform selection from none/dust/power_loss/rover_fault/oxygen_leak. Thus actual non-none incident probability=.8×that knob. These are fictional frequencies; no NASA failure rates or reliability models. Hazard severity/duration and equipment recovery rules require later mission implementation. Current output is a scenario descriptor only; Unity physics/VFX/environment remain unchanged.

Use fixed seeds0,1,42,uint32 max across all tiers; tests cover repeatability and range. Examples embedded in 30-day JSONs. Approved PDS sample is an image METADATA label from Uranius Fossae, not a weather observation or terrain DEM; it is never used to tune these parameters. Future calibrated use requires matching location, solar longitude, local time, processing level, units and uncertainty; orbital atmospheric temperature is not interchangeable with surface temperature.
