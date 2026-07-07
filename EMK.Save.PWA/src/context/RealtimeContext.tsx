import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import type { HubConnection } from '@microsoft/signalr'
import { getHubConnection, stopHubConnection } from '@/lib/signalr'
import { useAuth } from '@/context/AuthContext'

interface RealtimeContextValue {
  connection: HubConnection | null
  connected: boolean
}

const RealtimeContext = createContext<RealtimeContextValue>({ connection: null, connected: false })

export function RealtimeProvider({ children }: { children: ReactNode }) {
  const { user, status } = useAuth()
  const [connection, setConnection] = useState<HubConnection | null>(null)
  const [connected, setConnected] = useState(false)

  useEffect(() => {
    if (status !== 'authenticated' || !user) {
      void stopHubConnection()
      setConnection(null)
      setConnected(false)
      return
    }

    const conn = getHubConnection(() => user.Token)
    setConnection(conn)

    const joinGroup = () => {
      if (user.SharedBudgetId) void conn.invoke('JoinBudget', user.SharedBudgetId).catch(() => {})
    }

    conn.onreconnected(joinGroup)
    conn.onclose(() => setConnected(false))

    let cancelled = false
    conn
      .start()
      .then(() => {
        if (cancelled) return
        setConnected(true)
        joinGroup()
      })
      .catch(() => setConnected(false))

    return () => {
      cancelled = true
    }
  }, [status, user])

  const value = useMemo(() => ({ connection, connected }), [connection, connected])

  return <RealtimeContext.Provider value={value}>{children}</RealtimeContext.Provider>
}

export function useRealtime() {
  return useContext(RealtimeContext)
}

export function useRealtimeEvent<T = unknown>(eventName: string, handler: (payload: T) => void) {
  const { connection } = useRealtime()

  useEffect(() => {
    if (!connection) return
    connection.on(eventName, handler)
    return () => {
      connection.off(eventName, handler)
    }
  }, [connection, eventName, handler])
}
