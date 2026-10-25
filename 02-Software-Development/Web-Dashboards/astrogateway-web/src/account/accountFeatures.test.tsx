import { beforeEach, describe, expect, it, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { normalizeUsername, validatePhoto, ProfileExtras } from './ProfileExtras'
import { Context } from './accountState'
const fixture = vi.hoisted(()=>({rpc:vi.fn(), upload:vi.fn(), remove:vi.fn(), signed:vi.fn()}))
vi.mock('./client',()=>({client:{schema:()=>({rpc:fixture.rpc}),storage:{from:()=>({upload:fixture.upload,remove:fixture.remove,createSignedUrl:fixture.signed})}}}))
const user='11111111-1111-4111-8111-111111111111'
function page(avatar_path:string|null=null) {
  const reload=vi.fn(async()=>{})
  render(<Context.Provider value={{profile:{user_id:user,display_name:'Commander',username:null,avatar_path,trainee_profile:'astronaut_candidate',training_level:0,created_at:'2026-01-01'},session:null,loading:false,error:'',recovery:false,reload,logout:vi.fn(),saveName:vi.fn(),completeRecovery:vi.fn(),beginRecovery:vi.fn()}}><ProfileExtras/></Context.Provider>)
  return reload
}
beforeEach(()=>{fixture.rpc.mockReset().mockResolvedValue({error:null});fixture.remove.mockReset().mockResolvedValue({error:null});fixture.signed.mockReset().mockResolvedValue({data:{signedUrl:'https://example.invalid/photo'},error:null})})
describe('central profile controls',()=>{
  it('normalizes usernames without changing identity',()=>{expect(normalizeUsername(' Commander_1 ')).toBe('commander_1')})
  it.each(['ab','1commander','name space','x'.repeat(25),'a@example.com'])('rejects invalid username %s',name=>{expect(()=>normalizeUsername(name)).toThrow()})
  it('rejects SVG and oversized photos',()=>{expect(()=>validatePhoto(new File(['x'],'x.svg',{type:'image/svg+xml'}))).toThrow();expect(()=>validatePhoto(new File([new Uint8Array(2097153)],'x.png',{type:'image/png'}))).toThrow()})
  it('accepts supported bounded photos',()=>{expect(()=>validatePhoto(new File(['fixture'],'x.jpg',{type:'image/jpeg'}))).not.toThrow()})
  it('saves canonical username and rereads profile',async()=>{const reload=page();fireEvent.change(screen.getByLabelText('Username'),{target:{value:' Commander_1 '}});fireEvent.click(screen.getByRole('button',{name:'Save username'}));await waitFor(()=>expect(reload).toHaveBeenCalled());expect(fixture.rpc).toHaveBeenCalledWith('set_username',{p_username:'commander_1',p_expected_user:user})})
  it('keeps duplicate username editable without claiming success',async()=>{fixture.rpc.mockResolvedValue({error:{code:'PA409'}});const reload=page();fireEvent.change(screen.getByLabelText('Username'),{target:{value:'commander'}});fireEvent.click(screen.getByRole('button',{name:'Save username'}));expect(await screen.findByRole('alert')).toHaveProperty('textContent','That username is unavailable.');expect(reload).not.toHaveBeenCalled()})
  it('clears photo metadata before deleting own storage path',async()=>{page(user+'/avatar.png');fireEvent.click(screen.getByRole('button',{name:'Remove photo'}));await waitFor(()=>expect(fixture.remove).toHaveBeenCalledWith([user+'/avatar.png']));expect(fixture.rpc).toHaveBeenCalledWith('set_avatar',{p_avatar_path:null,p_expected_user:user});expect(fixture.rpc.mock.invocationCallOrder[0]).toBeLessThan(fixture.remove.mock.invocationCallOrder[0])})
  it('does not delete storage after metadata failure',async()=>{fixture.rpc.mockResolvedValue({error:{code:'PT401'}});page(user+'/avatar.png');fireEvent.click(screen.getByRole('button',{name:'Remove photo'}));await screen.findByRole('alert');expect(fixture.remove).not.toHaveBeenCalled()})
  it('uploads only a re-encoded PNG into the current owner path',async()=>{
    vi.stubGlobal('createImageBitmap',vi.fn(async()=>({width:100,height:100,close:vi.fn()})))
    vi.spyOn(HTMLCanvasElement.prototype,'getContext').mockReturnValue({drawImage:vi.fn()} as unknown as CanvasRenderingContext2D)
    vi.spyOn(HTMLCanvasElement.prototype,'toBlob').mockImplementation(callback=>callback(new Blob(['synthetic-png'],{type:'image/png'})))
    fixture.upload.mockResolvedValue({error:null});const reload=page()
    fireEvent.change(screen.getByLabelText('Change profile photo'),{target:{files:[new File(['synthetic-jpeg'],'photo.jpg',{type:'image/jpeg'})]}})
    await waitFor(()=>expect(reload).toHaveBeenCalled())
    expect(fixture.upload.mock.calls[0][0]).toBe(user+'/avatar.png')
    expect(fixture.upload.mock.calls[0][1].type).toBe('image/png')
    expect(fixture.rpc).toHaveBeenCalledWith('set_avatar',{p_avatar_path:user+'/avatar.png',p_expected_user:user})
  })

})
