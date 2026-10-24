"""Private PostgreSQL repository. Every transaction uses a non-owner role and local identity context."""
from contextlib import asynccontextmanager
from datetime import datetime, timezone
from uuid import UUID
import asyncpg
import hashlib
import ssl
from pathlib import Path
from urllib.parse import urlparse, parse_qs
from fastapi import HTTPException

class Database:
    def __init__(self,pool):self.pool=pool
    @asynccontextmanager
    async def transaction(self,user):
        async with self.pool.acquire() as connection:
            async with connection.transaction():
                await connection.execute('SET LOCAL ROLE astrogateway_api')
                await connection.fetchval("SELECT set_config('astrogateway.user_id',$1,true)",str(user.user_id))
                await connection.fetchval("SELECT set_config('astrogateway.auth_session_id',$1,true)",str(user.auth_session_id))
                yield connection
        # Commit/rollback and pool reset erase the settings and role before reuse.
    async def revoked(self,user):
        async with self.transaction(user) as c:
            return await c.fetchval('SELECT EXISTS(SELECT 1 FROM training.auth_session_revocations WHERE auth_session_id=$1 AND user_id=$2 AND expires_at>now())',user.auth_session_id,user.user_id)
    async def profile(self,user):
        async with self.transaction(user) as c:
            row=await c.fetchrow('SELECT user_id,display_name,trainee_profile,training_level,created_at FROM training.profiles WHERE user_id=$1',user.user_id)
            if row is None:raise HTTPException(404,'Profile unavailable. Complete account configuration.')
            return dict(row)
    async def initialize(self,user,payload):
        try:
            async with self.transaction(user) as c:
                row=await c.fetchrow('''INSERT INTO training.mission_sessions(user_id,auth_session_id,launch_id,mission_id)
                    VALUES($1,$2,$3,$4) RETURNING *''',user.user_id,user.auth_session_id,payload.launch_id,payload.mission_id)
                return dict(row)
        except asyncpg.UniqueViolationError:
            raise HTTPException(409,'Mission launch already exists. Start a new session.') from None
        except asyncpg.InsufficientPrivilegeError:
            raise HTTPException(403,'Mission authorization failed.') from None
    async def complete(self,user,session_id:UUID,payload):
        async with self.transaction(user) as c:
            row=await c.fetchrow('SELECT * FROM training.mission_sessions WHERE id=$1 AND user_id=$2 FOR UPDATE',session_id,user.user_id)
            if row is None:raise HTTPException(404,'Mission session unavailable.')
            if row['auth_session_id']!=user.auth_session_id or row['launch_id']!=payload.launch_id:raise HTTPException(409,'Mission launch or authenticated session does not match.')
            if row['status']=='integration_check_complete':return dict(row)
            if row['expires_at']<=datetime.now(timezone.utc):raise HTTPException(409,'Mission expired. Start a new session.')
            updated=await c.fetchrow('''UPDATE training.mission_sessions SET status='integration_check_complete',completed_at=now()
                WHERE id=$1 AND user_id=$2 AND auth_session_id=$3 AND launch_id=$4 AND status='initialized' AND expires_at>now() RETURNING *''',session_id,user.user_id,user.auth_session_id,payload.launch_id)
            if updated is None:raise HTTPException(409,'Mission session expired or changed.')
            return dict(updated)
    async def revoke(self,user):
        async with self.transaction(user) as c:
            await c.execute('''INSERT INTO training.auth_session_revocations(auth_session_id,user_id,expires_at)
                VALUES($1,$2,$3) ON CONFLICT(auth_session_id) DO NOTHING''',user.auth_session_id,user.user_id,user.expires_at)

# Public CA fingerprint, never a credential. Scoped legacy-profile exception only.
LEGACY_CA_SHA256 = '700723581420dd1ac98fd7e9ac529f0ef210eadcaf87fc868a3ad7d114c2f3b7'

def verified_ssl_context(dsn, ca_file, allow_legacy=False):
    parsed=urlparse(dsn)
    if parsed.scheme not in ('postgres','postgresql') or not parsed.hostname or parsed.hostname.startswith('/'):
        raise ValueError('A TCP PostgreSQL endpoint is required.')
    if parsed.username not in ('astrogateway_runtime',) and not (parsed.username or '').startswith('astrogateway_runtime.'):
        raise ValueError('Only the dedicated runtime login is supported.')
    query=parse_qs(parsed.query,keep_blank_values=True)
    if any(k.startswith('ssl') and (k!='sslmode' or v!=['verify-full']) for k,v in query.items()):
        raise ValueError('TLS configuration belongs to the verified runtime context.')
    path=Path(ca_file)
    if not ca_file or not path.is_file():raise ValueError('An explicit approved CA file is required.')
    ctx=ssl.create_default_context(cafile=str(path))
    ctx.minimum_version=ssl.TLSVersion.TLSv1_2
    ctx.verify_mode=ssl.CERT_REQUIRED
    ctx.check_hostname=True
    ctx.verify_flags |= ssl.VERIFY_X509_STRICT
    if allow_legacy:
        if hashlib.sha256(path.read_bytes()).hexdigest()!=LEGACY_CA_SHA256:
            raise ValueError('Legacy profile exception is limited to the reviewed CA.')
        ctx.verify_flags &= ~ssl.VERIFY_X509_STRICT
    return ctx

