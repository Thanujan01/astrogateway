"""Explainable next-mission and learning recommendations. RecommendationEngine is the replaceable interface:
a future ML model implements recommend() with the same inputs/outputs; the rules engine stays as fallback."""
from typing import Protocol


class RecommendationEngine(Protocol):
    def recommend(self, history: list, skills: dict) -> dict: ...


class RulesRecommendationEngine:
    """history: chronological session reports from scoring.score_session (completed sessions only).
    skills: output of SkillAssessor.assess()."""

    def __init__(self, config, catalog):
        self.c, self.catalog, self.cfg = config.recommendations, catalog, config

    def recommend(self, history, skills):
        fired, reasons = [], []
        mission = self.cfg.missions['canonical'][0]
        if not history:
            fired.append('R1_NEW_TRAINEE')
            reasons.append('No completed missions yet: start with the introductory difficulty.')
            return self._result(mission['mission_id'], 0, None, [], fired, reasons)

        last = history[-1]
        difficulty = last['difficulty']
        recent = [h['score'] for h in history[-len(self.c['raise_difficulty_after_scores']):]]
        if len(recent) == len(self.c['raise_difficulty_after_scores']) and all(s >= t for s, t in zip(recent, self.c['raise_difficulty_after_scores'])) and difficulty < self.c['max_difficulty']:
            difficulty += 1
            fired.append('R2_RAISE_DIFFICULTY')
            reasons.append(f'Last {len(recent)} scores {recent} met the raise threshold: difficulty {difficulty - 1} → {difficulty}.')
        elif len(recent) == len(self.c['lower_difficulty_after_scores']) and all(s < t for s, t in zip(recent, self.c['lower_difficulty_after_scores'])) and difficulty > 0:
            difficulty -= 1
            fired.append('R2_LOWER_DIFFICULTY')
            reasons.append(f'Last {len(recent)} scores {recent} were below the support threshold: difficulty {difficulty + 1} → {difficulty}.')
        else:
            fired.append('R2_KEEP_DIFFICULTY')
            reasons.append(f'Recent scores {recent} do not trigger a difficulty change.')

        order = self.cfg.skills['skills']
        per_skill = skills['skills']
        focus = min(order, key=lambda s: (per_skill[s]['level'], per_skill[s]['rolling_mean'], order.index(s)))
        fired.append('R3_FOCUS_WEAKEST_SKILL')
        reasons.append(f"Focus skill '{focus}' is the weakest (level {per_skill[focus]['level']}, recent average {per_skill[focus]['rolling_mean']:.0%}).")

        storm_from = mission['storm_scenario_from_difficulty']
        if per_skill['weather_safety']['assessments'] == 0 and difficulty < storm_from and 'R2_LOWER_DIFFICULTY' not in fired and last['score'] >= 70:
            difficulty = storm_from
            fired.append('R5_ASSESS_UNSEEN_SKILL')
            reasons.append(f'Weather safety has never been assessed and the last score was {last["score"]}: difficulty {storm_from} includes a dust storm.')

        modules = []
        for skill, value in last['subscores'].items():
            if value < self.c['learning_module_when_subscore_below']:
                module = self.catalog.module_for_skill(skill)
                if module:
                    modules.append(module['module_id'])
                    reasons.append(f"Last mission '{skill}' subscore {value:.0%} is below {self.c['learning_module_when_subscore_below']:.0%}: suggest '{module['title']}'.")
        if modules:
            fired.append('R4_LEARNING_MODULES')
        return self._result(mission['mission_id'], difficulty, focus, modules, fired, reasons)

    def _result(self, mission_id, difficulty, focus, modules, fired, reasons):
        return {'mission_id': mission_id, 'difficulty': difficulty, 'focus_skill': focus, 'learning_modules': modules,
                'rules_fired': fired, 'reasons': reasons, 'engine': self.c['engine'], 'rules_version': self.cfg.version,
                'provisional': True}
