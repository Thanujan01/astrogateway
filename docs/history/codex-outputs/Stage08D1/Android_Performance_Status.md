# Android performance status

**PHYSICAL DEVICE UNTESTED.** Approved installed ADB queried with `adb devices -l`: list empty. An initial sandbox-restricted daemon start failed; rerun with local device-access permission successfully started ADB and still found no device.

No Android evaluation build, install or profiling performed because the critical rover import blocked scene creation. No average FPS, GPU time, CPU time, memory, thermal, draw-call or rendering improvement claim. Prior software-emulator results are not measurements of this evaluation scene.

Next: after import/scene validation, connect an authorized physical device and record model/SoC/Android version, resolution and URP profile; collect repeatable clear-view measurements with the same camera, compare original/LOD variants, then tune texture compression and lighting based on evidence.
