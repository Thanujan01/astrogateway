"""Mission rule engines (Step 4).
Every value comes from mission_rules_v1.json (PROPOSED defaults, pending review). Simulation only.

Wiring status (keep this accurate when endpoints change):
  - config and catalog ARE on the request path. sensor_records.py imports load_config and
    MissionCatalog to resolve/validate mission ids, and main.py registers those routes.
  - The assessment engines below (life_support, weather, timeline, scoring, skills, peers,
    recommendations) are reachable from NO endpoint. /v1/reports and /v1/skills still return
    score=None / levels=None with rubric_status='pending_approval' until the rubric is approved.
"""
from .config import RulesConfig, load_config
from .catalog import MissionCatalog
from .life_support import LifeSupportModel
from .weather import WeatherModel
from .timeline import Command, build_timeline
from .scoring import score_session
from .skills import SkillAssessor
from .recommendations import RulesRecommendationEngine, RecommendationEngine
