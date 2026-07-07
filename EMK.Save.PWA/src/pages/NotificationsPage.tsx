import { useNavigate } from 'react-router-dom'
import { IconBellOff, IconBellRinging } from '@tabler/icons-react'
import { useAuth } from '@/context/AuthContext'
import { useMarkNotificationRead, useNotifications } from '@/hooks/useNotifications'
import { usePushSubscription } from '@/hooks/usePushSubscription'
import { Card } from '@/components/ui/Card'
import { Switch } from '@/components/ui/Switch'
import { EmptyState } from '@/components/ui/EmptyState'
import { FullPageSpinner } from '@/components/ui/Spinner'
import { formatDateTime } from '@/lib/format'
import type { PushNotification } from '@/types/models'

export function NotificationsPage() {
  const { user } = useAuth()
  const { data: notifications, isLoading } = useNotifications(user?.Id)
  const markRead = useMarkNotificationRead()
  const push = usePushSubscription(user?.Id)
  const navigate = useNavigate()

  const handleClick = (n: PushNotification) => {
    if (!n.IsRead) markRead.mutate(n.Id)
    if (n.ActionUrl) navigate(n.ActionUrl)
  }

  if (isLoading) return <FullPageSpinner />

  return (
    <div className="flex flex-col gap-5 pb-4">
      <h1 className="text-2xl font-semibold tracking-tight text-[var(--text-primary)]">Notifications</h1>

      <Card className="flex items-center justify-between gap-3 p-4">
        <div className="flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center rounded-full bg-brand-100 text-brand-700 dark:bg-brand-500/15 dark:text-brand-300">
            {push.enabled ? <IconBellRinging size={18} /> : <IconBellOff size={18} />}
          </div>
          <div>
            <p className="text-sm font-medium text-[var(--text-primary)]">Push notifications</p>
            <p className="text-xs text-[var(--text-muted)]">
              {push.supported ? 'Get alerted about overages and new transactions' : 'Not supported on this device'}
            </p>
          </div>
        </div>
        <Switch checked={push.enabled} onChange={push.toggle} disabled={!push.supported || push.busy} />
      </Card>

      {!notifications || notifications.length === 0 ? (
        <EmptyState icon={<IconBellOff size={22} />} title="No notifications yet" />
      ) : (
        <Card className="divide-y divide-[var(--border-subtle)] p-2">
          {notifications.map((n) => (
            <button
              key={n.Id}
              onClick={() => handleClick(n)}
              className="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left transition-colors hover:bg-[var(--surface-sunken)]"
            >
              {!n.IsRead && <span className="mt-1.5 h-2 w-2 shrink-0 rounded-full bg-brand-600" />}
              <div className={`min-w-0 flex-1 ${n.IsRead ? 'pl-5' : ''}`}>
                <p className="text-sm font-medium text-[var(--text-primary)]">{n.Title}</p>
                <p className="mt-0.5 text-sm text-[var(--text-secondary)]">{n.Body}</p>
                <p className="mt-1 text-xs text-[var(--text-muted)]">{formatDateTime(n.SentOn ?? n.ScheduledFor)}</p>
              </div>
            </button>
          ))}
        </Card>
      )}
    </div>
  )
}
