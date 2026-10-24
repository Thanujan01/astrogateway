#!/bin/zsh
set -eu
TASK_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
TASK_JAVA="/Applications/Unity/Hub/Editor/6000.6.4f1/PlaybackEngines/AndroidPlayer/OpenJDK/bin/java"
TASK_GRADLE="/Applications/Unity/Hub/Editor/6000.6.4f1/PlaybackEngines/AndroidPlayer/Tools/gradle/lib/gradle-launcher-9.3.1.jar"
python3 "$TASK_ROOT/tools/prepare_host.py"
cd "$TASK_ROOT/unity-export"
"$TASK_JAVA" -classpath "$TASK_GRADLE" org.gradle.launcher.GradleMain :app:assembleDebug --no-daemon --console=plain
