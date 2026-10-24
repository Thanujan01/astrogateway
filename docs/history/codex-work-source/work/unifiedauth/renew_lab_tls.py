from pathlib import Path
import subprocess,json,os
p=Path('work/unifiedauth/tls').resolve();p.mkdir(mode=0o700)
def run(*args):subprocess.run(['openssl',*args],check=True,stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL)
(p/'ca.cnf').write_text('[req]\ndistinguished_name=dn\nx509_extensions=ca\nprompt=no\n[dn]\nCN=AstroGateway UnifiedAuth Disposable CA\n[ca]\nbasicConstraints=critical,CA:true\nkeyUsage=critical,keyCertSign,cRLSign\nsubjectKeyIdentifier=hash\nauthorityKeyIdentifier=keyid:always\n')
(p/'leaf.cnf').write_text('[leaf]\nbasicConstraints=critical,CA:false\nkeyUsage=critical,digitalSignature,keyEncipherment\nextendedKeyUsage=serverAuth\nsubjectAltName=DNS:localhost\nsubjectKeyIdentifier=hash\nauthorityKeyIdentifier=keyid\n')
run('req','-x509','-newkey','rsa:2048','-nodes','-days','7','-config',str(p/'ca.cnf'),'-keyout',str(p/'ca.key'),'-out',str(p/'ca.crt'))
run('req','-new','-newkey','rsa:2048','-nodes','-subj','/CN=localhost','-keyout',str(p/'leaf.key'),'-out',str(p/'leaf.csr'))
run('x509','-req','-in',str(p/'leaf.csr'),'-CA',str(p/'ca.crt'),'-CAkey',str(p/'ca.key'),'-CAcreateserial','-days','7','-extfile',str(p/'leaf.cnf'),'-extensions','leaf','-out',str(p/'leaf.crt'))
for f in p.glob('*.key'):f.chmod(0o600)
cfg=json.loads(Path('work/stage05b2/private/lab.json').read_text());assert cfg['cluster'].endswith('work/stage05b2/private/cluster')
cluster=Path(cfg['cluster']);conf=cluster/'postgresql.conf';conf.write_text(conf.read_text()+f"\n# UnifiedAuth disposable lab TLS renewal only\nssl_cert_file='{p/'leaf.crt'}'\nssl_key_file='{p/'leaf.key'}'\n")
subprocess.run(['work/stage05b2/pg-install/bin/pg_ctl','-D',str(cluster),'reload'],check=True)
# New private gateway config, preserving prior fixture configuration.
old=Path('work/stage05b2/private/postgrest.conf').read_text();new=Path('work/unifiedauth/postgrest.local.conf');new.write_text(old.replace(cfg['ca_file'],str(p/'ca.crt')));new.chmod(0o600)
print('New localhost-only CA and leaf generated; verified TLS remains required.')
