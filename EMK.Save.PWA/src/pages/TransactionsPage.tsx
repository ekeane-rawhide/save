import { useMemo, useState } from 'react'
import { IconSearch, IconReceiptOff } from '@tabler/icons-react'
import { useAuth } from '@/context/AuthContext'
import { usePeriod } from '@/context/PeriodContext'
import { useTransactions, useAssignCategory } from '@/hooks/useTransactions'
import { useBudgetCategories } from '@/hooks/useBudgetCategories'
import { useToast } from '@/context/ToastContext'
import { MonthSwitcher } from '@/components/MonthSwitcher'
import { Card } from '@/components/ui/Card'
import { Input } from '@/components/ui/Input'
import { Modal } from '@/components/ui/Modal'
import { EmptyState } from '@/components/ui/EmptyState'
import { FullPageSpinner } from '@/components/ui/Spinner'
import { TransactionRow } from '@/components/TransactionRow'
import { CategoryBadge } from '@/components/CategoryBadge'
import { formatCurrency, formatDate } from '@/lib/format'
import type { Transaction } from '@/types/models'

export function TransactionsPage() {
  const { user } = useAuth()
  const { month, year } = usePeriod()
  const { data: transactions, isLoading } = useTransactions(user?.SharedBudgetId, month, year)
  const { data: categories } = useBudgetCategories(user?.SharedBudgetId)
  const assignCategory = useAssignCategory()
  const { show } = useToast()

  const [filter, setFilter] = useState<'all' | 'unassigned'>('all')
  const [search, setSearch] = useState('')
  const [selected, setSelected] = useState<Transaction | null>(null)

  const filtered = useMemo(() => {
    let rows = transactions ?? []
    if (filter === 'unassigned') rows = rows.filter((t) => !t.IsAssigned)
    if (search.trim()) {
      const q = search.toLowerCase()
      rows = rows.filter((t) => t.DisplayName.toLowerCase().includes(q) || t.CategoryName.toLowerCase().includes(q))
    }
    return rows
  }, [transactions, filter, search])

  const handleAssign = async (categoryId: string | null) => {
    if (!selected) return
    try {
      await assignCategory.mutateAsync({ transactionId: selected.Id, categoryId })
      show('Category updated', 'success')
      setSelected(null)
    } catch {
      show('Could not update category', 'error')
    }
  }

  if (isLoading) return <FullPageSpinner />

  return (
    <div className="flex flex-col gap-5 pb-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-2xl font-semibold tracking-tight text-[var(--text-primary)]">Transactions</h1>
        <MonthSwitcher />
      </div>

      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex rounded-xl bg-[var(--surface-sunken)] p-1">
          {(['all', 'unassigned'] as const).map((f) => (
            <button
              key={f}
              onClick={() => setFilter(f)}
              className={`rounded-lg px-3.5 py-1.5 text-sm font-medium capitalize transition-colors ${
                filter === f ? 'bg-[var(--surface-raised)] text-[var(--text-primary)] shadow-sm' : 'text-[var(--text-muted)]'
              }`}
            >
              {f}
            </button>
          ))}
        </div>
        <div className="sm:w-64">
          <Input
            placeholder="Search transactions"
            leftSlot={<IconSearch size={16} />}
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
      </div>

      <Card className="p-2">
        {filtered.length === 0 ? (
          <EmptyState icon={<IconReceiptOff size={22} />} title="No transactions found" />
        ) : (
          <div className="flex flex-col">
            {filtered.map((t) => (
              <TransactionRow key={t.Id} transaction={t} onClick={() => setSelected(t)} />
            ))}
          </div>
        )}
      </Card>

      <Modal open={!!selected} onClose={() => setSelected(null)} title="Assign category">
        {selected && (
          <div className="flex flex-col gap-4">
            <div className="rounded-xl bg-[var(--surface-sunken)] p-3">
              <p className="text-sm font-medium text-[var(--text-primary)]">{selected.DisplayName}</p>
              <p className="text-xs text-[var(--text-muted)]">
                {formatDate(selected.TransactionDate)} · {formatCurrency(selected.DisplayAmount)}
              </p>
            </div>
            <div className="flex max-h-72 flex-col gap-1 overflow-y-auto">
              <button
                onClick={() => handleAssign(null)}
                className="rounded-xl px-2 py-2 text-left text-sm font-medium text-[var(--text-muted)] hover:bg-[var(--surface-sunken)]"
              >
                Unassigned
              </button>
              {categories?.map((c) => (
                <button
                  key={c.Id}
                  onClick={() => handleAssign(c.Id)}
                  className="rounded-xl px-2 py-2 text-left hover:bg-[var(--surface-sunken)]"
                >
                  <CategoryBadge icon={c.Icon} color={c.Color} name={c.Name} size={30} />
                </button>
              ))}
            </div>
          </div>
        )}
      </Modal>
    </div>
  )
}
