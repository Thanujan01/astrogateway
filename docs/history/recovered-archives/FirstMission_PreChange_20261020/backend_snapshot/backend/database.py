"""User-scoped PostgREST repository. No SQL pool, DB password or identity-setting API."""
from fastapi import HTTPException

class Database:
    def __init__(self,http,settings):self.http,self.settings=http,settings
    async def rpc(self,user,name,body):
        if name not in ('session_revoked','current_profile','initialize_mission','complete_mission','revoke_session','update_display_name',
                        'submit_mission_command','mission_state','mission_report','skill_evidence'):
            raise RuntimeError('Unsupported data operation.')
        # Only the ORIGINAL user token crosses the independent JWT gateway.
        # Never forward incoming headers, synthesized claims, role/user/session overrides.
        try:
            response=await self.http.post(self.settings.supabase_url+'/rest/v1/rpc/'+name,
                headers={'apikey':self.settings.publishable_key,'Authorization':'Bearer '+user.access_token,
                         'Content-Profile':'mission_api','Accept-Profile':'mission_api'},json=body)
        except Exception:
            raise HTTPException(503,'Training data service unavailable.') from None
        if len(response.content)>65536:raise HTTPException(502,'Invalid training data response.')
        if response.status_code>=400:
            # SQL business codes are fixed constants. Never echo gateway/detail/log tokens.
            try:code=response.json().get('code')
            except (ValueError,AttributeError):code=None
            if response.status_code==401 or code=='PT401':raise HTTPException(401,'Session invalid or expired.')
            if code=='PT422':raise HTTPException(422,'Display name must contain 2–64 characters without control characters.')
            if code=='PC422':raise HTTPException(422,'Invalid mission command.')
            if code=='PT404':raise HTTPException(404,'Mission or profile unavailable.')
            if code=='PC409':raise HTTPException(409,'Mission revision or command conflict.')
            if code=='PC429':raise HTTPException(429,'Mission command limit reached.')
            if code in ('PT409','23505'):raise HTTPException(409,'Mission launch/session conflict.')
            if response.status_code==403 or code=='42501':raise HTTPException(403,'Training authorization failed.')
            if response.status_code==429:raise HTTPException(429,'Too many requests. Retry later.')
            raise HTTPException(503,'Training data service unavailable.')
        if response.status_code!=200:raise HTTPException(502,'Invalid training data response.')
        try:return response.json()
        except ValueError:raise HTTPException(502,'Invalid training data response.') from None
    async def revoked(self,user):
        value=await self.rpc(user,'session_revoked',{})
        if type(value) is not bool:raise HTTPException(502,'Invalid training data response.')
        return value
    async def profile(self,user):
        row=await self.rpc(user,'current_profile',{})
        if not isinstance(row,dict):raise HTTPException(502,'Invalid profile response.')
        return row
    async def update_profile(self,user,display_name):
        row=await self.rpc(user,'update_display_name',{'p_display_name':display_name})
        if not isinstance(row,dict) or str(row.get('user_id'))!=str(user.user_id):raise HTTPException(502,'Profile ownership validation failed.')
        return row
    async def initialize(self,user,payload):
        row=await self.rpc(user,'initialize_mission',{'p_launch_id':str(payload.launch_id),'p_mission_id':payload.mission_id})
        if not isinstance(row,dict):raise HTTPException(502,'Invalid mission response.')
        return row
    async def complete(self,user,session_id,payload):
        row=await self.rpc(user,'complete_mission',{'p_session_id':str(session_id),'p_launch_id':str(payload.launch_id),'p_outcome':payload.outcome})
        if not isinstance(row,dict):raise HTTPException(502,'Invalid mission response.')
        return row
    async def submit_command(self,user,session_id,command):
        row=await self.rpc(user,'submit_mission_command',{'p_session_id':str(session_id),'p_command_id':str(command.command_id),
            'p_expected_revision':command.expected_revision,'p_kind':command.kind,'p_payload':command.payload})
        if not isinstance(row,dict):raise HTTPException(502,'Invalid mission response.')
        return row
    async def mission_state(self,user,session_id):
        row=await self.rpc(user,'mission_state',{'p_session_id':str(session_id)})
        if not isinstance(row,dict):raise HTTPException(502,'Invalid mission response.')
        return row
    async def mission_report(self,user,session_id):
        row=await self.rpc(user,'mission_report',{'p_session_id':str(session_id)})
        if not isinstance(row,dict):raise HTTPException(502,'Invalid mission response.')
        return row
    async def skill_evidence(self,user):
        row=await self.rpc(user,'skill_evidence',{})
        if not isinstance(row,dict):raise HTTPException(502,'Invalid skill response.')
        return row
    async def revoke(self,user):
        if await self.rpc(user,'revoke_session',{}) is not True:raise HTTPException(502,'Invalid revocation response.')
