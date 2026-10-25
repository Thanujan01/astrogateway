import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'
export default defineConfig({ plugins: [react()], test: { env: { VITE_SUPABASE_URL: 'https://jvkwofmxtxjhdwtwdboz.supabase.co', VITE_SUPABASE_PUBLISHABLE_KEY: 'sb_publishable_synthetic_fixture_only' }, environment: 'jsdom', setupFiles: ['./src/account/test-setup.ts'], include: ['src/account/*.test.ts*'] } })
