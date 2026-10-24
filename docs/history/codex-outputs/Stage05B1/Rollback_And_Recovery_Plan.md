# Stage 05B.1 — Rollback and recovery plan

2026-10-09. **No remote changes occurred; remote rollback is not authorized.** Local recovery rehearsed; production/test-provider recovery readiness remains BLOCKED. Target any future separately approved execution only to `jvkwofmxtxjhdwtwdboz`.

## Local evidence and source recovery

SQL rehearsal applied candidate001→002→003 in a disposable PG17.5 fixture, tested collisions and late trigger/backfill failures, removed application objects using `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/SQL/910_local_down_only.sql`, restored schema privileges from captured schema ACLs and TEMP/function-default privileges from known fixture baseline rules, preserved all fixture Auth rows, then reinstalled successfully. This proves the exercised local order and transaction handling, not provider recovery. Initial and final logs are in `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/Evidence`.

FastAPI prior source copies/hashes: `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/Before` and `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/Evidence/source-before.json`. After stopping the local server, restore only the four prior backend source/template files if an operator chooses code rollback; remove the newly added hardening test only if reverting that stage. Keep existing secrets unchanged. Retest original contracts. Restoring old database.py restores the former weaker TLS/startup checks, so it is recovery to baseline, not a recommended secure deployment. No Android/Unity/Mars/React rollback needed because those sources were untouched.

## Before any future remote write

Require separately approved, private provider snapshot/PITR or database backup with successful restoration evidence and retention. Do not place Auth rows/password hashes, role-password dumps, DSNs or tokens in outputs. Capture exact schema/table/function/trigger/policy ownership and definitions, schema/database/table/column/routine grants with grantors/options, role attributes/memberships, owner-specific global/schema default ACLs, collision state and data counts. Use least-data access; all current approval is local, not remote backup administration.

002 converts PUBLIC grants into named-role grants and changes owner-global function execution defaults. Record **full prior and resulting ACL/default state**, including preexisting named grants; otherwise one cannot distinguish existing grants from added grants on rollback. Role additions after the snapshot and changes by other operators are conflicts requiring fresh review. A simple GRANT PUBLIC or blanket REVOKE is not an exact baseline restoration. Native provider recovery must compare effective access for Auth/PostgREST/storage/realtime/maintenance users and application roles.

## Failure handling

- **Before COMMIT:** stop on first error, explicitly ROLLBACK. A transaction can leave a session aborted until rolled back. Local early-role and late-trigger/backfill failure tests left no partial training schema/API role. Do not continue selected statements or hide collisions.
- **Unknown commit/network loss:** stop API/signup writes; reconnect only with verified admin TLS under read-only inspection; compare object hashes/catalogue/data and the execution journal. Do not rerun CREATE scripts blindly. Request approval for a diagnosed recovery action.
- **001 committed, later stage failed:** do not place postgres credentials in runtime or skip its isolation guard. Keep app runtime disabled; retain data for diagnosis; choose reviewed roll-forward or precise rollback after separate approval.
- **Trigger failure affects signup:** pause signup and review disabling/removing only `astrogateway_auth_user_profile` under approved rollback. Existing Auth users must not be deleted as cleanup. Restoration/backfill requires separately approved reviewed SQL and data checks.

## Post-commit remote rollback — procedure to finalize, not executable SQL

After separate explicit approval: stop runtime connections and signup/API writes; snapshot resulting data privately. Disable only the dedicated runtime login and terminate only its approved sessions. Remove the specific newly installed Auth trigger first; preserve Auth users/provider triggers. Remove mission/policy/helper/profile objects in known dependency order only when retention decision and exact ownership/version match. Revoke dedicated membership/settings/CONNECT and drop dedicated roles only once dependencies are removed. Check unexpected dependencies and stop; do not DROP SCHEMA CASCADE, DROP OWNED or revoke provider roles indiscriminately.

Restore ACL/default/membership baselines from the target-specific pre-change snapshot. Preserve preexisting named grants; restore default ACLs for the exact owner/schema/object type. Review unrelated objects created after002 because global defaults affected them: restoring defaults alone does not fix their already materialized ACLs. Compare grants with saved baseline, inspect effective privileges and test provider services. Auth rows are preserved; any account/data deletion needs separate explicit approval.

If exact restoration is unavailable, request approved provider snapshot restore or recreate **only** the designated disposable/non-production target after retention review. Neither operation is authorized by this plan. Never touch production.

## Acceptance and stop gates

Verify schema/role/trigger/ACL/default definitions match the chosen baseline; Auth/profile insert functionality and A/B isolation in the restored/repaired design; target identity, certificate/hostname verification and role separation; no new system/API permission escalation. Restore runtime traffic only after successful tests and operator approval. Publish sanitized counts/results, never credentials/backup contents. Full-path TLS remains unestablished unless independently verified. No actual remote recovery test or approved target-specific down SQL exists yet; these remain blockers.
