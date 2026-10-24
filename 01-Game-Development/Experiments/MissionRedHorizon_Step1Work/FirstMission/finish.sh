#!/bin/zsh
F=/Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMission
D=/Users/kapilash/Projects/MissionRedHorizon/Docs/FirstMission
P=/Users/kapilash/Projects/MissionRedHorizon
C=/Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMissionAndroid
U=/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity
$F/playtest.sh Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_HUDv2.unity $D/PlaytestHUDv2 playtest_hud5
cd $F; VU_QUIT=1 VU_OUT=$D/Regression/VisualUpgrade_hud ./run_unity.sh AstroGateway.VisualUpgrade.EditorTools.VisualUpgradePlaytest.Run regression_vu_hud gui > run_reg_hud.out 2>&1
(cd $P && git checkout -- ProjectSettings/ProjectSettings.asset); head -2 $D/Regression/VisualUpgrade_hud/VisualUpgradePlaytest_Results.txt; grep "^FAIL" $D/Regression/VisualUpgrade_hud/VisualUpgradePlaytest_Results.txt
rsync -a --delete $P/Assets/ $C/Assets/
FM_BUILD_SCENE=Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_HUDv2.unity FM_APK=$C/Build/HUDv2/AstroGateway_FirstMission_HUDv2_dev.apk $U -batchmode -projectPath $C -buildTarget Android -executeMethod AstroGateway.FirstMission.EditorTools.FirstMissionAndroidBuild.Build -logFile $D/logs/android_build_hudv2.log
echo "android exit $?"; head -6 $C/Build/HUDv2/Android_Build_Report.txt
