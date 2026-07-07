import { IconLoader2 } from '@tabler/icons-react'

export function Spinner({ size = 20, className = '' }: { size?: number; className?: string }) {
  return <IconLoader2 size={size} className={`animate-spin text-brand-600 ${className}`} />
}

export function FullPageSpinner() {
  return (
    <div className="flex h-full min-h-[60vh] w-full items-center justify-center">
      <Spinner size={28} />
    </div>
  )
}
