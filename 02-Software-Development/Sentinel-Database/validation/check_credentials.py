#!/usr/bin/env python3
"""Read-only test-configuration audit. Never prints key values or reads admin credentials."""
import argparse
import json
import os
from pathlib import Path
import shlex
import socket
import ssl
import stat
import urllib.error
import urllib.request
from urllib.parse import urlparse

ROOT = Path(__file__).resolve().parents[3]
REF = 'jvkwofmxtxjhdwtwdboz'
URL = 'https://' + REF + '.supabase.co'
BACKEND = '02-Software-Development/Backend/astrogateway-api/.env'
WEB = '02-Software-Development/Web-Dashboards/astrogateway-web/.env.local'
ANDROID = '02-Software-Development/Integrations/android-unity-host-poc/auth.local.properties'
ALLOWED = {
 BACKEND: {'ASTROGATEWAY_ENVIRONMENT','ASTROGATEWAY_APPROVED_PROJECT_REF','SUPABASE_URL','SUPABASE_PUBLISHABLE_KEY','SUPABASE_JWT_ISSUER','CORS_ALLOWED_ORIGINS','DATABASE_URL','DATABASE_SSL_ROOT_CERT','DATABASE_ALLOW_LEGACY_CA'},
 WEB: {'VITE_SUPABASE_URL','VITE_SUPABASE_PUBLISHABLE_KEY','VITE_AUTH_API_BASE','VITE_ACCOUNT_TURNSTILE_SITE_KEY'},
 ANDROID: {'supabase.url','supabase.publishableKey','auth.backendUrl'},
}


def read_config(path, allowed):
    if path.is_symlink() or not path.is_file():
        raise ValueError('Required regular local configuration file missing or symlinked.')
    mode = path.stat()
    if not stat.S_ISREG(mode.st_mode) or stat.S_IMODE(mode.st_mode) & 0o077 or mode.st_uid != os.getuid():
        raise ValueError('Configuration must be owned by the current user with no group/other access.')
    result = {}
    for line in path.read_text().splitlines():
        line = line.strip()
        if not line or line.startswith('#'):
            continue
        if '=' not in line:
            raise ValueError('Invalid configuration syntax.')
        key, value = line.split('=', 1)
        key = key.strip()
        if key not in allowed or key in result:
            raise ValueError('Unknown or duplicate configuration field.')
        parts = shlex.split(value, comments=True, posix=True)
        if len(parts) > 1:
            raise ValueError('Configuration values containing spaces must be quoted.')
        result[key] = parts[0] if parts else ''
    return result


def backend_url_valid(value):
    parsed = urlparse(value)
    return bool(parsed.hostname and not parsed.username and not parsed.password and not parsed.query and not parsed.fragment
                and (parsed.scheme == 'https' or (parsed.scheme == 'http' and parsed.hostname == '10.0.2.2' and parsed.port == 8000)))


def audit(root):
    checks = []
    configs = {}
    def add(name, ok):
        checks.append({'check':name, 'status':'PASS' if ok else 'FAIL'})
    for relative, allowed in ALLOWED.items():
        try:
            configs[relative] = read_config(root / relative, allowed)
            add(relative + ': owner-only regular file and allowed fields', True)
        except (OSError, ValueError):
            add(relative + ': owner-only regular file and allowed fields', False)
    if len(configs) != 3:
        return checks, None
    b, w, a = configs[BACKEND], configs[WEB], configs[ANDROID]
    add('Backend explicitly targets the approved test project', b.get('ASTROGATEWAY_ENVIRONMENT')=='test' and b.get('ASTROGATEWAY_APPROVED_PROJECT_REF')==REF)
    add('All clients target the approved HTTPS Supabase origin', all(v==URL for v in [b.get('SUPABASE_URL'),w.get('VITE_SUPABASE_URL'),a.get('supabase.url')]))
    keys = [b.get('SUPABASE_PUBLISHABLE_KEY',''),w.get('VITE_SUPABASE_PUBLISHABLE_KEY',''),a.get('supabase.publishableKey','')]
    add('App configuration uses one matching modern publishable key', len(set(keys))==1 and keys[0].startswith('sb_publishable_') and len(keys[0])>25 and 'REPLACE' not in keys[0])
    add('Backend issuer matches the approved project', b.get('SUPABASE_JWT_ISSUER')==URL+'/auth/v1')
    add('Direct database credentials and retired DB TLS options are absent', not b.get('DATABASE_URL') and not b.get('DATABASE_SSL_ROOT_CERT') and b.get('DATABASE_ALLOW_LEGACY_CA')=='false')
    add('Web uses the local same-origin API proxy', w.get('VITE_AUTH_API_BASE')=='/account-api')
    try:
        valid_backend = backend_url_valid(a.get('auth.backendUrl',''))
    except ValueError:
        valid_backend = False
    add('Android API URL uses HTTPS or the debug emulator endpoint', valid_backend)
    for relative in configs:
        add(relative + ': no secret/service-role/private-key marker', not any(marker in value for value in configs[relative].values() for marker in ('sb_secret_','-----BEGIN','postgres://','postgresql://')))
    return checks, keys[0] if all(x['status']=='PASS' for x in checks) else None


class NoRedirect(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, req, fp, code, msg, headers, newurl):
        return None


def provider_status(key):
    request = urllib.request.Request(URL+'/auth/v1/settings', headers={'apikey':key, 'User-Agent':'AstroGateway-test-config-check'})
    try:
        with urllib.request.build_opener(NoRedirect()).open(request, timeout=15) as response:
            return {'http_status': response.status, 'error': None}
    except urllib.error.HTTPError as exc:
        return {'http_status': exc.code, 'error': None}
    except urllib.error.URLError as exc:
        reason = exc.reason
        category = 'DNS_LOOKUP_FAILED' if isinstance(reason, socket.gaierror) else 'TLS_VALIDATION_FAILED' if isinstance(reason, ssl.SSLError) else 'NETWORK_ERROR'
        return {'http_status': None, 'error': category}
    except (OSError, ValueError):
        return {'http_status': None, 'error': 'NETWORK_ERROR'}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--check-provider',action='store_true',help='Read only the approved project public Auth settings endpoint; no account or database changes')
    parser.add_argument('--report',type=Path,help='Write sanitized results only; no configuration values')
    args = parser.parse_args()
    checks,key = audit(ROOT)
    report = {'scope':'approved Supabase test configuration only','checks':checks,
              'admin_credentials_read':False,'credentials_in_output':False,
              'not_verified':['Service/admin filesystem isolation','Live login, refresh, callbacks and application integration','Database migration/RLS deployment; local SQL checks pass; hosted deployment remains unverified']}
    if args.check_provider and key:
        actual = provider_status(key)
        control = provider_status('invalid-diagnostic-key')
        report['provider_read_only_check'] = {'configured_key':actual,'invalid_key_control':control,
                                            'status':'PASS' if actual['http_status']==200 and control['http_status'] in (401,403) else 'UNVERIFIED'}
    report['offline_configuration'] = 'PASS' if key else 'FAIL'
    if args.report:
        args.report.parent.mkdir(parents=True,exist_ok=True)
        fd=os.open(args.report,os.O_WRONLY|os.O_CREAT|os.O_TRUNC,0o600)
        with os.fdopen(fd,'w') as out:
            json.dump(report,out,indent=2);out.write('\n')
    print(json.dumps(report,indent=2))
    if not key:
        return 1
    return 2 if args.check_provider and report.get('provider_read_only_check',{}).get('status')!='PASS' else 0


if __name__=='__main__':
    raise SystemExit(main())
