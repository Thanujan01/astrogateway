import { lazy, Suspense, useEffect, useState } from 'react'
import type { FormEvent, ReactNode } from 'react'
import { Orbit } from 'lucide-react'
import './account.css'
import { loadProfile, saveDisplayName } from './profile'
// Small and bank-free: imports ./progress only, so the question data stays out of here.
import ReadinessRating from '../challenges/ReadinessRating'
import SiteNav from './SiteNav'

// 81 KB of question data, so keep it out of the account bundle.
const Challenges = lazy(() => import('../challenges/Challenges'))
const Tutorials = lazy(() => import('../tutorials/Tutorials'))

// No in-app download page: the nav link opens this Drive folder directly, in a new tab.
const MOBILE_GAME_DRIVE_FOLDER_URL = 'https://drive.google.com/drive/folders/1DsE8mD1pNGBWLC_IGZgl_6kA11O8pDza?usp=sharing'

/* ---------------------------------------------------------------------------
 * On GitHub Pages this app is served from /astrogateway/, not from the
 * domain root, so every pushed URL must carry that prefix or the address bar
 * ends up at a path (e.g. /tutorials) that doesn't belong to this site at
 * all -- refreshing it 404s before this app's own code ever runs. BASE is
 * Vite's own build-time base ('/astrogateway/' on Pages, '/' in dev), so
 * this tracks vite.config.ts automatically.
 * ------------------------------------------------------------------------- */
const BASE = import.meta.env.BASE_URL.replace(/\/$/, '')

function currentPath() {
  const { pathname } = location
  if (BASE && pathname.startsWith(BASE)) return pathname.slice(BASE.length) || '/'
  return pathname
}

function navigate(to: string) { history.pushState({}, '', BASE + to); window.dispatchEvent(new PopStateEvent('popstate')) }
function Link({ to, children, className = '' }: { to: string; children: ReactNode; className?: string }) {
  return <a href={BASE + to} className={className} onClick={e => { e.preventDefault(); navigate(to) }}>{children}</a>
}

export default function SoloAccountApp() {
  const [path, setPath] = useState(currentPath())
  const [profile, setProfile] = useState(() => loadProfile())
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')

  useEffect(() => {
    const pop = () => { setPath(currentPath()); setError(''); setMessage('') }
    window.addEventListener('popstate', pop)
    return () => window.removeEventListener('popstate', pop)
  }, [])

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const name = new FormData(event.currentTarget).get('name')
    setError(''); setMessage('')
    try {
      setProfile(current => saveDisplayName(current, String(name)))
      setMessage('Display name saved.')
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Could not save your profile.')
    }
  }

  return <div className="ag-shell">
    <header className="ag-header">
      <Link to="/welcome" className="ag-brand"><Orbit size={28}/><span>ASTROGATEWAY<small>SK2LB · LOCAL WORKSPACE</small></span></Link>
      <SiteNav>
        <Link to="/challenges">Challenges</Link>
        <Link to="/tutorials">Training videos</Link>
        <a href={MOBILE_GAME_DRIVE_FOLDER_URL} target="_blank" rel="noopener noreferrer">Download mobile game</a>
        <Link to="/dashboard">Dashboard</Link>
        <Link to="/profile">Profile</Link>
      </SiteNav>
    </header>
    <main className="ag-main">
      <div className="ag-eyebrow">LOCAL ASTRONAUT WORKSPACE</div>
      <p className="ag-small">Your profile is stored in this browser only. There is no account server and nothing is sent anywhere.</p>
      {error && <p className="ag-notice" role="alert">{error}</p>}
      {message && <p className="ag-notice" role="status">{message}</p>}

      {path === '/challenges' ? (
        <Suspense fallback={<p role="status">Loading challenges</p>}><Challenges /></Suspense>
      ) : path === '/tutorials' ? (
        <Suspense fallback={<p role="status">Loading training videos</p>}><Tutorials /></Suspense>
      ) : path === '/dashboard' ? (
        <>
          <h1>Welcome, {profile.display_name}.</h1>
          <article className="ag-panel">
            <h2>Your progress</h2>
            <p>Training score and skill levels: unassessed.</p>
            <Link to="/profile">Manage profile</Link>
          </article>
        </>
      ) : path === '/profile' ? (
        <section className="ag-profile-card">
          <h1>Your astronaut profile.</h1>
          <form onSubmit={submit}>
            <label>Display name<input key={profile.user_id} name="name" defaultValue={profile.display_name} minLength={2} maxLength={64} required/></label>
            <button className="ag-primary">Save display name</button>
          </form>
          <dl>
            <dt>Profile ID</dt><dd className="ag-id">{profile.user_id}</dd>
            <dt>Created</dt><dd>{new Date(profile.created_at).toLocaleDateString()}</dd>
          </dl>
          <ReadinessRating onOpen={navigate}/>
        </section>
      ) : (
        <section className="ag-hero">
          <div>
            <h1>Your Mars<br/>mission starts here.</h1>
            <p className="ag-lead">Explore Mission Red Horizon, right in your browser.</p>
            <div className="ag-actions">
              <Link to="/dashboard" className="ag-primary">Open dashboard</Link>
            </div>
          </div>
        </section>
      )}
    </main>
    <footer className="ag-footer"><span>MISSION RED HORIZON</span><span>Local workspace</span></footer>
  </div>
}
