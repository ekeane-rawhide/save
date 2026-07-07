// Fetch-based API client mirroring EMK.Utility/ApiClient.cs conventions:
// PascalCase JSON, Bearer auth, and the standard { id } / { rowsaffected } response envelopes.

export class ApiError extends Error {
  status: number
  constructor(message: string, status: number) {
    super(message)
    this.status = status
  }
}

const BASE_URL = import.meta.env.VITE_API_URL

type Rollback = boolean

class ApiClient {
  private token: string | null = null
  private onUnauthorized: (() => void) | null = null

  setToken(token: string | null) {
    this.token = token
  }

  setUnauthorizedHandler(handler: () => void) {
    this.onUnauthorized = handler
  }

  private headers(hasBody: boolean): HeadersInit {
    const headers: Record<string, string> = {}
    if (hasBody) headers['Content-Type'] = 'application/json'
    if (this.token) headers['Authorization'] = `Bearer ${this.token}`
    return headers
  }

  private async request<T>(path: string, init?: RequestInit): Promise<T> {
    const response = await fetch(`${BASE_URL}/${path}`, init)
    const text = await response.text()
    const data = text ? JSON.parse(text) : null

    if (!response.ok) {
      if (response.status === 401) this.onUnauthorized?.()
      const message = data?.message ?? data?.Message ?? response.statusText
      throw new ApiError(message, response.status)
    }

    return data as T
  }

  get<T>(path: string): Promise<T> {
    return this.request<T>(path, { headers: this.headers(false) })
  }

  post<T>(path: string, body?: unknown, rollback: Rollback = false): Promise<T> {
    const suffix = rollback ? `${path.includes('?') ? path : path}/${rollback}` : path
    return this.request<T>(suffix, {
      method: 'POST',
      headers: this.headers(body !== undefined),
      body: body !== undefined ? JSON.stringify(body) : undefined,
    })
  }

  put<T>(path: string, body: unknown, rollback: Rollback = false): Promise<T> {
    const suffix = rollback ? `${path}/${rollback}` : path
    return this.request<T>(suffix, {
      method: 'PUT',
      headers: this.headers(true),
      body: JSON.stringify(body),
    })
  }

  delete<T>(path: string, rollback: Rollback = false): Promise<T> {
    const suffix = rollback ? `${path}/${rollback}` : path
    return this.request<T>(suffix, { method: 'DELETE', headers: this.headers(false) })
  }

  // ── Response envelope helpers ──────────────────────────────────────────
  async getInsertedId(path: string, body: unknown, rollback: Rollback = false): Promise<string> {
    const result = await this.post<{ id: string }>(path, body, rollback)
    return result.id
  }

  async getRowsAffected(
    method: 'put' | 'delete',
    path: string,
    body?: unknown,
    rollback: Rollback = false,
  ): Promise<number> {
    const result =
      method === 'put'
        ? await this.put<{ rowsaffected: string }>(path, body, rollback)
        : await this.delete<{ rowsaffected: string }>(path, rollback)
    return parseInt(result.rowsaffected, 10)
  }
}

export const apiClient = new ApiClient()
