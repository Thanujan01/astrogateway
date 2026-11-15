/* ---------------------------------------------------------------------------
 * The single local profile. No sign-up, no login, no backend: this app is
 * built for one person. Display name and training-session count persist in
 * this browser only, via localStorage.
 * ------------------------------------------------------------------------- */

const PROFILE_KEY = 'ag-solo-profile-v1'
const SESSIONS_KEY = 'ag-solo-sessions-v1'
const DEFAULT_NAME = 'Thanujan'

export interface Profile {
  user_id: string
  display_name: string
  trainee_profile: 'astronaut_candidate'
  training_level: number
  created_at: string
}

function isProfile(value: unknown): value is Profile {
  if (typeof value !== 'object' || value === null) return false
  const v = value as Record<string, unknown>
  return typeof v.user_id === 'string' && v.user_id.length > 0
    && typeof v.display_name === 'string' && v.display_name.length >= 2 && v.display_name.length <= 64
    && v.trainee_profile === 'astronaut_candidate'
    && Number.isInteger(v.training_level)
    && typeof v.created_at === 'string' && !Number.isNaN(Date.parse(v.created_at))
}

function fresh(): Profile {
  return {
    user_id: crypto.randomUUID(),
    display_name: DEFAULT_NAME,
    trainee_profile: 'astronaut_candidate',
    training_level: 0,
    created_at: new Date().toISOString(),
  }
}

/** Storage can throw outright (private windows, blocked site data), not just return null. */
export function loadProfile(): Profile {
  try {
    const raw = localStorage.getItem(PROFILE_KEY)
    if (raw) {
      const parsed: unknown = JSON.parse(raw)
      if (isProfile(parsed)) return parsed
    }
  } catch {
    // Fall through to a fresh, in-memory-only profile for this session.
  }
  const profile = fresh()
  try { localStorage.setItem(PROFILE_KEY, JSON.stringify(profile)) } catch { /* best-effort */ }
  return profile
}

export function validateName(name: string) {
  const value = name.trim()
  if ([...value].length < 2 || [...value].length > 64 || [...value].some(c => c.codePointAt(0)! < 32 || (c.codePointAt(0)! >= 127 && c.codePointAt(0)! <= 159))) {
    throw new Error('Use 2–64 characters without control characters.')
  }
  return value
}

export function saveDisplayName(profile: Profile, name: string): Profile {
  const updated: Profile = { ...profile, display_name: validateName(name) }
  try { localStorage.setItem(PROFILE_KEY, JSON.stringify(updated)) } catch { /* best-effort */ }
  return updated
}

export function readSessionCount(): number {
  try {
    const raw = localStorage.getItem(SESSIONS_KEY)
    const n: unknown = raw ? JSON.parse(raw) : 0
    return typeof n === 'number' && Number.isFinite(n) && n >= 0 ? Math.floor(n) : 0
  } catch {
    return 0
  }
}

export function recordSessionStart(): number {
  const next = readSessionCount() + 1
  try { localStorage.setItem(SESSIONS_KEY, JSON.stringify(next)) } catch { /* best-effort */ }
  return next
}
