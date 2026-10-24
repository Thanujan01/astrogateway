# Stage 05B.1 — Final review SQL manifest

2026-10-09. **Final artifact list for this preparation stage; NOT approved/cleared for remote execution.** Identity-compromise protection FAILS; provider-wide ACL/default compatibility, native runtime connection and recovery prerequisites remain BLOCKED. “Final” pins review bytes, not permission or a safety certification.

Approved future target only: `astrogateway-test`, ref `jvkwofmxtxjhdwtwdboz`. Administrative postgres credentials are server/operator-only; restricted runtime must never borrow them. SQL does not independently prove project reference (many projects use database postgres): any future runner must verify approved endpoint/ref/TLS identity before executing. No remote runner was started.

## Intended candidate order and SHA256

| Order | Exact file | SHA256 | Status |
|---|---|---|---|
| 0 | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/SQL/000_read_only_preflight.sql` | `a170c66c35b965e52138312ac22f16e43a7e941a5e5a175ed97c6cf97d159ef3` | Read-only |
| 1 | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/SQL/001_identity_and_mission_sessions.sql` | `fc95c36e2505b681e308d4ab310dc8cc932ad354daa44231f9eb7608f94825f7` | Writes; local rehearsal only |
| 2 | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/SQL/002_public_acl_isolation.sql` | `9e7502d7bb67972d4099f93e8bf26be8bbfef2987f577cf67a5763ca0fdc07e0` | Writes; local rehearsal only |
| 3 | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/SQL/003_runtime_role.sql` | `364d991fa3bb10f34876dd2ca0ad89cb4724cb551e41f9f4baec296b01db1c51` | Writes; local rehearsal only |
| 4 | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/SQL/004_post_migration_metadata.sql` | `ba8319b0e7f00978ba997a27642405f22b1abd8b59f3e10b0f051ac80f7ff713` | Read-only |

Machine-readable hashes: `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/Evidence/sql-manifest.json`. Both the original migration and project copy remain unchanged. Candidate001 differs only by explicit NOREPLICATION; it preserves the baseline policies and therefore the documented database impersonation defect. A design that resolves the defect will require new SQL/test evidence/hashes and a **new review manifest**, not silent replacement.

000 is a metadata preflight, not a backup/effective-privilege proof. Refresh inventory of every schema/default grant/grantor/provider role/trigger/object and null Auth timestamps; validate collisions before001.001 is transactional CREATE, no overwrite/IF NOT EXISTS; profile trigger/backfill could abort signup if incompatible.002 is transactional PUBLIC isolation and owner-global function defaults; inspect its affected roles/schemas/defaults first.003 rejects remaining unrelated PUBLIC schema usage and PUBLIC database TEMP; creates runtime with no password/admin rights; explicit SET-only API membership.004 is read-only metadata inspection; catalogue output still needs assertion review and real user/native connection tests.

**Never run 001/002/003 merely because local tests passed.** Execute neither live signup nor migrations concurrently. Never wrap001 inside a BEGIN→ROLLBACK remote “dry run”: it contains COMMIT. Establish bounded locks/statement timeouts outside scripts, stop on any error, no unattended retry after unknown commit.

## Explicit exclusions

`/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/SQL/900_local_fixture_only.sql` creates fake provider/Auth objects and is **local-only, NEVER Supabase**. `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/SQL/910_local_down_only.sql` removes application objects in dependency order only in the disposable fixture, and does not restore real provider ACL/default baselines; **NEVER use it as a remote down migration**. Rehearsal code and fixture users are not deployment assets/accounts.

Original `outputs/Stage05B/database/001_identity_and_mission_sessions.sql` and older runtime drafts are superseded only as *review candidates* by this manifest; do not mix old/new bytes. No copy was applied to the remote database or project migration source.

## Required next gates and approval

1. Resolve database-level impersonation under backend/runtime compromise, then re-run negative tests. It is blocking, not resolved by server JWT validation or an approval to provision a role.
2. Review current provider-specific privilege inventory and named-role/default preservation against all services; isolated provider-stack/native PG17 testing required. Existing local engine17.5 does not certify remote17.11 patch/service behaviour.
3. Approve runtime trust configuration/compliant CA or pinned legacy profile exception; verify runtime TCP hostname/chain, credential separation and pool resets; maintain full-path TLS limitation until separately evidenced.
4. Establish private recoverable backups and a target-specific ACL/default/membership restoration manifest. No remote backup or restoration proof exists in this stage.
5. Obtain separate **explicit operator approval** naming the non-production reference, revised file hashes/order, roles/privilege changes and rollback scope before any remote change. Live account creation/email tests need their own approval/inbox access. Current authorization covers local preparation only.

Remote command examples/password-bearing SQL intentionally omitted. Runtime password must be generated/set through an approved hidden operator channel after separate provisioning approval, stored only in private backend configuration; Android/Unity contain no DB or long-lived credential. No migration command is embedded in FastAPI startup.
