"""Suit oxygen budget (simulation). Rates are metabolic-equivalent assumptions in kg O2 per simulated hour;
real time is multiplied by time_compression to get simulated time. No penalties are applied by this module."""


class LifeSupportModel:
    def __init__(self, config):
        self.c = config.life_support

    @property
    def capacity_kg(self):
        return self.c['suit_o2_capacity_kg']

    def rate_kg_per_h(self, activity, weather_state):
        return self.c['activity_rate_kg_per_h'][activity] * self.c['weather_workload_multiplier'][weather_state] + self.c['leak_rate_kg_per_h']

    def consume(self, o2_kg, activity, weather_state, real_seconds):
        """Remaining kg after real_seconds of play. Never below 0."""
        sim_hours = real_seconds * self.c['time_compression'] / 3600.0
        return max(0.0, o2_kg - self.rate_kg_per_h(activity, weather_state) * sim_hours)

    def percent_per_real_second(self, activity, weather_state):
        """Same rate expressed the way the Unity HUD drains oxygen (% of tank per real second)."""
        return self.rate_kg_per_h(activity, weather_state) * self.c['time_compression'] / 3600.0 / self.capacity_kg * 100.0

    def status(self, o2_kg):
        f = o2_kg / self.capacity_kg
        t = self.c['thresholds_fraction']
        if f <= t['critical']:
            return 'critical'
        if f <= t['warning']:
            return 'warning'
        if f <= t['caution']:
            return 'caution'
        return 'nominal'

    def endurance_real_seconds(self, o2_kg, activity, weather_state):
        rate = self.rate_kg_per_h(activity, weather_state)
        return o2_kg / rate * 3600.0 / self.c['time_compression']
