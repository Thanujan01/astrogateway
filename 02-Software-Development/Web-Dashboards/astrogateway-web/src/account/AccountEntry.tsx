import { lazy, Suspense } from 'react'
const ActiveAccountApp = lazy(() => import.meta.env.VITE_AUTH_PROVIDER === 'supabase' ? import('./AccountApp') : import('./MongoAccountApp'))
export default function AccountEntry() { return <Suspense fallback={<p role="status">Loading account workspace…</p>}><ActiveAccountApp /></Suspense> }
