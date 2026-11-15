import { lazy, Suspense } from 'react'
const SoloAccountApp = lazy(() => import('./SoloAccountApp'))
export default function AccountEntry() { return <Suspense fallback={<p role="status">Loading account workspace…</p>}><SoloAccountApp /></Suspense> }
