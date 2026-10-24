# Mission and challenge architecture — plan, not completed systems

Implemented milestone uses Mission01Flow screen states and one proximity-gated interaction. It does not implement the full modular MissionManager or ChallengeManager. Local boarding completion is not sent into Stage07 telemetry; existing provisional telemetry contracts remain unchanged.

Next safe dependency: data-driven mission definitions with stable mission/objective IDs, prerequisites, recoverable failure conditions, verified transitions, versioned local snapshots and explicit host identity handoff. Extend the first mission only after its manual review.

ChallengeManager proposal: typed incident definition (ID, trigger, severity, duration, consequences, warnings, safe recovery actions, deterministic seed), scheduler, active incident state and recovery handler. Scripted onboarding first; controlled randomness later. Validate safe shelter/equipment accessibility before incident activation. Offer warning and recovery windows and prohibit unavoidable terminal failures.

Planned dust consequences: visibility/solar reduction and plausible equipment issues. Communications fail through equipment/power/antenna causes, not an unsupported claim that ordinary Martian wind blocks radio. Oxygen leak, rover fault and radiation shelter scenarios require later reviewed gameplay/resource models. None implemented here.

Backend Stage07 scoring and deterministic simulation remain authoritative only within their documented local foundation. Do not submit local signed client scores as tamper-proof evidence; account identity belongs to the existing host and authorization path.
