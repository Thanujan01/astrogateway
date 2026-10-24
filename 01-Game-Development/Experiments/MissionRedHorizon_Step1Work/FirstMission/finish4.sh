#!/bin/zsh
F=/Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMission
D=/Users/kapilash/Projects/MissionRedHorizon/Docs/FirstMission
P=/Users/kapilash/Projects/MissionRedHorizon
C=/Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMissionAndroid
U=/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity
$F/playtest.sh Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Explorer.unity $D/PlaytestExplorer $1
if grep -q "FAIL 0" $D/PlaytestExplorer/FirstMission_Playtest_Results.txt; then
rsync -a --delete $P/Assets/ $C/Assets/
FM_BUILD_SCENE=Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Explorer.unity FM_APK=$C/Build/Explorer/AstroGateway_FirstMission_Explorer_dev.apk $U -batchmode -projectPath $C -buildTarget Android -executeMethod AstroGateway.FirstMission.EditorTools.FirstMissionAndroidBuild.Build -logFile $D/logs/android_build_explorer2.log
echo "android exit $?"; sed -n 2p $C/Build/Explorer/Android_Build_Report.txt
fi
echo done
