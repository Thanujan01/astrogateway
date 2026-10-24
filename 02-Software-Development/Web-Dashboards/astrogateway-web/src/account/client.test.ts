import { describe, expect, it } from 'vitest'
import { validateName, safeAuthMessage } from './client'
describe('profile input contract', () => {
  it.each(['தமிழ் வீரர்', '🚀🌒', 'A'.repeat(64), '  Astronaut  '])('validates %s', name => expect(validateName(name)).toBe(name.trim()))
  it.each(['x', '', ' '.repeat(8), 'A'.repeat(65), 'ab\ncd', 'ab\u007f', 'ab\u009f'])('rejects invalid name %s', name => expect(() => validateName(name)).toThrow())
  it('auth errors never reveal provider payloads', () => { expect(safeAuthMessage(400)).toContain('credentials'); expect(safeAuthMessage(429)).toContain('Too many'); expect(safeAuthMessage(500)).toContain('unavailable') })
})
