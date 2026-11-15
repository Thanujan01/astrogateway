import { useEffect, useState } from 'react'
import { Award, ChevronRight } from 'lucide-react'
import { MAX_STARS, readProgress, summarise } from './progress'
import './rating.css'

/* ---------------------------------------------------------------------------
 * Readiness rating for the profile page: the average star rating earned across
 * Mission Readiness Trials.
 *
 * Reads through ./progress, so this never imports the question bank -- the
 * profile page stays free of the 81 KB of question data.
 * ------------------------------------------------------------------------- */

/** Star row with fractional fill, so 4.2 of 5 reads as four and a bit. */
function StarBar({ value, max = MAX_STARS }: { value: number; max?: number }) {
  const ratio = Math.max(0, Math.min(value / max, 1))
  return (
    <span
      className="agr-bar"
      role="img"
      aria-label={value.toFixed(1) + ' out of ' + max + ' stars'}
    >
      <span className="agr-bar-empty" aria-hidden="true">{'★'.repeat(max)}</span>
      <span className="agr-bar-fill" style={{ width: ratio * 100 + '%' }} aria-hidden="true">
        {'★'.repeat(max)}
      </span>
    </span>
  )
}

export default function ReadinessRating({ onOpen }: { onOpen: (path: string) => void }) {
  const [summary, setSummary] = useState(() => summarise(readProgress()))

  // Trials run on another route in the same tab, and in other tabs. Re-read when the
  // page is shown again or when storage changes elsewhere.
  useEffect(() => {
    const refresh = () => setSummary(summarise(readProgress()))
    const onStorage = (event: StorageEvent) => {
      if (event.key === null || event.key === 'ag-challenges-v1') refresh()
    }
    const onVisible = () => { if (!document.hidden) refresh() }
    window.addEventListener('storage', onStorage)
    document.addEventListener('visibilitychange', onVisible)
    window.addEventListener('focus', refresh)
    return () => {
      window.removeEventListener('storage', onStorage)
      document.removeEventListener('visibilitychange', onVisible)
      window.removeEventListener('focus', refresh)
    }
  }, [])

  const open = (event: React.MouseEvent<HTMLAnchorElement>) => {
    event.preventDefault()
    onOpen('/challenges')
  }

  if (summary.average === null) {
    return (
      <section className="agr">
        <div className="ag-eyebrow">READINESS RATING</div>
        <h2>No trial scores yet.</h2>
        <p className="ag-small">
          Mission Readiness Trials rate every decision from one to five stars. Complete a trial
          and your average rating appears here.
        </p>
        <a href="/challenges" className="ag-text-link" onClick={open}>
          Start a trial <ChevronRight size={18} />
        </a>
      </section>
    )
  }

  const percent = Math.round((summary.stars / summary.max) * 100)

  return (
    <section className="agr">
      <div className="ag-eyebrow">READINESS RATING</div>

      <div className="agr-headline">
        <div>
          <strong className="agr-average">{summary.average.toFixed(1)}</strong>
          <span className="agr-of">/ {MAX_STARS}</span>
        </div>
        <StarBar value={summary.average} />
      </div>

      <p className="ag-small agr-sub">
        Average across {summary.scenarios} rated {summary.scenarios === 1 ? 'scenario' : 'scenarios'}
        {' '}in {summary.levelsAttempted} {summary.levelsAttempted === 1 ? 'trial' : 'trials'}
        {' '}· {summary.stars} of {summary.max} stars ({percent}%)
      </p>

      <dl className="agr-tracks">
        {summary.tracks.map(track => (
          <div key={track.trackId} className="agr-track">
            <dt>
              {track.label}
              {track.ageLabel && <small> · {track.ageLabel}</small>}
            </dt>
            <dd>
              {track.average === null ? (
                <em>Awaiting a rated run</em>
              ) : (
                <>
                  <StarBar value={track.average} />
                  <span className="agr-track-value">
                    {track.average.toFixed(1)} · {track.stars}/{track.max} stars ·{' '}
                    {track.levelsAttempted} {track.levelsAttempted === 1 ? 'level' : 'levels'}
                  </span>
                </>
              )}
            </dd>
          </div>
        ))}
      </dl>

      {summary.unrated > 0 && (
        <p className="ag-small agr-note">
          {summary.unrated} earlier {summary.unrated === 1 ? 'score' : 'scores'} predate star
          totals being stored and are left out of the average. Replay{' '}
          {summary.unrated === 1 ? 'that level' : 'those levels'} to include{' '}
          {summary.unrated === 1 ? 'it' : 'them'}.
        </p>
      )}

      <p className="ag-small agr-note">
        <Award size={13} /> Best run per level, saved in this browser only. Not part of your
        training record: no rubric score is issued by the mission API.
      </p>

      <a href="/challenges" className="ag-text-link" onClick={open}>
        Open Mission Readiness Trials <ChevronRight size={18} />
      </a>
    </section>
  )
}
