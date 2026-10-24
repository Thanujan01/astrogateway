# Security tests

| Suite | Result | Evidence |
|---|---|---|
| Backend mock contracts/regressions | 68 PASS | Evidence/backend-tests.txt |
| Same backend suite under kernel confinement | 68 PASS (same tests, not an additional unique count) | Evidence/isolated-unit_probe.py.log |
| Actual confined-process boundaries | 18 PASS | Evidence/isolated-probe.py.log |
| PostgreSQL + independent gateway profile rehearsal | 30 PASS | Evidence/local-profile-rehearsal.json |
| Confined FastAPI → real local PostgREST | 21 PASS | Evidence/isolated-api-integration.json |
| Web simulated account contracts/UI | 34 PASS | Evidence/web-tests.txt |
| Android synthetic UI checks | 6 PASS on corrected run | Evidence/android-profile-ui-test-retry.txt |
| Hosted cross-platform authentication/RLS | BLOCKED, not run | No hosted users/migrations/allowlist evidence |

Tests use disposable synthetic identities/data and an RSA fixture issuer. Real local PostgREST verifies JWT signatures; this is stronger than mocked RLS but is not Supabase-hosted evidence. The backend Auth transport is a local fixture, not a replacement production identity system.

Own-row reads/updates, direct attempts to update another user, immutable UUID/training level, unconfirmed/expired sessions, user-ID injection, forged/malformed tokens, unsupported RPCs and retired DB credentials were exercised. Arbitrary PostgreSQL claim settings by an untrusted role confer no rights. An explicitly compromised host verifier could not forge another subject at the independent gateway. Replay of a genuine B bearer remains possible and is documented.

The confined process cannot read administrator configuration, local signing private keys or gateway DB configuration; inherited secret environment, child-process escape, parent inspection, trusted-code writes, administrative network access and privileged operations are denied. The launched service uses only a cleared environment plus public test settings. Its localhost-only egress is intentionally retained; hosted provider traffic is currently blocked.

The disposable lab CA/leaf had expired. NEW seven-day localhost certificates were issued under work/unifiedauth/tls, with strict chain/hostname validation retained; remote Supabase certificates and compatibility settings were untouched. No full-path hosted TLS claim is made. No TLS bypass or wildcard network exception was introduced.

No DB credential, service-role key or private signing key enters web/Android/runtime bundles. Web `.env.local` is chmod 600 and ignored; private lab key/config files are protected/ignored. Reports/manifests omit credential values. Supabase browser session storage is vulnerable to XSS/extension compromise; production CSP/hosting and real supported service isolation remain deployment gates.

Malformed profile JSON is converted to a generic error; full profile field types/ranges are checked before rendering.

Known test/runtime incidents: initial test-runner ANR; corrected runner completed successfully. Starlette/httpx test-client deprecation and Gradle 10/Jetifier warnings remain. Web lint is clean. Initial local expired certificate, localhost inet-format assertion, inactive gateway and interpreter-symlink launcher issues were corrected without relaxing verification. No incident is represented as a successful hosted test.
