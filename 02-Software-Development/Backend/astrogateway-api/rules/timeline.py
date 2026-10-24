"""Builds a server-side mission timeline from server-recorded commands.
Only the command kind, its revision order and the SERVER accepted_at time are used. Client-reported ticks,
oxygen, weather or positions are ignored, so a modified client cannot award itself evidence beyond its actions."""
from dataclasses import dataclass, field
from datetime import datetime


@dataclass(frozen=True)
class Command:
    revision: int
    kind: str
    accepted_at: datetime
    payload: dict = field(default_factory=dict)
    participant_id: str = 'solo'   # Multiplayer-ready field; only 'solo' is used today.


@dataclass
class Segment:
    start_s: float          # active (unpaused) mission seconds
    end_s: float
    mode: str               # 'eva' or 'rover'


@dataclass
class Timeline:
    segments: list
    active_seconds: float
    scans: list
    board_exit_cycles: int
    completed: bool


def build_timeline(session_created_at, commands, completed_at=None, evaluated_at=None):
    """Turns ordered commands into EVA/rover segments in active mission seconds (pauses removed).
    Completed sessions end at completed_at; open or abandoned sessions at evaluated_at (server time, e.g. expiry)."""
    ordered = sorted(commands, key=lambda c: c.revision)
    end_wall = completed_at or evaluated_at or (ordered[-1].accepted_at if ordered else session_created_at)
    segments, scans = [], []
    mode, paused = 'eva', False
    active, last_wall = 0.0, session_created_at
    boards = exits = 0
    seen_samples = set()

    def advance(until):
        nonlocal active, last_wall
        dt = max(0.0, (min(until, end_wall) - last_wall).total_seconds())
        if not paused and dt > 0:
            segments.append(Segment(active, active + dt, mode))
            active += dt
        last_wall = max(last_wall, min(until, end_wall))

    for c in ordered:
        if c.accepted_at > end_wall:
            break                     # Ignore anything recorded after completion.
        advance(c.accepted_at)
        if c.kind == 'pause':
            paused = True
        elif c.kind == 'resume':
            paused = False
        elif c.kind == 'board_rover' and mode == 'eva':
            mode, boards = 'rover', boards + 1
        elif c.kind == 'exit_rover' and mode == 'rover':
            mode, exits = 'eva', exits + 1
        elif c.kind == 'scan_target' and not paused and mode == 'eva':
            sample = c.payload.get('sample_id')
            if sample and sample not in seen_samples:
                seen_samples.add(sample)
                scans.append(active)
    advance(end_wall)
    return Timeline(segments, active, scans, min(boards, exits), completed_at is not None)
