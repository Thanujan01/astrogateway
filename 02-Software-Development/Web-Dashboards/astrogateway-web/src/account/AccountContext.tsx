import { useEffect, useRef, useState } from 'react'
import type { ReactNode } from 'react'
import { AccountError, client, profileRequest } from './client'
import { Context } from './accountState'
import type { State, Account } from './accountState'
export function AccountProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<State>({ session: null, profile: null, loading: true, error: '', recovery: localStorage.getItem('ag-recovery-active') === 'yes' })
  const current = useRef(state)
  useEffect(() => { current.current = state }, [state])
  const signingOut = useRef(false)
  const generation = useRef(0)
  const controller = useRef<AbortController | null>(null)
  async function load() {
    if (signingOut.current) return
    const version = ++generation.current
    controller.current?.abort(); const abort = new AbortController(); controller.current = abort
    setState(s => ({ ...s, loading: true, profile: null, error: '' }))
    if (!client) { setState(s => ({ ...s, session: null, loading: false, error: 'Account service is not configured.' })); return }
    try {
      const { data, error } = await client.auth.getSession()
      if (error) throw new Error('Session could not be restored. Sign in again.')
      if (!data.session || data.session.expires_at! * 1000 <= Date.now()) {
        if (version === generation.current) setState(s => ({ ...s, loading: false, session: null, profile: null }))
        return
      }
      const verified = await client.auth.getUser()
      if (verified.error || !verified.data.user || verified.data.user.id !== data.session.user.id) throw new AccountError(401, 'Your session is invalid. Sign in again.')
      if (!verified.data.user.email_confirmed_at) throw new AccountError(403, 'Verify your email before continuing.')
      if (current.current.recovery) {
        if (version === generation.current) setState(s => ({ ...s, session: data.session, loading: false, profile: null }))
        return
      }
      const profile = await profileRequest(data.session.access_token, verified.data.user.id, undefined, abort.signal)
      if (version === generation.current) setState(s => ({ ...s, session: data.session, profile, loading: false, error: '' }))
    } catch (error) {
      if (version !== generation.current) return
      const invalid = error instanceof AccountError && error.status === 401
      setState(s => ({ ...s, session: invalid ? null : s.session, profile: null, loading: false, error: error instanceof Error ? error.message : 'Account validation unavailable.' }))
    }
  }
  useEffect(() => {
    queueMicrotask(() => { if (location.pathname === '/auth/callback') setState(s => ({ ...s, loading: false })); else void load() })
    const subscription = client?.auth.onAuthStateChange((event) => {
      if (event === 'SIGNED_OUT') {
        ++generation.current; controller.current?.abort(); localStorage.removeItem('ag-recovery-active')
        setState({ session: null, profile: null, loading: false, error: '', recovery: false })
      } else if (!signingOut.current && location.pathname !== '/auth/callback') queueMicrotask(() => { void load() })
    })
    const expiry = window.setInterval(() => {
      if (current.current.session && (current.current.session.expires_at ?? 0) * 1000 <= Date.now()) void load()
    }, 1000)
    // Cancel the latest async request, not a captured DOM ref.
    // eslint-disable-next-line react-hooks/exhaustive-deps
    return () => { ++generation.current; controller.current?.abort(); subscription?.data.subscription.unsubscribe(); clearInterval(expiry) }
  }, [])
  const value: Account = { ...state, reload: load,
    beginRecovery: () => { localStorage.setItem('ag-recovery-active', 'yes'); current.current = { ...current.current, recovery: true }; setState(s => ({ ...s, recovery: true, profile: null })) },
    completeRecovery: () => { localStorage.removeItem('ag-recovery-active'); current.current = { ...current.current, recovery: false }; setState(s => ({ ...s, recovery: false })); void load() },
    logout: async () => {
      if (signingOut.current) return
      signingOut.current = true
      ++generation.current; controller.current?.abort(); const previous = current.current.session
      localStorage.removeItem('ag-recovery-active'); localStorage.removeItem('ag-pending-flow')
      setState({ session: null, profile: null, loading: false, error: '', recovery: false })
      let revoked = true
      if (previous) {
        try { const r = await fetch('/account-api/v1/auth-session/revoke', { method: 'POST', redirect: 'error', signal: AbortSignal.timeout(5000), headers: { Authorization: `Bearer ${previous.access_token}` } }); revoked = r.ok } catch { revoked = false }
      }
      try { if (client) { const { error } = await client.auth.signOut(); if (error) { revoked = false; await client.auth.signOut({ scope: 'local' }) } } } catch { revoked = false; await client?.auth.signOut({ scope: 'local' }) }
      signingOut.current = false
      if (!revoked) setState(s => ({ ...s, error: 'Local session cleared. Server revocation could not be confirmed; existing tokens follow their expiry policy.' }))
    },
    saveName: async (name) => {
      const before = current.current
      if (!before.session || !before.profile || before.recovery || (before.session.expires_at ?? 0) * 1000 <= Date.now()) throw new Error('Sign in before editing your profile.')
      const version = generation.current
      await profileRequest(before.session.access_token, before.profile.user_id, name)
      if (version !== generation.current) throw new Error('Your session changed. Sign in and retry.')
      // Re-read canonical profile after the write, not user metadata or the submitted form.
      const profile = await profileRequest(before.session.access_token, before.profile.user_id)
      if (version !== generation.current) throw new Error('Your session changed. Sign in and retry.')
      setState(s => ({ ...s, profile }))
    },
  }
  return <Context.Provider value={value}>{children}</Context.Provider>
}
