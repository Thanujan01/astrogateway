# Physical Android connection follow-up

The validation Mac currently detects no physical phone in ADB or its USB host inventory. This does not identify the precise cause. An emulator is used separately for functional tests.

## USB data connection

1. Unlock both the Mac and phone. Connect the phone directly using a known data-capable cable; try another port/cable and bypass hubs.
2. Accept macOS's accessory connection alert if it appears. Check **System Settings → Privacy & Security → Allow accessories to connect**. Approve the specific accessory; this validation did not change the global setting. A phone may charge while data access is blocked. [Apple's accessory guidance](https://support.apple.com/en-ie/102282).
3. On Android, enable **Developer options → USB debugging**. If the phone offers a USB mode selector, select File Transfer to check the data connection. Settings locations vary by manufacturer.
4. Run the installed ADB below. If it shows `unauthorized`, keep the phone unlocked and accept **Allow USB debugging?** for this Mac. The optional “Always allow” checkbox is the user's choice.
5. If that prompt is absent while the device is `unauthorized`, disconnect/reconnect and toggle USB debugging. As a last resort, the phone's **Revoke USB debugging authorizations** resets previously trusted computers, so use it deliberately before reconnecting.

```sh
TASK_ADB='/Applications/Unity/Hub/Editor/6000.6.4f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb'
"$TASK_ADB" devices -l
```

Expected usable status is `device`. An empty list differs from `unauthorized`: there is no visible transport to authorize. [Android device setup](https://developer.android.com/studio/run/device.html).

## Wireless debugging alternative — Android 11+

Connect phone and Mac to the same Wi-Fi network. On the phone, enable **Developer options → Wireless debugging → Pair device with pairing code**. Run the pairing command using that screen's pairing address, enter its temporary code interactively, then connect using the separate address/port on the main Wireless debugging screen:

```sh
"$TASK_ADB" pair 'PHONE_IP:PAIRING_PORT'
"$TASK_ADB" connect 'PHONE_IP:CONNECTION_PORT'
"$TASK_ADB" devices -l
```

Replace the placeholders; pairing and connection ports can differ. No wireless pairing was attempted in this validation because no phone address/code was supplied. [Official ADB wireless-debugging guide](https://developer.android.com/tools/adb).

## Reuse the existing APK

The existing debug APK is ARM64 and requires Android 8/API 26 or newer. After authorization, select the actual device serial explicitly, install the same APK and repeat the message/return cycles. Emulator success does not establish physical phone graphics, thermals, touch behavior or lifecycle compatibility. Android Back is a known unresolved POC issue; use the native return button for the validated route.
