#!/bin/zsh
set -eu
TASK_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$TASK_ROOT/UnityIntegrationProject" -buildTarget Android -executeMethod AndroidPocExporter.Export -logFile "$TASK_ROOT/logs/unity-export.log"
python3 "$TASK_ROOT/tools/prepare_host.py"
