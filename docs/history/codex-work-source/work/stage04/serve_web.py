from http.server import ThreadingHTTPServer,SimpleHTTPRequestHandler
from pathlib import Path
import mimetypes
root=Path('/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage04')
class Handler(SimpleHTTPRequestHandler):
 def __init__(self,*args,**kwargs): super().__init__(*args,directory=str(root),**kwargs)
 def guess_type(self,path):
  base=path.removesuffix('.br').removesuffix('.gz')
  if base.endswith('.wasm'):return 'application/wasm'
  if base.endswith('.js'):return 'application/javascript'
  if base.endswith('.data'):return 'application/octet-stream'
  return super().guess_type(base)
 def end_headers(self):
  if self.path.endswith('.br'):self.send_header('Content-Encoding','br')
  elif self.path.endswith('.gz'):self.send_header('Content-Encoding','gzip')
  self.send_header('Cross-Origin-Opener-Policy','same-origin')
  self.send_header('Cross-Origin-Embedder-Policy','require-corp')
  super().end_headers()
ThreadingHTTPServer(('127.0.0.1',8767),Handler).serve_forever()
