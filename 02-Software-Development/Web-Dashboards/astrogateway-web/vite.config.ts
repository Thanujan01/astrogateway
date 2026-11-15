import { copyFileSync } from 'node:fs'
import react from '@vitejs/plugin-react'
import { defineConfig, type Plugin } from 'vite'

/* ---------------------------------------------------------------------------
 * Routing here is a client-side history.pushState switch, not a real router,
 * so there is never a real file at e.g. /dashboard or /challenges. GitHub
 * Pages serves plain static files: a direct load or refresh on one of those
 * paths asks the server for a file that doesn't exist and gets a 404. GitHub
 * Pages' documented fix is a 404.html that IS the app shell -- the server
 * falls back to it on any unknown path, and the app then reads the real
 * location.pathname itself. Cloning index.html after the build keeps this in
 * sync with every build automatically, with the correct base-path asset URLs
 * already baked in.
 * ------------------------------------------------------------------------- */
function spaFallback404(): Plugin {
  return {
    name: 'spa-fallback-404',
    closeBundle() {
      copyFileSync('dist/index.html', 'dist/404.html')
    },
  }
}

// Frontend-only: no backend/account API, so no dev proxy is needed.
export default defineConfig({
  base: process.env.GITHUB_PAGES ? '/astrogateway/' : '/',
  plugins: [react(), spaFallback404()],
  server: {
    host: '127.0.0.1',
  },
})
