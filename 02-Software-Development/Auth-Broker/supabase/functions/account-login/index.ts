import { makeHandler } from './handler.ts'
Deno.serve(makeHandler({
  url: Deno.env.get('SUPABASE_URL') ?? '',
  publicKey: Deno.env.get('ACCOUNT_PUBLIC_KEY') ?? '',
  serviceKey: Deno.env.get('SUPABASE_SERVICE_ROLE_KEY') ?? '',
  captchaSecret: Deno.env.get('ACCOUNT_TURNSTILE_SECRET') ?? '',
  pepper: Deno.env.get('ACCOUNT_LOGIN_PEPPER') ?? '',
  origins: (Deno.env.get('ACCOUNT_ALLOWED_ORIGINS') ?? '').split(',').map(s => s.trim()).filter(Boolean),
}))
