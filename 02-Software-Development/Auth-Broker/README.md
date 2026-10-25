# Isolated username login broker

Candidate Supabase Edge Function. **Not deployed.** See [account architecture and operator prerequisites](../../docs/ACCOUNT_IDENTITY.md).

The public pre-login `account-login` function requires CAPTCHA, validates an approved browser origin, obtains a rate-limited username resolution through the service-role-only RPC, and delegates password verification to Supabase Auth. It returns only the authenticated caller's tokens. No administrator key or email lookup is exposed to clients. Passwords and tokens are never logged by this code.

Keep this privileged runtime isolated from the user-scoped FastAPI service and client builds. Do not add secrets to this folder. Provision them only through the operator's provider secret manager after review. No hosted configuration or access was changed by adding these files.

Handler tests live with the web account test suite and mock upstream calls. Deno deployment/runtime, real CAPTCHA and hosted integration require separate verification.
