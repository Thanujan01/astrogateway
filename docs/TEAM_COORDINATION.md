# Team coordination and real-time co-op

[README](../README.md) · [Identity](ACCOUNT_IDENTITY.md) · [Database validation](SENTINEL_DATABASE_VALIDATION.md)

## User-selected product scope

- Same canonical Supabase Auth UUID across software, Android and Unity; no second team/game account or shared password.
- Teams of **2–4 accepted players**, including the creator. Sending an invitation does not grant membership.
- Suggestions use **existing scoring rules**, not a newly invented rubric.
- Accepted teammates collaborate in the **same Unity world**, with shared mission objectives. A chat room alone does not satisfy this requirement.
- Member-only text chat, voice, screenshot sharing and user-initiated screen sharing.

## What exists in this change

The new [rules/peers.py](../02-Software-Development/Backend/astrogateway-api/rules/peers.py) compares players through the unchanged `SkillAssessor` and `mission_rules_v1.json`. Its inputs must be server-sourced completed mission reports with rules-version compatibility, not a public client score upload. It deduplicates session evidence, rejects conflicting/invalid reports, excludes self/opted-out/blocked candidates and excludes comparisons with no shared assessed skills.

Only the existing five skills are compared: observation, life_support, weather_safety, rover_operation and mission_planning. `similarity = 1 - mean(abs(level_a - level_b)) / max_rubric_level` over shared assessed skills. This is a matching/ranking policy, not a new mission score or new proficiency rubric. Coverage ranks before similarity to avoid recommending a one-skill-only profile above a fully assessed equivalent. The output lists omitted skills and level differences and remains explicitly **provisional**, just like the existing assessor. Equal computed levels do not establish identical actual ability. No email, raw mission history, credentials or tokens are returned.

The module is deliberately **not exposed as a public API** before trusted score persistence and discovery authorization exist. Callers must obtain discovery consent and a union of both directions' blocks from the database. A Python object itself does not authenticate a report. Tests use synthetic reports produced by the current scorer.

| Capability | Actual implementation status |
|---|---|
| Existing rubric-based peer comparison | Implemented as offline-tested server module; no live endpoint |
| Live player skill synchronization / score persistence | Pending; current `/v1/skills` still returns counts, `levels=null`, rubric pending |
| Player discovery cards, invitations and atomic team membership | Specified below; not implemented/deployed |
| Same-world Unity multiplayer | Not implemented; current Unity package manifest contains no NGO/Mirror/Photon networking |
| Team chat and screenshot storage | Not implemented |
| Voice/screen sharing | Not implemented; no media server/TURN deployment or credentials provisioned |
| Live account connectivity | Existing approved test hostname DNS failure remains unresolved |

## Required architecture and implementation sequence

### 1. Identity and trusted gameplay evidence

Restore the approved Supabase test target first, verify deployed schema and real cross-client identity. The account UUID stays canonical; display names/usernames can change. Add immutable assessed mission reports indexed by `(user_id, session_id, rules_version)` with server completion timestamps. The scoring worker derives timelines from accepted, ordered server events and the existing mission catalog. Clients never submit final levels, scores or someone else's owner ID. Replaying completion cannot create duplicate skill demonstrations. Existing unassessed legacy sessions remain unassessed until their evidence is actually sufficient; do not convert command counts into fabricated skill levels.

The current rules engine is a candidate and labels its results provisional. Selection of existing rules does not prove its events/difficulty/catalog are already wired to current gameplay. Match schema/version and event semantics before enabling live score generation. Preserve independent gateway verification and user-scoped access; do not put administrative keys into the user API or clients.

### 2. Discoverable player cards and team lifecycle

Store discoverability separately from private profiles, disabled by default. Share only chosen display name/username, compatible aggregate skill summary and presence. Never expose emails or a searchable private account directory. Synchronize each user's own skills regardless of whether they opt into discovery. Suggestions are not an automatic invitation, friendship or membership grant.

Team creator becomes member 1; pending invites consume no slots. Invitation state machine: pending → accepted / declined / cancelled / expired. Only the recipient may accept/decline; sender may cancel. Expiry and membership checks are enforced on the server. The accepting transaction locks the team and verifies capacity (four), block status, current roster, duplicate acceptance and invitation expiry before inserting membership. A fifth concurrent acceptance must fail safely. Crossed invitations and retries must be idempotent. Only accepted members can see room messages, attachments, objectives or obtain game/media tickets.

