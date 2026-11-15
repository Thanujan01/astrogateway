import { beforeEach, describe, expect, it } from 'vitest'
import { loadProfile, readSessionCount, recordSessionStart, saveDisplayName, validateName } from './profile'

beforeEach(() => localStorage.clear())

describe('the single local profile', () => {
  it('creates a default profile named Thanujan on first load', () => {
    const profile = loadProfile()
    expect(profile.display_name).toBe('Thanujan')
    expect(profile.trainee_profile).toBe('astronaut_candidate')
    expect(profile.training_level).toBe(0)
    expect(profile.user_id).toMatch(/^[0-9a-f-]{36}$/)
  })

  it('persists the same profile id across reloads', () => {
    const first = loadProfile()
    const second = loadProfile()
    expect(second.user_id).toBe(first.user_id)
  })

  it('saves a renamed display name and keeps the same id', () => {
    const profile = loadProfile()
    const updated = saveDisplayName(profile, '  Commander  ')
    expect(updated.display_name).toBe('Commander')
    expect(updated.user_id).toBe(profile.user_id)
    expect(loadProfile().display_name).toBe('Commander')
  })

  it.each(['', 'A', 'x'.repeat(65), 'bad' + String.fromCharCode(1) + 'name'])(
    'rejects invalid display name %#',
    name => { expect(() => validateName(name)).toThrow() },
  )

  it('recovers with a fresh profile if storage holds garbage', () => {
    localStorage.setItem('ag-solo-profile-v1', '{"not":"a profile"}')
    expect(loadProfile().display_name).toBe('Thanujan')
  })
})

describe('local training-session count', () => {
  it('starts at zero and increments on each session start', () => {
    expect(readSessionCount()).toBe(0)
    expect(recordSessionStart()).toBe(1)
    expect(recordSessionStart()).toBe(2)
    expect(readSessionCount()).toBe(2)
  })

  it('treats corrupt session storage as zero', () => {
    localStorage.setItem('ag-solo-sessions-v1', '"not a number"')
    expect(readSessionCount()).toBe(0)
  })
})
