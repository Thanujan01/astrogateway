# Test-project credential configuration

[README](../README.md) · [Setup](SETUP_GUIDE.md) · [Sentinel validation](SENTINEL_DATABASE_VALIDATION.md) · [Team plan](TEAM_PLAN.md)

## Scope and measured status

Target selected by Kapilash: **the existing Supabase test project**, not production. The current repository configuration names project reference `jvkwofmxtxjhdwtwdboz`. Three local application configuration files were created in the migration working checkout using an existing modern publishable key; the original project folders were preserved. Key/password values are not included in this guide, result logs or commits.

| Check | Actual result |
|---|---|
| Backend, web and Android local files | Created, regular files, owner-only POSIX mode `600`, ignored by Git |
| Offline project/key/issuer/configuration checks | **13 PASS** |
| Synthetic configuration-boundary scenarios | **13 PASS** |
| Test project public endpoint | **UNVERIFIED: DNS_LOOKUP_FAILED** for the configured project hostname; control lookups for Supabase's main site and GitHub resolved |
| Dashboard project status | Sign-in required in the browser session available during the check; no active/paused/deleted status inferred from DNS alone |
| Administrative credential rotation/storage | Not performed; no administrator DSN/password or secret/service-role key was read or copied |
| Service/admin isolation | **NOT VERIFIED**; owner-only files do not isolate two processes running as the same OS user |
| Database readiness | Separate local SQL rehearsal remains **35 PASS / 0 FAIL**; candidate SQL fixes were retested locally; hosted integration remains unverified |

The local files are intentionally absent from GitHub and fresh clones. A matching format/project reference does not prove a key is accepted by Supabase. Do not describe the deployment as ready until provider access, isolated runtime configuration and the remaining database checks are verified.

## Credential classes and application boundaries

Supabase distinguishes app-identifying publishable keys from privileged secret keys; signed-in user identity is a separate Auth layer. See [official API-key documentation](https://supabase.com/docs/guides/getting-started/api-keys). This project's user-facing FastAPI adapter deliberately forwards the original user bearer with public app configuration, preserving the intended gateway/RLS boundary; it is not a generic privileged server-key design.

| Configuration class | Appropriate use in this repository |
|---|---|
| Project URL / reference | Approved test origin and identifier, consistent across components. |
| `sb_publishable_…` | Public app configuration in backend, browser and Android. For this test setup one existing key is aligned across the three files; shared-key equality is a project convention, not a general Supabase requirement. |
| User access/refresh session | Managed by the existing Auth clients/session-store logic; never used as a static shared team credential. |
| Admin database password / privileged DSN | Operator-only administration outside the application's runtime filesystem and environment; not a runtime setting for this API. |
| `sb_secret_…`, legacy `service_role`, issuer signing material | Not permitted in this repository's user-facing API, web build, Android APK, Unity descriptors or shared configuration. |

An API key identifies an application, not a person. Each team member should use their own authorized account where needed, rather than sharing the database administrator's identity. No team account or provider permission was granted by this configuration task.

## Local files and expected fields

| Component | Local file | Safe configuration fields |
|---|---|---|
| Backend | `02-Software-Development/Backend/astrogateway-api/.env` | `ASTROGATEWAY_ENVIRONMENT=test`, approved project ref, Supabase URL/publishable key, matching issuer, approved CORS origins. `DATABASE_URL` and DB CA path empty; legacy CA flag false. |
| Web | `02-Software-Development/Web-Dashboards/astrogateway-web/.env.local` | `VITE_SUPABASE_URL`, `VITE_SUPABASE_PUBLISHABLE_KEY`, `VITE_AUTH_API_BASE=/account-api`. `VITE_*` is public build configuration. |
| Android | `02-Software-Development/Integrations/android-unity-host-poc/auth.local.properties` | `supabase.url`, `supabase.publishableKey`, `auth.backendUrl`. Backend URL must be HTTPS, or the permitted debug emulator endpoint. |

The Android checker permits `http://10.0.2.2:8000` only as a local emulator profile, matching the tracked Gradle debug allowance. It is not a deployed HTTPS endpoint or a physical-device connectivity test. The actual URL was preserved from the existing native test configuration when available.

Tracked templates: [backend](../02-Software-Development/Backend/astrogateway-api/.env.example), [web](../02-Software-Development/Web-Dashboards/astrogateway-web/.env.example), [Android](../02-Software-Development/Integrations/android-unity-host-poc/auth.local.properties.example), [administrative reference](../02-Software-Development/Sentinel-Database/admin.env.example). The admin template is reference material; keep actual operator credentials outside the application checkout/runtime. Additional ignore rules guard against accidentally staging administrative local files and environment variants.

## Reproduce the checks

From the repository root:

```bash
# Tested: read-only audit of existing local files. No values are printed.
python3 02-Software-Development/Sentinel-Database/validation/check_credentials.py

# Tested: synthetic positive/negative scenarios, no real config or network used.
python3 02-Software-Development/Sentinel-Database/validation/test_credentials.py

# Attempted: public Auth settings reachability and invalid-key control.
# Current result is UNVERIFIED because the project hostname does not resolve.
python3 02-Software-Development/Sentinel-Database/validation/check_credentials.py --check-provider
```

The checker reads only the three app files above, does not read administrative credentials and never prints key values. It checks owner/mode, allowlisted fields, explicit test target, aligned modern public keys, issuer, retired DB options and client API origins. These are bounded configuration checks, not an exhaustive secret scanner or OS security certification.

Optional `--report PATH` writes sanitized check results. Exit **0** means the requested checks passed; **1** means offline configuration failed; **2** means provider verification was requested but could not be established. A DNS/TLS/network failure is not reported as an invalid password or accepted key. The read-only provider probe compares the configured key with one invalid-key control; it creates no account, SQL, password change or data write.

Synthetic scenarios cover valid input, secret-key rejection, mismatched client key/project/issuer, retired DB credentials, production marker, insecure file mode, duplicate/unknown fields, symlink, unsupported Android HTTP URL and incomplete key configuration. Their key-like strings are labelled synthetic fixtures, not credentials.

## Complete live verification

1. Sign in to [Supabase Dashboard for the configured test project](https://supabase.com/dashboard/project/jvkwofmxtxjhdwtwdboz) using your own account. If signed in elsewhere, the browser session used for inspection must also reach the project page.
2. Confirm the actual project reference, Project URL and operational status in the dashboard. A DNS failure alone cannot establish whether a project is paused, removed or using a different reference. Do not generate replacement keys or invent a password to work around an unknown target.
3. Confirm the existing publishable key belongs to the approved target. If a replacement/rotation is needed, the owner should perform that explicit credential action and update local files privately; never paste it into chat, issues, PPTs or commit messages.
4. Re-run the provider check. Only successful configured-key access plus the rejected invalid-key control counts as that limited public-endpoint check passing.
5. Run the user-facing service with a genuinely isolated identity/filesystem that cannot read operator/gateway/signing credentials. The current known-path guard and mode `600` alone do not prove this boundary.
6. Review the [local SQL fixes and passing results](SENTINEL_DATABASE_VALIDATION.md), then perform separately scoped login, refresh, callback and game/client integration checks.

Database connection/admin credentials are a separate operator workflow; consult [official database connection documentation](https://supabase.com/docs/guides/database/connecting-to-postgres) for the chosen target and connection method. This application does not gain a raw PostgreSQL password by completing public app configuration. No hosted settings, accounts, roles, keys or passwords were changed in this task.
