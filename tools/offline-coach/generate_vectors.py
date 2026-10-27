"""Generate exhaustive threshold fixtures using the existing backend teacher; no player data."""
import json
import sys
from pathlib import Path
ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / '02-Software-Development/Backend/astrogateway-api'))
from rules.config import load_config
from rules.catalog import MissionCatalog
from rules.recommendations import RulesRecommendationEngine
from rules.skills import SkillAssessor
cfg = load_config()
engine = RulesRecommendationEngine(cfg, MissionCatalog(cfg))
rows = []
points = [0, 49.999, 50, 69.999, 70, 84.999, 85, 100]
for count in (0, 1, 2):
    for difficulty in range(3):
        for weather in (0, 1):
            for previous in (points if count == 2 else [0]):
                for latest in (points if count else [0]):
                    scores = [] if count == 0 else ([previous, latest] if count == 2 else [latest])
                    history = [dict(difficulty=difficulty, score=s, subscores={}) for s in scores]
                    skills = SkillAssessor(cfg).assess([])
                    skills['skills']['weather_safety']['assessments'] = weather
                    expected = engine.recommend(history, skills)
                    rows.append(dict(scores=scores, difficulty=difficulty, weather=weather,
                                     expected=expected['difficulty']))
out = ROOT / '01-Game-Development/MissionRedHorizon/Assets/AstroGateway/MissionRules/Config/offline_coach_vectors.json'
out.write_text(json.dumps(dict(rulesVersion=cfg.version, cases=rows), separators=(',', ':')) + '\n')
print(f'{len(rows)} synthetic boundary cases -> {out}')
