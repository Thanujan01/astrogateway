/* ---------------------------------------------------------------------------
 * Trial progress: the single owner of the challenge-score storage schema.
 *
 * Both the trial runner and the profile rating read through here, so the shape
 * is defined once. Records carry their own `max` and track labels, which keeps
 * the profile page from having to import the 81 KB question bank just to work
 * out denominators and headings.
 *
 * This is per-browser convenience data. It is deliberately not sent anywhere:
 * the API has no quiz endpoint, and /v1/reports still returns score: null with
 * rubric_status "pending_approval".
 * ------------------------------------------------------------------------- */

const KEY = 'ag-challenges-v1'
export const MAX_STARS = 5

export interface LevelRecord {
  /** Best total stars earned on this level. */
  best: number
  /** Stars available on that run (scenarios x MAX_STARS). 0 means unknown. */
  max: number
  attempts: number
  updatedAt: string | null
}

export interface TrackRecord {
  label: string
  ageLabel: string
  levels: Record<string, LevelRecord>
}

export interface Progress {
  version: 2
  tracks: Record<string, TrackRecord>
}

export const EMPTY: Progress = { version: 2, tracks: {} }

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === 'object' && value !== null && !Array.isArray(value)
}

function finite(value: unknown, fallback = 0) {
  return typeof value === 'number' && Number.isFinite(value) ? value : fallback
}

/**
 * Version 1 stored `{ [trackId]: { [levelN]: bestStars } }` with no denominator.
 * Those entries are kept so nobody loses a score, with max 0 meaning "unknown";
 * `summarise` counts them as attempted but leaves them out of the average until
 * the level is replayed, at which point a real max is written.
 */
function migrate(raw: unknown): Progress {
  if (!isRecord(raw)) return EMPTY
  if (raw.version === 2 && isRecord(raw.tracks)) {
    const tracks: Record<string, TrackRecord> = {}
    for (const [trackId, value] of Object.entries(raw.tracks)) {
      if (!isRecord(value)) continue
      const levels: Record<string, LevelRecord> = {}
      if (isRecord(value.levels)) {
        for (const [levelN, entry] of Object.entries(value.levels)) {
          if (!isRecord(entry)) continue
          levels[levelN] = {
            best: finite(entry.best),
            max: finite(entry.max),
            attempts: finite(entry.attempts, 1),
            updatedAt: typeof entry.updatedAt === 'string' ? entry.updatedAt : null,
          }
        }
      }
      tracks[trackId] = {
        label: typeof value.label === 'string' ? value.label : trackId,
        ageLabel: typeof value.ageLabel === 'string' ? value.ageLabel : '',
        levels,
      }
    }
    return { version: 2, tracks }
  }
  // Version 1 shape.
  const tracks: Record<string, TrackRecord> = {}
  for (const [trackId, value] of Object.entries(raw)) {
    if (!isRecord(value)) continue
    const levels: Record<string, LevelRecord> = {}
    for (const [levelN, best] of Object.entries(value)) {
      if (typeof best !== 'number' || !Number.isFinite(best)) continue
      levels[levelN] = { best, max: 0, attempts: 1, updatedAt: null }
    }
    if (Object.keys(levels).length) {
      tracks[trackId] = { label: trackId, ageLabel: '', levels }
    }
  }
  return { version: 2, tracks }
}

/** Storage can throw outright (private windows, blocked site data), not just return null. */
export function readProgress(): Progress {
  try {
    const raw = localStorage.getItem(KEY)
    return raw ? migrate(JSON.parse(raw)) : EMPTY
  } catch {
    return EMPTY
  }
}

function write(progress: Progress) {
  try {
    localStorage.setItem(KEY, JSON.stringify(progress))
  } catch {
    // Progress is a nicety; losing it must never break a trial.
  }
}

export interface TrialResult {
  trackId: string
  label: string
  ageLabel: string
  levelN: number
  earned: number
  max: number
}

/**
 * Fold a finished trial into stored progress and persist it. Returns the new
 * state so callers can keep React state in step without re-reading storage.
 * Only the best run per level counts, but every run bumps `attempts`.
 */
export function recordTrial(current: Progress, result: TrialResult): Progress {
  const key = String(result.levelN)
  const track = current.tracks[result.trackId]
  const previous = track?.levels[key]
  const improved = !previous || result.earned > previous.best

  const level: LevelRecord = {
    best: improved ? result.earned : previous.best,
    // Always refresh the denominator: a v1 record or a changed bank may have had 0.
    max: result.max,
    attempts: (previous?.attempts ?? 0) + 1,
    updatedAt: new Date().toISOString(),
  }

  const next: Progress = {
    version: 2,
    tracks: {
      ...current.tracks,
      [result.trackId]: {
        label: result.label,
        ageLabel: result.ageLabel,
        levels: { ...track?.levels, [key]: level },
      },
    },
  }
  write(next)
  return next
}

export interface TrackSummary {
  trackId: string
  label: string
  ageLabel: string
  levelsAttempted: number
  attempts: number
  stars: number
  max: number
  scenarios: number
  /** Mean stars per scenario, 0-5. Null when nothing rateable is stored. */
  average: number | null
}

export interface Summary {
  stars: number
  max: number
  scenarios: number
  levelsAttempted: number
  attempts: number
  average: number | null
  /** Levels kept from v1 storage that have no denominator yet. */
  unrated: number
  tracks: TrackSummary[]
}

export function summarise(progress: Progress): Summary {
  const tracks: TrackSummary[] = []
  let unrated = 0

  for (const [trackId, record] of Object.entries(progress.tracks ?? {})) {
    // readProgress sanitises, but this is exported: never trust the argument.
    if (!isRecord(record) || !isRecord(record.levels)) continue
    const entries = Object.values(record.levels).filter(isRecord) as unknown as LevelRecord[]
    if (!entries.length) continue
    const rated = entries.filter(level => level.max > 0)
    unrated += entries.length - rated.length
    const stars = rated.reduce((sum, level) => sum + level.best, 0)
    const max = rated.reduce((sum, level) => sum + level.max, 0)
    const scenarios = max / MAX_STARS
    tracks.push({
      trackId,
      label: record.label || trackId,
      ageLabel: record.ageLabel,
      levelsAttempted: entries.length,
      attempts: entries.reduce((sum, level) => sum + level.attempts, 0),
      stars,
      max,
      scenarios,
      average: scenarios > 0 ? stars / scenarios : null,
    })
  }

  const stars = tracks.reduce((sum, track) => sum + track.stars, 0)
  const max = tracks.reduce((sum, track) => sum + track.max, 0)
  const scenarios = max / MAX_STARS

  return {
    stars,
    max,
    scenarios,
    levelsAttempted: tracks.reduce((sum, track) => sum + track.levelsAttempted, 0),
    attempts: tracks.reduce((sum, track) => sum + track.attempts, 0),
    average: scenarios > 0 ? stars / scenarios : null,
    unrated,
    tracks,
  }
}
