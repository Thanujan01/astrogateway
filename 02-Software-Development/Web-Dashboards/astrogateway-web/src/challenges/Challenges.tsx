import { useCallback, useEffect, useMemo, useState } from 'react'
import { Award, ChevronLeft, RotateCcw, Shield, Sparkles } from 'lucide-react'
import bank from './questions.json'
import { MAX_STARS, readProgress, recordTrial } from './progress'
import './challenges.css'

/* ---------------------------------------------------------------------------
 * Mission Readiness Trials.
 *
 * Question banks are generated from the two supplied .docx sets by
 *   python tools/web-assets/parse_challenges.py <open_age.docx> <age_7_12.docx>
 *
 * These are NOT right/wrong questions. Every option carries a 1-5 star
 * assessment rating from the source documents, exactly one option per question
 * is rated 5, and a trial score is the sum of the stars actually earned. The
 * source documents' own caveats about those ratings are shown in the UI.
 * ------------------------------------------------------------------------- */

interface Option { key: string; text: string; stars: number }
interface Question { id: string; n: number; title: string; scenario: string; options: Option[] }
interface Level { n: number; title: string; difficulty: string; questions: Question[] }
interface Track { id: string; label: string; ageLabel: string; source: string; levels: Level[]; questionCount: number }
interface Bank { title: string; ratingNote: string; scienceNote: string; tracks: Track[] }

const data = bank as Bank

/** Fisher-Yates. A fresh shuffle is taken per trial start, not derived from the bank order. */
function shuffled<T>(items: T[]): T[] {
  const copy = items.slice()
  for (let i = copy.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [copy[i], copy[j]] = [copy[j], copy[i]]
  }
  return copy
}

function Stars({ value, max = MAX_STARS }: { value: number; max?: number }) {
  const filled = '★'.repeat(Math.max(0, Math.min(value, max)))
  const empty = '★'.repeat(Math.max(0, max - value))
  return (
    <span className="agc-stars" role="img" aria-label={value + ' out of ' + max + ' stars'}>
      {filled}
      <span className="agc-stars-dim">{empty}</span>
    </span>
  )
}

