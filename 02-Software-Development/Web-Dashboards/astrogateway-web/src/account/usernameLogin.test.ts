import { beforeEach, expect, it, vi } from 'vitest'
const fixture=vi.hoisted(()=>{vi.stubEnv('VITE_ACCOUNT_TURNSTILE_SITE_KEY','synthetic-public-site-key');return {invoke:vi.fn(),setSession:vi.fn()}})
vi.mock('./client',()=>({client:{functions:{invoke:fixture.invoke},auth:{setSession:fixture.setSession}}}))
import { loginWithUsername } from './UsernameLogin'
beforeEach(()=>{fixture.invoke.mockReset().mockResolvedValue({data:{access_token:'synthetic-access',refresh_token:'synthetic-refresh'},error:null});fixture.setSession.mockReset().mockResolvedValue({error:null})})
it('uses the broker and restores the same Supabase session',async()=>{await loginWithUsername(' Commander ','synthetic-password','synthetic-challenge');expect(fixture.invoke).toHaveBeenCalledWith('account-login',{body:{identifier:'commander',password:'synthetic-password',captchaToken:'synthetic-challenge'}});expect(fixture.setSession).toHaveBeenCalledWith({access_token:'synthetic-access',refresh_token:'synthetic-refresh'})})
it('sends nothing before verification',async()=>{await expect(loginWithUsername('commander','synthetic','')).rejects.toThrow('verification');expect(fixture.invoke).not.toHaveBeenCalled()})
it('does not restore session on failed password',async()=>{fixture.invoke.mockResolvedValue({error:{private:'fixture'}});await expect(loginWithUsername('commander','synthetic','challenge')).rejects.toThrow('Sign-in failed');expect(fixture.setSession).not.toHaveBeenCalled()})
it('reports failed session restoration',async()=>{fixture.setSession.mockResolvedValue({error:{}});await expect(loginWithUsername('commander','synthetic','challenge')).rejects.toThrow('restore')})
