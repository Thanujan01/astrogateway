"""Private PostgreSQL repository. Every transaction uses a non-owner role and local identity context."""
from contextlib import asynccontextmanager
from datetime import datetime, timezone
from uuid import UUID
import asyncpg
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

async def create_restricted_pool(dsn):
    pool=None
    try:
        pool=await asyncpg.create_pool(dsn,min_size=1,max_size=5,command_timeout=10,ssl=True,statement_cache_size=0)
        async with pool.acquire() as connection:
            role=await connection.fetchrow('SELECT rolsuper,rolbypassrls FROM pg_roles WHERE rolname=current_user')
            membership=await connection.fetchval("SELECT pg_has_role(current_user,'astrogateway_api','MEMBER')")
            owns=await connection.fetchval("SELECT EXISTS(SELECT 1 FROM pg_class t JOIN pg_namespace s ON t.relnamespace=s.oid WHERE s.nspname='training' AND pg_get_userbyid(t.relowner)=current_user)")
            if role['rolsuper'] or role['rolbypassrls'] or owns or not membership:
                raise RuntimeError('Database login must be a non-owner, non-superuser, non-BYPASSRLS member of astrogateway_api.')
            await connection.execute('SET ROLE astrogateway_api')
            restricted=await connection.fetchrow('SELECT rolsuper,rolbypassrls FROM pg_roles WHERE rolname=current_user')
            api_owns=await connection.fetchval("SELECT EXISTS(SELECT 1 FROM pg_class t JOIN pg_namespace s ON t.relnamespace=s.oid WHERE s.nspname='training' AND pg_get_userbyid(t.relowner)=current_user)")
            if restricted['rolsuper'] or restricted['rolbypassrls'] or api_owns:raise RuntimeError('API role must enforce RLS.')
            await connection.execute('RESET ROLE')
        return pool
    except Exception:
        if pool:await pool.close()
        # Never emit the DSN, password or underlying driver exception.
        raise RuntimeError('Restricted database connection failed. Review server-only credentials, role grants and TLS.') from None
