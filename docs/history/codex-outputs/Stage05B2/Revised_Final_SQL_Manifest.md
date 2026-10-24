# Stage05B.2 — Revised final SQL manifest

2026-10-09. **Review candidate only. Remote execution NOT authorized and remains BLOCKED.** Target for future separately approved non-production work: astrogateway-test / `jvkwofmxtxjhdwtwdboz`. No remote writes were executed.

## Exact intended order and SHA256

| Order | File | SHA256 | Status |
|---|---|---|---|
| 0 | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/SQL/000_read_only_preflight.sql` | `10a0b57a34de6491bbd8367d86545234310c45ebfa8aa3f5ce08db27aa198eea` | Read-only candidate |
| 1 | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/SQL/001_verified_jwt_training.sql` | `f88737a63921642a6646c6883906f820491a7b422a747863aa4b1681f656bd3b` | Writes; rehearsed locally only |
| 2 | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/SQL/002_retire_direct_runtime.sql` | `9e76d544a44a0815ea4d0263d15be381bba646aa1d68e1cf38dbaf26a19fc22f` | Writes; rehearsed locally only |
| 3 | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/SQL/003_read_only_verification.sql` | `ee46f60ae220ffa929214e9725020ca115905be0549ff2e2894ea7872839138d` | Read-only candidate |

000 was prepared as a read-only future target preflight, not run remotely; native fixtures exercised equivalent dependency/collision/role checks.001 final bytes were freshly reinstalled in native17.11 and regressed. It requires Auth id/created_at/raw_user_meta_data/email_confirmed_at/is_anonymous and auth.uid/auth.jwt, safe NOLOGIN authenticated with no unexpected parents, absent training/mission_api/schema_owner collisions, compatible trigger/backfill data.002 was exercised/repeated; retires existing exact legacy roles if present, revokes memberships/table AND column/function/schema rights and terminates exact retired-role login sessions. Termination is a non-transactional operational effect, even though DDL is transactional: obtain explicit approval/traffic pause and preflight termination rights. It never targets provider/admin sessions.003 is read-only post-install catalogue review, not automatic live acceptance.

Machine hashes: `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/revised-sql-manifest.json`. These bytes supersede the previous review bundles only as candidates. Do not combine old 05B/05B1 policy/runtime provisioning/PUBLIC-default SQL with this redesign. Original project migration was not overwritten. No application runtime login/password is required by the new architecture. Quarantine existing roles only; no role/password creation for FastAPI.

## Data API configuration and service boundaries

After separately approved SQL: expose mission_api as the new application API surface; leave training/auth private. Audit all other existing exposed-schema routines for arbitrary SQL/session setters, privileged role switches or unsafe definers before leaving them enabled. Do not remove unrelated provider schemas/settings blindly. Remote configuration changes/cache refresh need explicit approval too. No exposed SECURITY DEFINER is created; only five invoker RPCs with fixed empty search_path and typed mission arguments.

Only public URL/key/same-project issuer in isolated FastAPI env; DATABASE_URL/DB certificate empty and legacy DB flag false. Keep administration, gateway authenticator credentials and signing authority outside service-readable filesystem/env. Known startup file guard does not provide OS containment. Keep provider gateway signature/issuer/audience/expiry configuration verified. Admin backup/execution credentials remain operator-only; never borrow them as runtime, embed in Android or send to Unity.

## Approval gates — no execution request is actionable yet

1. Resolve current same-OS-account admin-file exposure with real isolated service authority; record inaccessible admin/gateway/signing files and no privileged network credentials. Valid user bearer theft remains a bounded residual; if all bearer misuse under host compromise must be prevented, this design is insufficient and remains blocked for that stronger requirement.
2. Refresh approved target inventory/read-only preflight, role/membership/ownership/default ACLs, Auth helper/column semantics and all exposed functions. Verify provider compatibility independently; local PostgREST16.4 is not hosted evidence. Fresh-install collisions require separate reviewed transition, never overwrite/drop live data.
3. Obtain private tested backup/recovery and target-specific rollback manifest/traffic pause. Do not wrap transactional001 in a remote BEGIN→ROLLBACK rehearsal: inner COMMIT can commit. Stop on errors, bounded lock/statement timeout, no blind retry after unknown commit.
4. Operator must separately explicitly approve exact revised SQL hashes/order, reference, quarantine/session termination, Data API exposure/cache handling and rollback scope before any remote change. Current authorization covers local work only. Live account creation/email/inbox workflows need separate approval.
5. After approved install, verify real gateway JWT checks, data isolation with approved accounts, native API HTTPS, email/refresh/return/relaunch and sanitized logs. Do not claim these passed now. Hosted full-path TLS remains unestablished.

## Excluded local-only files

`/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/SQL/900_local_fixture_only.sql` creates fixture provider/Auth/attacker roles/tables — NEVER Supabase. `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/SQL/910_local_down_only.sql` removes app objects only in the disposable lab — NOT approved remote rollback. Private native lab passwords/issuer keys are excluded from manifest/deliverables. No remote runner or password-bearing command provided.

Stop condition observed: implementation/local validation completed, local lab stopped, no remote DB writes/configuration/live accounts. Request separate explicit remote authorization only after the stated deployment/preflight/recovery gates are satisfied; this manifest alone grants none.
