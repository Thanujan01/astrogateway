// Separate privileged runtime. Do not import this into the user-scoped FastAPI service.
export interface BrokerConfig { url: string; publicKey: string; serviceKey: string; captchaSecret: string; pepper: string; origins: string[] }
export function makeHandler(config: BrokerConfig, transport: (input: string, init: RequestInit) => Promise<Response> = fetch) {
  return async (request: Request): Promise<Response> => {
    const origin = request.headers.get('Origin')
    const headers = { 'Content-Type': 'application/json', 'Cache-Control': 'no-store', 'Vary': 'Origin',
      ...(origin && config.origins.includes(origin) ? { 'Access-Control-Allow-Origin': origin } : {}) }
    const reply = (status: number, body: object) => new Response(JSON.stringify(body), { status, headers })
    if (origin && !config.origins.includes(origin)) return reply(403, { error: 'Account request rejected.' })
    if (request.method === 'OPTIONS') return new Response(null, { status: 204, headers: { ...headers, 'Access-Control-Allow-Methods': 'POST', 'Access-Control-Allow-Headers': 'content-type,apikey,authorization' } })
    if (request.method !== 'POST') return reply(405, { error: 'Account request rejected.' })
    // Fail closed until separately provisioned operator secrets and exact target exist.
    if (config.url !== 'https://jvkwofmxtxjhdwtwdboz.supabase.co' || !config.publicKey.startsWith('sb_publishable_') || !config.serviceKey || !config.captchaSecret || config.pepper.length < 32) return reply(503, { error: 'Account service unavailable.' })
    try {
      // Bounded streamed body: Content-Length alone is not a trustworthy limit.
      const reader = request.body?.getReader(); if (!reader) return reply(400, { error: 'Account request rejected.' })
      let body = ''; let size = 0; const decoder = new TextDecoder()
      while (true) { const chunk = await reader.read(); if (chunk.done) break; size += chunk.value.byteLength; if (size > 8192) { await reader.cancel(); return reply(413, { error: 'Account request rejected.' }) }; body += decoder.decode(chunk.value, { stream: true }) }
      body += decoder.decode()
      const input = JSON.parse(body)
      if (!input || Object.keys(input).some(k => !['identifier','password','captchaToken'].includes(k)) || typeof input.identifier !== 'string' || typeof input.password !== 'string' || typeof input.captchaToken !== 'string') return reply(400, { error: 'Account request rejected.' })
      const identifier = input.identifier.trim().toLowerCase()
      if (!/^[a-z][a-z0-9_]{2,23}$/.test(identifier) || input.password.length < 1 || input.password.length > 1024 || input.captchaToken.length < 1 || input.captchaToken.length > 4096) return reply(400, { error: 'Account request rejected.' })
      const captcha = await transport('https://challenges.cloudflare.com/turnstile/v0/siteverify', { method: 'POST', redirect: 'error', signal: AbortSignal.timeout(10000), body: new URLSearchParams({ secret: config.captchaSecret, response: input.captchaToken }) })
      const challenge = await captcha.json()
      if (!captcha.ok || challenge.success !== true || challenge.action !== 'username-login' || !config.origins.some(allowed=>new URL(allowed).hostname === challenge.hostname)) return reply(401, { error: 'Sign-in failed. Check your account details and verification.' })
      const signingKey = await crypto.subtle.importKey('raw', new TextEncoder().encode(config.pepper), { name: 'HMAC', hash: 'SHA-256' }, false, ['sign'])
      const digest = await crypto.subtle.sign('HMAC', signingKey, new TextEncoder().encode(identifier))
      const hash = Array.from(new Uint8Array(digest), b => b.toString(16).padStart(2,'0')).join('')
      const lookup = await transport(config.url+'/rest/v1/rpc/broker_login_identity', { method: 'POST', redirect: 'error', signal: AbortSignal.timeout(10000), headers: { apikey: config.serviceKey, Authorization: 'Bearer '+config.serviceKey, 'Content-Type': 'application/json' }, body: JSON.stringify({ p_identifier: identifier, p_key_hash: hash }) })
      if (!lookup.ok) return reply(503, { error: 'Account service unavailable.' })
      const identity = await lookup.json()
      if (identity?.limited) return reply(429, { error: 'Too many requests. Wait and retry.' })
      const email = typeof identity?.email === 'string' ? identity.email : 'unregistered-account@example.invalid'
      const auth = await transport(config.url+'/auth/v1/token?grant_type=password', { method: 'POST', redirect: 'error', signal: AbortSignal.timeout(10000), headers: { apikey: config.publicKey, 'Content-Type': 'application/json' }, body: JSON.stringify({ email, password: input.password }) })
      if (!auth.ok || !identity?.email) return reply(auth.status === 429 ? 429 : 401, { error: 'Sign-in failed. Check your account details and verification.' })
      const session = await auth.json()
      if (typeof session.access_token !== 'string' || typeof session.refresh_token !== 'string') return reply(503, { error: 'Account service unavailable.' })
      // Tokens go only to the successful caller. Never return resolved email/profile or admin key.
      return reply(200, { access_token: session.access_token, refresh_token: session.refresh_token })
    } catch { return reply(503, { error: 'Account service unavailable.' }) }
  }
}
