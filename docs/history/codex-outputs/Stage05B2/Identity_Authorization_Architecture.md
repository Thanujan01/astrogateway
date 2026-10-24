# Stage05B.2 — Identity authorization architecture

2026-10-09. Selected: **FastAPI contracts + independent Supabase PostgREST JWT verification + native RLS**. Implemented and validated locally only. No remote SQL/accounts/configuration changes. Remote deployment remains BLOCKED.

## Complete path

1. Android Supabase SDK signs in to the single provider. PKCE callback/session/refresh state is owned by Kotlin and stored through Android Keystore encryption. It refreshes the access bearer as needed; no Unity login/provider or second issuer exists.
2. Android calls existing FastAPI profile/mission endpoints with its access bearer, not a user-selected identity. FastAPI asks the configured Auth service to authenticate it, then validates signed subject against returned UUID, issuer/audience/role, expiry, session UUID and confirmed email. Request bodies forbid extra owner/score fields.
3. The repository sends the ORIGINAL bearer plus publishable key to fixed `/rest/v1/rpc/` names in `mission_api`. No caller headers, fabricated claims, owner IDs, Auth-session IDs, SQL text, database password, service-role key or signing key are forwarded. Mission/session/launch UUIDs are typed operation arguments, not proof of identity.
4. Provider-owned PostgREST independently verifies the JWT signature and claims before selecting its NOLOGIN authenticated role. Its private authenticator credential is outside FastAPI. PostgreSQL authorization uses auth.uid/auth.jwt established by this trusted gateway; application-controlled astrogateway settings have no role in new policies.
5. Invoker RPCs operate on private training tables under authenticated RLS. Policies bind owner and mission Auth session to verified gateway claims; strict expiry, confirmed non-anonymous Auth state, revocation, fixed mission ID and bounded lifecycle remain enforced. The current_user/claims assertion is defence in depth; it is NOT database-side signature verification.
6. FastAPI returns the existing profile/mission JSON; Kotlin checks IDs and sends only bounded mission/launch/user/session identifiers and lease to the private Unity bridge. Passwords, refresh/access tokens and database credentials never enter Unity.

## Options evaluated

| Option | Security/fit | Decision |
|---|---|---|
| Direct PG role + arbitrary GUC/RPC UUIDs | Shared SQL credential can change claims; role restriction/RLS alone does not authenticate a user | Reject |
| Direct PG with independent cryptographic verification | Possible with a separately trusted verifier and protected signing/broker authority; requires supported database verification/rotation and removal of every bypass | Too much unproven infrastructure for this stage; no home-grown DB JWT verification |
| Supabase PostgREST with original user bearer | Existing maintained independent signature verifier; native user RLS; no host DB authority; fits current issuer/HTTP contracts | **Select** |
| Hybrid user PostgREST + privileged worker | Suitable for future server-only jobs only if separate OS/credential/network trust compartment and narrow semantic operations | Not needed now; no privileged worker added |

Gateway authentication role switching is documented by [PostgREST](https://docs.postgrest.org/en/v14/references/auth.html); native identity/RLS by [Supabase](https://supabase.com/docs/guides/database/postgres/row-level-security). PostgreSQL variables remain mutable in SQL: security comes from excluding untrusted application roles from data privileges/role membership and preventing arbitrary SQL on the gateway surface, not pretending GUCs are cryptographically immutable.

## Permission model

- authenticated: provider NOLOGIN, non-admin/non-BYPASSRLS; explicit private schema usage/select and limited mission/revocation column writes; five mission RPC executions. No profile writes, deletes, truncate, ownership, DDL or session-setting RPC. No Auth user-table read grant.
- astrogateway_schema_owner: dedicated NOLOGIN/non-admin owner of application schemas/tables/invoker functions; no application membership. Its owner-scoped global function defaults remove PUBLIC execute, avoiding shared provider owners' global-default changes. Future app functions must use this owner and explicit grants; admin-created future objects still require review.
- Private confirmed_user/profile-trigger functions: bounded SECURITY DEFINER with fixed empty search_path, migration-admin owner; never in exposed mission_api. The boolean helper reads only current gateway subject's confirmed/non-anonymous status. Profile trigger ignores privilege/training-level metadata. This high-trust ownership must remain audited.
- Legacy astrogateway_runtime/astrogateway_api, if present: NOLOGIN, no role memberships or protected schema/table/column/function privileges. Exact retired-login sessions are terminated; revocation alone cannot unset a role already selected in a live session. No provider/admin session is targeted. No app runtime DB role/password is provisioned.

The new candidate does not revoke provider-wide PUBLIC/default privileges to achieve app isolation: it removes the host SQL authority and uses a dedicated schema owner. Exposed-schema functions and direct role membership must still be inventoried. No generic SQL, set_config, privileged role switch or exposed SECURITY DEFINER is permitted. Private training/auth schemas stay outside the Data API.

## Guarantee and limits

**Locally proven:** saved retired runtime password fails native login; a separate native LOGIN attacker with no data memberships can set arbitrary claim variables but still cannot access training/RPCs or become authenticated/owner/gateway. Forged B claims with A token are rejected by real PostgREST; even a forged internal Principal B carrying token A yields A data only. This closes the original tokenless DB-credential/GUC impersonation path under the tested permissions.

**Not a universal backend-compromise guarantee:** a compromised host can steal/replay valid B bearers it receives, alter client responses and misuse that bearer until expiry/revocation. Local tests demonstrate valid B replay. Compromise of provider authenticator/admin/signing authority defeats the trust boundary. These authorities must be outside the app service filesystem/environment/network authority.

The current operator OS account can read saved administration credentials. The real project startup guard refuses this layout, but a code-compromised process could bypass a guard. Actual OS/container filesystem isolation, not this guard, is required. No system account/container was provisioned in this stage; deployment remains BLOCKED. Do not mount project/database, operator home or Codex private labs into the service. Supply backend code/public configuration only, no DATABASE_URL, gateway/admin secrets or issuer keys. Validate independently that service identity cannot read those files.

Hosted gateway version, exposed schemas, callbacks, live user/RLS/email flows and provider full-path TLS are unverified. Local PostgREST16.4/native PostgreSQL17.11 evidence is not hosted compatibility evidence. Data API HTTPS uses ordinary Web CA chain/hostname validation, not the prior legacy DB CA exception. That exception stays an admin-only historical issue; direct DB TLS settings are rejected by the new user API. No end-to-end hosted TLS claim.
