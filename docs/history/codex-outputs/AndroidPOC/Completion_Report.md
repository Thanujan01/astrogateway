# AstroGateway Android integration POC 01 — build complete, runtime blocked

8 October 2026. This is an isolated integration POC, not the production dashboard or authentication stage.

## Completion criteria

| # | Criterion | Status / evidence |
|---|---|---|
| 1 | Android host created | DONE — Kotlin dashboard and private Unity Activity source created |
| 2 | Unity Android export completed | DONE — Unity 6000.6.4f1 reports `ASTROGATEWAY_EXPORT_RESULT Succeeded` |
| 3 | Gradle integration compiled | DONE — Kotlin compilation and Unity IL2CPP library integrated; final `BUILD SUCCESSFUL` |
| 4 | Debug APK generated | DONE — signed debug APK, 36,835,048 bytes (35.13 MiB) |
| 5 | App launched successfully | NOT TESTED — no device detected; no emulator installed |
| 6 | Unity opened from host | NOT TESTED |
| 7 | Native-to-Unity message received | NOT TESTED — protocol implemented, not runtime verified |
| 8 | Unity-to-native message received | NOT TESTED — protocol implemented, not runtime verified |
| 9 | Dashboard return successful | NOT TESTED |
| 10 | Unity relaunch successful | NOT TESTED |

**The integration is proven at export/build/package level only. The complete runtime POC has not passed.** No APK was installed and no physical-device or emulator test occurred. No runtime screenshots are available.

## Exact working paths

- Android host: `/Users/kapilash/Projects/AstroGatewayAndroid`
- Dashboard: `/Users/kapilash/Projects/AstroGatewayAndroid/app/src/main/java/com/sk2lb/astrogateway/poc/MainActivity.kt`
- Unity Activity: `/Users/kapilash/Projects/AstroGatewayAndroid/app/src/main/java/com/sk2lb/astrogateway/poc/PocUnityActivity.kt`
- Host manifest/build: `app/src/main/AndroidManifest.xml`, `app/build.gradle` under the host root.
- Isolated copy of existing Unity project: `/Users/kapilash/Projects/AstroGatewayAndroid/UnityIntegrationProject`
- Temporary scene: `/Users/kapilash/Projects/AstroGatewayAndroid/UnityIntegrationProject/Assets/AstroGateway/IntegrationPOC/UnityIntegration_Test.unity`
- C# bridge: `/Users/kapilash/Projects/AstroGatewayAndroid/UnityIntegrationProject/Assets/AstroGateway/IntegrationPOC/AstroGatewayPocBridge.cs`
- Scene/export automation: `/Users/kapilash/Projects/AstroGatewayAndroid/UnityIntegrationProject/Assets/AstroGateway/IntegrationPOC/Editor/AndroidPocExporter.cs`
- Unity Gradle export/build root: `/Users/kapilash/Projects/AstroGatewayAndroid/unity-export`
- Debug APK: `/Users/kapilash/Projects/AstroGatewayAndroid/app/build/outputs/apk/debug/app-debug.apk`
- Build tools: `/Users/kapilash/Projects/AstroGatewayAndroid/tools/export.sh`, `build.sh`, `prepare_host.py`, `device_smoke.py`
- Original logs: `/Users/kapilash/Projects/AstroGatewayAndroid/logs/`

The host app source stays outside the generated export. Export settings map `:app` to `../app`. Only the temporary camera/light/cube/bridge scene is exported; the copied approved Mars scenes remain untouched and are not included as playable scenes in this APK.

## Toolchain verification

Android Build Support, SDK, NDK and OpenJDK were installed through Unity CLI for the exact Editor, following explicit permission to accept Google's SDK/NDK terms and install under `/Applications/Unity`.

| Component | Verified status |
|---|---|
| Unity | 6000.6.4f1, Activity entry |
| URP | Existing project 17.6.0, used in isolated copy |
| Android module root | `/Applications/Unity/Hub/Editor/6000.6.4f1/PlaybackEngines/AndroidPlayer` |
| SDK | Matching Unity module; API 36 used for compile/target, min API 26 |
| NDK | r27c, 27.2.12479018; compiler executed successfully |
| OpenJDK used | Bundled Temurin 17.0.18+8; separate preexisting 17.0.20.1 also found |
| Gradle / AGP | Actual generated export 9.3.1 / 9.1.1; final build succeeded |
| Kotlin | AGP built-in Kotlin enabled; native classes confirmed in DEX |
| ADB | Platform Tools 37.0.1; device list empty |
| Android Studio | Not found under `/Applications`; not installed because supported CLI export/build did not require it |
| Emulator | Not installed or tested; user selected physical-device validation |

