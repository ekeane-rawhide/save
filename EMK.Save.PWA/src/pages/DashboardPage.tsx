import { Link } from 'react-router-dom'
import {
  IconArrowUpRight, IconArrowDownRight, IconPigMoney, IconTrendingUp,
  IconAlertCircle, IconBuildingBank, IconChevronRight,
} from '@tabler/icons-react'
import { useAuth } from '@/context/AuthContext'
import { usePeriod } from '@/context/PeriodContext'
import { useDashboard } from '@/hooks/useDashboard'
import { MonthSwitcher } from '@/components/MonthSwitcher'
import { StatCard } from '@/components/StatCard'
import { Card } from '@/components/ui/Card'
import { Badge } from '@/components/ui/Badge'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { EmptyState } from '@/components/ui/EmptyState'
import { FullPageSpinner } from '@/components/ui/Spinner'
import { TransactionRow } from '@/components/TransactionRow'
import { InsightCard } from '@/components/InsightCard'
import { CategoryBadge } from '@/components/CategoryBadge'
import { formatCurrency } from '@/lib/format'

export function DashboardPage() {
  const { user } = useAuth()
  const { month, year } = usePeriod()
  const { data, isLoading } = useDashboard(user?.SharedBudgetId, user?.Id, month, year)

  if (isLoading || !data) return <FullPageSpinner />

  return (
    <div className="flex flex-col gap-6 pb-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-[var(--text-primary)]">
            {data.SharedBudget?.Name ?? 'Dashboard'}
          </h1>
          <p className="text-sm text-[var(--text-secondary)]">Here's how {data.MonthLabel} is looking.</p>
        </div>
        <MonthSwitcher />
      </div>

      {data.UnassignedCount > 0 && (
        <Link
          to="/transactions"
          className="flex items-center gap-3 rounded-2xl border border-amber-200 bg-amber-50 px-4 py-3 text-amber-800 transition-colors hover:bg-amber-100 dark:border-amber-500/25 dark:bg-amber-500/10 dark:text-amber-300"
        >
          <IconAlertCircle size={19} className="shrink-0" />
          <p className="flex-1 text-sm font-medium">
            {data.UnassignedCount} transaction{data.UnassignedCount === 1 ? '' : 's'} need a category
          </p>
          <IconChevronRight size={16} />
        </Link>
      )}

      <div className="grid grid-cols-2 gap-3 md:grid-cols-4">
        <StatCard label="Income" value={formatCurrency(data.TotalIncome)} icon={<IconArrowUpRight size={18} />} tone="positive" />
        <StatCard label="Expenses" value={formatCurrency(data.TotalExpenses)} icon={<IconArrowDownRight size={18} />} tone="negative" />
        <StatCard
          label="Net cash flow"
          value={formatCurrency(data.NetCashFlow)}
          icon={<IconTrendingUp size={18} />}
          tone={data.NetCashFlow >= 0 ? 'positive' : 'negative'}
        />
        <StatCard label="Savings rate" value={`${data.SavingsRate}%`} icon={<IconPigMoney size={18} />} />
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
        <Card className="p-4 lg:col-span-2">
          <div className="mb-3 flex items-center justify-between">
            <h2 className="font-semibold text-[var(--text-primary)]">Budget categories</h2>
            <Link to="/budgets" className="flex items-center gap-0.5 text-sm font-medium text-brand-600 hover:underline">
              View all <IconChevronRight size={14} />
            </Link>
          </div>
          {data.CategorySummaries.length === 0 ? (
            <EmptyState title="No budgets set yet" description="Set planned amounts per category to start tracking." />
          ) : (
            <div className="flex flex-col gap-4">
              {data.CategorySummaries.slice(0, 6).map((c) => (
                <div key={c.CategoryId}>
                  <div className="mb-1.5 flex items-center justify-between gap-2">
                    <CategoryBadge icon={c.CategoryIcon} color={c.CategoryColor} name={c.CategoryName} size={30} />
                    <span className={`shrink-0 text-sm font-medium ${c.IsOverBudget ? 'text-red-500' : 'text-[var(--text-secondary)]'}`}>
                      {formatCurrency(c.ActualAmount)} / {formatCurrency(c.PlannedAmount)}
                    </span>
                  </div>
                  <ProgressBar percent={c.PercentUsed} color={c.CategoryColor} />
                </div>
              ))}
            </div>
          )}
        </Card>

        <Card className="p-4">
          <div className="mb-3 flex items-center justify-between">
            <h2 className="font-semibold text-[var(--text-primary)]">Insights</h2>
            <Link to="/insights" className="flex items-center gap-0.5 text-sm font-medium text-brand-600 hover:underline">
              View all <IconChevronRight size={14} />
            </Link>
          </div>
          {data.Insights.length === 0 ? (
            <EmptyState title="All quiet" description="No insights for this period yet." />
          ) : (
            <div className="flex flex-col gap-3">
              {data.Insights.slice(0, 3).map((i) => (
                <InsightCard key={i.Id} insight={i} />
              ))}
            </div>
          )}
        </Card>
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
        <Card className="p-4 lg:col-span-2">
          <div className="mb-1 flex items-center justify-between">
            <h2 className="font-semibold text-[var(--text-primary)]">Recent transactions</h2>
            <Link to="/transactions" className="flex items-center gap-0.5 text-sm font-medium text-brand-600 hover:underline">
              View all <IconChevronRight size={14} />
            </Link>
          </div>
          {data.RecentTransactions.length === 0 ? (
            <EmptyState title="No transactions yet" description="Synced transactions will show up here." />
          ) : (
            <div className="flex flex-col">
              {data.RecentTransactions.slice(0, 6).map((t) => (
                <TransactionRow key={t.Id} transaction={t} />
              ))}
            </div>
          )}
        </Card>

        <Card className="p-4">
          <div className="mb-3 flex items-center justify-between">
            <h2 className="font-semibold text-[var(--text-primary)]">Linked accounts</h2>
            <Link to="/accounts" className="flex items-center gap-0.5 text-sm font-medium text-brand-600 hover:underline">
              Manage <IconChevronRight size={14} />
            </Link>
          </div>
          {data.LinkedAccounts.length === 0 ? (
            <EmptyState icon={<IconBuildingBank size={22} />} title="No accounts linked" />
          ) : (
            <div className="flex flex-col gap-3">
              {data.LinkedAccounts.map((a) => (
                <div key={a.Id} className="flex items-center justify-between gap-2">
                  <div className="min-w-0">
                    <p className="truncate text-sm font-medium text-[var(--text-primary)]">{a.InstitutionName}</p>
                    <p className="truncate text-xs text-[var(--text-muted)]">
                      {a.AccountName} •••• {a.Mask}
                    </p>
                  </div>
                  <Badge tone="brand">{formatCurrency(a.CurrentBalance)}</Badge>
                </div>
              ))}
            </div>
          )}
        </Card>
      </div>
    </div>
  )
}
