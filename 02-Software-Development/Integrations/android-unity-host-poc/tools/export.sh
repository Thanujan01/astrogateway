#!/bin/zsh
set -eu
TASK_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
TASK_UNITY="${ASTROGATEWAY_UNITY_EDITOR:-/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity}"
TASK_PROJECT="${ASTROGATEWAY_GAME_PROJECT:-$TASK_ROOT/../../../01-Game-Development/MissionRedHorizon}"
mkdir -p "$TASK_ROOT/logs"
export ASTROGATEWAY_UNITY_EXPORT="$TASK_ROOT/unity-export"
"$TASK_UNITY" -batchmode -quit -projectPath "$TASK_PROJECT" -buildTarget Android -executeMethod MissionsAndroidBuild.ExportHost -logFile "$TASK_ROOT/logs/unity-export.log"
python3 "$TASK_ROOT/tools/prepare_host.py"
