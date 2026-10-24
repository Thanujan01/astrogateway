#!/bin/zsh
F=/Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMission
D=/Users/kapilash/Projects/MissionRedHorizon/Docs/FirstMission
P=/Users/kapilash/Projects/MissionRedHorizon
C=/Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMissionAndroid
U=/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity
$F/playtest.sh Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Explorer.unity $D/PlaytestExplorer playtest_ex2
$F/playtest.sh Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_v1.unity $D/Playtest playtest_v1_after_ex
cd $F; VU_QUIT=1 VU_OUT=$D/Regression/VisualUpgrade_ex ./run_unity.sh AstroGateway.VisualUpgrade.EditorTools.VisualUpgradePlaytest.Run regression_vu_ex gui > /dev/null 2>&1
(cd $P && git checkout -- ProjectSettings/ProjectSettings.asset); head -2 $D/Regression/VisualUpgrade_ex/VisualUpgradePlaytest_Results.txt
rsync -a --delete $P/Assets/ $C/Assets/
FM_BUILD_SCENE=Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Explorer.unity FM_APK=$C/Build/Explorer/AstroGateway_FirstMission_Explorer_dev.apk $U -batchmode -projectPath $C -buildTarget Android -executeMethod AstroGateway.FirstMission.EditorTools.FirstMissionAndroidBuild.Build -logFile $D/logs/android_build_explorer.log
echo "android exit $?"; sed -n 2,3p $C/Build/Explorer/Android_Build_Report.txt
echo done
