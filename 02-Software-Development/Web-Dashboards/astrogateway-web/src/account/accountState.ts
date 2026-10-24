import { createContext, useContext } from 'react'
import type { Session } from '@supabase/supabase-js'
import type { Profile } from './client'
export interface State { session: Session | null; profile: Profile | null; loading: boolean; error: string; recovery: boolean }
export interface Account extends State { reload: () => Promise<void>; logout: () => Promise<void>; saveName: (name: string) => Promise<void>; completeRecovery: () => void; beginRecovery: () => void }
export const Context = createContext<Account | null>(null)
export function useAccount() { const account = useContext(Context); if (!account) throw new Error('Account provider required'); return account }
