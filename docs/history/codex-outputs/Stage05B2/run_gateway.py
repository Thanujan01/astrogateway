import os
from pathlib import Path
root=Path(__file__).resolve().parent
(root/'private/postgrest.pid').write_text(str(os.getpid()))
f=os.open(root/'private/postgrest.log',os.O_WRONLY|os.O_CREAT|os.O_APPEND,0o600);os.dup2(f,1);os.dup2(f,2)
os.execv(str(root/'native/postgrest/postgrest'),[str(root/'native/postgrest/postgrest'),str(root/'private/postgrest.conf')])
