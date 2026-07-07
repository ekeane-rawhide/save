import type { HTMLAttributes } from 'react'

export function Card({ className = '', children, ...props }: HTMLAttributes<HTMLDivElement>) {
  return (
    <div
      className={`rounded-2xl border border-[var(--border-subtle)] bg-[var(--surface-raised)] shadow-sm ${className}`}
      {...props}
    >
      {children}
    </div>
  )
}
