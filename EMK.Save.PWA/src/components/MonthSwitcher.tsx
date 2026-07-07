import { IconChevronLeft, IconChevronRight } from '@tabler/icons-react'
import { usePeriod } from '@/context/PeriodContext'

export function MonthSwitcher() {
  const { monthLabel, next, prev, isCurrentMonth } = usePeriod()

  return (
    <div className="inline-flex items-center gap-1 rounded-xl border border-[var(--border-subtle)] bg-[var(--surface-raised)] p-1">
      <button
        onClick={prev}
        className="rounded-lg p-1.5 text-[var(--text-secondary)] hover:bg-[var(--surface-sunken)]"
        aria-label="Previous month"
      >
        <IconChevronLeft size={17} />
      </button>
      <span className="min-w-[9.5rem] text-center text-sm font-medium text-[var(--text-primary)]">
        {monthLabel}
        {isCurrentMonth && <span className="ml-1.5 text-xs font-normal text-mint-500">• current</span>}
      </span>
      <button
        onClick={next}
        className="rounded-lg p-1.5 text-[var(--text-secondary)] hover:bg-[var(--surface-sunken)]"
        aria-label="Next month"
      >
        <IconChevronRight size={17} />
      </button>
    </div>
  )
}
