#!/bin/zsh
set -eu
TASK_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
if [[ ! -x "$TASK_ROOT/backend/.venv/bin/python" ]]; then
  python3 -m venv "$TASK_ROOT/backend/.venv"
fi
"$TASK_ROOT/backend/.venv/bin/python" -m pip install --disable-pip-version-check -r "$TASK_ROOT/backend/requirements.txt"
