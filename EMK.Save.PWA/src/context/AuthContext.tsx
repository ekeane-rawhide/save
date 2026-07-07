import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import { apiClient } from '@/lib/apiClient'
import { clearStoredAuth, loadStoredAuth, saveStoredAuth } from '@/lib/storage'
import type { AuthenticateResponse } from '@/types/models'

export interface RegisterFields {
  UserId: string
  Password: string
  FirstName: string
  LastName: string
  Email: string
}

interface AuthContextValue {
  user: AuthenticateResponse | null
  status: 'loading' | 'authenticated' | 'unauthenticated'
  login: (userId: string, password: string) => Promise<void>
  register: (fields: RegisterFields) => Promise<void>
  logout: () => void
  updateUser: (patch: Partial<AuthenticateResponse>) => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthenticateResponse | null>(null)
  const [status, setStatus] = useState<'loading' | 'authenticated' | 'unauthenticated'>('loading')

  useEffect(() => {
    apiClient.setUnauthorizedHandler(() => {
      clearStoredAuth()
      setUser(null)
      setStatus('unauthenticated')
    })

    const stored = loadStoredAuth()
    if (stored) {
      apiClient.setToken(stored.Token)
      setUser(stored)
      setStatus('authenticated')
    } else {
      setStatus('unauthenticated')
    }
  }, [])

  const login = async (userId: string, password: string) => {
    const response = await apiClient.post<AuthenticateResponse>('User/authenticate', {
      UserId: userId,
      Password: password,
    })
    apiClient.setToken(response.Token)
    saveStoredAuth(response)
    setUser(response)
    setStatus('authenticated')
  }

  const register = async (fields: RegisterFields) => {
    const response = await apiClient.post<AuthenticateResponse>('User/register', fields)
    apiClient.setToken(response.Token)
    saveStoredAuth(response)
    setUser(response)
    setStatus('authenticated')
  }

  const logout = () => {
    apiClient.setToken(null)
    clearStoredAuth()
    setUser(null)
    setStatus('unauthenticated')
  }

  const updateUser = (patch: Partial<AuthenticateResponse>) => {
    setUser((prev) => {
      if (!prev) return prev
      const next = { ...prev, ...patch }
      saveStoredAuth(next)
      return next
    })
  }

  const value = useMemo(
    () => ({ user, status, login, register, logout, updateUser }),
    [user, status],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
