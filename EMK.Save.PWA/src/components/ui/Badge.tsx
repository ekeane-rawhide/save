import type { ReactNode } from 'react'

type Tone = 'neutral' | 'brand' | 'success' | 'warning' | 'danger'

const toneClasses: Record<Tone, string> = {
  neutral: 'bg-[var(--surface-sunken)] text-[var(--text-secondary)]',
  brand: 'bg-brand-100 text-brand-700 dark:bg-brand-500/15 dark:text-brand-300',
  success: 'bg-mint-100 text-mint-600 dark:bg-mint-500/15 dark:text-mint-400',
  warning: 'bg-amber-100 text-amber-700 dark:bg-amber-500/15 dark:text-amber-400',
  danger: 'bg-red-100 text-red-700 dark:bg-red-500/15 dark:text-red-400',
}

export function Badge({ tone = 'neutral', children }: { tone?: Tone; children: ReactNode }) {
  return (
    <span className={`inline-flex items-center gap-1 rounded-full px-2.5 py-1 text-xs font-medium ${toneClasses[tone]}`}>
      {children}
    </span>
  )
}
