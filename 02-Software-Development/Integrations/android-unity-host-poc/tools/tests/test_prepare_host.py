"""Unity export wiring must preserve fresh settings and be idempotent."""
from pathlib import Path
import subprocess
import json
import tempfile
import unittest

SCRIPT = Path(__file__).resolve().parents[1] / 'prepare_host.py'
class PrepareHostTests(unittest.TestCase):
    def test_fresh_export_supersedes_old_backup_and_repeated_runs_are_stable(self):
        with tempfile.TemporaryDirectory() as folder:
            root=Path(folder);(root/'tools').mkdir();(root/'tools/prepare_host.py').write_bytes(SCRIPT.read_bytes())
            export=root/'unity-export';(export/'unityLibrary').mkdir(parents=True)
            (export/'astrogateway-missions-export.json').write_text(json.dumps({'schema_version':1,'scene':'Mars_FirstMission_Missions','application_id':'com.sk2lb.astrogateway'}))
            settings=export/'settings.gradle';props=export/'gradle.properties'
            settings.write_text("include ':launcher'\ninclude ':unityLibrary'\n// fresh exporter configuration\n")
            (export/'settings.unity-original.gradle.txt').write_text("include ':launcher'\n// obsolete settings\n")
            props.write_text('android.builtInKotlin=false\n')
            run=lambda:subprocess.run(['python3',str(root/'tools/prepare_host.py')],check=True,capture_output=True)
            run();first=settings.read_text();run()
            self.assertEqual(first,settings.read_text());self.assertIn('fresh exporter configuration',first)
            self.assertNotIn('obsolete',first);self.assertEqual(first.count("include ':app'"),1)
            self.assertEqual(first.count("project(':app').projectDir"),1)
            self.assertIn('android.builtInKotlin=true',props.read_text())
            (export/'astrogateway-missions-export.json').unlink()
            stale=subprocess.run(['python3',str(root/'tools/prepare_host.py')],capture_output=True)
            self.assertNotEqual(stale.returncode,0)
            self.assertEqual(first,settings.read_text())
if __name__=='__main__':unittest.main()