async def enforce_role_boundary(connection):
    # Fail closed for this dedicated architecture; provider/admin memberships are forbidden.
    login=await connection.fetchval('SELECT current_user')
    if login!='astrogateway_runtime':raise RuntimeError('Unexpected runtime role.')
    members=await connection.fetch('''SELECT parent.rolname,m.admin_option,m.inherit_option,m.set_option
        FROM pg_auth_members m JOIN pg_roles parent ON parent.oid=m.roleid
        JOIN pg_roles child ON child.oid=m.member WHERE child.rolname=$1''',login)
    if len(members)!=1 or members[0]['rolname']!='astrogateway_api' or members[0]['admin_option'] or members[0]['inherit_option'] or not members[0]['set_option']:
        raise RuntimeError('Unexpected role membership options.')
    if await connection.fetchval("SELECT EXISTS(SELECT 1 FROM pg_auth_members WHERE member=(SELECT oid FROM pg_roles WHERE rolname='astrogateway_api'))"):
        raise RuntimeError('API role must not inherit other roles.')
    for name in (login,'astrogateway_api'):
        role=await connection.fetchrow('SELECT rolcanlogin,rolinherit,rolsuper,rolbypassrls,rolcreaterole,rolcreatedb,rolreplication FROM pg_roles WHERE rolname=$1',name)
        if not role or any(role[k] for k in ('rolsuper','rolbypassrls','rolcreaterole','rolcreatedb','rolreplication')) or role['rolinherit'] or role['rolcanlogin']!=(name==login):
            raise RuntimeError('Unsafe database role attributes.')
        if await connection.fetchval('''SELECT EXISTS(SELECT 1 FROM pg_class WHERE relowner=(SELECT oid FROM pg_roles WHERE rolname=$1))
            OR EXISTS(SELECT 1 FROM pg_namespace WHERE nspowner=(SELECT oid FROM pg_roles WHERE rolname=$1))
            OR EXISTS(SELECT 1 FROM pg_proc WHERE proowner=(SELECT oid FROM pg_roles WHERE rolname=$1))
            OR EXISTS(SELECT 1 FROM pg_database WHERE datdba=(SELECT oid FROM pg_roles WHERE rolname=$1))''',name):
            raise RuntimeError('Runtime/API must not own database objects.')
        if await connection.fetchval("SELECT has_database_privilege($1,current_database(),'CREATE') OR has_database_privilege($1,current_database(),'TEMP')",name):
            raise RuntimeError('Runtime/API cannot create schemas or temporary objects.')
        if await connection.fetchval('''SELECT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname NOT IN ('training','pg_catalog','information_schema')
            AND left(nspname,3) <> 'pg_' AND (has_schema_privilege($1,oid,'USAGE') OR has_schema_privilege($1,oid,'CREATE')))''',name):
            raise RuntimeError('Unrelated schema privileges remain.')
        if await connection.fetchval("SELECT has_schema_privilege($1,'training','CREATE')",name):
            raise RuntimeError('Training schema is not writable by runtime roles.')
    await connection.execute('SET ROLE astrogateway_api')
    try:
        if await connection.fetchval("SELECT current_user")!='astrogateway_api':raise RuntimeError('Role switch failed.')
        if await connection.fetchval("SELECT count(*) FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace WHERE n.nspname='training' AND c.relkind='r' AND c.relrowsecurity")!=3:
            raise RuntimeError('Expected RLS tables missing.')
    finally:await connection.execute('RESET ROLE')

async def create_restricted_pool(dsn, ca_file='', allow_legacy=False):
    pool=None
    try:
        context=verified_ssl_context(dsn,ca_file,allow_legacy)
        pool=await asyncpg.create_pool(dsn,min_size=1,max_size=5,command_timeout=10,ssl=context,statement_cache_size=0)
        async with pool.acquire() as connection:await enforce_role_boundary(connection)
        return pool
    except Exception:
        if pool:await pool.close()
        # Never emit the DSN, password or underlying driver exception.
        raise RuntimeError('Restricted database connection failed. Review server-only credentials, role grants and verified TLS.') from None
