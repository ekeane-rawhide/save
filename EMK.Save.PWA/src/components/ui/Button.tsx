import { forwardRef, type ButtonHTMLAttributes } from 'react'
import { IconLoader2 } from '@tabler/icons-react'

type Variant = 'primary' | 'secondary' | 'ghost' | 'danger'
type Size = 'sm' | 'md' | 'lg'

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: Variant
  size?: Size
  loading?: boolean
  fullWidth?: boolean
}

const variantClasses: Record<Variant, string> = {
  primary:
    'bg-brand-600 text-white hover:bg-brand-700 active:bg-brand-800 shadow-sm shadow-brand-600/20 disabled:bg-brand-600/50',
  secondary:
    'bg-[var(--surface-sunken)] text-[var(--text-primary)] hover:brightness-95 dark:hover:brightness-110 border border-[var(--border-subtle)]',
  ghost: 'bg-transparent text-[var(--text-primary)] hover:bg-[var(--surface-sunken)]',
  danger: 'bg-red-600 text-white hover:bg-red-700 active:bg-red-800 disabled:bg-red-600/50',
}

const sizeClasses: Record<Size, string> = {
  sm: 'h-8 px-3 text-sm gap-1.5 rounded-lg',
  md: 'h-10 px-4 text-sm gap-2 rounded-xl',
  lg: 'h-12 px-5 text-base gap-2 rounded-xl',
}

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(function Button(
  { variant = 'primary', size = 'md', loading, fullWidth, disabled, className = '', children, ...props },
  ref,
) {
  return (
    <button
      ref={ref}
      disabled={disabled || loading}
      className={`inline-flex items-center justify-center font-medium transition-colors duration-150 disabled:cursor-not-allowed active:scale-[0.98] transition-transform ${variantClasses[variant]} ${sizeClasses[size]} ${fullWidth ? 'w-full' : ''} ${className}`}
      {...props}
    >
      {loading && <IconLoader2 size={16} className="animate-spin" />}
      {children}
    </button>
  )
})
