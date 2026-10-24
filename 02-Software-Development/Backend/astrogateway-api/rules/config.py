"""Loads and validates the rules configuration. Invalid configuration fails loudly instead of scoring silently."""
import json
from dataclasses import dataclass
from pathlib import Path

DEFAULT_PATH = Path(__file__).with_name('mission_rules_v1.json')
ACTIVITIES = ('idle', 'scanning', 'driving', 'walking', 'running')
WEATHER_STATES = ('clear', 'light_dust', 'dust_storm')
SKILLS = ('observation', 'life_support', 'weather_safety', 'rover_operation', 'mission_planning')


@dataclass(frozen=True)
class RulesConfig:
    raw: dict

    @property
    def version(self): return self.raw['rules_version']
    @property
    def life_support(self): return self.raw['life_support']
    @property
    def weather(self): return self.raw['weather']
    @property
    def scoring(self): return self.raw['scoring']
    @property
    def skills(self): return self.raw['skills']
    @property
    def recommendations(self): return self.raw['recommendations']
    @property
    def missions(self): return self.raw['missions']


def _require(condition, message):
    if not condition:
        raise ValueError('Invalid mission rules: ' + message)


def validate(raw):
    ls = raw['life_support']
    _require(ls['suit_o2_capacity_kg'] > 0 and ls['time_compression'] > 0, 'capacity and time compression must be positive')
    _require(set(ls['activity_rate_kg_per_h']) == set(ACTIVITIES), 'activity rates must cover ' + ', '.join(ACTIVITIES))
    _require(all(0 < r < 1 for r in ls['activity_rate_kg_per_h'].values()), 'activity rates must be between 0 and 1 kg/h')
    _require(set(ls['weather_workload_multiplier']) == set(WEATHER_STATES), 'workload multipliers must cover all weather states')
    t = ls['thresholds_fraction']
    _require(0 < t['critical'] < t['warning'] < t['caution'] < 1, 'thresholds must satisfy 0 < critical < warning < caution < 1')
    w = raw['weather']
    _require(set(w['states']) == set(WEATHER_STATES), 'weather states must be ' + ', '.join(WEATHER_STATES))
    for difficulty, steps in w['scenarios'].items():
        starts = [s['start_s'] for s in steps]
        _require(starts[0] == 0 and starts == sorted(starts) and len(set(starts)) == len(starts), f'scenario {difficulty} must start at 0 and increase')
        _require(all(s['state'] in WEATHER_STATES for s in steps), f'scenario {difficulty} uses an unknown state')
    weights = raw['scoring']['weights']
    _require(set(weights) == set(SKILLS), 'score weights must cover every skill')
    _require(abs(sum(weights.values()) - 1.0) < 1e-9 and all(v >= 0 for v in weights.values()), 'score weights must be non-negative and sum to 1')
    levels = raw['skills']['levels']
    _require([l['level'] for l in levels] == list(range(len(levels))), 'skill levels must be numbered 0..n')
    for a, b in zip(levels, levels[1:]):
        _require(all(b[k] >= a[k] for k in ('min_demonstrations', 'min_distinct_sessions', 'min_rolling_mean', 'min_difficulty_demonstrated')), 'level criteria must not decrease')
    canonical = {m['mission_id'] for m in raw['missions']['canonical']}
    aliases = [a['alias'] for a in raw['missions']['aliases']]
    _require(len(aliases) == len(set(aliases)) and not canonical & set(aliases), 'aliases must be unique and distinct from canonical ids')
    _require(all(a['canonical'] in canonical for a in raw['missions']['aliases']), 'every alias must map to a canonical mission')
    return raw


def load_config(path=DEFAULT_PATH):
    return RulesConfig(validate(json.loads(Path(path).read_text(encoding='utf-8'))))
