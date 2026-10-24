# Player consent and privacy

Status: offline policy prototype, not deployed consent UI or compliance certification. Default collection OFF; model-training consent separate and OFF. Current preparation policy denies collection/training for unknown ages and all minors, even a asserted guardian flag; no real collection occurs.

Proposed explicit notice: exact event categories, learning purpose, storage/retention, withdrawal, account access/deletion, external processors and optional research/model use. Refusal must permit non-personalized practice. Do not bundle telemetry permission with authentication or marketing. Versioned adult opt-in plus revocation, server enforcement and audit receipts required before activation. Determine jurisdiction/age assurance and verified guardian process with legal/privacy review before enabling younger players. Do not collect exact birth date unnecessarily or use self-declared guardian flags as proof.

Minimize to scoped mission/session identifiers, bounded measurements, event offsets, equipment outcomes and validated provenance. Exclude email, name, location, voice, chat, advertising/device IDs, passwords and tokens from offline datasets. Hashing a real player ID is pseudonymization, not anonymization. Reidentification/linkage risk remains. Synthetic IDs here represent no person.

Proposed retention: raw telemetry 14 days, derived progress 90 days, backup expiry within 30 days; these are product-policy proposals, not deployed jobs. Separate consent/operational security records need an approved retention basis. Account export and deletion require verified JWT subject and RLS; never arbitrary target ID. Prototype owner filtering on in-memory rows is not a live endpoint or RLS validation. Deletion must cover raw records, derived features, exports, retrieval indexes, caches and future dataset versions; tombstones prevent restoration from backups. Models require lineage and retraining/unlearning policy; cannot claim a row delete removes learned influence.

Training gate rejects real/client-only data in this milestone. Future authorized real data requires current purpose-specific consent, retention checks, trusted labels, review of subgroup risks and audit trail. Only synthetic rows bypass real-person consent checks.

References: [FTC COPPA guidance](https://www.ftc.gov/business-guidance/resources/complying-coppa-frequently-asked-questions) (updated Rule and applicability require legal review; do not rely on older FAQ alone); [UNICEF AI and children](https://www.unicef.org/innocenti/reports/policy-guidance-ai-children). No legal compliance claim.

Scope: local preparation only, 2026-10-09. No remote database operations, live accounts, player collection or changes to Android, Unity or React. Stage05B original-user-JWT → PostgREST validation → native RLS remains the supported user-data path. No service-role/admin credentials in this package.
