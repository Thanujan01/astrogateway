# AstroGateway Android POC runtime validation

8 October 2026 · Team SK2LB-AstroLab · Existing APK tested without rebuilding.

**The native-button integration flow passed ten complete cycles on an Android emulator. Physical-device validation remains blocked. Android Back navigation is an unresolved lifecycle issue.** This is functional evidence for this exact APK and emulator configuration, not a production-readiness or phone-performance certification.

## Required results

| Requested result | Observed outcome |
|---|---|
| Device model / Android version | Emulated `Google sdk_gphone64_arm64`, Android 16/API 36, ARM64 Google APIs image revision 7; Pixel 5 AVD profile |
| ADB authorization | `emulator-5556` online with status `device`; no physical phone detected |
| APK installation | `Success`; existing APK installed once, then reused; installed SHA-256 matched local APK |
| Host launch | Native dashboard visually verified with title and mission launch button |
| Unity launch | Actual Unity 6000.6.4f1 integration scene rendered; screenshot shows the ochre cube and native mission descriptor |
| Native-to-Unity | Correct mission identifier received for all ten unique launch UUIDs |
| Unity-to-native | ACK and RESULT logged for each launch; dashboard visibly displayed `POC_RESULT_OK` |
| Dashboard return | Native return button succeeded in all ten completed cycles |
| Unity relaunch | Ten warm relaunches completed; same Unity Activity/process reused |
| Ten-cycle result | 10/10 core native-button cycles completed; cycle five continued after failed supplementary Back checks |
| Crashes / ANRs | None observed in captured application logs, queried system errors or app exit history |
| Remaining blockers | Physical USB connection; Android Back key/gesture return; nonfatal startup/renderer messages |

Host PID **4382** and Unity PID **4500** were preserved across all ten cycles. There was no app restart between them. An earlier preflight captured initial Unity creation, startup and first message exchange. The ten reported cycles were subsequent relaunches with fresh UUIDs, not ten cold starts.

## Cycle evidence

Each row has a distinct launch UUID. Pass checks require native launch, C# receipt, native ACK/RESULT callbacks, dashboard-return log and visible dashboard result. Counts are not inferred from repeated log lines.

| Cycle | Launch UUID | Native → Unity | ACK / RESULT | Native-button return | Visible result |
|---|---|---|---|---|---|
| 1 | `e4475cf6-f3c6-4eae-91d6-3f06a108e237` | Pass | Pass | Pass | Pass |
| 2 | `1f109282-220d-4b5a-a3cd-6f805b1a0ede` | Pass | Pass | Pass | Pass |
| 3 | `2ecc0d54-3fc0-499d-b02f-d1cb70ea011e` | Pass | Pass | Pass | Pass |
| 4 | `8cbb393c-341f-40c8-943c-1a1be3d5f2f2` | Pass | Pass | Pass | Pass |
| 5 | `55d51b68-48bf-4f39-8ec4-57efbb323cd5` | Pass | Pass | Pass | Pass |
| 6 | `63e2a00c-61a2-49b4-9517-853a80dd2823` | Pass | Pass | Pass | Pass |
| 7 | `0d976da1-f112-4e6b-9109-00cc12e8b4f9` | Pass | Pass | Pass | Pass |
| 8 | `20123632-537c-44c3-bbb0-91275359b423` | Pass | Pass | Pass | Pass |
| 9 | `0717bba8-f2b7-4481-9797-b1735856a220` | Pass | Pass | Pass | Pass |
| 10 | `5b339811-54fb-4f84-8e65-d3204dd633b6` | Pass | Pass | Pass | Pass |

Cycle five first attempted an injected Back key and an edge-swipe gesture; neither returned to the dashboard. The explicit return button then completed that same launch, after which cycles six through ten continued. The test was interrupted for diagnosis, so this is not a claim of ten uninterrupted cycles on the first attempt. See [machine-readable results](Evidence/runtime-result.json) and [runtime logcat](Evidence/runtime-logcat.log).

## Errors, warnings and lifecycle findings

