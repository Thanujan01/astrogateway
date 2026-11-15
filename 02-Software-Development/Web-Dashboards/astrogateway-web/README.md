# astrogateway-web

A frontend-only React 19 + TypeScript + Vite app for a single local profile (Thanujan). There is
no account server and no database: the profile, training-session count and Mission Readiness
Trials scores all live in this browser's `localStorage`. It opens a local "training" flow and
renders a lightweight in-browser Mars scene.

**It is not a mission-control dashboard.** Despite living under `Web-Dashboards/`, there is no
telemetry view, no sensor table, no charts and no scores. `/dashboard` shows a greeting, one
"Start training" panel and a local session count.

## Requirements

Node **^22.22.2 || >=24.15.0** (enforced by `engines`; see `.nvmrc`). The lockfile resolves
Vite 8, Vitest 5 and jsdom 30, which all refuse older releases. `npm ci` on Node 20 prints
`EBADENGINE` warnings and still installs, but do not trust `npm run build` or `npm test` there.

## Run it

```bash
npm ci
npm run dev        # http://127.0.0.1:5173
```

No backend, no database, no environment variables -- this is a static app. `vite.config.ts` binds
**127.0.0.1 only**; for another device on your network use `npm run dev -- --host`.

| Script | What it does |
|---|---|
| `npm run dev` | Vite dev server |
| `npm run build` | `tsc -b` then `vite build` |
| `npm test` | Vitest (jsdom), see `vitest.config.ts` |
| `npm run lint` | oxlint, see `.oxlintrc.json` |

## The single local profile

`src/main.tsx` mounts `AccountEntry`, which renders `src/account/SoloAccountApp.tsx` -- the only
account implementation. There is no sign-up, login, password, username or avatar; `src/account/
profile.ts` creates one fixed profile named "Thanujan" on first load (random id, persisted in
`localStorage` under `ag-solo-profile-v1`) and the app treats it as always signed in. Only the
display name is editable, from `/profile`. Training-session starts increment a counter under
`ag-solo-sessions-v1` -- see `recordSessionStart`/`readSessionCount` in `profile.ts`.

A multi-account version (Supabase Auth and a local MongoDB API, plus a "Players" discovery/chat
feature) existed earlier and was deliberately removed in favour of this single-profile, zero-
backend shape; if you're looking for that, it's in git history, not in this tree.

## Routes

Hand-rolled with `history.pushState` + a synthetic `popstate` event -- there is no router
library; `navigate()` is defined in `SoloAccountApp.tsx`.

| Route | Notes |
|---|---|
| `/welcome` | hero landing page |
| `/mars` | Three.js scene, `src/App.tsx`, WASD + Shift |
| `/challenges` | Mission Readiness Trials, `src/challenges/Challenges.tsx` |
| `/tutorials` | curated YouTube training videos, `src/tutorials/Tutorials.tsx` |
| `/dashboard` | greeting, Start training, local session count |
| `/profile` | display name, profile id, readiness rating |
| `/training` | renders the same Three.js scene as `/mars`, reached via Start training |

`/training` does **not** launch the Unity game. The Unity prototype in `01-Game-Development` is a
separate application.

## Conventions worth knowing before you edit

- **`profile.ts` owns the local profile and session-count schema.** Both read and write go
  through `loadProfile`/`saveDisplayName`/`readSessionCount`/`recordSessionStart` -- don't read or
  write `localStorage['ag-solo-profile-v1']`/`['ag-solo-sessions-v1']` directly elsewhere.
- **Storage can throw outright** (private windows, blocked site data), not just return null.
  `loadProfile`/`readSessionCount` catch and fall back rather than crash the app.
- **`isProfile` validates stored data defensively** before trusting it, so corrupted/foreign
  `localStorage` contents fall back to a fresh default profile instead of crashing the app.
- **Known style problem:** several files are written at 400-900 characters per line, with whole
  JSX trees on one line. New code should not copy that; wrap at a sane width.

## Tests

```bash
npm test
```

`src/account/profile.test.ts` covers the local profile and session-count logic. There are no
tests for `App.tsx` (the Three.js scene) or `SoloAccountApp.tsx` itself.

