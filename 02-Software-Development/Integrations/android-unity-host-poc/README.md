# AstroGateway Android integration POC

Native Kotlin host source and isolated Unity integration copy. Production Unity/React projects are not edited. Temporary scene contains only a camera, directional light, a one-meter cube and the test receiver. Export only that scene, not the Mars environment.

Unity 6000.6.4f1 Activity entry, separate :unity process. One runtime instance is retained when the dashboard returns; Activity pause/resume is inherited from Unity. Relaunch reuses the existing Activity, sends a fresh launch UUID and repeats ACK/RESULT. This POC deliberately does not claim to free Unity memory on dashboard return. Host process death, Unity exit and unload have distinct behavior and require device tests.

Native host sends version 1 mission descriptor after scene-ready handshake. C# acknowledges then returns a test result through Activity callbacks. Package-restricted non-exported receiver records evidence on the dashboard. Identifiers are test data, not authorization credentials. No backend/Auth services exist.

Build and test status are tracked in logs and the output completion report; source presence is not proof of a working APK.

## Reproduce

1. `zsh tools/export.sh` exports the isolated project and records `logs/unity-export.log`.
2. `zsh tools/build.sh` uses Unity's JDK/Gradle and produces `app/build/outputs/apk/debug/app-debug.apk`.
3. Connect exactly one authorized ARM64 Android device, keep it unlocked, then run `python3 tools/device_smoke.py` for install, launch, ten message/return cycles and screenshots.

The exported `unity-export` folder is the Gradle build root; the app module points to the separate `../app` source. Android Studio is optional for this CLI workflow. SDK 36/min 26, JDK 17.0.18, NDK 27.2.12479018, Gradle 9.3.1, AGP 9.1.1. Unity export disables Kotlin by default; the preparation tool enables AGP built-in Kotlin without adding the legacy Kotlin Android plugin. Native code calls the static `UnityPlayer.UnitySendMessage` API.

The Unity library's generated launcher Activity is removed by manifest merging; only the Kotlin dashboard is exported as LAUNCHER. The custom Unity Activity is private, in a separate process and reused with reorder-to-front plus singleTop. Dashboard return does not destroy or unload it. Source and build outputs contain no account credentials.

If the phone is locked, unauthorized or absent, runtime checks must remain NOT TESTED. Do not label a compiled APK as device validation.

Result delivery also accompanies the explicit dashboard-return Intent, since newer Android versions can defer broadcasts to a cached host process. The launch ID is checked before displaying it. This debug test result is not a trusted mission score.
