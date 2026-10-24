#!/bin/zsh
# usage: record.sh <tag>  — rebuild Missions scene, record the autopilot video, encode MP4
F=/Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMission
D=/Users/kapilash/Projects/MissionRedHorizon/Docs/FirstMission
P=/Users/kapilash/Projects/MissionRedHorizon
V=$F/Video
mkdir -p $V
pkill -f "AssetImportWorker.*MissionRedHorizon"; (cd $P && git checkout -- ProjectSettings/ProjectSettings.asset)
$F/run_unity.sh AstroGateway.FirstMission.EditorTools.FirstMissionBuilder.BuildMissions build_video_$1 batch
(cd $P && git checkout -- ProjectSettings/ProjectSettings.asset)
rm -f $V/recording.txt
FM_QUIT=1 FM_FRAMES=$V/frames $F/run_unity.sh AstroGateway.FirstMission.EditorTools.FirstMissionDemoRecorder.Run video_$1 gui > $V/run_$1.out 2>&1 &
for i in $(seq 1 400); do [ -f $V/recording.txt ] && break; sleep 3; done
sleep 12
pgrep -f "Unity.app/Contents/MacOS/Unity -projectPath" >/dev/null && kill -TERM $(pgrep -f "Unity.app/Contents/MacOS/Unity -projectPath")
sleep 6; pkill -f "AssetImportWorker.*MissionRedHorizon"; (cd $P && git checkout -- ProjectSettings/ProjectSettings.asset)
cat $V/recording.txt
grep -E "error CS|Exception" $D/logs/video_$1.log | sort -u | head
ls $V/frames | wc -l
swift $F/encode_video.swift $V/frames $V/AstroGateway_Missions_Gameplay_$1.mp4 30 6000000
ls -la $V/*.mp4