export default function Challenges() {
  const [trackId, setTrackId] = useState<string | null>(null)
  const [levelN, setLevelN] = useState<number | null>(null)
  const [index, setIndex] = useState(0)
  const [picks, setPicks] = useState<Record<string, string>>({})
  const [progress, setProgress] = useState(() => readProgress())
  // A per-trial shuffle of the bank's questions (and each question's options), taken fresh
  // on every start/retry, so the fixed bank order -- and the always-first correct option --
  // never reaches the screen.
  const [quiz, setQuiz] = useState<Question[] | null>(null)

  const track = useMemo(
    () => data.tracks.find(t => t.id === trackId) ?? null,
    [trackId],
  )
  const level = useMemo(
    () => track?.levels.find(l => l.n === levelN) ?? null,
    [track, levelN],
  )
  const question = quiz?.[index] ?? null
  const picked = question ? picks[question.id] : undefined
  const answered = picked !== undefined
  const finished = Boolean(quiz && index >= quiz.length)

  const earned = useMemo(() => {
    if (!quiz) return 0
    return quiz.reduce((sum, q) => {
      const choice = q.options.find(o => o.key === picks[q.id])
      return sum + (choice ? choice.stars : 0)
    }, 0)
  }, [quiz, picks])

  const choose = useCallback((key: string) => {
    if (!question || picks[question.id] !== undefined) return
    setPicks(current => ({ ...current, [question.id]: key }))
  }, [question, picks])

  // Recording the trial belongs to the event that ends it, not to an effect watching
  // `finished` -- an effect there would cascade an extra render on completion. The write
  // happens outside the state updater so StrictMode's double invocation cannot double-write.
  const advance = useCallback(() => {
    const next = index + 1
    if (track && level && quiz && next >= quiz.length) {
      setProgress(recordTrial(progress, {
        trackId: track.id,
        label: track.label,
        ageLabel: track.ageLabel,
        levelN: level.n,
        earned,
        max: quiz.length * MAX_STARS,
      }))
    }
    setIndex(next)
  }, [index, track, level, quiz, progress, earned])

  const startLevel = useCallback((n: number) => {
    const picked = track?.levels.find(l => l.n === n) ?? null
    setLevelN(n)
    setIndex(0)
    setPicks({})
    setQuiz(picked ? shuffled(picked.questions).map(q => ({ ...q, options: shuffled(q.options) })) : null)
  }, [track])

  // Number keys choose an option, Enter/Space moves on. Skipped while the user is
  // typing in a field so this never fights a form elsewhere on the page.
  useEffect(() => {
    if (!question) return
    const onKey = (event: KeyboardEvent) => {
      const target = event.target as HTMLElement | null
      if (target && /^(INPUT|TEXTAREA|SELECT)$/.test(target.tagName)) return
      if (!answered) {
        const slot = Number(event.key)
        if (Number.isInteger(slot) && slot >= 1 && slot <= question.options.length) {
          event.preventDefault()
          choose(question.options[slot - 1].key)
        }
        return
      }
      if (event.key === 'Enter') {
        event.preventDefault()
        advance()
      }
    }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [question, answered, choose, advance])

  /* --- Track picker ------------------------------------------------------- */
  if (!track) {
    return (
      <section className="agc">
        <div className="ag-eyebrow">MISSION READINESS TRIALS</div>
        <h1>Choose your challenge level.</h1>
        <p className="ag-lead">
          Mars outpost decision scenarios. Each answer is rated one to five stars by how well it
          protects the crew, supports the mission and uses resources responsibly.
        </p>

        <div className="agc-tracks">
          {data.tracks.map(item => {
            const done = Object.keys(progress.tracks[item.id]?.levels ?? {}).length
            return (
              <button
                key={item.id}
                type="button"
                className="agc-track"
                onClick={() => { setTrackId(item.id); setLevelN(null) }}
              >
                <span className="agc-track-age">{item.ageLabel}</span>
                <strong>{item.label}</strong>
                <span className="agc-track-meta">
                  {item.questionCount} questions · {item.levels.length} levels
                </span>
                <span className="agc-track-progress">
                  {done ? done + ' of ' + item.levels.length + ' levels attempted' : 'Not started'}
                </span>
              </button>
            )
          })}
        </div>

        <Caveats />
      </section>
    )
  }

  /* --- Level picker ------------------------------------------------------- */
  if (!level) {
    const levels = progress.tracks[track.id]?.levels ?? {}
    return (
      <section className="agc">
        <button type="button" className="ag-quiet agc-back" onClick={() => setTrackId(null)}>
          <ChevronLeft size={16} /> All challenge levels
        </button>
        <div className="ag-eyebrow">{track.label.toUpperCase()} · {track.ageLabel.toUpperCase()}</div>
        <h1>Pick a trial.</h1>
        <p className="ag-lead">
          Six trials of ten scenarios each. Difficulty rises from the first to the last.
        </p>

        <ol className="agc-levels">
          {track.levels.map(item => {
            const score = levels[String(item.n)]?.best
            const max = item.questions.length * MAX_STARS
            return (
              <li key={item.n}>
                <button type="button" className="agc-level" onClick={() => startLevel(item.n)}>
                  <span className="agc-level-n">{String(item.n).padStart(2, '0')}</span>
                  <span className="agc-level-body">
                    <strong>{item.title}</strong>
                    <small>{item.difficulty} · {item.questions.length} scenarios</small>
                  </span>
                  <span className="agc-level-score">
                    {score === undefined
                      ? <em>Not attempted</em>
                      : <><Award size={14} /> {score} / {max}</>}
                  </span>
                </button>
              </li>
            )
          })}
        </ol>

        <Caveats />
      </section>
    )
  }

  /* --- Results ------------------------------------------------------------ */
  if (finished) {
    if (!quiz) return null
    const max = quiz.length * MAX_STARS
    const percent = Math.round((earned / max) * 100)
    const perfect = quiz.filter(q => {
      const choice = q.options.find(o => o.key === picks[q.id])
      return choice?.stars === MAX_STARS
    }).length

    return (
      <section className="agc">
        <div className="ag-eyebrow">TRIAL COMPLETE</div>
        <h1>{earned} of {max} stars.</h1>
        <p className="ag-lead">
          {track.label} · Level {level.n}: {level.title}. You chose the strongest action in{' '}
          {perfect} of {quiz.length} scenarios ({percent}% of available stars).
        </p>

        <div className="agc-actions">
          <button type="button" className="ag-primary" onClick={() => startLevel(level.n)}>
            <RotateCcw size={16} /> Retry this trial
          </button>
          <button type="button" className="ag-secondary" onClick={() => setLevelN(null)}>
            Choose another trial
          </button>
        </div>

        <ol className="agc-recap">
          {quiz.map(q => {
            const choice = q.options.find(o => o.key === picks[q.id])
            const strongest = q.options.find(o => o.stars === MAX_STARS)
            return (
              <li key={q.id}>
                <div className="agc-recap-head">
                  <strong>{q.title}</strong>
                  <Stars value={choice ? choice.stars : 0} />
                </div>
                <p className="ag-small">{q.scenario}</p>
                <p className="ag-small">
                  <span className="agc-tag">Your answer</span> {choice ? choice.text : 'Not answered'}
                </p>
                {choice?.stars !== MAX_STARS && strongest && (
                  <p className="ag-small">
                    <span className="agc-tag agc-tag-best">Strongest</span> {strongest.text}
                  </p>
                )}
              </li>
            )
          })}
        </ol>

        <Caveats />
      </section>
    )
  }

  /* --- Question runner ---------------------------------------------------- */
  if (!question || !quiz) return null

  const total = quiz.length
  const chosen = question.options.find(o => o.key === picked)
  const strongest = question.options.find(o => o.stars === MAX_STARS)
  const last = index === total - 1

  return (
    <section className="agc agc-quiz">
      <button type="button" className="ag-quiet agc-back" onClick={() => setLevelN(null)}>
        <ChevronLeft size={16} /> Leave trial
      </button>

      <div className="agc-progress-row">
        <span className="ag-eyebrow">
          {track.label.toUpperCase()} · LEVEL {level.n} · {level.difficulty.toUpperCase()}
        </span>
        <span className="ag-small">
          Scenario {index + 1} of {total} · {earned} stars so far
        </span>
      </div>
      <div className="agc-progress" role="progressbar" aria-valuenow={index + 1} aria-valuemin={1} aria-valuemax={total}>
        <span style={{ width: ((index + (answered ? 1 : 0)) / total) * 100 + '%' }} />
      </div>

      <h2 className="agc-title">{question.title}</h2>
      <p className="agc-scenario">{question.scenario}</p>

      <ul className="agc-options">
        {question.options.map((option, slot) => {
          const isPick = option.key === picked
          const isBest = option.stars === MAX_STARS
          const classes = ['agc-option']
          if (answered && isPick) classes.push('is-pick')
          if (answered && isBest) classes.push('is-best')
          return (
            <li key={option.key}>
              <button
                type="button"
                className={classes.join(' ')}
                onClick={() => choose(option.key)}
                disabled={answered}
                aria-pressed={isPick}
              >
                <span className="agc-option-key" aria-hidden="true">{slot + 1}</span>
                <span className="agc-option-text">{option.text}</span>
                {answered && <Stars value={option.stars} />}
              </button>
            </li>
          )
        })}
      </ul>

      {answered && chosen && (
        <div className="agc-feedback" role="status">
          <p>
            <strong>{chosen.stars} of {MAX_STARS} stars.</strong>{' '}
            {chosen.stars === MAX_STARS
              ? 'Strongest decision for this situation and the stated mission priorities.'
              : chosen.stars >= 3
                ? 'Workable, but it carries a meaningful disadvantage.'
                : 'Risky or unsafe for the crew or the mission.'}
          </p>
          {chosen.stars !== MAX_STARS && strongest && (
            <p className="ag-small">Strongest action: {strongest.text}</p>
          )}
          <button type="button" className="ag-primary" onClick={advance}>
            {last ? 'See trial results' : 'Next scenario'}
          </button>
        </div>
      )}

      {!answered && (
        <p className="ag-small agc-hint">
          Pick an action to see how it is rated. Keys 1-{question.options.length} also work.
        </p>
      )}
    </section>
  )
}

/* Both source documents carry these statements; they are not ours to drop. */
function Caveats() {
  return (
    <div className="agc-caveats">
      <p className="ag-small">
        <Sparkles size={13} /> {data.ratingNote}
      </p>
      <p className="ag-small">
        <Shield size={13} /> {data.scienceNote}
      </p>
      <p className="ag-small agc-caveat-dim">
        Trial scores are saved in this browser only. They are not part of your training record and
        no score is sent to the mission API.
      </p>
    </div>
  )
}
