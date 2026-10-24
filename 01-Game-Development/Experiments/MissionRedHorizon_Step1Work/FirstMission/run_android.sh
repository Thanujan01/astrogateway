#!/bin/zsh
U=/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity
C=/Users/kapilash/Projects/MissionRedHorizon_Step1Work/FirstMissionAndroid
L=/Users/kapilash/Projects/MissionRedHorizon/Docs/FirstMission/logs
FM_APK=$C/Build/AstroGateway_FirstMission_dev.apk $U -batchmode -projectPath $C -buildTarget Android -executeMethod AstroGateway.FirstMission.EditorTools.FirstMissionAndroidBuild.Build -logFile $L/android_build.log
echo "exit $?" > $L/android_build.exit
