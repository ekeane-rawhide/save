import { useMemo, useState } from 'react'
import { IconSettings, IconPlus } from '@tabler/icons-react'
import { useAuth } from '@/context/AuthContext'
import { usePeriod } from '@/context/PeriodContext'
import { useBudgetCategories } from '@/hooks/useBudgetCategories'
import { useBudgets, useCreateBudget, useUpdateBudget } from '@/hooks/useBudgets'
import { useToast } from '@/context/ToastContext'
import { MonthSwitcher } from '@/components/MonthSwitcher'
import { Card } from '@/components/ui/Card'
import { Button } from '@/components/ui/Button'
import { Input } from '@/components/ui/Input'
import { Modal } from '@/components/ui/Modal'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { FullPageSpinner } from '@/components/ui/Spinner'
import { EmptyState } from '@/components/ui/EmptyState'
import { CategoryBadge } from '@/components/CategoryBadge'
import { CategoryManagerModal } from '@/components/CategoryManagerModal'
import { formatCurrency } from '@/lib/format'
import type { Budget, BudgetCategory } from '@/types/models'

export function BudgetsPage() {
  const { user } = useAuth()
  const { month, year } = usePeriod()
  const { data: categories, isLoading: loadingCategories } = useBudgetCategories(user?.SharedBudgetId)
  const { data: budgets, isLoading: loadingBudgets } = useBudgets(user?.SharedBudgetId, month, year)
  const createBudget = useCreateBudget()
  const updateBudget = useUpdateBudget()
  const { show } = useToast()

  const [managingCategories, setManagingCategories] = useState(false)
  const [editing, setEditing] = useState<{ category: BudgetCategory; budget?: Budget } | null>(null)
  const [amount, setAmount] = useState('')

  const rows = useMemo(() => {
    return (categories ?? [])
      .filter((c) => c.IsActive)
      .map((category) => ({
        category,
        budget: budgets?.find((b) => b.CategoryId === category.Id),
      }))
  }, [categories, budgets])

  const openEditor = (row: { category: BudgetCategory; budget?: Budget }) => {
    setEditing(row)
    setAmount(row.budget ? String(row.budget.PlannedAmount) : '')
  }

  const saveAmount = async () => {
    if (!editing || !user?.SharedBudgetId) return
    const planned = parseFloat(amount)
    if (Number.isNaN(planned) || planned < 0) return

    try {
      if (editing.budget) {
        await updateBudget.mutateAsync({ id: editing.budget.Id, budget: { ...editing.budget, PlannedAmount: planned } })
      } else {
        await createBudget.mutateAsync({
          SharedBudgetId: user.SharedBudgetId,
          CategoryId: editing.category.Id,
          CategoryName: editing.category.Name,
          CategoryIcon: editing.category.Icon,
          CategoryColor: editing.category.Color,
          CategoryType: editing.category.CategoryType,
          Month: month,
          Year: year,
          PlannedAmount: planned,
          RolloverAmount: 0,
          Notes: '',
        })
      }
      show('Budget updated', 'success')
      setEditing(null)
    } catch {
      show('Could not save budget', 'error')
    }
  }

  if (loadingCategories || loadingBudgets) return <FullPageSpinner />

  return (
    <div className="flex flex-col gap-5 pb-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-2xl font-semibold tracking-tight text-[var(--text-primary)]">Budgets</h1>
        <div className="flex items-center gap-2">
          <MonthSwitcher />
          <Button variant="secondary" size="md" onClick={() => setManagingCategories(true)}>
            <IconSettings size={16} /> Categories
          </Button>
        </div>
      </div>

      {rows.length === 0 ? (
        <EmptyState
          title="No categories yet"
          description="Add your first category to start planning a budget."
          action={
            <Button onClick={() => setManagingCategories(true)}>
              <IconPlus size={16} /> Add category
            </Button>
          }
        />
      ) : (
        <Card className="divide-y divide-[var(--border-subtle)] p-2">
          {rows.map(({ category, budget }) => (
            <button
              key={category.Id}
              onClick={() => openEditor({ category, budget })}
              className="flex w-full flex-col gap-2 rounded-xl px-3 py-3 text-left transition-colors hover:bg-[var(--surface-sunken)]"
            >
              <div className="flex items-center justify-between gap-2">
                <CategoryBadge icon={category.Icon} color={category.Color} name={category.Name} />
                {budget ? (
                  <span className={`text-sm font-medium ${budget.IsOverBudget ? 'text-red-500' : 'text-[var(--text-secondary)]'}`}>
                    {formatCurrency(budget.AmountSpent)} / {formatCurrency(budget.PlannedAmount)}
                  </span>
                ) : (
                  <span className="text-sm font-medium text-brand-600">Set budget</span>
                )}
              </div>
              {budget && <ProgressBar percent={budget.PercentUsed} color={category.Color} />}
            </button>
          ))}
        </Card>
      )}

      <Modal open={!!editing} onClose={() => setEditing(null)} title={editing?.category.Name}>
        {editing && (
          <div className="flex flex-col gap-4">
            <Input
              label="Planned amount"
              type="number"
              min="0"
              step="0.01"
              leftSlot={<span>$</span>}
              value={amount}
              onChange={(e) => setAmount(e.target.value)}
              autoFocus
            />
            {editing.budget && (
              <div className="flex justify-between rounded-xl bg-[var(--surface-sunken)] px-3 py-2.5 text-sm">
                <span className="text-[var(--text-secondary)]">Spent so far</span>
                <span className="font-medium text-[var(--text-primary)]">{formatCurrency(editing.budget.AmountSpent)}</span>
              </div>
            )}
            <Button fullWidth onClick={saveAmount} loading={createBudget.isPending || updateBudget.isPending}>
              Save
            </Button>
          </div>
        )}
      </Modal>

      {user?.SharedBudgetId && (
        <CategoryManagerModal
          open={managingCategories}
          onClose={() => setManagingCategories(false)}
          sharedBudgetId={user.SharedBudgetId}
        />
      )}
    </div>
  )
}
