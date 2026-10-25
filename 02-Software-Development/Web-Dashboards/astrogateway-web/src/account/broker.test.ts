/// <reference types="node" />
import { beforeEach, expect, it, vi } from 'vitest'
import { webcrypto } from 'node:crypto'
import { makeHandler, type BrokerConfig } from '../../../../Auth-Broker/supabase/functions/account-login/handler'
const config:BrokerConfig={url:'https://jvkwofmxtxjhdwtwdboz.supabase.co',publicKey:'sb_publishable_synthetic_fixture_only',serviceKey:'synthetic-operator-fixture-only',captchaSecret:'synthetic-challenge-fixture',pepper:'synthetic-private-pepper-for-tests-only',origins:['https://app.example.test']}
const json=(data:unknown,status=200)=>new Response(JSON.stringify(data),{status})
const request=(body:unknown={identifier:'commander',password:'synthetic-password',captchaToken:'synthetic-challenge'})=>new Request(config.url+'/functions/v1/account-login',{method:'POST',headers:{Origin:config.origins[0]},body:JSON.stringify(body)})
let transport=vi.fn<(input:string,init:RequestInit)=>Promise<Response>>()
beforeEach(()=>{vi.stubGlobal('crypto',webcrypto);transport=vi.fn<(input:string,init:RequestInit)=>Promise<Response>>().mockResolvedValueOnce(json({success:true,action:'username-login',hostname:'app.example.test'})).mockResolvedValueOnce(json({email:'private-fixture@example.test'})).mockResolvedValueOnce(json({access_token:'synthetic-access',refresh_token:'synthetic-refresh'}))})
it('successful username login returns only caller tokens',async()=>{const r=await makeHandler(config,transport)(request());expect(r.status).toBe(200);expect(await r.json()).toEqual({access_token:'synthetic-access',refresh_token:'synthetic-refresh'});expect(new Headers(transport.mock.calls[2][1].headers).get('apikey')).toBe(config.publicKey);expect(transport.mock.calls[2][1].body).not.toContain(config.serviceKey)})
it('resolves only after verified challenge and hashes identifier for throttle',async()=>{await makeHandler(config,transport)(request());expect(transport.mock.calls[0][0]).toContain('siteverify');const lookup=JSON.parse(String(transport.mock.calls[1][1].body));expect(lookup.p_key_hash).toMatch(/^[a-f0-9]{64}$/);expect(lookup.p_key_hash).not.toContain('commander')})
it('rejects unknown fields before upstream transmission',async()=>{const r=await makeHandler(config,transport)(request({identifier:'commander',password:'synthetic',captchaToken:'fixture',role:'admin'}));expect(r.status).toBe(400);expect(transport).not.toHaveBeenCalled()})
it('fails closed without operator secrets',async()=>{const r=await makeHandler({...config,serviceKey:''},transport)(request());expect(r.status).toBe(503);expect(transport).not.toHaveBeenCalled()})
it('rejects unapproved browser origin',async()=>{const r=await makeHandler(config,transport)(new Request(config.url,{method:'POST',headers:{Origin:'https://other.example.test'}}));expect(r.status).toBe(403);expect(transport).not.toHaveBeenCalled()})
it('never resolves identity on challenge failure',async()=>{transport.mockReset().mockResolvedValue(json({success:false}));expect((await makeHandler(config,transport)(request())).status).toBe(401);expect(transport).toHaveBeenCalledTimes(1)})
it('stops before password grant when throttled',async()=>{transport.mockReset().mockResolvedValueOnce(json({success:true,action:'username-login',hostname:'app.example.test'})).mockResolvedValueOnce(json({limited:true}));expect((await makeHandler(config,transport)(request())).status).toBe(429);expect(transport).toHaveBeenCalledTimes(2)})
it('unknown username and wrong password have identical public error',async()=>{
 const run=async(email:string|null)=>{const t=vi.fn().mockResolvedValueOnce(json({success:true,action:'username-login',hostname:'app.example.test'})).mockResolvedValueOnce(json({email})).mockResolvedValueOnce(json({error:'private-upstream-detail'},400));const r=await makeHandler(config,t)(request());return [r.status,await r.text()]}
 expect(await run(null)).toEqual(await run('private-fixture@example.test'))
})
it('sanitizes upstream errors',async()=>{transport.mockReset().mockRejectedValue(new Error('sensitive-fixture-canary'));const r=await makeHandler(config,transport)(request());expect(r.status).toBe(503);expect(await r.text()).not.toContain('canary')})
it('bounds streamed request bodies',async()=>{const r=await makeHandler(config,transport)(request({identifier:'x'.repeat(9000)}));expect(r.status).toBe(413);expect(transport).not.toHaveBeenCalled()})
