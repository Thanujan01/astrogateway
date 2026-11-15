import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'
// Frontend-only: no backend/account API, so no dev proxy is needed.
export default defineConfig({
  base: process.env.GITHUB_PAGES ? '/astrogateway/' : '/',
  plugins: [react()],
  server: {
    host: '127.0.0.1',
  },
})
