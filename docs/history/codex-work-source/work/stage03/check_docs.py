from pathlib import Path
import json,hashlib
for f,h in json.loads(Path('work/stage03/design-hashes.json').read_text()).items():
 assert hashlib.sha256(Path(f).read_bytes()).hexdigest()==h,f
print('All seven approved design documents unchanged.')
