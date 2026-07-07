import { IconTag } from '@tabler/icons-react'
import { CategoryIcon } from '@/components/CategoryIcon'
import { formatCurrency, formatDate } from '@/lib/format'
import type { Transaction } from '@/types/models'

export function TransactionRow({ transaction, onClick }: { transaction: Transaction; onClick?: () => void }) {
  const t = transaction
  return (
    <button
      onClick={onClick}
      className="flex w-full items-center gap-3 rounded-xl px-2 py-2.5 text-left transition-colors hover:bg-[var(--surface-sunken)]"
    >
      <div
        className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full"
        style={{
          backgroundColor: t.IsAssigned ? `${t.CategoryColor}22` : 'var(--surface-sunken)',
          color: t.IsAssigned ? t.CategoryColor : 'var(--text-muted)',
        }}
      >
        {t.IsAssigned ? <CategoryIcon icon={t.CategoryIcon} size={19} stroke={1.75} /> : <IconTag size={18} />}
      </div>
      <div className="min-w-0 flex-1">
        <p className="truncate text-sm font-medium text-[var(--text-primary)]">{t.DisplayName}</p>
        <p className="truncate text-xs text-[var(--text-muted)]">
          {formatDate(t.TransactionDate)} · {t.IsAssigned ? t.CategoryName : 'Unassigned'}
          {t.IsPending ? ' · Pending' : ''}
        </p>
      </div>
      <span className={`shrink-0 text-sm font-semibold ${t.IsIncome ? 'text-mint-600' : 'text-[var(--text-primary)]'}`}>
        {t.IsIncome ? '+' : '−'}
        {formatCurrency(t.DisplayAmount)}
      </span>
    </button>
  )
}
