-- Read-only metadata inspection, only after approved migration/provisioning.
-- Missing objects are a failed acceptance check, not a reason to auto-create them.
BEGIN TRANSACTION READ ONLY;
SELECT current_database(),current_user,current_setting('server_version');
SELECT rolname,rolcanlogin,rolsuper,rolcreatedb,rolcreaterole,rolreplication,rolbypassrls
 FROM pg_roles WHERE rolname IN ('astrogateway_api','astrogateway_runtime');
SELECT granted.rolname AS granted_role,member.rolname AS member_role,
 m.admin_option,m.inherit_option,m.set_option
 FROM pg_auth_members m JOIN pg_roles granted ON granted.oid=m.roleid
 JOIN pg_roles member ON member.oid=m.member
 WHERE granted.rolname IN ('astrogateway_api','astrogateway_runtime')
 OR member.rolname IN ('astrogateway_api','astrogateway_runtime');
SELECT n.nspname,c.relname,pg_get_userbyid(c.relowner) AS owner,
 c.relrowsecurity,c.relforcerowsecurity
 FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace
 WHERE n.nspname='training' AND c.relkind='r';
SELECT schemaname,tablename,policyname,roles,cmd,qual,with_check
 FROM pg_policies WHERE schemaname='training';
SELECT p.proname,pg_get_userbyid(p.proowner) AS owner,p.prosecdef,p.proconfig
 FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace
 WHERE n.nspname='training';
SELECT tgname FROM pg_trigger WHERE tgrelid=to_regclass('auth.users') AND NOT tgisinternal;
SELECT n.nspname,a.privilege_type FROM pg_namespace n
 CROSS JOIN LATERAL aclexplode(COALESCE(n.nspacl,acldefault('n',n.nspowner))) a
 WHERE a.grantee=0 AND n.nspname NOT LIKE 'pg_%';
COMMIT;
