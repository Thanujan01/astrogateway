"""Deterministic Mars weather scenario (simulation). Values are gameplay approximations informed by published
ranges (dust optical depth, wind, Gale-crater-like temperatures); they are not measurements or forecasts."""
import math


class WeatherModel:
    def __init__(self, config):
        self.w = config.weather

    def state_at(self, difficulty, mission_seconds):
        steps = self.w['scenarios'][str(difficulty)]
        current = steps[0]['state']
        for step in steps:
            if mission_seconds >= step['start_s']:
                current = step['state']
        return current

    def storm_intervals(self, difficulty, end_s):
        """[(start, end)] of 'dust_storm' periods within [0, end_s]."""
        steps = self.w['scenarios'][str(difficulty)]
        out = []
        for i, step in enumerate(steps):
            if step['state'] != 'dust_storm':
                continue
            start = step['start_s']
            stop = steps[i + 1]['start_s'] if i + 1 < len(steps) else end_s
            if start < end_s:
                out.append((start, min(stop, end_s)))
        return out

    def conditions(self, difficulty, mission_seconds, local_hour=12.0):
        state = self.state_at(difficulty, mission_seconds)
        s = self.w['states'][state]
        d = self.w['diurnal_temperature_c']
        # Simple cosine diurnal cycle peaking at peak_hour; dust narrows the range via a warm night-time offset.
        phase = math.cos((local_hour - d['peak_hour']) / 24.0 * 2 * math.pi)
        temperature = d['min'] + (d['max'] - d['min']) * (phase + 1) / 2 + s['temp_offset_c']
        return {
            'state': state,
            'tau': s['tau'],
            'wind_mps': s['wind_mps'],
            'gust_mps': s['gust_mps'],
            'visibility_m': s['visibility_m'],
            'temperature_c': round(temperature, 3),
            'solar_power_factor': round(math.exp(-self.w['solar_power_extinction_k'] * s['tau']), 6),
            'advisory': self.w['advisory_by_state'][state],
            'source': 'simulation',
        }
