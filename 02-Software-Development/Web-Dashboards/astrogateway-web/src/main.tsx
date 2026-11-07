import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import AccountEntry from './account/AccountEntry'
import './index.css'
createRoot(document.getElementById('root')!).render(<StrictMode><AccountEntry /></StrictMode>)
