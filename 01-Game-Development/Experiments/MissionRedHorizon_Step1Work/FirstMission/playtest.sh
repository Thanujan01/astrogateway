#!/bin/zsh
# usage: playtest.sh <scene path> <out dir> <log name>
cd /Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMission
P=/Users/kapilash/Projects/MissionRedHorizon
pkill -f "AssetImportWorker.*MissionRedHorizon"; (cd $P && git checkout -- ProjectSettings/ProjectSettings.asset)
rm -rf $2
FM_QUIT=1 FM_SCENE=$1 FM_OUT=$2 ./run_unity.sh AstroGateway.FirstMission.EditorTools.FirstMissionPlaytest.Run $3 gui > run_$3.out 2>&1 &
R=$2/FirstMission_Playtest_Results.txt
for i in $(seq 1 300); do [ -f $R ] && break; sleep 3; done
sleep 15
pgrep -f "Unity.app/Contents/MacOS/Unity -projectPath" >/dev/null && kill -TERM $(pgrep -f "Unity.app/Contents/MacOS/Unity -projectPath")
sleep 8; pkill -f "AssetImportWorker.*MissionRedHorizon"; (cd $P && git checkout -- ProjectSettings/ProjectSettings.asset)
head -2 $R; grep -E "^FAIL|^UNTESTED|EXCEPTION" $R | cut -c1-300
grep -E "error CS" $P/Docs/FirstMission/logs/$3.log | sort -u | head
