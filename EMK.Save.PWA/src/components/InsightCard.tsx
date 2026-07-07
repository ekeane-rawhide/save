import { IconInfoCircle, IconCircleCheck, IconAlertTriangle, IconAlertOctagon, IconX } from '@tabler/icons-react'
import { Card } from '@/components/ui/Card'
import { InsightSeverity, type TrackingInsight } from '@/types/models'

const SEVERITY_STYLE: Record<number, { icon: typeof IconInfoCircle; classes: string }> = {
  [InsightSeverity.Info]: { icon: IconInfoCircle, classes: 'bg-brand-100 text-brand-600 dark:bg-brand-500/15 dark:text-brand-300' },
  [InsightSeverity.Success]: { icon: IconCircleCheck, classes: 'bg-mint-100 text-mint-600 dark:bg-mint-500/15 dark:text-mint-400' },
  [InsightSeverity.Warning]: { icon: IconAlertTriangle, classes: 'bg-amber-100 text-amber-600 dark:bg-amber-500/15 dark:text-amber-400' },
  [InsightSeverity.Critical]: { icon: IconAlertOctagon, classes: 'bg-red-100 text-red-600 dark:bg-red-500/15 dark:text-red-400' },
}

export function InsightCard({ insight, onDismiss }: { insight: TrackingInsight; onDismiss?: () => void }) {
  const style = SEVERITY_STYLE[insight.Severity] ?? SEVERITY_STYLE[InsightSeverity.Info]
  const Icon = style.icon

  return (
    <Card className="flex items-start gap-3 p-4">
      <div className={`flex h-9 w-9 shrink-0 items-center justify-center rounded-full ${style.classes}`}>
        <Icon size={18} />
      </div>
      <div className="min-w-0 flex-1">
        <p className="text-sm font-semibold text-[var(--text-primary)]">{insight.Title}</p>
        <p className="mt-0.5 text-sm text-[var(--text-secondary)]">{insight.Message}</p>
      </div>
      {onDismiss && (
        <button
          onClick={onDismiss}
          className="shrink-0 rounded-lg p-1.5 text-[var(--text-muted)] hover:bg-[var(--surface-sunken)]"
          aria-label="Dismiss"
        >
          <IconX size={16} />
        </button>
      )}
    </Card>
  )
}
