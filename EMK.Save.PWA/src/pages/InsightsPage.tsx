import { IconBulb, IconSparkles } from '@tabler/icons-react'
import { useAuth } from '@/context/AuthContext'
import { usePeriod } from '@/context/PeriodContext'
import { useDismissInsight, useGenerateInsights, useInsights } from '@/hooks/useInsights'
import { useToast } from '@/context/ToastContext'
import { MonthSwitcher } from '@/components/MonthSwitcher'
import { Button } from '@/components/ui/Button'
import { EmptyState } from '@/components/ui/EmptyState'
import { FullPageSpinner } from '@/components/ui/Spinner'
import { InsightCard } from '@/components/InsightCard'

export function InsightsPage() {
  const { user } = useAuth()
  const { month, year } = usePeriod()
  const { data: insights, isLoading } = useInsights(user?.SharedBudgetId, month, year)
  const generateInsights = useGenerateInsights()
  const dismissInsight = useDismissInsight()
  const { show } = useToast()

  const handleGenerate = async () => {
    if (!user?.SharedBudgetId) return
    try {
      await generateInsights.mutateAsync({ sharedBudgetId: user.SharedBudgetId, month, year })
      show('Insights refreshed', 'success')
    } catch {
      show('Could not generate insights', 'error')
    }
  }

  const handleDismiss = async (id: string) => {
    try {
      await dismissInsight.mutateAsync(id)
    } catch {
      show('Could not dismiss insight', 'error')
    }
  }

  if (isLoading) return <FullPageSpinner />

  return (
    <div className="flex flex-col gap-5 pb-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-2xl font-semibold tracking-tight text-[var(--text-primary)]">Insights</h1>
        <div className="flex items-center gap-2">
          <MonthSwitcher />
          <Button onClick={handleGenerate} loading={generateInsights.isPending}>
            <IconSparkles size={16} /> Refresh
          </Button>
        </div>
      </div>

      {!insights || insights.length === 0 ? (
        <EmptyState
          icon={<IconBulb size={22} />}
          title="No insights yet"
          description="Generate insights to spot spending spikes, over-budget categories, and more."
          action={
            <Button onClick={handleGenerate} loading={generateInsights.isPending}>
              <IconSparkles size={16} /> Generate insights
            </Button>
          }
        />
      ) : (
        <div className="flex flex-col gap-3">
          {insights.map((insight) => (
            <InsightCard key={insight.Id} insight={insight} onDismiss={() => handleDismiss(insight.Id)} />
          ))}
        </div>
      )}
    </div>
  )
}
