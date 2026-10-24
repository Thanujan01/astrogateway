# AstroGateway unified platform architecture proposal

Team SK2LB-AstroLab · 8 October 2026 · Planning only, awaiting approval.

**Recommendation: a native Kotlin Android host, a bundled Unity as a Library module, one FastAPI service, and one Supabase Auth/PostgreSQL project.** Start with Android Views for the small host interface. Preserve the existing React prototype for later web work. Do not introduce React Native in the seven-day MVP.

This proposal is based on read-only project inspection and official documentation. It is not proof that an integrated Android build works. Android Build Support is currently missing; the first implementation milestone must resolve that dependency and test the exact Unity export on a physical phone.

| Requested deliverable | Document |
|---|---|
| System architecture, Android recommendation, integration comparison | [01_System_And_Platform.md](01_System_And_Platform.md) |
| SSO, secure handoff, lifecycle and navigation | [02_Identity_And_User_Journey.md](02_Identity_And_User_Journey.md) |
| Database proposal, FastAPI contracts, educational assessment | [03_Data_API_And_Learning.md](03_Data_API_And_Learning.md) |
| Packaging, future platforms, folder structure, seven-day roadmap | [04_Delivery_And_Roadmap.md](04_Delivery_And_Roadmap.md) |
| Project findings, references, risks, decisions | [05_Evidence_And_Decisions.md](05_Evidence_And_Decisions.md) |

The MVP is one complete, measurable training mission: prepare for EVA, investigate solar generation and load priorities, record geological evidence, return safely, and receive a saved scientific reasoning report. Oxygen, water recycling, radiation and hydroponics receive short authored learning decisions; full simulations of all systems are outside the seven-day scope.

One account means one Supabase user identity and shared progress across clients. Within Android, the host and Unity share the host's authenticated session. Different devices have their own sessions for that identity; signing in on a phone does not automatically authenticate a browser on another device.

The plan assumes two implementation contributors and part-time scientific/QA review. With one developer, the complete scope is unlikely to fit seven days. Approval should precede any dependency installation, application creation, project edits, cloud configuration or build.

No application code, migrations, dependencies or builds were created in this planning stage. The preservation check records hashes of 334 existing project files.
