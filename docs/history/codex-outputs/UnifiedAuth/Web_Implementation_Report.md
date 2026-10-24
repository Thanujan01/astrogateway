# Web implementation

## Pages and states

| Route | Behavior |
|---|---|
| `/` and `/mars` | Preserved Mars preview; account-workspace link |
| `/welcome` | Responsive aerospace account entry, signup/login links |
| `/signup` | Display name, email/password validation; SDK signup; email-verification state or confirmed immediate session |
| `/login` | SDK login; safe invalid-credential errors; verified dashboard access only |
| `/verify-email` | Non-enumerating inbox instructions; no automatic profile authorization |
| `/forgot-password` | Non-enumerating recovery email request |
| `/auth/callback` | PKCE code exchange; pending-browser-flow correlation; query scrub; invalid/expired links rejected |
| `/reset-password` | Recovery context required; confirmation fields; Auth update; canonical profile revalidation |
| `/dashboard` | Protected workspace; no invented mission scores |
| `/profile` | Canonical UUID/email/profile view; edit/save/cancel display name |
| Other paths | Page-not-found state |

The existing App.tsx is lazy loaded and byte-identical to its pre-change copy. No Mars geometry, React controls or simulation rules were rewritten. Browser Back/Forward uses History API and popstate. The existing root URL still opens the Mars preview; account pages use explicit paths.

Dark navy surfaces, pale readable text, restrained Mars accents, reusable form/card/notice components, semantic labels, visible focus rings and minimum 44–50px principal controls form the design system. A 390px signup viewport had content width exactly 390px, with no horizontal overflow. This is a breakpoint check, not a full accessibility certification.

Configuration: /Users/kapilash/Projects/astrogateway/.env.local (owner-only, ignored); [.env.example](/Users/kapilash/Projects/astrogateway/.env.example). Only publishable project settings enter the client bundle. No key values are printed in reports. Vite's loopback proxy targets the confined local API at 127.0.0.1:8000. Deployed API URLs must use HTTPS; deployment is outside scope.

PKCE links should open in the browser where requested; Android links should open on their requesting device. Missing verifiers/pending flows are rejected. Callback codes are removed from the address bar before exchange; referrer policy is no-referrer. SDK sessions are persisted in origin-scoped browser storage, so XSS or malicious browser extensions can steal bearer/refresh tokens. No HTML rendering of user names is used. Production still needs CSP/HTTPS and a deployment security review.

Build: PASS. Lint: PASS without warnings in the final run. Vitest: 34 simulated checks (see final evidence); no hosted requests/accounts are part of those tests. The pre-existing Mars bundle remains ~925kB minified and emits Vite's >500kB chunk warning; it is lazy loaded outside account pages. Account entry bundle is ~463kB minified. No web performance target was measured.

A development hot-reload context mismatch was found and fixed by moving the stable context/hook into accountState.ts, separate from the provider. A page reload restored the workspace; this was a development refresh issue, not a hosted authentication result.

Files: [modified-source-manifest.json](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/Evidence/modified-source-manifest.json). Dev-only Vitest/jsdom/Testing Library support local tests; the official Supabase SDK is the added runtime authentication dependency.
