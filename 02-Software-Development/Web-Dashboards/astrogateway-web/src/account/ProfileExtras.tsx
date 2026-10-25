import { useEffect, useState } from 'react'
import { client } from './client'
import { useAccount } from './accountState'
export const usernamePattern = /^[a-z][a-z0-9_]{2,23}$/
export function normalizeUsername(value: string) {
  const clean = value.trim().toLowerCase()
  if (!usernamePattern.test(clean)) throw new Error('Use 3–24 letters, numbers or underscores; start with a letter.')
  return clean
}
export function validatePhoto(file: File) {
  if (!['image/jpeg','image/png','image/webp'].includes(file.type) || file.size < 1 || file.size > 2 * 1024 * 1024) throw new Error('Choose a PNG, JPEG or WebP image up to 2 MB.')
}
// Decode and re-encode as a small PNG; do not preserve EXIF or upload SVG/HTML.
export async function photoPng(file: File): Promise<Blob> {
  validatePhoto(file)
  const image = await createImageBitmap(file)
  try {
    if (image.width < 1 || image.height < 1 || image.width > 8192 || image.height > 8192 || image.width * image.height > 16000000) throw new Error('Choose an image smaller than 16 megapixels.')
    const canvas = document.createElement('canvas'); canvas.width = canvas.height = 512
    const context = canvas.getContext('2d'); if (!context) throw new Error('Image editing is unavailable.')
    const edge = Math.min(image.width,image.height)
    context.drawImage(image,(image.width-edge)/2,(image.height-edge)/2,edge,edge,0,0,512,512)
    return await new Promise((resolve,reject) => canvas.toBlob(blob => blob && blob.size <= 2097152 ? resolve(blob) : reject(new Error('Could not prepare image.')), 'image/png'))
  } finally { image.close() }
}
export function ProfileExtras() {
  const account = useAccount(); const id = account.profile!.user_id
  const [username,setUsername] = useState(account.profile!.username ?? '')
  const [image,setImage] = useState(''); const [busy,setBusy] = useState(false); const [error,setError] = useState(''); const [message,setMessage] = useState('')
  const [version,setVersion] = useState(0)
  useEffect(() => {
    let active = true
    const path = account.profile?.avatar_path
    setImage('')
    if (!client || path !== id+'/avatar.png') return
    const refresh = () => { void client!.storage.from('profile-photos').createSignedUrl(path,120).then(({data,error}) => { if(active) { setImage(data?.signedUrl ?? ''); if(error) setError('Profile photo unavailable. Retry when connected.') } }) }
    refresh(); const timer = setInterval(refresh,90000)
    return () => { active=false; clearInterval(timer) }
  },[id,account.profile?.avatar_path,version])
  async function update(operation: () => Promise<void>) {
    if(busy || !client) return
    setBusy(true);setError('');setMessage('')
    try { await operation(); await account.reload(); setVersion(v=>v+1); setMessage('Profile updated.') }
    catch(error) { setError(error instanceof Error ? error.message : 'Profile update failed.') }
    finally { setBusy(false) }
  }
  return <section className="ag-account-extras" aria-label="Username and profile photo">
    <h2>Username and photo</h2><p>One account and password. After email verification, choose a username to sign in on the web using either your email or username.</p>
    {error && <p role="alert">{error}</p>}{message && <p role="status">{message}</p>}
    {image ? <img src={image} alt="Your profile" width="112" height="112" style={{borderRadius:'50%',objectFit:'cover'}}/> : <div className="ag-avatar-placeholder" aria-label="No profile photo">{account.profile!.display_name.slice(0,1).toUpperCase()}</div>}
    <label>Change profile photo<input type="file" accept="image/png,image/jpeg,image/webp" disabled={busy} onChange={event=>{
      const file=event.target.files?.[0];event.target.value='';if(!file)return
      void update(async()=>{
        const blob=await photoPng(file);const path=id+'/avatar.png'
        const uploaded=await client!.storage.from('profile-photos').upload(path,blob,{upsert:true,contentType:'image/png',cacheControl:'0'})
        if(uploaded.error)throw new Error('Photo upload failed. Check storage access and retry.')
        const saved=await client!.schema('mission_api').rpc('set_avatar',{p_avatar_path:path,p_expected_user:id})
        if(saved.error)throw new Error('Photo uploaded, but profile update failed. Retry to link it.')
      })
    }}/></label><small>PNG, JPEG or WebP, up to 2 MB. Cropped to a square; visible only to your account.</small>
    <button className="ag-secondary" disabled={busy || !account.profile?.avatar_path} onClick={()=>{ void update(async()=>{
      // Clear profile first: failure cannot leave a profile pointing to a deleted object.
      const saved=await client!.schema('mission_api').rpc('set_avatar',{p_avatar_path:null,p_expected_user:id});if(saved.error)throw new Error('Could not remove photo.')
      const removed=await client!.storage.from('profile-photos').remove([id+'/avatar.png']);if(removed.error)throw new Error('Photo hidden; stored image cleanup failed. Upload again or contact the operator.')
    }) }}>Remove photo</button>
    <form onSubmit={event=>{event.preventDefault();void update(async()=>{
      const clean=normalizeUsername(username)
      const result=await client!.schema('mission_api').rpc('set_username',{p_username:clean,p_expected_user:id})
      if(result.error)throw new Error(result.error.code==='PA409'?'That username is unavailable.':'Could not save username. Check access and retry.')
      setUsername(clean)
    })}}><label>Username<input value={username} onChange={event=>setUsername(event.target.value)} autoComplete="username" minLength={3} maxLength={24} required disabled={busy}/></label><small>3–24 lowercase letters, numbers or underscores. Start with a letter. Your account ID stays the same when this changes.</small><button className="ag-primary" disabled={busy}>Save username</button></form>
  </section>
}
