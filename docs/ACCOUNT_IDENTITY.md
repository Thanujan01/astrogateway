# One account, username login and profile photo

[README](../README.md) · [Credentials](CREDENTIALS_GUIDE.md) · [Database checks](SENTINEL_DATABASE_VALIDATION.md)

Use an existing email address to register **one AstroGateway account**, confirm the email and choose an AstroGateway password. This is not the mailbox/Gmail password. Web and Android authenticate against the same Supabase Auth project; `auth.users.id` is the stable account key. A username is an alternative login label, not a second account, password or identity issuer. Password reset continues to use the verified email address.

## Implemented versus deployed

| Surface | Current implementation / limit |
|---|---|
| Web email login | Existing Supabase password login, unchanged. |
| Web username login | New separate `account-login` Edge broker plus CAPTCHA; restores the same Supabase Auth session. Unavailable until operator configuration/deployment. |
| Username selection/change | After verified signup, in Profile. Case-normalized unique handle; 3–24 ASCII letters/numbers/underscores, starting with a letter. Existing accounts remain valid with a NULL username until they choose one. Display names still support Tamil and other Unicode. |
| Web photo controls | Upload/change/remove; PNG/JPEG/WebP input up to 2 MB, center-cropped and re-encoded to 512×512 PNG without original EXIF. |
| Central profile | Optional `username` and `avatar_path` returned by `/v1/me`; UUID unchanged. Username/photo writes use user-scoped invoker RPCs, verified ownership and revocation checks. |
| Storage | Private `profile-photos` bucket; exact owner path `<auth.uid()>/avatar.png`; 2 MB limit, PNG MIME allowlist, owner-only SELECT/INSERT/UPDATE/DELETE policies. Short signed preview URLs are bearer links valid for 120 seconds; do not share them. |
| Android | Existing email/password login uses the same central identity. Native username/CAPTCHA and photo editor UI are **not implemented in this change**. |
| Main Unity | Still defaults to offline services. Authenticated Android integration scene is a prototype. Full gameplay account/sync wiring remains pending. |
| Hosted environment | **NOT DEPLOYED OR VERIFIED**. Approved test hostname DNS lookup failed in the preceding provider check. No keys/passwords/accounts, hosted database, bucket or permissions changed. |

## Implementation and privilege boundary

The user-facing FastAPI service still receives no administrator credentials or password login endpoint. The separate [Auth-Broker](../02-Software-Development/Auth-Broker) runtime alone performs a narrow, service-role-only username-to-email lookup, then asks Supabase Auth to verify the supplied password. It returns only the successful caller's access/refresh tokens; it never returns a resolved email, profile directory or operator key. Client session/profile validation still checks the canonical user UUID and verified email.

The new username endpoint receives a password and must run only on trusted HTTPS infrastructure with request-body/token logging disabled. It requires a successful Turnstile challenge, configured hostname/action and a database-backed throttle of six attempts per normalized username per ten minutes. HMAC hash keys use a private operator pepper; no raw usernames/passwords are stored in the rate table. Invalid username and invalid password have the same public error body/status; this does not prove timing indistinguishability or exhaustive abuse resistance. Old rate rows are pruned in bounded batches. Set retention/monitoring and perimeter request/body/time/rate limits before exposure.

The privileged resolver in `public` is SECURITY DEFINER and **cannot be executed by anon or authenticated**. Only service_role may execute it. New `mission_api` profile functions remain SECURITY INVOKER. Clients pass their expected UUID to prevent an in-flight change targeting a newly switched account. The broker has no mission/data administration endpoints. Its service credential must never be copied into the FastAPI runtime, web/Android/Unity configuration or this repository. This is an additional privileged deployment surface requiring operator review; no credential was provisioned during implementation.

## Candidate installation order and operator setup

Review against the actual target before any deployment. These are tracked candidates, not proof of a hosted migration. Do not run the local fixture scripts on Supabase.

1. Reviewed Stage05B2 base/retirement, UnifiedAuth 004 profile policy, mission command 005 and sensor 006 prerequisites must already be installed compatibly; preserve existing data and verify rollback/backup separately.
2. [007_account_profile.sql](../02-Software-Development/Sentinel-Database/migrations/007_account_profile.sql): nullable username/photo fields and user-scoped setters. No password column or account backfill/duplication.
3. [008_private_avatars.sql](../02-Software-Development/Sentinel-Database/migrations/008_private_avatars.sql): private bucket and policies. Review **all existing storage policies**, because permissive policies combine with OR; this script cannot neutralize an existing broad policy. Confirm bucket limits and actual Storage HTTP enforcement.
4. [009_private_login_broker.sql](../02-Software-Development/Sentinel-Database/migrations/009_private_login_broker.sql): isolated resolver and rate-window table. Function owner must have only the reviewed runtime permissions; migration executor ownership must be checked on the actual provider.
5. Deploy the separate `Auth-Broker/supabase` project only after reviewing pre-login public access (`verify_jwt=false` for **this function only**), CAPTCHA, transport, CORS/perimeter limits and operator secret isolation. Other functions' JWT verification remains unchanged.
6. Broker operator settings: Supabase-managed `SUPABASE_URL`, `SUPABASE_SERVICE_ROLE_KEY`; privately provisioned `ACCOUNT_PUBLIC_KEY`, `ACCOUNT_LOGIN_PEPPER` (at least 32 characters, high entropy), `ACCOUNT_TURNSTILE_SECRET`; exact comma-separated HTTPS `ACCOUNT_ALLOWED_ORIGINS`. No values are included here. Turnstile hostname must match an approved origin and action `username-login`. Do not use the same one-time challenge token for a separate provider CAPTCHA check; coordinate provider anti-abuse configuration and test it before rollout.
7. Web public build setting: `VITE_ACCOUNT_TURNSTILE_SITE_KEY`. An empty value disables username login with a clear message; existing email login continues. Existing test profile origin stays fixed. Expose reviewed `mission_api` RPCs through the approved gateway and reload its schema cache as appropriate.
8. Test mailbox registration/verification, email and username login producing the same UUID, duplicate handles, wrong password/unknown handle, CAPTCHA replay/expiry, recovery, cross-account profile/storage denial, photo replace/remove across browser refresh, revocation and account switching on the actual hosted gateway. Verify native and game integration separately.

## Measured local validation

- **59 SQL checks passed, zero failed** in disposable PostgreSQL 17.11, including case normalization, uniqueness, ownership, stale-account denial, broker privileges/throttle and avatar policy operations. Minimal synthetic Storage tables test SQL policies, **not the hosted Storage service or MIME/size HTTP enforcement**.
- **61 web/broker tests passed** with synthetic fixtures and mocked provider/Storage transports. Includes successful/failing username session restoration, CAPTCHA failure, unknown-username/wrong-password error parity, upload re-encoding/owner path and photo removal failure ordering.
- **233 backend tests passed**, including propagation of canonical username/avatar metadata. One existing Starlette test-client deprecation warning remains.
- Web TypeScript + Vite production build passed. Existing large bundle warning remains.
- Deno Edge runtime deployment, live Turnstile/Supabase Auth, Storage HTTP enforcement, real Android/Unity integration and UI device tests remain unverified. Passing mock tests do not imply a deployed feature.

Official references: [Supabase Edge authentication](https://supabase.com/docs/guides/functions/auth), [private storage buckets](https://supabase.com/docs/guides/storage/buckets/fundamentals), [signed downloads](https://supabase.com/docs/guides/storage/serving/downloads).