Lobby state: forming → ready → running → completed / aborted. Start requires 2–4 accepted, connected/ready members with matching game build/scene/protocol versions. The owner initiates start; authoritative server allocates a room and seals roster membership policy for the running mission. Explicit leave/kick/block/logout/session-revocation actions remove future access. Define owner departure and team disband behavior; do not silently appoint an attacker or keep a departed host's room alive without a policy.

### 3. Same-world Unity gameplay

Use a dedicated, server-authoritative Unity game instance per team/session. Evaluate Unity Netcode for GameObjects + Unity Transport with the installed Unity 6000.6 editor; compile and run compatibility tests before pinning packages. Unity documents NGO's server/client approval flow and [Relay integration](https://docs.unity.com/en-us/mps-sdk/tutorials/relay-and-ngo); a Relay join code alone is not accepted-team authorization.

The software control plane issues short-lived, single-use join tickets bound to canonical user UUID, team, server, build version and accepted roster. Consume tickets atomically during connection approval; enforce four-player capacity on the game server too. Never send a persistent Supabase bearer/refresh token in unauthenticated plaintext connection data. Internet deployment needs a reviewed encrypted transport/relay, certificate handling and a provisioned game-server host. Player-hosted execution would change the trust/failure model and is not silently treated as trustworthy scoring.

Spawn a distinct network player/avatar for each accepted UUID in the existing Missions environment. Server owns authoritative movement validation, interactions, shared rover/equipment, objectives, survival/resource state and score-relevant events. Clients submit semantic actions/input rather than outcome/resource totals. Separate camera, joystick, audio listener and HUD for the local player; remote avatars must not create extra local input/audio components. Shared objective completion happens once, while individual evidence credits participants by defined existing-rule semantics. Late join, packet loss, reconnect, duplicate input, scene loading and rejected/kicked clients need explicit tests. Current single-player controllers cannot become multiplayer merely by copying transforms or adding a lobby button.

### 4. Chat, screenshots and media

Persist bounded, plain-text chat under member-only authorization, with server timestamps, message idempotency, pagination, rate limits and explicit retention. Realtime subscriptions recheck member scope; guessed team IDs must not allow reads or writes. Leave/kick/block should close subscriptions as well as reject future sends.

Room screenshots belong in a separate **private** attachment bucket, not the profile-photo bucket. Validate image bytes, MIME/size, path/team ownership and active membership for upload/read/delete. Share short signed links only with the accepted room; prior links may remain valid until expiry, which must be documented and bounded. Do not automatically upload local screenshots or capture another application.

Voice and screen sharing use WebRTC with a deployed SFU/TURN service and room-specific grants. Microphone/screen capture starts only after the user's explicit UI action and platform chooser/permission, with visible active indicators and stop/mute controls. Joining a team does not start recording, microphone or screen capture. Recording is outside this request. On leave/kick/logout, stop capture/tracks, close peers/subscriptions and revoke SFU room membership; token expiry alone is insufficient. Web screen sharing and native Android screen capture have different platform flows and must be implemented/tested separately. Do not put TURN permanent secrets or media server API secrets into web/Android/Unity builds.

## Acceptance evidence before declaring the feature complete

1. Two real accounts on separate clients show the same canonical IDs as their existing profiles and server-assessed skills derived from actual mission evidence.
2. No-evidence users are labelled unassessed; compatible scored users see explained similar-skill suggestions; private/blocked users are excluded.
3. Recipient acceptance creates membership once; concurrent attempts cannot exceed four. Nonmembers, declined/expired invites and revoked sessions cannot get room/game/media access.
4. Two separate Unity clients join the **same Missions instance**, see distinct avatars and share objective/resource changes. Four clients work; fifth denied. Server rejects client-forged outcomes and duplicate completions.
5. Authenticated teammates can exchange text/screenshot and voice/screen media; nonmembers cannot subscribe/read/join. Leave/kick/block/log out actually terminates live access and capture.
6. Android/Web/Unity lifecycle, reconnect, network loss, game-server restart, media provider failure, transport encryption and restore/retention behavior pass on supported devices.

This change adds the matcher foundation and this concrete implementation contract. It does not claim the above multi-service feature has shipped. Required external dependencies are a reachable approved Auth/database target, game-server hosting/encrypted networking, and media/SFU/TURN deployment. No paid services, cloud permissions or remote schema changes were made.
