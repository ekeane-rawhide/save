import type { ReactNode } from 'react'
import { Card } from '@/components/ui/Card'

export function StatCard({
  label,
  value,
  hint,
  icon,
  tone = 'neutral',
}: {
  label: string
  value: string
  hint?: string
  icon?: ReactNode
  tone?: 'neutral' | 'positive' | 'negative'
}) {
  const valueColor =
    tone === 'positive' ? 'text-mint-600' : tone === 'negative' ? 'text-red-500' : 'text-[var(--text-primary)]'

  return (
    <Card className="p-4">
      <div className="flex items-center justify-between">
        <p className="text-sm font-medium text-[var(--text-secondary)]">{label}</p>
        {icon && <div className="text-[var(--text-muted)]">{icon}</div>}
      </div>
      <p className={`mt-1.5 text-2xl font-semibold tracking-tight ${valueColor}`}>{value}</p>
      {hint && <p className="mt-1 text-xs text-[var(--text-muted)]">{hint}</p>}
    </Card>
  )
}
