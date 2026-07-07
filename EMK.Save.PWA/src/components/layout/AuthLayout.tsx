import type { ReactNode } from 'react'
import { Logo } from '@/components/Logo'

export function AuthLayout({
  title,
  subtitle,
  children,
  footer,
}: {
  title: string
  subtitle?: string
  children: ReactNode
  footer?: ReactNode
}) {
  return (
    <div className="relative flex min-h-screen items-center justify-center overflow-hidden bg-[var(--surface)] px-4 py-10">
      <div className="pointer-events-none absolute -left-32 -top-32 h-96 w-96 rounded-full bg-mint-400/20 blur-3xl" />
      <div className="pointer-events-none absolute -bottom-32 -right-32 h-96 w-96 rounded-full bg-brand-500/20 blur-3xl" />

      <div className="relative w-full max-w-sm animate-slide-up">
        <div className="mb-8 flex flex-col items-center gap-4 text-center">
          <Logo size={52} />
          <div>
            <h1 className="text-2xl font-semibold tracking-tight text-[var(--text-primary)]">{title}</h1>
            {subtitle && <p className="mt-1.5 text-sm text-[var(--text-secondary)]">{subtitle}</p>}
          </div>
        </div>

        <div className="rounded-3xl border border-[var(--border-subtle)] bg-[var(--surface-raised)] p-6 shadow-xl shadow-black/5">
          {children}
        </div>

        {footer && <div className="mt-6 text-center text-sm text-[var(--text-secondary)]">{footer}</div>}
      </div>
    </div>
  )
}
