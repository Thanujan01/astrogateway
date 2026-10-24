from pathlib import Path
import subprocess,os
root=Path('work/stage05b1-hardening/tls');root.mkdir(mode=0o700,exist_ok=True)
def run(*cmd):
 subprocess.run(['openssl',*cmd],check=True,stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL)
(root/'ca.cnf').write_text('[req]\ndistinguished_name=dn\nx509_extensions=ca\nprompt=no\n[dn]\nCN=AstroGateway Disposable CA\n[ca]\nbasicConstraints=critical,CA:true\nkeyUsage=critical,keyCertSign,cRLSign\nsubjectKeyIdentifier=hash\nauthorityKeyIdentifier=keyid:always\n')
(root/'leaf.cnf').write_text('[leaf]\nbasicConstraints=critical,CA:false\nkeyUsage=critical,digitalSignature,keyEncipherment\nextendedKeyUsage=serverAuth\nsubjectAltName=DNS:localhost\nsubjectKeyIdentifier=hash\nauthorityKeyIdentifier=keyid\n')
run('req','-x509','-newkey','rsa:2048','-nodes','-days','2','-config',str(root/'ca.cnf'),'-keyout',str(root/'ca.key'),'-out',str(root/'ca.crt'))
run('req','-new','-newkey','rsa:2048','-nodes','-subj','/CN=localhost','-keyout',str(root/'leaf.key'),'-out',str(root/'leaf.csr'))
run('x509','-req','-in',str(root/'leaf.csr'),'-CA',str(root/'ca.crt'),'-CAkey',str(root/'ca.key'),'-CAcreateserial','-days','2','-extfile',str(root/'leaf.cnf'),'-extensions','leaf','-out',str(root/'leaf.crt'))
for p in root.glob('*.key'):p.chmod(0o600)
print('Disposable TLS certificates generated locally; private keys stay in work/.')
