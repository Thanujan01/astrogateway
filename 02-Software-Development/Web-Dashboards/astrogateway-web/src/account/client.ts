import { createClient } from '@supabase/supabase-js'
const url = import.meta.env.VITE_SUPABASE_URL?.trim() ?? ''
const key = import.meta.env.VITE_SUPABASE_PUBLISHABLE_KEY?.trim() ?? ''
export const configurationReady = url === 'https://jvkwofmxtxjhdwtwdboz.supabase.co' && key.startsWith('sb_publishable_')
export const client = configurationReady ? createClient(url, key, { auth: {
  flowType: 'pkce', detectSessionInUrl: false, persistSession: true, autoRefreshToken: true,
}, global: { fetch: (input, init) => fetch(input, { ...init, signal: init?.signal ? AbortSignal.any([init.signal, AbortSignal.timeout(15000)]) : AbortSignal.timeout(15000) }) } }) : null
export interface Profile { user_id: string; display_name: string; trainee_profile: string; training_level: number; created_at: string }
export class AccountError extends Error { status: number; constructor(status: number, message: string) { super(message); this.status = status } }
export function validateName(name: string) {
  const value = name.trim()
  if ([...value].length < 2 || [...value].length > 64 || [...value].some(c => c.codePointAt(0)! < 32 || (c.codePointAt(0)! >= 127 && c.codePointAt(0)! <= 159))) throw new Error('Use 2–64 characters without control characters.')
  return value
}
export async function profileRequest(token: string, expectedId: string, name?: string, signal?: AbortSignal): Promise<Profile> {
  const base = import.meta.env.VITE_AUTH_API_BASE || '/account-api'
  if (base !== '/account-api' && !/^https:\/\//.test(base)) throw new Error('Profile API requires HTTPS.')
  let response: Response
  try { response = await fetch(base + (name === undefined ? '/v1/me' : '/v1/me/display-name'), {
    method: name === undefined ? 'GET' : 'POST', signal: signal ? AbortSignal.any([signal, AbortSignal.timeout(10000)]) : AbortSignal.timeout(10000), redirect: 'error',
    headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
    ...(name === undefined ? {} : { body: JSON.stringify({ display_name: validateName(name) }) }),
  }) } catch { throw new AccountError(503, 'Unable to reach the profile service. Check your connection and retry.') }
  if (!response.ok) throw new AccountError(response.status, response.status === 401 ? 'Your session expired. Sign in again.' : response.status === 422 ? 'Use 2–64 characters without control characters.' : 'Profile service unavailable or access denied. Retry when connected.')
  const text = await response.text()
  if (text.length > 32768) throw new AccountError(502, 'Invalid profile response.')
  let profile: Profile
  try { profile = JSON.parse(text) as Profile } catch { throw new AccountError(502, 'Invalid profile response.') }
  if (profile.user_id !== expectedId || typeof profile.display_name !== 'string' || typeof profile.created_at !== 'string' || Number.isNaN(Date.parse(profile.created_at)) || profile.trainee_profile !== 'astronaut_candidate' || !Number.isInteger(profile.training_level) || profile.training_level < 0 || profile.training_level > 10) throw new AccountError(502, 'Profile ownership validation failed.')
  return profile
}
export const callbackUrl = () => location.origin + '/auth/callback'
export function safeAuthMessage(status?: number) {
  return status === 429 ? 'Too many requests. Wait and try again.' : status && status >= 500 ? 'Account service unavailable. Retry when connected.' : 'Account request rejected. Check your credentials and email verification, then retry.'
}
