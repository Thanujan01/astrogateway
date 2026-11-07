#!/bin/zsh
set -eu
export PATH="/opt/homebrew/bin:/usr/local/bin:$PATH"
TASK_ROOT="$(cd "$(dirname "$0")" && pwd)"
exec python3 "$TASK_ROOT/tools/prototype/start.py" "$@"
