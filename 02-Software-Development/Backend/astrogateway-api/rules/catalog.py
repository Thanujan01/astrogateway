"""Canonical mission identifiers with a read-only alias map. Stored identifiers are never rewritten:
callers keep the original value and use resolve() to find the canonical mission for rules and reporting."""
from dataclasses import dataclass


@dataclass(frozen=True)
class ResolvedMission:
    canonical_id: str
    original_id: str
    is_alias: bool
    build_variant: str | None
    definition: dict


class MissionCatalog:
    def __init__(self, config):
        self._missions = {m['mission_id']: m for m in config.missions['canonical']}
        self._aliases = {a['alias']: a for a in config.missions['aliases']}
        self.learning_modules = list(config.missions['learning_modules'])

    def resolve(self, mission_id):
        if mission_id in self._missions:
            return ResolvedMission(mission_id, mission_id, False, None, self._missions[mission_id])
        alias = self._aliases.get(mission_id)
        if alias is None:
            raise KeyError('Unknown mission id: ' + str(mission_id))
        return ResolvedMission(alias['canonical'], mission_id, True, alias.get('build_variant'), self._missions[alias['canonical']])

    def missions_for_skill(self, skill):
        return [m for m in self._missions.values() if skill in m['focus_skills']]

    def module_for_skill(self, skill):
        return next((m for m in self.learning_modules if m['skill'] == skill), None)
