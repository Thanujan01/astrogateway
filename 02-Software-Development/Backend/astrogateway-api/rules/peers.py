"""Peer comparison over SERVER-SOURCED mission reports using the existing skill rubric.

No HTTP endpoint accepts these inputs. The future repository adapter must supply
verified ownership, completed sessions, opt-in discovery and bilateral block data.
This module does not authenticate reports or create multiplayer rooms.
"""
from dataclasses import dataclass
from math import isfinite
from uuid import UUID
from .skills import SkillAssessor


@dataclass(frozen=True)
class CompletedEvidence:
    session_id: str
    completed_order: int
    report: dict


@dataclass(frozen=True)
class Peer:
    user_id: str
    display_name: str
    discoverable: bool
    history: tuple[CompletedEvidence, ...]


class PeerMatcher:
    def __init__(self, config):
        self.config = config
        self.assessor = SkillAssessor(config)
        self.skills = tuple(config.skills['skills'])
        self.max_level = max(x['level'] for x in config.skills['levels'])

    def assessment(self, history):
        """Deduplicate sessions before passing evidence to the unchanged assessor."""
        unique = {}
        for item in history:
            if not isinstance(item.completed_order, int) or isinstance(item.completed_order, bool):
                raise ValueError('Invalid server completion order.')
            session = str(UUID(item.session_id))
            report = item.report
            if report.get('rules_version') != self.config.version:
                raise ValueError('Incompatible rules version.')
            difficulty = report.get('difficulty')
            if type(difficulty) is not int or difficulty not in (0, 1, 2):
                raise ValueError('Invalid assessed difficulty.')
            subscores = report.get('subscores')
            if not isinstance(subscores, dict) or not subscores or set(subscores) - set(self.skills):
                raise ValueError('Invalid assessed skill names.')
            if any(type(v) not in (int, float) or not isfinite(v) or not 0 <= v <= 1 for v in subscores.values()):
                raise ValueError('Invalid assessed subscore.')
            # Only rubric-relevant facts form the comparison, never client labels/score totals.
            facts = (item.completed_order, difficulty, tuple(sorted(subscores.items())))
            if session in unique:
                if unique[session][0] != facts:
                    raise ValueError('Conflicting evidence for one completed session.')
                continue
            unique[session] = (facts, item)
        evidence = []
        for _, item in sorted(unique.values(), key=lambda pair: (pair[1].completed_order, pair[1].session_id)):
            evidence.extend(self.assessor.evidence_from_report(item.session_id, item.completed_order, item.report))
        return self.assessor.assess(evidence)

    def suggest(self, viewer, candidates, blocked_ids=(), limit=10):
        if type(limit) is not int or not 1 <= limit <= 20:
            raise ValueError('Limit must be 1–20.')
        owner = str(UUID(viewer.user_id))
        blocked = {str(UUID(x)) for x in blocked_ids}
        own = self.assessment(viewer.history)
        rows, seen = [], set()
        for candidate in candidates:
            peer_id = str(UUID(candidate.user_id))
            if peer_id in seen:
                raise ValueError('Duplicate candidate identity.')
            seen.add(peer_id)
            if peer_id == owner or peer_id in blocked or not candidate.discoverable:
                continue
            other = self.assessment(candidate.history)
            shared = [s for s in self.skills if own['skills'][s]['assessments'] > 0 and other['skills'][s]['assessments'] > 0]
            if not shared:
                continue  # no evidence is not an equal-skill match
            differences = {s: abs(own['skills'][s]['level'] - other['skills'][s]['level']) for s in shared}
            distance = sum(differences.values()) / (len(shared) * self.max_level)
            rows.append({
                'user_id': peer_id, 'display_name': candidate.display_name,
                'rules_version': self.config.version,
                'comparison': 'existing_rubric_levels', 'provisional': True,
                'similarity': round(1 - distance, 4),
                'shared_assessed_skills': shared,
                'uncompared_skills': [s for s in self.skills if s not in shared],
                'level_differences': differences,
                'explanation': 'Comparison of existing rubric levels on shared assessed skills; not proof of identical ability.',
            })
        # Coverage comes first: one matching skill must not outrank a full matching profile.
        return sorted(rows, key=lambda row: (-len(row['shared_assessed_skills']), -row['similarity'], row['user_id']))[:limit]