1. **Back navigation unresolved:** `KEYCODE_BACK` and a scripted left-edge swipe left Unity visible. The native button returned successfully. Root cause has not been established; investigate Unity/native callback routing before claiming a complete Android Back experience. [Failed Back-check snapshot](Evidence/back-failure-attempt.json) and [gesture screen hierarchy](Evidence/back-gesture.xml).
2. **Nonfatal startup exception:** Unity logged `ClassNotFoundException` for `com.google.android.play.core.assetpacks.AssetPackManager`. The bundled scene loaded and all core exchanges succeeded. No Play asset packs were used by this POC; optional asset-delivery initialization/dependency configuration needs review. No dependency was added during validation.
3. **Renderer messages:** URP reported an unsupported grayscale light-cookie format and fallback to RGBA8, plus Gaussian depth-of-field shaders unsupported or stripped. The basic cube rendered correctly. This does not validate those effects or establish that physical hardware will show the same messages. [Startup logcat](Evidence/startup-logcat.log).
4. **Duplicate evidence logs:** Broadcast and dashboard-return callbacks can emit repeated HOST RESULT/RETURN entries for a single UUID. Ten-cycle counting uses unique UUIDs. Production result handling will need idempotency; this POC has no scoring/database writes.
5. **Test-only interruptions:** Android's first-run “Viewing full screen” tutorial hid the native button from the UI hierarchy until dismissed. A helper text check initially missed Android's uppercase button label and was corrected. These did not require application changes or an APK rebuild. The initial screenshots/logs were retained separately.

No Java fatal exception, native fatal signal, app process exit or app ANR was observed during the completed cycle window. This conclusion is bounded to the captured test session. Unload/quit, process-death recovery, background restoration, physical thermal behavior and comprehensive device coverage were not tested. Returning intentionally retains the Unity runtime in memory.

## Environment and setup

ADB is version 37.0.1 at `/Applications/Unity/Hub/Editor/6000.6.4f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb`. Server restart succeeded. The SDK contains the previously installed matching build components. This stage installed only the missing emulator/image, with explicit approval for the terms, and reused existing ADB and OpenJDK.

Google Emulator **37.2.12.0**, API 36 ARM64 image revision 7, Apple Hypervisor acceleration and **SwiftShader software graphics** were used. Configuration: 4 virtual cores, 3072 MB requested RAM, 1080 × 2340 display. Emulator timings include UI automation overhead and do not measure game performance.

- Isolated emulator SDK: `/Users/kapilash/Projects/AstroGatewayAndroid/EmulatorSDK`
- AVD: `/Users/kapilash/Projects/AstroGatewayAndroid/EmulatorAVD/AstroGateway_POC_API36_ARM64.avd`
- APK: `/Users/kapilash/Projects/AstroGatewayAndroid/app/build/outputs/apk/debug/app-debug.apk`
- Raw runtime artifacts: `/Users/kapilash/Projects/AstroGatewayAndroid/RuntimeValidation`
- Setup/command logs: `/Users/kapilash/Projects/AstroGatewayAndroid/logs`

APK SHA-256 before installation and from the installed `base.apk`: `37c9de93f904b812b5e737396bd55fb641f07abd7f4e12a00d99b6437337d269`. No APK rebuild or Unity export occurred in this stage.

## Physical device diagnosis

Before emulator setup, `adb devices -l` and wireless discovery returned empty lists. Restarting ADB did not expose a phone. macOS USB host inventory contained USB buses without a phone, so the exact cable/port/accessory-permission/phone-setting cause could not be isolated. No physical model or Android version could be queried. This is an undetected device, not a detected device in `unauthorized` state.

[Connection guide](Device_Connection_Guide.md) provides exact USB authorization steps and Android 11+ wireless pairing commands. Physical results remain **NOT TESTED**. Do not substitute this emulator result for phone validation.

## Screenshots and evidence

- [Native dashboard](Evidence/01-native-dashboard.png)
- [Unity scene receiving the mission](Evidence/02-unity-bridge-result.png)
- [Dashboard after cycle ten](Evidence/03-dashboard-result-cycle-10.png)
- [Initial Android tutorial overlay](Evidence/first-run-tutorial.png)
- [APK installation](Evidence/apk-install.log), [host launch](Evidence/host-launch.log)
- [App exit history](Evidence/app-exit-info.log), [app system-error query](Evidence/app-system-errors.log)
- [Preservation check](Preservation_Check.json): 357 existing source/configuration/APK files, zero changes

Original Unity/React projects, approved Mars scenes, and POC application source remain unchanged. No authentication, database integration, production UI or gameplay work was started. The previous build-only report is historical; this report supplies the subsequent emulator runtime evidence and its limits.

## Supported references

[Android emulator acceleration](https://developer.android.com/studio/run/emulator-acceleration) supports Apple Silicon/ARM64 matching and the selected software-rendering option. [Android ADB documentation](https://developer.android.com/tools/adb) supports device status and wireless-debugging procedures. [Apple accessory guidance](https://support.apple.com/en-ie/102282) explains USB data approval on Apple Silicon laptops. These support setup procedures, while the logs/screenshots above establish this run's results.

Stop at POC validation. Back-navigation remediation and physical-device testing remain follow-up work; no next product stage was implemented.
