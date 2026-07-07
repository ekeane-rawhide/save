import { createContext, useContext, useMemo, useState, type ReactNode } from 'react'

interface PeriodContextValue {
  month: number
  year: number
  monthLabel: string
  next: () => void
  prev: () => void
  isCurrentMonth: boolean
}

const PeriodContext = createContext<PeriodContextValue | null>(null)

export function PeriodProvider({ children }: { children: ReactNode }) {
  const now = new Date()
  const [month, setMonth] = useState(now.getMonth() + 1)
  const [year, setYear] = useState(now.getFullYear())

  const next = () => {
    if (month === 12) {
      setMonth(1)
      setYear((y) => y + 1)
    } else {
      setMonth((m) => m + 1)
    }
  }

  const prev = () => {
    if (month === 1) {
      setMonth(12)
      setYear((y) => y - 1)
    } else {
      setMonth((m) => m - 1)
    }
  }

  const monthLabel = useMemo(
    () => new Date(year, month - 1, 1).toLocaleDateString('en-US', { month: 'long', year: 'numeric' }),
    [month, year],
  )

  const isCurrentMonth = month === now.getMonth() + 1 && year === now.getFullYear()

  const value = useMemo(
    () => ({ month, year, monthLabel, next, prev, isCurrentMonth }),
    [month, year, monthLabel, isCurrentMonth],
  )

  return <PeriodContext.Provider value={value}>{children}</PeriodContext.Provider>
}

export function usePeriod() {
  const ctx = useContext(PeriodContext)
  if (!ctx) throw new Error('usePeriod must be used within PeriodProvider')
  return ctx
}
