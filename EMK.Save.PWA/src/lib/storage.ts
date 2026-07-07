import type { AuthenticateResponse } from '@/types/models'

const AUTH_KEY = 'save.auth'

export function loadStoredAuth(): AuthenticateResponse | null {
  const raw = localStorage.getItem(AUTH_KEY)
  if (!raw) return null
  try {
    return JSON.parse(raw) as AuthenticateResponse
  } catch {
    return null
  }
}

export function saveStoredAuth(auth: AuthenticateResponse) {
  localStorage.setItem(AUTH_KEY, JSON.stringify(auth))
}

export function clearStoredAuth() {
  localStorage.removeItem(AUTH_KEY)
}
