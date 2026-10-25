import { useEffect, useRef } from 'react'
import { client } from './client'
import { normalizeUsername } from './ProfileExtras'
interface Turnstile { render: (node: HTMLElement, options: object) => string; remove: (id: string) => void }
declare global { interface Window { turnstile?: Turnstile } }
const siteKey = import.meta.env.VITE_ACCOUNT_TURNSTILE_SITE_KEY ?? ''
let script: Promise<void> | null = null
export function UsernameChallenge({ onToken }: { onToken: (token: string) => void }) {
  const container=useRef<HTMLDivElement>(null); const callback=useRef(onToken);callback.current=onToken
  useEffect(()=>{
    if(!siteKey)return
    let active=true;let widget:string|undefined
    if(!script)script=new Promise((resolve,reject)=>{
      if(window.turnstile){resolve();return}
      const tag=document.createElement('script');tag.src='https://challenges.cloudflare.com/turnstile/v0/api.js?render=explicit';tag.async=true
      tag.onload=()=>resolve();tag.onerror=()=>{script=null;reject(new Error('Challenge unavailable'))};document.head.appendChild(tag)
    })
    void script.then(()=>{if(active&&container.current&&window.turnstile)widget=window.turnstile.render(container.current,{sitekey:siteKey,action:'username-login',callback:(token:string)=>callback.current(token),'expired-callback':()=>callback.current(''),'error-callback':()=>callback.current('')})}).catch(()=>callback.current(''))
    return()=>{active=false;if(widget)window.turnstile?.remove(widget)}
  },[])
  return siteKey ? <div ref={container} aria-label="Username login verification"/> : <p role="status">Username login requires the operator to configure account verification. You can still sign in with your email.</p>
}
export async function loginWithUsername(identifier:string,password:string,captchaToken:string) {
  if(!client || !siteKey || !captchaToken)throw new Error('Complete username login verification, or sign in with your email.')
  const username=normalizeUsername(identifier)
  const {data,error}=await client.functions.invoke('account-login',{body:{identifier:username,password,captchaToken}})
  if(error || !data || typeof data.access_token!=='string' || typeof data.refresh_token!=='string')throw new Error('Sign-in failed. Check your account details and verification, or retry later.')
  const restored=await client.auth.setSession({access_token:data.access_token,refresh_token:data.refresh_token})
  if(restored.error)throw new Error('Could not restore your account session. Sign in again.')
}
