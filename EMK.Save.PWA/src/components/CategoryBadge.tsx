import { CategoryIcon } from '@/components/CategoryIcon'

export function CategoryBadge({
  icon,
  color,
  name,
  size = 36,
}: {
  icon: string
  color: string
  name: string
  size?: number
}) {
  return (
    <div className="flex items-center gap-2.5">
      <div
        className="flex shrink-0 items-center justify-center rounded-full"
        style={{ width: size, height: size, backgroundColor: `${color}22`, color }}
      >
        <CategoryIcon icon={icon} size={size * 0.52} stroke={1.75} />
      </div>
      <span className="truncate text-sm font-medium text-[var(--text-primary)]">{name}</span>
    </div>
  )
}
