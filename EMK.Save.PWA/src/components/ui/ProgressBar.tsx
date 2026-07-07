export function ProgressBar({ percent, color }: { percent: number; color?: string }) {
  const clamped = Math.min(Math.max(percent, 0), 100)
  const isOver = percent > 100
  return (
    <div className="h-2 w-full overflow-hidden rounded-full bg-[var(--surface-sunken)]">
      <div
        className="h-full rounded-full transition-[width] duration-500 ease-out"
        style={{
          width: `${clamped}%`,
          backgroundColor: isOver ? '#ef4444' : (color ?? 'var(--color-brand-600)'),
        }}
      />
    </div>
  )
}
