import pytest,os
raise SystemExit(pytest.main(['tests','-q','-p','no:cacheprovider','--basetemp='+os.environ['TMPDIR']+'/pytest']))
