import { forwardRef, type InputHTMLAttributes } from 'react'

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string
  error?: string
  leftSlot?: React.ReactNode
}

export const Input = forwardRef<HTMLInputElement, InputProps>(function Input(
  { label, error, leftSlot, className = '', id, ...props },
  ref,
) {
  const inputId = id ?? props.name
  return (
    <div className="flex flex-col gap-1.5">
      {label && (
        <label htmlFor={inputId} className="text-sm font-medium text-[var(--text-secondary)]">
          {label}
        </label>
      )}
      <div className="relative flex items-center">
        {leftSlot && <div className="absolute left-3 flex items-center text-[var(--text-muted)]">{leftSlot}</div>}
        <input
          ref={ref}
          id={inputId}
          className={`h-11 w-full rounded-xl border bg-[var(--surface-raised)] px-3.5 text-[var(--text-primary)] outline-none transition-colors placeholder:text-[var(--text-muted)] focus:border-brand-500 focus:ring-2 focus:ring-brand-500/20 ${
            error ? 'border-red-400' : 'border-[var(--border-subtle)]'
          } ${leftSlot ? 'pl-10' : ''} ${className}`}
          {...props}
        />
      </div>
      {error && <span className="text-xs font-medium text-red-500">{error}</span>}
    </div>
  )
})
