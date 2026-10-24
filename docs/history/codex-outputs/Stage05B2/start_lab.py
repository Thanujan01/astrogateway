import json,secrets,subprocess,os,socket,base64,hashlib,shutil,time
from pathlib import Path
root=Path('work/stage05b2').resolve();private=root/'private';private.mkdir(mode=0o700,exist_ok=True)
pg=root/'pg-install/bin';cluster=private/'cluster';tls=Path('work/stage05b1-hardening/tls').resolve()
if cluster.exists() and ((private/'lab.json').exists() or (cluster/'postmaster.pid').exists()):raise SystemExit('Refusing to overwrite an initialized/running lab.')
admin_password=secrets.token_urlsafe(32);gateway_password=secrets.token_urlsafe(32);old_password=secrets.token_urlsafe(32)
password_file=private/'admin-password';
if cluster.exists():admin_password=password_file.read_text()
else:password_file.write_text(admin_password);password_file.chmod(0o600)
with socket.socket() as sock:sock.bind(('127.0.0.1',0));port=sock.getsockname()[1]
with socket.socket() as sock:sock.bind(('127.0.0.1',0));rest_port=sock.getsockname()[1]
if not cluster.exists():
 subprocess.run([str(pg/'initdb'),'-D',str(cluster),'-L',str(root/'pg-install/share/postgresql'),'-U','lab_admin','--auth-host=scram-sha-256','--auth-local=scram-sha-256','--pwfile='+str(password_file),'--no-locale','-E','UTF8'],check=True,stdout=(root/'initdb.log').open('w'),stderr=subprocess.STDOUT)
(cluster/'postgresql.conf').write_text((cluster/'postgresql.conf').read_text()+f"\nlisten_addresses='localhost'\nport={port}\nunix_socket_directories=''\nssl=on\nssl_cert_file='{tls/'leaf.crt'}'\nssl_key_file='{tls/'leaf.key'}'\nlog_statement='none'\nlog_min_error_statement='panic'\nlog_parameter_max_length_on_error=0\nlog_connections=off\nlog_disconnections=off\n")
(cluster/'pg_hba.conf').write_text('hostssl all all 127.0.0.1/32 scram-sha-256\nhostssl all all ::1/128 scram-sha-256\nhostnossl all all 127.0.0.1/32 reject\nhostnossl all all ::1/128 reject\n')
subprocess.run([str(pg/'pg_ctl'),'-D',str(cluster),'-l',str(private/'postgres.log'),'-w','start'],check=True,stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL)
key=private/'issuer.key';pub=private/'issuer.pub'
subprocess.run(['openssl','genrsa','-out',str(key),'2048'],check=True,stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL);key.chmod(0o600)
subprocess.run(['openssl','rsa','-in',str(key),'-pubout','-out',str(pub)],check=True,stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL)
mod=subprocess.run(['openssl','rsa','-pubin','-in',str(pub),'-noout','-modulus'],check=True,capture_output=True,text=True).stdout.strip().split('=',1)[1]
encode=lambda b:base64.urlsafe_b64encode(b).decode().rstrip('=')
(public_jwks:=private/'public-jwks.json').write_text(json.dumps({'keys':[{'kty':'RSA','alg':'RS256','kid':'disposable-stage05b2','use':'sig','key_ops':['verify'],'n':encode(bytes.fromhex(mod)),'e':'AQAB'}]}))
config={'port':port,'rest_port':rest_port,'admin_password':admin_password,'gateway_password':gateway_password,'old_password':old_password,'ca_file':str(tls/'ca.crt'),'issuer_key':str(key),'public_jwks':str(public_jwks),'cluster':str(cluster)}
p=private/'lab.json';p.write_text(json.dumps(config));p.chmod(0o600)
print('Disposable native PostgreSQL cluster started with SCRAM and verified local TLS. Fixture credentials remain private.')
