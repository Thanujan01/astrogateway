#!/usr/bin/env python3
"""Hidden local password entry only; no connection, migration, role or account creation."""
import getpass,os,shlex,sys,tempfile
from pathlib import Path
from urllib.parse import quote
ROOT=Path(__file__).resolve().parent.parent
PATH=ROOT/'database/admin.local.env'
def main():
    if not sys.stdin.isatty() or not sys.stderr.isatty():
        print('Run directly in a local Terminal; noninteractive password entry is refused.');return 1
    if PATH.is_symlink() or PATH.stat().st_mode & 0o077:
        print('Administration configuration must be a regular owner-only file.');return 1
    settings={}
    for line in PATH.read_text().splitlines():
        if '=' in line and not line.lstrip().startswith('#'):
            k,v=line.split('=',1);parts=shlex.split(v,comments=True);settings[k.strip()]=parts[0] if parts else ''
    expected={'ASTROGATEWAY_ENVIRONMENT':'test','ASTROGATEWAY_APPROVED_PROJECT_REF':'jvkwofmxtxjhdwtwdboz','DB_ADMIN_HOST':'aws-0-ap-southeast-2.pooler.supabase.com','DB_ADMIN_PORT':'5432','DB_ADMIN_DATABASE':'postgres','DB_ADMIN_USERNAME':'postgres.jvkwofmxtxjhdwtwdboz','DB_ADMIN_SSL_MODE':'require'}
    if any(settings.get(k)!=v for k,v in expected.items()):
        print('Approved TEST pooler details do not match; nothing saved.');return 1
    if settings.get('DB_ADMIN_DSN'):
        print('An administration URI already exists; nothing overwritten.');return 1
    password=getpass.getpass('Approved astrogateway-test database admin password (hidden): ')
    if not password:
        print('No password entered; nothing saved.');return 1
    dsn='postgresql://'+quote(settings['DB_ADMIN_USERNAME'],safe='')+':'+quote(password,safe='')+'@'+settings['DB_ADMIN_HOST']+':5432/postgres?sslmode=require'
    lines=PATH.read_text().splitlines();lines=[('DB_ADMIN_DSN="'+dsn+'"') if line.startswith('DB_ADMIN_DSN=') else line for line in lines]
    fd,name=tempfile.mkstemp(dir=PATH.parent,prefix='.admin-entry-');os.fchmod(fd,0o600)
    try:
        with os.fdopen(fd,'w') as f:f.write('\n'.join(lines)+'\n')
        os.replace(name,PATH)
    finally:
        if os.path.exists(name):os.unlink(name)
    print('Owner-only admin configuration saved. No connection or database changes performed.');return 0
if __name__=='__main__':
    try:raise SystemExit(main())
    except (KeyboardInterrupt,EOFError):print('Cancelled; no password saved.');raise SystemExit(1)
    except Exception:print('Configuration failed; no credential details are displayed.');raise SystemExit(1)
