from pathlib import Path
import json
root = Path(__file__).resolve().parents[1]
export = root / "unity-export"
settings = export / "settings.gradle"
if not (export / "unityLibrary").exists():
    raise SystemExit("Export UnityIntegrationProject first: unityLibrary missing")
marker = export / "astrogateway-missions-export.json"
try:
    stamp = json.loads(marker.read_text())
except (OSError, ValueError):
    raise SystemExit("Export the actual Missions game with tools/export.sh first; historical cube exports are not accepted")
if stamp.get("schema_version") != 1 or stamp.get("scene") != "Mars_FirstMission_Missions" or stamp.get("application_id") != "com.sk2lb.astrogateway":
    raise SystemExit("Wrong Unity export: expected the current Missions game")
original = settings.read_text()
backup = export / "settings.unity-original.gradle.txt"
if "include ':launcher'" in original:
    # A fresh Unity export supersedes the prior generated settings backup.
    backup.write_text(original)
elif "include ':app'" in original and backup.exists():
    original = backup.read_text()
else:
    raise SystemExit("Expected a Unity launcher export or an already prepared host")
modified = original.replace("include ':launcher'", "include ':app'")
modified += "\nproject(':app').projectDir = new File(settingsDir, '../app')\n"
settings.write_text(modified)
print("Host app wired to Unity export. Original settings backed up.")

properties = export / "gradle.properties"
properties.write_text(properties.read_text().replace("android.builtInKotlin=false", "android.builtInKotlin=true"))
print("AGP built-in Kotlin enabled for native host compilation.")
