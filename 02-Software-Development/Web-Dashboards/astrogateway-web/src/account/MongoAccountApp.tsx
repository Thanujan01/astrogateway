import { lazy, Suspense, useEffect, useRef, useState } from 'react'
import type { FormEvent, ReactNode } from 'react'
import { Orbit } from 'lucide-react'
import type { Profile } from './client'
import './account.css'
import { mongoRequest } from './mongoClient'

const MarsPreview = lazy(() => import('../App'))
function navigate(to: string) { history.pushState({}, '', to); window.dispatchEvent(new PopStateEvent('popstate')) }
function Link({ to, children, className = '' }: { to: string; children: ReactNode; className?: string }) {
  return <a href={to} className={className} onClick={e => { e.preventDefault(); navigate(to) }}>{children}</a>
}
function profileValue(value: Profile) {
  if (!value || !/^[0-9a-f-]{36}$/.test(value.user_id) || typeof value.display_name !== 'string' || value.trainee_profile !== 'astronaut_candidate' || !Number.isInteger(value.training_level)) throw new Error('Invalid account profile.')
  return value
}
export default function MongoAccountApp() {
  const [path, setPath] = useState(location.pathname)
  const [profile, setProfile] = useState<Profile | null>(null)
  const [busy, setBusy] = useState(false)
  const [loading, setLoading] = useState(() => sessionStorage.getItem('ag-mongo-signed-out') !== 'yes')
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')
  const [summary, setSummary] = useState<{ sessions: number } | null>(null)
  const generation = useRef(0)
  useEffect(() => {
    const pop = () => { setPath(location.pathname); setError(''); setMessage('') }
    const signout = (event: StorageEvent) => { if (event.key === 'ag-mongo-signout') { ++generation.current; setProfile(null); sessionStorage.setItem('ag-mongo-signed-out', 'yes') } }
    window.addEventListener('popstate', pop); window.addEventListener('storage', signout)
    const version = ++generation.current
    if (sessionStorage.getItem('ag-mongo-signed-out') !== 'yes') void mongoRequest<Profile>('/v1/me').then(p => { if (version === generation.current) setProfile(profileValue(p)) }).catch(() => {}).finally(() => { if (version === generation.current) setLoading(false) })
    return () => { ++generation.current; window.removeEventListener('popstate', pop); window.removeEventListener('storage', signout) }
  }, [])
  useEffect(() => {
    if (!profile || path !== '/dashboard') return
    const version = generation.current
    let active = true
    void mongoRequest<{ sessions: number }>('/v1/skills').then(s => {
      if (active && version === generation.current) setSummary(s)
    }).catch(() => {})
    return () => { active = false }
  }, [profile, path])
  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault(); const form = event.currentTarget; const values = new FormData(form)
    const version = ++generation.current; setBusy(true); setError(''); setMessage('')
    try {
      if (path === '/profile') {
        const before = profile
        if (!before) throw new Error('Sign in before editing your profile.')
        const updated = profileValue(await mongoRequest<Profile>('/v1/me/display-name', { display_name: values.get('name') }))
        if (updated.user_id !== before.user_id) throw new Error('Profile ownership check failed.')
        if (version === generation.current) { setProfile(updated); setMessage('Display name saved.') }
      } else {
        const signup = path === '/signup'
        const result = await mongoRequest<{ profile: Profile }>(signup ? '/v1/auth/register' : '/v1/auth/login', signup ? {
          display_name: values.get('name'), username: values.get('username') || null, email: values.get('email'), password: values.get('password'),
        } : { identifier: values.get('email'), password: values.get('password') })
        if (version === generation.current) { sessionStorage.removeItem('ag-mongo-signed-out'); setSummary(null); setProfile(profileValue(result.profile)); navigate('/dashboard') }
      }
    } catch (reason) { if (version === generation.current) setError(reason instanceof Error ? reason.message : 'Could not reach the local service.') }
    finally {
      const password = form.elements.namedItem('password') as HTMLInputElement | null; if (password) password.value = ''
      if (version === generation.current) setBusy(false)
    }
  }
  async function logout() {
    ++generation.current; sessionStorage.setItem('ag-mongo-signed-out', 'yes'); localStorage.setItem('ag-mongo-signout', String(Date.now()))
    setProfile(null); setSummary(null); setBusy(true); setError('')
    try { await mongoRequest('/v1/auth/logout', {}); navigate('/login') }
    catch { setError('Local view signed out. Server revocation could not be confirmed; reconnect and retry sign-out.'); setPath('/login') }
    finally { setBusy(false) }
  }
  async function startTraining() {
    const version = generation.current; setBusy(true); setError('')
    try {
      const result = await mongoRequest<{ id: string; user_id: string }>('/v1/mission-sessions', { launch_id: crypto.randomUUID(), mission_id: 'mission-red-horizon-poc' })
      if (result.user_id !== profile?.user_id) throw new Error('Mission ownership check failed.')
      if (version === generation.current && location.pathname === '/dashboard') navigate('/training')
    } catch (reason) { if (version === generation.current) setError(reason instanceof Error ? reason.message : 'Mission service unavailable.') }
    finally { if (version === generation.current) setBusy(false) }
  }
  const training = path === '/training'
  if (path === '/mars' || (training && profile && !loading)) {
    return <>
      <Suspense fallback={<p role="status">Loading {training ? 'training' : 'Mars preview'}…</p>}>
        <MarsPreview />
      </Suspense>
      <Link to={profile ? '/dashboard' : '/welcome'} className="ag-preview-account ag-preview-return">
        {profile ? 'Return to dashboard' : 'Return to welcome'}
      </Link>
    </>
  }
  const protectedPage = training || path === '/profile' || path === '/dashboard'
  const signup = path === '/signup'
  return <div className="ag-shell"><header className="ag-header"><Link to="/welcome" className="ag-brand"><Orbit size={28}/><span>ASTROGATEWAY<small>SK2LB · LOCAL PROTOTYPE</small></span></Link><nav aria-label="Main navigation"><Link to="/mars">Mars preview</Link>{profile ? <><Link to="/dashboard">Dashboard</Link><Link to="/profile">Profile</Link><button className="ag-quiet" disabled={busy} onClick={() => { void logout() }}>Sign out</button></> : <Link to="/login">Sign in</Link>}</nav></header>
    <main className="ag-main"><div className="ag-eyebrow">MONGODB · LOCAL ACCOUNT WORKSPACE</div>
      <p className="ag-small">Accounts and sessions are stored on this Mac. Email delivery, cloud sync and profile photos are not enabled in this local prototype.</p>
      {error && <p className="ag-notice" role="alert">{error}</p>}{message && <p className="ag-notice" role="status">{message}</p>}
      {loading ? <p role="status">Restoring your local account…</p> : protectedPage && !profile ? <section className="ag-form-card"><h1>Sign in to continue.</h1><Link to="/login" className="ag-primary">Sign in</Link></section> : path === '/dashboard' && profile ? <><h1>Welcome, {profile.display_name}.</h1><div className="ag-dashboard-grid"><article className="ag-panel"><h2>Mission Red Horizon</h2><p>Practise moving your astronaut and explore the Mars outpost in the browser.</p><button className="ag-primary" disabled={busy} onClick={() => { void startTraining() }}>{busy ? 'Starting training…' : 'Start training'}</button></article><article className="ag-panel"><h2>Your progress</h2><p>Mission sessions: {summary?.sessions ?? 'Loading…'}</p><p>Training score and skill levels: unassessed.</p><Link to="/profile">Manage profile</Link></article></div></> : path === '/profile' && profile ? <section className="ag-profile-card"><h1>Your astronaut profile.</h1><form onSubmit={e => { void submit(e) }}><label>Display name<input key={profile.user_id} name="name" defaultValue={profile.display_name} minLength={2} maxLength={64} required disabled={busy}/></label><button className="ag-primary" disabled={busy}>Save display name</button></form><dl><dt>Account ID</dt><dd className="ag-id">{profile.user_id}</dd><dt>Username</dt><dd>{profile.username ?? 'Not selected'}</dd><dt>Created</dt><dd>{new Date(profile.created_at).toLocaleDateString()}</dd></dl></section> : path === '/login' || signup ? <section className="ag-form-card"><h1>{signup ? 'Create your local account.' : 'Welcome back.'}</h1><form onSubmit={e => { void submit(e) }}>{signup && <><label>Display name<input name="name" autoComplete="nickname" minLength={2} maxLength={64} required disabled={busy}/></label><label>Username (optional)<input name="username" autoComplete="username" pattern="[a-zA-Z][a-zA-Z0-9_]{2,23}" maxLength={24} disabled={busy}/></label></>}<label>{signup ? 'Email address' : 'Email address or username'}<input name="email" type={signup ? 'email' : 'text'} autoComplete={signup ? 'email' : 'username'} maxLength={254} required disabled={busy}/></label><label>Password<input name="password" type="password" autoComplete={signup ? 'new-password' : 'current-password'} minLength={signup ? 12 : 1} maxLength={128} required disabled={busy}/></label><button className="ag-primary" disabled={busy}>{busy ? 'Please wait…' : signup ? 'Create account' : 'Sign in'}</button></form><p><Link to={signup ? '/login' : '/signup'}>{signup ? 'Already have an account? Sign in' : 'Create an account'}</Link></p></section> : <section className="ag-hero"><div><h1>Your Mars<br/>mission starts here.</h1><p className="ag-lead">Create a local astronaut account and explore Mission Red Horizon.</p><div className="ag-actions"><Link to={profile ? '/dashboard' : '/login'} className="ag-primary">{profile ? 'Open dashboard' : 'Sign in to train'}</Link>{!profile && <Link to="/signup" className="ag-secondary">Create account</Link>}<Link to="/mars" className="ag-secondary">Mars preview</Link></div></div></section>}
    </main><footer className="ag-footer"><span>MISSION RED HORIZON</span><span>Local prototype · MongoDB</span></footer></div>
}
