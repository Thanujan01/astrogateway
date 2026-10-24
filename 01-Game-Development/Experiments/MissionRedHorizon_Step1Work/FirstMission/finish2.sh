#!/bin/zsh
F=/Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMission
D=/Users/kapilash/Projects/MissionRedHorizon/Docs/FirstMission
P=/Users/kapilash/Projects/MissionRedHorizon
C=/Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMissionAndroid
U=/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity
$F/playtest.sh Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_HUDv2.unity $D/PlaytestHUDv2 playtest_hud8
rsync -a --delete $P/Assets/ $C/Assets/
FM_BUILD_SCENE=Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_HUDv2.unity FM_APK=$C/Build/HUDv2/AstroGateway_FirstMission_HUDv2_dev.apk $U -batchmode -projectPath $C -buildTarget Android -executeMethod AstroGateway.FirstMission.EditorTools.FirstMissionAndroidBuild.Build -logFile $D/logs/android_build_hudv2b.log
echo "android exit $?"; sed -n 2,3p $C/Build/HUDv2/Android_Build_Report.txt
export ANDROID_SERIAL=ZY22LDFDTT; A=/opt/homebrew/share/android-commandlinetools/platform-tools/adb; mkdir -p $D/Device/HUDv2
$A install -r $C/Build/HUDv2/AstroGateway_FirstMission_HUDv2_dev.apk | tail -1
$A shell am force-stop com.astrogateway.firstmission.dev; $A logcat -c; $A shell monkey -p com.astrogateway.firstmission.dev -c android.intent.category.LAUNCHER 1 >/dev/null 2>&1; sleep 30
$A exec-out screencap -p > $D/Device/HUDv2/03_telemetry_controls.png; $A logcat -d | grep -E " E Unity" | grep -v AssetPackManager | head -3
echo done
