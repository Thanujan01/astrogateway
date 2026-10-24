"""Transparent mission score: score = 100 * sum(w_i * s_i). Each subscore s_i is in [0,1] and comes with a
plain-language explanation. Criteria that cannot be assessed in a session (e.g. no dust storm in the scenario)
are excluded and the remaining weights are renormalised, which is reported explicitly."""
from .life_support import LifeSupportModel
from .weather import WeatherModel

SERVER_ACTIVITY = {'eva': 'walking', 'rover': 'driving'}   # Server cannot see speed: EVA is costed as walking.


def _clamp(x):
    return max(0.0, min(1.0, x))


def _split_by_weather(weather, difficulty, segment):
    steps = weather.w['scenarios'][str(difficulty)]
    cuts = [segment.start_s] + [s['start_s'] for s in steps if segment.start_s < s['start_s'] < segment.end_s] + [segment.end_s]
    for a, b in zip(cuts, cuts[1:]):
        yield a, b, weather.state_at(difficulty, a)


def simulate_oxygen(config, difficulty, timeline):
    ls, weather = LifeSupportModel(config), WeatherModel(config)
    o2 = ls.capacity_kg * config.life_support['start_fraction']
    minimum = o2
    for seg in timeline.segments:
        for a, b, state in _split_by_weather(weather, difficulty, seg):
            o2 = ls.consume(o2, SERVER_ACTIVITY[seg.mode], state, b - a)
            minimum = min(minimum, o2)
    return o2, minimum


def score_session(config, mission, difficulty, timeline):
    sc, ls_cfg = config.scoring, config.life_support
    weather = WeatherModel(config)
    sub, notes = {}, {}

    required = mission.definition['required_scans']
    n = len(timeline.scans)
    sub['observation'] = _clamp(n / required)
    notes['observation'] = f'{n} of {required} required simulated scans recorded (distinct sample ids, EVA only).'

    end_kg, min_kg = simulate_oxygen(config, difficulty, timeline)
    t = ls_cfg['thresholds_fraction']
    frac = end_kg / LifeSupportModel(config).capacity_kg
    sub['life_support'] = 0.0 if min_kg <= 0 else _clamp((frac - t['critical']) / (t['caution'] - t['critical']))
    notes['life_support'] = f'Server-estimated suit O2 at end {frac:.0%} (full marks at or above {t["caution"]:.0%}, zero at {t["critical"]:.0%}).'

    storms = weather.storm_intervals(difficulty, timeline.active_seconds)
    storm_total = sum(b - a for a, b in storms)
    if storm_total > 0:
        sheltered = sum(max(0.0, min(b, s.end_s) - max(a, s.start_s)) for a, b in storms for s in timeline.segments if s.mode == 'rover')
        sub['weather_safety'] = _clamp(sheltered / storm_total)
        notes['weather_safety'] = f'Sheltered in the rover for {sheltered:.0f} of {storm_total:.0f} s of simulated dust storm.'
    else:
        notes['weather_safety'] = 'Not assessed: no dust storm in this scenario.'

    target_cycles = sc['rover_operation_target_cycles']
    sub['rover_operation'] = _clamp(timeline.board_exit_cycles / target_cycles)
    notes['rover_operation'] = f'{timeline.board_exit_cycles} completed board/exit cycle(s); target {target_cycles}.'

    target, limit = mission.definition['target_duration_s'], mission.definition['max_duration_s']
    d = timeline.active_seconds
    if not timeline.completed:
        sub['mission_planning'] = 0.0
        notes['mission_planning'] = 'Mission not completed.'
    else:
        sub['mission_planning'] = 1.0 if d <= target else _clamp((limit - d) / (limit - target))
        notes['mission_planning'] = f'Completed in {d:.0f} s active time (full marks up to {target} s, zero at {limit} s).'

    weights = {k: v for k, v in sc['weights'].items() if k in sub}
    total_w = sum(weights.values())
    used = {k: v / total_w for k, v in weights.items()}
    score = round(100 * sum(used[k] * sub[k] for k in used), 1)
    grade = next(b['label'] for b in sc['grade_bands'] if score >= b['min'])
    return {
        'rules_version': config.version,
        'mission_id': mission.canonical_id,
        'original_mission_id': mission.original_id,
        'difficulty': difficulty,
        'score': score,
        'grade': grade,
        'subscores': {k: round(v, 4) for k, v in sub.items()},
        'weights_applied': {k: round(v, 4) for k, v in used.items()},
        'not_assessed': [k for k in sc['weights'] if k not in sub],
        'explanations': notes,
        'oxygen_end_fraction': round(frac, 4),
        'active_seconds': round(d, 1),
        'provisional': True,
        'evidence_source': sc['evidence_sources'],
    }
