# Development workflow and terminal navigation

[README](../README.md) · [Setup](SETUP_GUIDE.md) · [Responsibilities](DEVELOPER_RESPONSIBILITIES.md) · [Tree](REPOSITORY_TREE.md)

## Clone and branch

The examples describe the intended workflow; branch creation, PR creation and application commands below were **NOT TESTED** in this documentation update. Authenticate with your own authorized GitHub account. Keep the repository private.

```bash
git clone https://github.com/kapilashkapilash2025-netizen/Kabylash-Development-SK2LB.git
cd Kabylash-Development-SK2LB
git lfs install
git lfs pull
git status
git switch main
git pull --ff-only origin main
git switch -c feature/describe-your-change
```

Use a clean working tree before switching or pulling. A failed fast-forward is a signal to inspect divergence; do not force-push to resolve it. Local environment files, access tokens, private keys and generated caches stay outside commits.

## Terminal navigation

| Command | Actual use |
|---|---|
| `pwd` | Print your current working directory before running path-sensitive scripts. |
| `ls` | List visible entries in that directory. |
| `ls -lah` | Include dotfiles such as `.gitattributes`, with readable sizes and permissions. |
| `cd PATH` / `cd ..` | Enter a real component / move to its parent. Quote paths containing spaces. |
| `find` | Locate files in the working copy; prune dependency/cache folders. |
| `tree` | Optional directory display; it is not installed automatically by this guide. |
| `git ls-files` | Inspect tracked paths; use this rather than caches to update the repository inventory. |
| `git status` | Review branch, staged/unstaged changes and untracked files. |
| `git pull --ff-only origin main` | Fetch and fast-forward the current branch from main; use deliberately from the correct branch. |
| `git log --oneline -10` | Read recent commit history. |
| `git lfs pull` | Download/checkout LFS content for the checked-out revision. |

Run these from the clone root:

```bash
pwd
ls -lah
find 01-Game-Development/MissionRedHorizon/Assets/AstroGateway -type f -name '*.cs'
find 01-Game-Development/MissionRedHorizon/Assets/AstroGateway -type f -name '*.unity'
find 01-Game-Development/MissionRedHorizon/Assets/AstroGateway -type f -name '*.prefab'
find 02-Software-Development/Backend/astrogateway-api -name .venv -prune -o -type f -name '*.py' -print
find 02-Software-Development/Integrations/android-unity-host-poc/app -type f -name '*.kt'
find 02-Software-Development/Web-Dashboards/astrogateway-web/src -type f -name '*.tsx'
git ls-files '02-Software-Development/*' 'docs/*'
# Optional:
tree -a -L 5 -I '.git|Library|Temp|node_modules|.venv'
```

Source entry points: [API routes](../02-Software-Development/Backend/astrogateway-api/main.py), [sensor routes](../02-Software-Development/Backend/astrogateway-api/sensor_records.py), [web entry](../02-Software-Development/Web-Dashboards/astrogateway-web/src/main.tsx), [Android source](../02-Software-Development/Integrations/android-unity-host-poc/app/src/main/java/com/sk2lb/astrogateway/poc). For a portable shallow listing without `tree`:

```bash
python3 - <<'PYTHON'
from pathlib import Path
skip = {'.git', 'Library', 'Temp', 'node_modules', '.venv'}
for path in sorted(Path('.').glob('*')):
    if path.name not in skip:
        print(str(path) + ('/' if path.is_dir() else ''))
PYTHON
```

This shallow working-copy listing is not the complete inventory; [REPOSITORY_TREE.md](REPOSITORY_TREE.md) contains every tracked path.

## Edit and verify

Make focused changes in the primary module. Use [Setup](SETUP_GUIDE.md) for commands and constraints. Do not silently copy code across independent experimental Unity projects. For shared JSON changes, update/review both Unity/backend copies and their cross-checks. For assets, stage `.meta` sidecars and respect LFS attributes. Review candidates before remote SQL changes; ordinary source editing does not authorize deployment or provider administration.

```bash
git status --short
git diff --check
git diff --stat
git diff
# Add only the intended files, then inspect staged content:
git add README.md docs/ARCHITECTURE.md
git diff --cached --stat
git diff --cached
```

The `git add` paths above illustrate a documentation change; choose the actual files for your work. Avoid blanket staging that could include generated outputs. Check that credentials are not present even when a file is ignored by default.

## Commit, pull request and merge

After relevant checks and review of staged content (**commands NOT TESTED here**):

```bash
git commit -m "docs: clarify the affected module"
git push -u origin HEAD
# If GitHub CLI is installed and authenticated:
gh pr create --base main --fill
```

Describe the problem, final behavior, affected components, actual test results and any NOT TESTED areas. Request reviewers using the [proposed responsibility map](DEVELOPER_RESPONSIBILITIES.md). Merge only under the repository owner's actual review policy. Do not claim required reviews, CODEOWNERS or CI gates exist merely because this guide recommends them. This documentation update was separately authorized to push directly to main; that is not a blanket future bypass policy.

## Documentation maintenance

- Regenerate the complete tree from `git ls-files -z` plus newly added documentation paths. Include only tracked/being-committed files, with no cache directories or illustrative names.
- Keep README navigation concise; put contracts/evidence in Architecture and commands in Setup.
- Validate relative links, headings, both Mermaid types and the documentation-only diff when making guide-only changes.
- Keep `docs/migration/file-manifest.csv` as the immutable import snapshot. It is not a live inventory of future development changes.
- Preserve source stage reports; add current observations to the live guides rather than overwriting evidence.
