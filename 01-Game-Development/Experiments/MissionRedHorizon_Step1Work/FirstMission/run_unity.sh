#!/bin/zsh
# usage: run_unity.sh <method> <logname> [batch|gui]   (env passes through)
U=/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity
P=/Users/kapilash/Projects/MissionRedHorizon
L=/Users/kapilash/Projects/MissionRedHorizon/Docs/FirstMission/logs
mkdir -p $L
if pgrep -f "Unity.app/Contents/MacOS/Unity.*MissionRedHorizon$|projectPath $P( |$)" >/dev/null; then echo "Unity already running on project"; exit 3; fi
if [ "$3" = "batchloop" ]; then
  $U -batchmode -projectPath $P -executeMethod $1 -logFile $L/$2.log
elif [ "$3" = "gui" ]; then
  $U -projectPath $P -executeMethod $1 -logFile $L/$2.log
else
  $U -batchmode -quit -projectPath $P -executeMethod $1 -logFile $L/$2.log
fi
echo "exit $?" > $L/$2.exit
