from pathlib import Path
root = Path(__file__).resolve().parents[1]
export = root / "unity-export"
settings = export / "settings.gradle"
if not (export / "unityLibrary").exists():
    raise SystemExit("Export UnityIntegrationProject first: unityLibrary missing")
original = settings.read_text()
backup = export / "settings.unity-original.gradle.txt"
if not backup.exists():
    backup.write_text(original)
else:
    original = backup.read_text()
modified = original.replace(":launcher", ":app")
modified += "\nproject(':app').projectDir = new File(settingsDir, '../app')\n"
settings.write_text(modified)
print("Host app wired to Unity export. Original settings backed up.")

properties = export / "gradle.properties"
properties.write_text(properties.read_text().replace("android.builtInKotlin=false", "android.builtInKotlin=true"))
print("AGP built-in Kotlin enabled for native host compilation.")
