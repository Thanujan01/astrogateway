export async function mongoRequest<T>(path: string, body?: unknown): Promise<T> {
  const response = await fetch('/account-api' + path, { method: body === undefined ? 'GET' : 'POST',
    credentials: 'same-origin', redirect: 'error', signal: AbortSignal.timeout(12000),
    headers: { 'Content-Type': 'application/json' }, ...(body === undefined ? {} : { body: JSON.stringify(body) }) })
  if (!response.ok) {
    throw new Error(response.status === 401 ? 'Sign-in details are invalid or your session expired.' : response.status === 409 ? 'Those account details are already in use, or the mission changed. Retry with current details.' : response.status === 429 ? 'Too many requests. Wait and retry.' : response.status === 422 ? 'Check your input. Use a valid email, a 12-character password and a 2–64 character display name.' : 'Local account service is unavailable. Start Run Prototype.command and retry.')
  }
  const text = await response.text()
  if (text.length > 131072) throw new Error('Invalid account response.')
  return JSON.parse(text) as T
}
