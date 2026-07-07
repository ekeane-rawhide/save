import type { ReactNode } from 'react'

export function EmptyState({
  icon,
  title,
  description,
  action,
}: {
  icon?: ReactNode
  title: string
  description?: string
  action?: ReactNode
}) {
  return (
    <div className="flex flex-col items-center justify-center gap-3 rounded-2xl border border-dashed border-[var(--border-subtle)] px-6 py-12 text-center">
      {icon && <div className="text-[var(--text-muted)]">{icon}</div>}
      <div>
        <p className="font-medium text-[var(--text-primary)]">{title}</p>
        {description && <p className="mt-1 text-sm text-[var(--text-secondary)]">{description}</p>}
      </div>
      {action}
    </div>
  )
}
