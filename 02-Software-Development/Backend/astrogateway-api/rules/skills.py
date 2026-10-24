"""Skill proficiency from repeated mission evidence. Inputs are per-session subscores from scoring.py,
never playtime. A level is reached only when ALL of its criteria hold (demonstrations, distinct sessions,
rolling mean of recent evidence, and the hardest difficulty at which the skill was demonstrated)."""
from dataclasses import dataclass


@dataclass(frozen=True)
class SkillEvidence:
    session_id: str
    skill: str
    value: float          # subscore in [0,1]
    difficulty: int
    order: int            # chronological order (server completion time)


class SkillAssessor:
    def __init__(self, config):
        self.c = config.skills

    @staticmethod
    def evidence_from_report(session_id, order, report):
        return [SkillEvidence(session_id, k, v, report['difficulty'], order) for k, v in report['subscores'].items()]

    def assess_skill(self, skill, evidence):
        items = sorted((e for e in evidence if e.skill == skill), key=lambda e: e.order)
        window = items[-self.c['rolling_window']:]
        demos = [e for e in items if e.value >= self.c['demonstration_threshold']]
        facts = {
            'assessments': len(items),
            'demonstrations': len(demos),
            'distinct_sessions': len({e.session_id for e in demos}),
            'rolling_mean': round(sum(e.value for e in window) / len(window), 4) if window else 0.0,
            'max_difficulty_demonstrated': max((e.difficulty for e in demos), default=-1),
        }
        level, name, missing = 0, self.c['levels'][0]['name'], []
        for spec in self.c['levels'][1:]:
            gaps = []
            if facts['demonstrations'] < spec['min_demonstrations']:
                gaps.append(f"{spec['min_demonstrations'] - facts['demonstrations']} more demonstration(s)")
            if facts['distinct_sessions'] < spec['min_distinct_sessions']:
                gaps.append(f"demonstrations in {spec['min_distinct_sessions']} different sessions")
            if facts['rolling_mean'] < spec['min_rolling_mean']:
                gaps.append(f"recent average of at least {spec['min_rolling_mean']:.0%}")
            if facts['max_difficulty_demonstrated'] < spec['min_difficulty_demonstrated']:
                gaps.append(f"a demonstration at difficulty {spec['min_difficulty_demonstrated']} or higher")
            if gaps:
                missing = gaps
                next_name = spec['name']
                break
            level, name = spec['level'], spec['name']
        else:
            next_name = None
        return {'skill': skill, 'level': level, 'name': name, **facts,
                'next_level': next_name, 'to_reach_next_level': missing if next_name else []}

    def assess(self, evidence):
        skills = {s: self.assess_skill(s, evidence) for s in self.c['skills']}
        trainee = min(s['level'] for s in skills.values())
        return {'skills': skills, 'trainee_level': trainee,
                'trainee_level_name': self.c['levels'][trainee]['name'],
                'rule': self.c['trainee_level_rule'], 'provisional': True}
