#!/usr/bin/env python3
"""Local test runner. Parses private .env as data, never shell code; never applies migrations."""
import os,shlex,stat,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parent
ALLOWED={'SUPABASE_URL','SUPABASE_PUBLISHABLE_KEY','DATABASE_URL','SUPABASE_JWT_ISSUER','CORS_ALLOWED_ORIGINS','ASTROGATEWAY_ENVIRONMENT','ASTROGATEWAY_APPROVED_PROJECT_REF'}
def load_local(path):
    if not path.exists():return
    if path.is_symlink() or stat.S_IMODE(path.stat().st_mode)&0o077:raise ValueError('Use a regular owner-only .env file (chmod 600).')
    for line in path.read_text().splitlines():
        line=line.strip()
        if not line or line.startswith('#'):continue
        if '=' not in line:raise ValueError('Invalid .env syntax.')
        key,value=line.split('=',1);key=key.strip()
        if key not in ALLOWED:raise ValueError('Unexpected .env key. Use the provided template.')
        parts=shlex.split(value,comments=True,posix=True)
        if len(parts)>1:raise ValueError('Quote .env values containing spaces.')
        os.environ[key]=parts[0] if parts else ''
def main():
    try:
        load_local(ROOT/'.env')
        if os.getenv('ASTROGATEWAY_ENVIRONMENT')!='test':raise ValueError('Only explicitly marked test configuration is supported.')
        required=['SUPABASE_URL','SUPABASE_PUBLISHABLE_KEY','DATABASE_URL']
        supplied=[bool(os.getenv(k)) for k in required]
        if any(supplied) and not all(supplied):raise ValueError('Fill all three required server settings together, or leave all blank for connectivity-only mode.')
        if all(supplied):
            ref=os.getenv('ASTROGATEWAY_APPROVED_PROJECT_REF','')
            if not ref or os.environ['SUPABASE_URL'].rstrip('/')!='https://'+ref+'.supabase.co':raise ValueError('The configured project must match the explicitly approved TEST project reference.')
        from main import app
        import uvicorn
        print('Local TEST server: http://127.0.0.1:8000; access logging disabled. '+('Provider configuration present; live validation still required.' if all(supplied) else 'Connectivity-only mode; live authentication is unavailable.'),flush=True)
        uvicorn.run(app,host='127.0.0.1',port=8000,access_log=False,log_level='warning')
    except Exception:
        print('Server setup failed. Check owner-only .env syntax, complete test configuration, installed dependencies, port availability and restricted TLS database access. No configuration values are printed.',file=sys.stderr)
        return 1
    return 0
if __name__=='__main__':raise SystemExit(main())
