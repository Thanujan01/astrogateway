import { lazy, Suspense, useEffect, useState } from 'react'
import type { FormEvent, ReactNode } from 'react'
import { ArrowUpRight, ChevronRight, Compass, Orbit, ShieldCheck, UserRound, LogOut } from 'lucide-react'
import { AccountProvider } from './AccountContext'
import { useAccount } from './accountState'
import { callbackUrl, client, configurationReady, safeAuthMessage, validateName } from './client'
import './account.css'
const MarsPreview = lazy(() => import('../App'))
function navigate(path: string, replace = false) { history[replace ? 'replaceState' : 'pushState']({}, '', path); dispatchEvent(new PopStateEvent('popstate')) }
function Link({ to, children, className = '' }: { to: string; children: ReactNode; className?: string }) { return <a href={to} className={className} onClick={e => { if (!e.ctrlKey && !e.metaKey) { e.preventDefault(); navigate(to) } }}>{children}</a> }
function usePath() { const [path, set] = useState(location.pathname); useEffect(() => { const fn = () => set(location.pathname); addEventListener('popstate', fn); return () => removeEventListener('popstate', fn) }, []); return path }
function Notice({ children, error = false }: { children: ReactNode; error?: boolean }) { return children ? <div className={`ag-notice ${error ? 'error' : ''}`} role={error ? 'alert' : 'status'}>{children}</div> : null }
function Shell({ children }: { children: ReactNode }) {
  const account = useAccount()
  return <div className="ag-shell"><header className="ag-header"><Link to="/welcome" className="ag-brand"><Orbit size={28}/><span>ASTROGATEWAY<small>SK2LB · ASTROLAB</small></span></Link><nav aria-label="Main navigation"><Link to="/mars">Mars preview <ArrowUpRight size={15}/></Link>{account.profile ? <><Link to="/dashboard">Dashboard</Link><Link to="/profile">Profile</Link><button className="ag-quiet" onClick={() => { void account.logout().then(() => navigate('/welcome', true)) }}><LogOut size={16}/> Sign out</button></> : <Link to="/login">Sign in</Link>}</nav></header><main className="ag-main">{children}</main><footer className="ag-footer"><span>MISSION RED HORIZON</span><span>Science. Practice. Progress.</span></footer></div>
}
function Welcome() { const account = useAccount(); return <Shell><div className="ag-hero"><section><div className="ag-eyebrow">ASTRONAUT TRAINING / ACCOUNT WORKSPACE</div><h1>Your next<br/>horizon starts here.</h1><p className="ag-lead">Prepare for Mars. Build your scientific skills.<br/>Keep your training identity with you, across web and Android.</p><div className="ag-actions"><Link to="/signup" className="ag-primary">Create your account <ArrowUpRight size={18}/></Link><Link to="/login" className="ag-secondary">Sign in</Link></div><p className="ag-caption"><ShieldCheck size={16}/> One account. One persistent astronaut identity.</p><Notice error>{account.error}</Notice></section><aside className="ag-mission-card"><div className="ag-orbit-art" aria-hidden="true"><div className="ag-planet"/><div className="ag-orbit-line"/></div><div className="ag-eyebrow">MISSION 01</div><h2>Red Horizon</h2><p>Mars systems exploration</p><div className="ag-card-rule"/><p className="ag-small">Explore the existing browser prototype. Account-linked mission gameplay is still in development.</p><Link to="/mars" className="ag-text-link">Open Mars preview <ChevronRight size={18}/></Link></aside></div><div className="ag-feature-row"><article><Compass/><h3>Scientific preparation</h3><p>Navigation, equipment and survival skills.</p></article><article><UserRound/><h3>A shared identity</h3><p>Your Supabase user ID stays the same on both platforms.</p></article><article><ShieldCheck/><h3>You control your profile</h3><p>Manage your display name and recover account access.</p></article></div></Shell> }
function AuthForm({ kind }: { kind: 'login' | 'signup' | 'forgot' }) {
  const account = useAccount(); const [busy, setBusy] = useState(false); const [message, setMessage] = useState(''); const [error, setError] = useState('')
  async function submit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault(); if (busy) return; setError(''); setMessage('')
    if (!client) { setError('Account service is not configured. No request was sent.'); return }
    const form = e.currentTarget; const fields = new FormData(form); const email = String(fields.get('email')).trim(); let password = String(fields.get('password') || '')
    try {
      if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) throw new Error('Enter a valid email address.')
      if (kind !== 'forgot' && password.length < 12) throw new Error('Use a password with at least 12 characters.')
      const name = kind === 'signup' ? validateName(String(fields.get('name'))) : ''
      setBusy(true)
      if (kind === 'login') {
        const result = await client.auth.signInWithPassword({ email, password }); password = ''; (form.elements.namedItem('password') as HTMLInputElement).value = ''
        if (result.error) throw new Error(safeAuthMessage(result.error.status))
        await account.reload(); navigate('/dashboard', true)
      } else {
        localStorage.setItem('ag-pending-flow', JSON.stringify({ kind: kind === 'forgot' ? 'recovery' : 'verification', at: Date.now() }))
        const result = kind === 'signup' ? await client.auth.signUp({ email, password, options: { data: { display_name: name }, emailRedirectTo: callbackUrl() } }) : await client.auth.resetPasswordForEmail(email, { redirectTo: callbackUrl() })
        password = ''; if (kind === 'signup') (form.elements.namedItem('password') as HTMLInputElement).value = ''
        if (result.error) { localStorage.removeItem('ag-pending-flow'); throw new Error(safeAuthMessage(result.error.status)) }
        if (kind === 'signup') { if (result.data && 'session' in result.data && result.data.session) { await account.reload(); navigate('/dashboard', true) } else navigate('/verify-email') }
        else setMessage('If this address has an account, a recovery email has been requested. Open it in this browser.')
      }
    } catch (err) { setError(err instanceof Error ? err.message : 'Could not reach the account service. Retry when connected.') }
    finally { password = ''; const input = form.elements.namedItem('password') as HTMLInputElement | null; if (input) input.value = ''; setBusy(false) }
  }
  return <Shell><section className="ag-form-card"><div className="ag-eyebrow">YOUR ASTROGATEWAY ACCOUNT</div><h1>{kind === 'login' ? 'Welcome back.' : kind === 'signup' ? 'Begin your journey.' : 'Recover your account.'}</h1><p>{kind === 'forgot' ? 'We’ll send a secure recovery link. Open it in the same browser where you request it.' : 'Use the same email and password in the Android app.'}</p><Notice error>{error}</Notice><Notice>{message}</Notice>{!configurationReady && <Notice error>Account configuration is missing.</Notice>}<form onSubmit={e => { void submit(e) }}>{kind === 'signup' && <label>Display name<input name="name" autoComplete="nickname" required minLength={2} maxLength={128} placeholder="Your astronaut name" disabled={busy}/><small>2–64 characters. You can change this later.</small></label>}<label>Email address<input name="email" type="email" autoComplete="email" required placeholder="you@example.com" disabled={busy}/></label>{kind !== 'forgot' && <label>Password<input name="password" type="password" required minLength={12} autoComplete={kind === 'signup' ? 'new-password' : 'current-password'} disabled={busy}/><small>At least 12 characters. Managed securely by Supabase Auth.</small></label>}<button className="ag-primary" disabled={busy || !configurationReady}>{busy ? 'Please wait…' : kind === 'login' ? 'Sign in' : kind === 'signup' ? 'Create account' : 'Send recovery email'}<ArrowUpRight size={18}/></button></form><div className="ag-form-links">{kind === 'login' && <Link to="/forgot-password">Forgot password?</Link>}<Link to={kind === 'signup' ? '/login' : '/signup'}>{kind === 'signup' ? 'Already have an account? Sign in' : 'Create an account'}</Link>{kind === 'forgot' && <Link to="/login">Back to sign in</Link>}</div></section></Shell>
}
let callbackPromise: Promise<'recovery' | 'verification'> | null = null
async function exchangeCallback() {
  if (callbackPromise && !location.search && !location.hash) return callbackPromise
  const uri = new URL(location.href); history.replaceState({}, '', '/auth/callback')
  callbackPromise = (async () => {
    if (!client || uri.pathname !== '/auth/callback' || uri.hash || uri.searchParams.has('error')) throw new Error('Verification link is invalid or expired. Request a new email.')
    const code = uri.searchParams.get('code'); const pending = JSON.parse(localStorage.getItem('ag-pending-flow') || 'null')
    if (!code || code.length > 2048 || !pending || !['verification', 'recovery'].includes(pending.kind) || typeof pending.at !== 'number' || Date.now() - pending.at > 86400000 || pending.at > Date.now()) throw new Error('No valid pending request in this browser. Request a new verification or recovery email.')
    const { error } = await client.auth.exchangeCodeForSession(code)
    if (error) throw new Error('Verification link could not be accepted. Request a new email in this browser.')
    localStorage.removeItem('ag-pending-flow'); return pending.kind as 'recovery' | 'verification'
  })(); return callbackPromise
}
function Callback() {
  const account = useAccount(); const [error, setError] = useState('')
  // Exchange once per mounted callback, including StrictMode; never rerun for provider updates.
  // eslint-disable-next-line react-hooks/exhaustive-deps
  useEffect(() => { let alive = true; void exchangeCallback().then(async kind => { if (!alive) return; if (kind === 'recovery') { account.beginRecovery(); await account.reload(); navigate('/reset-password', true) } else { await account.reload(); navigate('/dashboard', true) } }).catch(() => { if (alive) setError('Verification link is invalid, expired or was requested in another browser. Request a new email and retry.') }); return () => { alive = false } }, [])
  return <Shell><section className="ag-form-card"><h1>Verifying your link.</h1><Notice error>{error}</Notice>{!error && <p role="status">Checking your secure email callback…</p>}<Link to="/login">Return to sign in</Link><Link to="/forgot-password">Request another recovery email</Link></section></Shell>
}
function ResetPassword() {
  const account = useAccount(); const [busy, setBusy] = useState(false); const [error, setError] = useState('')
  if (!account.recovery) return <Shell><section className="ag-form-card"><h1>Recovery link required.</h1><Link to="/forgot-password">Request password recovery</Link></section></Shell>
  async function submit(e: FormEvent<HTMLFormElement>) { e.preventDefault(); const form = e.currentTarget; const fields = new FormData(form); const password = String(fields.get('password')); if (password.length < 12 || password !== fields.get('confirm')) { setError('Use at least 12 characters and matching passwords.'); return } setBusy(true); setError(''); try { if (!client) throw new Error('Account service unavailable.'); const result = await client.auth.updateUser({ password }); if (result.error) throw new Error(safeAuthMessage(result.error.status)); account.completeRecovery(); navigate('/dashboard', true) } catch (err) { setError(err instanceof Error ? err.message : 'Recovery failed. Retry when connected.') } finally { form.reset(); setBusy(false) } }
  return <Shell><section className="ag-form-card"><h1>Choose a new password.</h1><Notice error>{error}</Notice><form onSubmit={e => { void submit(e) }}><label>New password<input name="password" type="password" autoComplete="new-password" minLength={12} required disabled={busy}/></label><label>Confirm password<input name="confirm" type="password" autoComplete="new-password" minLength={12} required disabled={busy}/></label><button className="ag-primary" disabled={busy}>{busy ? 'Saving…' : 'Save new password'}</button></form><button className="ag-quiet" disabled={busy} onClick={() => { void account.logout().then(() => navigate('/welcome', true)) }}>Cancel recovery and sign out</button></section></Shell>
}
function Protected({ children }: { children: ReactNode }) {
  const account = useAccount()
  if (account.loading) return <Shell><section className="ag-form-card"><h1>Preparing your workspace.</h1><p role="status">Validating your account and profile…</p></section></Shell>
  if (!account.profile || !account.session || account.recovery) return <Shell><section className="ag-form-card"><h1>Sign in to continue.</h1><p>Your training workspace requires a verified account.</p><Notice error>{account.error}</Notice><Link to={account.recovery ? '/reset-password' : '/login'} className="ag-primary">{account.recovery ? 'Complete password recovery' : 'Sign in'}</Link><button className="ag-secondary" onClick={() => { void account.reload() }}>Retry account validation</button></section></Shell>
  return <>{children}</>
}
function Dashboard() { const { profile } = useAccount(); return <Shell><div className="ag-eyebrow">ASTRONAUT WORKSPACE</div><h1>Welcome, {profile!.display_name}.</h1><p className="ag-lead">Your next step in scientific preparation.</p><div className="ag-dashboard-grid"><article className="ag-panel"><div className="ag-eyebrow">MISSION RED HORIZON</div><h2>Explore the Mars outpost.</h2><p>The browser prototype is available. It is separate from the Android Unity integration and does not submit account-linked training scores.</p><Link to="/mars" className="ag-primary">Open Mars preview <ArrowUpRight size={18}/></Link></article><article className="ag-panel"><h2>Your astronaut profile</h2><p>Training level {profile!.training_level}</p><p>No verified mission assessment is shown here.</p><Link to="/profile" className="ag-text-link">Manage your profile <ChevronRight size={18}/></Link></article></div><div className="ag-feature-row">{['Navigation', 'Survival & systems', 'Equipment handling'].map(x => <article key={x}><div className="ag-eyebrow">TRAINING SKILL</div><h3>{x}</h3><p>Practice and progress reporting are not yet connected.</p></article>)}</div></Shell> }
function ProfilePage() {
  const account = useAccount(); const [editing, setEditing] = useState(false); const [draft, setDraft] = useState(account.profile!.display_name); const [busy, setBusy] = useState(false); const [error, setError] = useState(''); const [message, setMessage] = useState('')
  async function submit(e: FormEvent) { e.preventDefault(); setError(''); setMessage(''); try { const name = validateName(draft); setBusy(true); await account.saveName(name); setEditing(false); setMessage('Display name saved and profile refreshed.') } catch (err) { setError(err instanceof Error ? err.message : 'Could not save your profile.') } finally { setBusy(false) } }
  return <Shell><section className="ag-profile-card"><div className="ag-eyebrow">PERSONAL PROFILE</div><h1>Your astronaut identity.</h1><p>One user ID across web and Android. Your display name can change.</p><Notice error>{error}</Notice><Notice>{message}</Notice>{editing ? <form onSubmit={e => { void submit(e) }}><label>Display name<input value={draft} onChange={e => setDraft(e.target.value)} name="name" autoComplete="nickname" required disabled={busy}/></label><div className="ag-actions"><button className="ag-primary" disabled={busy}>{busy ? 'Saving…' : 'Save display name'}</button><button className="ag-secondary" type="button" disabled={busy} onClick={() => { setEditing(false); setDraft(account.profile!.display_name); setError('') }}>Cancel</button></div></form> : <div className="ag-profile-name"><h2>{account.profile!.display_name}</h2><button className="ag-secondary" onClick={() => { setDraft(account.profile!.display_name); setMessage(''); setEditing(true) }}>Edit display name</button></div>}<dl><dt>Canonical user ID</dt><dd className="ag-id">{account.profile!.user_id}</dd><dt>Email address</dt><dd>{account.session!.user.email}</dd><dt>Training level</dt><dd>{account.profile!.training_level}</dd><dt>Profile created</dt><dd>{new Date(account.profile!.created_at).toLocaleDateString()}</dd></dl><div className="ag-card-rule"/><p className="ag-small">Email and password are managed by Supabase Auth. Passwords are never stored in your training profile.</p><Link to="/forgot-password" className="ag-text-link">Request password recovery <ChevronRight size={18}/></Link></section></Shell>
}
function Routes() {
  const path = usePath()
  useEffect(() => { if (path !== '/auth/callback') callbackPromise = null; window.scrollTo(0, 0); const titles: Record<string, string> = { '/welcome': 'Welcome', '/login': 'Sign in', '/signup': 'Create account', '/profile': 'Profile', '/dashboard': 'Dashboard' }; document.title = `${titles[path] || 'Mission Red Horizon'} · AstroGateway` }, [path])
  if (path === '/' || path === '/mars') return <><Suspense fallback={<p role="status">Loading Mars preview…</p>}><MarsPreview/></Suspense><Link to="/welcome" className="ag-preview-account">Account workspace <ArrowUpRight size={15}/></Link></>
  if (path === '/welcome') return <Welcome/>
  if (path === '/login' || path === '/signup' || path === '/forgot-password') return <AuthForm key={path} kind={path === '/login' ? 'login' : path === '/signup' ? 'signup' : 'forgot'}/>
  if (path === '/auth/callback') return <Callback/>
  if (path === '/reset-password') return <ResetPassword/>
  if (path === '/verify-email') return <Shell><section className="ag-form-card"><ShieldCheck size={36}/><h1>Check your inbox.</h1><p>If registration can proceed, you’ll receive an email verification link. Open it in this browser, then sign in.</p><p>No profile access is granted until your email and session are verified.</p><Link to="/login" className="ag-primary">Return to sign in</Link></section></Shell>
  if (path === '/dashboard' || path === '/profile') return <Protected>{path === '/dashboard' ? <Dashboard/> : <ProfilePage/>}</Protected>
  return <Shell><section className="ag-form-card"><h1>Page not found.</h1><Link to="/welcome">Return to AstroGateway</Link></section></Shell>
}
export default function AccountApp() { return <AccountProvider><Routes/></AccountProvider> }
