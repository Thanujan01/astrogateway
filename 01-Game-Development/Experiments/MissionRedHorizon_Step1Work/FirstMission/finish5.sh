#!/bin/zsh
# usage: finish5.sh <log suffix> [apk]  — rebuild Missions scene, playtest it, optionally build the APK
F=/Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMission
D=/Users/kapilash/Projects/MissionRedHorizon/Docs/FirstMission
P=/Users/kapilash/Projects/MissionRedHorizon
C=/Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMissionAndroid
U=/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity
$F/run_unity.sh AstroGateway.FirstMission.EditorTools.FirstMissionBuilder.BuildMissions build_missions_$1 batch
cat $D/logs/build_missions_$1.exit; grep -E "error CS" $D/logs/build_missions_$1.log | sort -u | head
(cd $P && git checkout -- ProjectSettings/ProjectSettings.asset)
$F/playtest.sh Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Missions.unity $D/PlaytestMissions playtest_mi_$1
if [ "$2" = "apk" ] && grep -q "FAIL 0" $D/PlaytestMissions/FirstMission_Playtest_Results.txt; then
rsync -a --delete $P/Assets/ $C/Assets/
mkdir -p $C/Build/Missions
FM_BUILD_SCENE=Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Missions.unity FM_APK=$C/Build/Missions/AstroGateway_FirstMission_Missions_dev.apk $U -batchmode -projectPath $C -buildTarget Android -executeMethod AstroGateway.FirstMission.EditorTools.FirstMissionAndroidBuild.Build -logFile $D/logs/android_build_missions_$1.log
echo "android exit $?"; sed -n 2p $C/Build/Missions/Android_Build_Report.txt
fi
echo done
