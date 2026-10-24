from http.server import HTTPServer, BaseHTTPRequestHandler
from urllib.parse import parse_qs
from pathlib import Path
import base64
out=Path('/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage04')
class Handler(BaseHTTPRequestHandler):
 def do_GET(self):
  self.send_response(200);self.send_header('Content-Type','text/html');self.end_headers();self.wfile.write(b'<title>Local capture archive</title><form method="post"><label>Screenshot data<textarea name="capture" aria-label="Screenshot data"></textarea></label><button>Save browser capture</button></form>')
 def do_POST(self):
  size=int(self.headers.get('Content-Length','0'))
  if size>2000000:self.send_error(413);return
  value=parse_qs(self.rfile.read(size).decode()).get('capture',[''])[0];data=base64.b64decode(value,validate=True)
  if not data.startswith(b'\xff\xd8'):self.send_error(400);return
  (out/'Browser_Run.jpg').write_bytes(data)
  self.send_response(200);self.send_header('Content-Type','text/html');self.end_headers();self.wfile.write(b'<title>Capture saved</title><p>Saved Browser_Run.jpg in Stage04 outputs.</p>')
HTTPServer(('127.0.0.1',8768),Handler).serve_forever()
