import { AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, ReferenceLine } from 'recharts'
import { IconRefresh, IconChartLine } from '@tabler/icons-react'
import { useAuth } from '@/context/AuthContext'
import { usePeriod } from '@/context/PeriodContext'
import { useBuildCashFlow, useCashFlow } from '@/hooks/useCashFlow'
import { useToast } from '@/context/ToastContext'
import { MonthSwitcher } from '@/components/MonthSwitcher'
import { Card } from '@/components/ui/Card'
import { Button } from '@/components/ui/Button'
import { EmptyState } from '@/components/ui/EmptyState'
import { FullPageSpinner } from '@/components/ui/Spinner'
import { formatCurrency, formatDate } from '@/lib/format'

export function CashFlowPage() {
  const { user } = useAuth()
  const { month, year } = usePeriod()
  const { data: entries, isLoading } = useCashFlow(user?.SharedBudgetId, month, year)
  const buildCashFlow = useBuildCashFlow()
  const { show } = useToast()

  const handleBuild = async () => {
    if (!user?.SharedBudgetId) return
    try {
      await buildCashFlow.mutateAsync({ sharedBudgetId: user.SharedBudgetId, month, year })
      show('Cash flow rebuilt', 'success')
    } catch {
      show('Could not rebuild cash flow', 'error')
    }
  }

  if (isLoading) return <FullPageSpinner />

  const chartData = (entries ?? []).map((e) => ({
    day: formatDate(e.EntryDate),
    balance: e.IsFuture ? null : e.RunningBalance,
    projected: e.ProjectedBalance,
  }))

  return (
    <div className="flex flex-col gap-5 pb-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-2xl font-semibold tracking-tight text-[var(--text-primary)]">Cash Flow</h1>
        <div className="flex items-center gap-2">
          <MonthSwitcher />
          <Button variant="secondary" onClick={handleBuild} loading={buildCashFlow.isPending}>
            <IconRefresh size={16} /> Rebuild
          </Button>
        </div>
      </div>

      <Card className="p-4">
        {!entries || entries.length === 0 ? (
          <EmptyState
            icon={<IconChartLine size={22} />}
            title="No cash flow data yet"
            description="Rebuild to project your balance from this month's transactions."
            action={
              <Button onClick={handleBuild} loading={buildCashFlow.isPending}>
                <IconRefresh size={16} /> Rebuild
              </Button>
            }
          />
        ) : (
          <div className="h-80 w-full">
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart data={chartData} margin={{ top: 10, right: 10, left: 0, bottom: 0 }}>
                <defs>
                  <linearGradient id="balanceFill" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="0%" stopColor="var(--color-brand-500)" stopOpacity={0.35} />
                    <stop offset="100%" stopColor="var(--color-brand-500)" stopOpacity={0} />
                  </linearGradient>
                  <linearGradient id="projectedFill" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="0%" stopColor="var(--color-mint-500)" stopOpacity={0.25} />
                    <stop offset="100%" stopColor="var(--color-mint-500)" stopOpacity={0} />
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" stroke="var(--border-subtle)" vertical={false} />
                <XAxis dataKey="day" tick={{ fontSize: 11, fill: 'var(--text-muted)' }} axisLine={false} tickLine={false} />
                <YAxis
                  tick={{ fontSize: 11, fill: 'var(--text-muted)' }}
                  axisLine={false}
                  tickLine={false}
                  tickFormatter={(v) => formatCurrency(v)}
                  width={70}
                />
                <ReferenceLine y={0} stroke="var(--border-subtle)" />
                <Tooltip
                  contentStyle={{
                    background: 'var(--surface-raised)',
                    border: '1px solid var(--border-subtle)',
                    borderRadius: 12,
                    fontSize: 12,
                  }}
                  formatter={(value) => formatCurrency(Number(value))}
                />
                <Area
                  type="monotone"
                  dataKey="projected"
                  stroke="var(--color-mint-500)"
                  strokeDasharray="4 4"
                  fill="url(#projectedFill)"
                  strokeWidth={2}
                />
                <Area
                  type="monotone"
                  dataKey="balance"
                  stroke="var(--color-brand-600)"
                  fill="url(#balanceFill)"
                  strokeWidth={2.5}
                  connectNulls={false}
                />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        )}
      </Card>

      {entries && entries.length > 0 && (
        <div className="grid grid-cols-3 gap-3">
          <Card className="p-4 text-center">
            <p className="text-xs font-medium text-[var(--text-muted)]">Income</p>
            <p className="mt-1 text-lg font-semibold text-mint-600">
              {formatCurrency(entries.reduce((sum, e) => sum + e.DayIncome, 0))}
            </p>
          </Card>
          <Card className="p-4 text-center">
            <p className="text-xs font-medium text-[var(--text-muted)]">Expenses</p>
            <p className="mt-1 text-lg font-semibold text-red-500">
              {formatCurrency(entries.reduce((sum, e) => sum + e.DayExpenses, 0))}
            </p>
          </Card>
          <Card className="p-4 text-center">
            <p className="text-xs font-medium text-[var(--text-muted)]">Ending balance</p>
            <p className="mt-1 text-lg font-semibold text-[var(--text-primary)]">
              {formatCurrency(entries[entries.length - 1]?.ProjectedBalance ?? 0)}
            </p>
          </Card>
        </div>
      )}
    </div>
  )
}