## Mission Readiness Trials (`/challenges`)

120 Mars-outpost MCQ scenarios in two tracks, selectable by age group:

| Track | Audience | Content |
|---|---|---|
| Cadet | Ages 7-12 | 6 levels x 10 scenarios, Introductory to Moderate/Challenging |
| Specialist | Ages 13 and above | 6 levels x 10 scenarios, Foundation to Expert |

**These are not right/wrong questions.** Every option carries a 1-5 star assessment rating from
the source documents, exactly one option per question is rated 5, and a trial score is the sum of
stars actually earned. Do not refactor this into a boolean correct/incorrect model -- it would
discard the grading the questions were written around. Both the question order and each
question's option order are shuffled fresh per attempt (`Challenges.tsx`'s `shuffled` helper) so
the fixed bank order, and the always-first correct option, never reach the screen.

Scores are kept in `localStorage` under `ag-challenges-v1` (best stars per track and level).

### Regenerating the question bank

`src/challenges/questions.json` is generated, not hand-edited:

```bash
python tools/web-assets/parse_challenges.py   <Mars_Astronaut_Challenges_Open_Age_60_MCQs.docx>   <Mars_Astronaut_Challenges_Age_7-12_60_MCQs.docx>
```

The parser fails loudly rather than emitting a partial bank: it asserts 60 questions per file,
at least two options each, exactly one 5-star option, and non-empty scenario text.

The two caveats shown at the bottom of the page (ratings are preliminary assessment-design
ratings, not official NASA judgements; the scenarios are educational, not operational
instructions) are carried from the source documents and should not be removed.

### Readiness rating on the profile

`/profile` shows the average star rating earned across trials, overall and per track, rendered as
a fractional star bar.

`src/challenges/progress.ts` is the **single owner** of the stored schema; the trial runner and
the profile both go through it. Keep it that way -- two readers with their own parsing is how the
two views drift apart. It stores each level's `max` and the track labels alongside the score, so
the profile never imports `questions.json` and the account bundle stays free of the 81 KB bank.

- Only the best run per level counts toward the rating; every run increments `attempts`.
- `average` is mean stars per scenario, `stars / (max / 5)`, in the range 0-5.
- Version 1 records (best score with no denominator) are migrated rather than discarded. They
  are counted as attempted but excluded from the average, and reported as `unrated`, until the
  level is replayed and a real denominator is written.
- `summarise` is defensive about its argument: malformed stored data yields a null average rather
  than throwing.

## Physical training tutorials (`/tutorials`)

`src/tutorials/videos.ts` is a static list of `{id, title, channel}` for curated YouTube videos on
astronaut fitness, selection and training. `Tutorials.tsx` embeds each as a `youtube-nocookie.com`
iframe in a responsive grid. No network calls happen from this app beyond loading those embeds.

## Header navigation and mobile layout

`src/account/SiteNav.tsx` renders the header nav. Above 760px it is the same inline row it always
was; below that the links collapse behind a three-bar toggle.

It takes the links as `children`, so it needs no knowledge of the app's `Link` component -- keep
it that way rather than moving the link list inside it.

The panel closes on: a click on any child (one native listener on the panel, which covers links
and buttons without enumerating them), `popstate` (so in-app navigation closes it), Escape (which
also returns focus to the toggle), a click outside, and crossing back above the breakpoint.
`aria-expanded` and `aria-controls` are wired to the panel.

Breakpoints in use across the app:

| Width | What changes |
|---|---|
| 760px | Nav collapses to the toggle; header forced to one row; hero, dashboard and profile grids go single-column; trial and rating rows restack |
| 700px | Mars preview HUD repositions (`src/App.css`) |
| 400px | Toggle drops the word "Menu" and keeps the icon; menu width clamps to the viewport |

Note that Lightning CSS rewrites `max-width: 760px` to the range form `(width<=760px)` in the
built sheet. That is the same query, not a mistake.

**Known gap:** `/mars` and `/training` have no touch movement control. Look-around works on touch
(the camera uses pointer events), but moving the astronaut is still WASD only, so the scene is
look-only on a phone. An on-screen joystick would be a separate piece of work.
