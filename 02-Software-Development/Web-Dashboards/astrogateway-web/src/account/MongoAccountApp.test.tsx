import { beforeEach, describe, expect, it, vi } from 'vitest'
import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import MongoAccountApp from './MongoAccountApp'
vi.mock('../App', () => ({ default: () => <h2>Mars outpost trainer</h2> }))
const id = '11111111-1111-4111-8111-111111111111'
const profile = { user_id: id, display_name: 'Local Trainee', username: 'trainee', trainee_profile: 'astronaut_candidate', training_level: 0, created_at: '2026-01-01T00:00:00Z' }
let signedIn = false
beforeEach(() => {
  signedIn = false; localStorage.clear(); sessionStorage.clear(); history.replaceState({}, '', '/login')
  vi.stubGlobal('fetch', vi.fn(async (url: string, options?: RequestInit) => {
    const path = String(url)
    if (path.endsWith('/v1/auth/login') || path.endsWith('/v1/auth/register')) { signedIn = true; return new Response(JSON.stringify({ profile }), { status: 200 }) }
    if (path.endsWith('/v1/auth/logout')) { signedIn = false; return new Response('{"revoked":true}', { status: 200 }) }
    if (path.endsWith('/v1/mission-sessions')) return new Response(JSON.stringify({ id, user_id: id }), { status: 201 })
    if (path.endsWith('/v1/skills')) return new Response('{"sessions":2}', { status: 200 })
    if (path.endsWith('/display-name')) return new Response(JSON.stringify({ ...profile, display_name: JSON.parse(String(options?.body)).display_name }), { status: 200 })
    return new Response(JSON.stringify(signedIn ? profile : { detail: 'Sign-in required' }), { status: signedIn ? 200 : 401 })
  }))
})
function fill() {
  fireEvent.change(screen.getByLabelText(/Email address/), { target: { value: 'local@example.invalid' } })
  fireEvent.change(screen.getByLabelText('Password'), { target: { value: 'Local-Example-Password' } })
}
describe('MongoDB local account UI', () => {
  it('signs in through the API, uses cookies and stores no bearer token', async () => {
    render(<MongoAccountApp/>); await screen.findByRole('heading', { name: 'Welcome back.' }); fill()
    fireEvent.click(screen.getByRole('button', { name: 'Sign in' }))
    expect(await screen.findByRole('heading', { name: 'Welcome, Local Trainee.' })).toBeTruthy()
    expect(vi.mocked(fetch).mock.calls.some(([url, init]) => String(url).endsWith('/v1/auth/login') && init?.credentials === 'same-origin')).toBe(true)
    expect(JSON.stringify(localStorage)).not.toContain('access_token')
  })
  it('denies a protected route when signed out', async () => {
    history.replaceState({}, '', '/profile'); render(<MongoAccountApp/>)
    expect(await screen.findByRole('heading', { name: 'Sign in to continue.' })).toBeTruthy()
    expect(screen.queryByText(id)).toBeNull()
  })
  it('opens browser training from the signed-in dashboard after creating a mission session', async () => {
    signedIn = true; history.replaceState({}, '', '/dashboard'); render(<MongoAccountApp/>)
    fireEvent.click(await screen.findByRole('button', { name: 'Start training' }))
    expect(await screen.findByRole('heading', { name: 'Mars outpost trainer' })).toBeTruthy()
    expect(location.pathname).toBe('/training')
    const request = vi.mocked(fetch).mock.calls.find(([url]) => String(url).endsWith('/v1/mission-sessions'))
    expect(JSON.parse(String(request?.[1]?.body)).mission_id).toBe('mission-red-horizon-poc')
  })
  it('asks for sign-in when opening training directly without an account session', async () => {
    history.replaceState({}, '', '/training'); render(<MongoAccountApp/>)
    expect(await screen.findByRole('heading', { name: 'Sign in to continue.' })).toBeTruthy()
    expect(screen.queryByRole('heading', { name: 'Mars outpost trainer' })).toBeNull()
  })
  it('saves the current user profile without an owner override', async () => {
    signedIn = true; history.replaceState({}, '', '/profile'); render(<MongoAccountApp/>)
    const name = await screen.findByLabelText('Display name'); fireEvent.change(name, { target: { value: 'Mars Commander' } })
    fireEvent.click(screen.getByRole('button', { name: 'Save display name' }))
    expect(await screen.findByText('Display name saved.')).toBeTruthy()
    const call = vi.mocked(fetch).mock.calls.find(([url]) => String(url).endsWith('/display-name'))!
    expect(JSON.parse(String(call[1]?.body))).toEqual({ display_name: 'Mars Commander' })
    expect(screen.getByText(id)).toBeTruthy()
  })
  it('clears the private view and revokes the server session on logout', async () => {
    signedIn = true; history.replaceState({}, '', '/dashboard'); render(<MongoAccountApp/>)
    await screen.findByRole('heading', { name: 'Welcome, Local Trainee.' })
    fireEvent.click(screen.getByRole('button', { name: 'Sign out' }))
    await waitFor(() => expect(screen.queryByText('Welcome, Local Trainee.')).toBeNull())
    expect(await screen.findByRole('heading', { name: 'Welcome back.' })).toBeTruthy()
    expect(sessionStorage.getItem('ag-mongo-signed-out')).toBe('yes')
    expect(vi.mocked(fetch).mock.calls.some(([url]) => String(url).endsWith('/v1/auth/logout'))).toBe(true)
  })
})
