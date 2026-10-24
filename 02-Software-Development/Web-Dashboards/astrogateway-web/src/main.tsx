import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import AccountApp from './account/AccountApp.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AccountApp />
  </StrictMode>,
)