## Implemented test protocol and lifecycle

The dashboard launches a private Activity in a separate `:unity` process with a fresh launch UUID and test mission identifier. The C# receiver announces readiness, native code calls static `UnityPlayer.UnitySendMessage`, C# sends ACK and RESULT through native Activity callbacks, and the host displays the bounded test result. Launch IDs reject stale exchanges. This protocol contains no passwords, refresh tokens, service-role credentials or account system.

A package-restricted, non-exported receiver handles live evidence. Result data also accompanies the explicit dashboard-return Intent because Android can defer broadcasts to a cached host process. These debug results are not authenticated mission scores.

Return reorders the existing dashboard to the front without finishing/unloading Unity. Relaunch reuses the Activity and sends a new descriptor. Unity's inherited pause/resume callbacks remain in place. Modern Android Back and legacy Back both route to dashboard. Unload/quit callbacks have explicit return handling. The separate process protects the host from Unity's process-exit behavior, but runtime confirmation remains pending. Returning retains Unity memory; no complete memory-release claim is made.

The installed Unity 6000.6.4f1 Java source and class signatures were checked. Kotlin must call the static `UnityPlayer.UnitySendMessage` API, rather than Java's tolerated instance-style call in the sample. Unity export disables built-in Kotlin by default; the reproducible preparation step enables it through AGP's supported configuration. Manifest Back settings and native-library packaging were aligned before the final successful build.

## Package evidence

[APK inspection](APK_Inspection.json) confirms the dashboard and Unity Activity classes in DEX, Unity data and ARM64 `libunity`, `libil2cpp`, `libmain` and Burst libraries. The final manifest has exactly one launcher: `MainActivity`; `PocUnityActivity` is non-exported in `:unity`. APK signature verification passed using v2 signing. SHA-256: `37c9de93f904b812b5e737396bd55fb641f07abd7f4e12a00d99b6437337d269`.

The APK is debug-only, ARM64, Android 8/API 26 or newer, with the graphics requirements inherited from this Unity project. There is no production signing, store submission or performance certification. Nonblocking native stripping/Gradle deprecation warnings remain in build evidence; no final compilation errors remain.

## Connection blocker and next action

ADB was checked repeatedly after the user confirmed connection/debugging authorization. Restarting ADB and trying both native and libusb backends still yielded no devices. macOS's current USB host inventory showed USB buses without a phone. This establishes that the toolchain cannot see a usable device connection; it does not prove which cable/phone/permission is at fault.

Check a data-capable cable and another port, keep the phone unlocked, accept any macOS accessory permission, and verify the phone's USB-debugging authorization. Once exactly one authorized device appears in ADB, the saved `python3 tools/device_smoke.py` tool can install the APK and record ten launch/message/return cycles with screenshots. A wireless-debugging or emulator alternative requires a separate setup decision; neither was attempted without authorization.

## Preservation and scope

[Preservation check](Preservation_Check.json): 337 existing source/configuration/design files checked, zero changed or missing. No original Unity/React source, approved scene or design document was edited. No astronaut gameplay, production environment art, Auth, Supabase, FastAPI or commercial assets were added.

## Deliverables and logs

- [Installable debug APK](AstroGateway-POC-debug.apk)
- [POC source archive](POC_Source.zip) — host, bridge, temporary scene and reproduction tools; the existing project copy remains at the working path.
- [Final Gradle build log](Logs/gradle-build.log)
- [Unity export log](Logs/unity-export.log) — machine/licensing identifier lines redacted in this output copy.
- [Unity import/compile log](Logs/unity-import.log)
- [Module installation log](Logs/android-module-install.log)
- [Final APK manifest](Logs/apk-final-manifest.xml)
- [APK signature verification](Logs/apk-signature.log)
- [ADB device list](Logs/adb-devices.log)

## Official references

[Unity's Android integration example](https://github.com/Unity-Technologies/uaal-example/blob/master/docs/android.md) informed Activity-based integration and the separate process. [Unity 6000.6 Activity extension documentation](https://docs.unity3d.com/6000.6/Documentation/Manual/AndroidUnityPlayerActivity.html) and the actual installed source guided class/API selection. [Android's built-in Kotlin migration documentation](https://developer.android.com/build/migrate-to-built-in-kotlin) supports enabling Kotlin in AGP 9 without the legacy Kotlin Android plugin.

Stop at this POC. Authentication and the production dashboard require further approval. Runtime validation can resume when a usable authorized device is available.
