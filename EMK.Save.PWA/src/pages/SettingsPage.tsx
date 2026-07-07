import { useState } from 'react'
import { IconCopy, IconCheck, IconLogout, IconDoorExit } from '@tabler/icons-react'
import { useAuth } from '@/context/AuthContext'
import { useSharedBudget, useLeaveSharedBudget } from '@/hooks/useSharedBudget'
import { useNotificationPreference, useUpdateNotificationPreference } from '@/hooks/useNotifications'
import { usePushSubscription } from '@/hooks/usePushSubscription'
import { useToast } from '@/context/ToastContext'
import { Card } from '@/components/ui/Card'
import { Button } from '@/components/ui/Button'
import { Switch } from '@/components/ui/Switch'
import { FullPageSpinner } from '@/components/ui/Spinner'
import { initials } from '@/lib/format'

const PREFERENCE_TOGGLES = [
  { key: 'NotifyOnNewTransaction', label: 'New transactions' },
  { key: 'NotifyOnBudgetOverage', label: 'Budget overages' },
  { key: 'NotifyOnBudgetWarning', label: 'Approaching budget limits' },
  { key: 'NotifyOnLargeTransaction', label: 'Large transactions' },
  { key: 'NotifyWeeklySummary', label: 'Weekly summary' },
  { key: 'NotifyMonthlySummary', label: 'Monthly summary' },
  { key: 'NotifyOnSyncError', label: 'Account sync errors' },
] as const

export function SettingsPage() {
  const { user, logout } = useAuth()
  const { data: budget, isLoading } = useSharedBudget(user?.SharedBudgetId)
  const { data: preference } = useNotificationPreference(user?.Id)
  const updatePreference = useUpdateNotificationPreference()
  const push = usePushSubscription(user?.Id)
  const leaveBudget = useLeaveSharedBudget()
  const { show } = useToast()
  const [copied, setCopied] = useState(false)

  const copyInviteCode = async () => {
    if (!budget) return
    await navigator.clipboard.writeText(budget.InviteCode)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }

  const togglePreference = async (key: (typeof PREFERENCE_TOGGLES)[number]['key'], value: boolean) => {
    if (!preference) return
    try {
      await updatePreference.mutateAsync({ id: preference.Id, pref: { ...preference, [key]: value } })
    } catch {
      show('Could not update preference', 'error')
    }
  }

  const handleLeave = async () => {
    if (!user || !confirm('Leave this shared budget? You can rejoin later with an invite code.')) return
    try {
      await leaveBudget.mutateAsync(user.Id)
      window.location.href = '/onboarding'
    } catch {
      show('Could not leave budget', 'error')
    }
  }

  if (isLoading) return <FullPageSpinner />

  return (
    <div className="flex flex-col gap-5 pb-4">
      <h1 className="text-2xl font-semibold tracking-tight text-[var(--text-primary)]">Settings</h1>

      <Card className="flex items-center gap-4 p-4">
        <div className="flex h-14 w-14 shrink-0 items-center justify-center rounded-full bg-brand-100 text-lg font-semibold text-brand-700 dark:bg-brand-500/15 dark:text-brand-300">
          {user && initials(user.FirstName, user.LastName)}
        </div>
        <div className="min-w-0">
          <p className="truncate font-semibold text-[var(--text-primary)]">
            {user?.FirstName} {user?.LastName}
          </p>
          <p className="truncate text-sm text-[var(--text-muted)]">{user?.Email}</p>
          <p className="truncate text-xs text-[var(--text-muted)]">@{user?.UserId}</p>
        </div>
      </Card>

      {budget && (
        <Card className="flex flex-col gap-4 p-4">
          <div className="flex items-center justify-between">
            <h2 className="font-semibold text-[var(--text-primary)]">{budget.Name}</h2>
            <span className="text-xs text-[var(--text-muted)]">
              {budget.MemberCount}/{budget.MaxMembers} members
            </span>
          </div>
          {budget.Description && <p className="text-sm text-[var(--text-secondary)]">{budget.Description}</p>}

          <div className="flex items-center justify-between rounded-xl bg-[var(--surface-sunken)] px-3 py-2.5">
            <div>
              <p className="text-xs text-[var(--text-muted)]">Invite code</p>
              <p className="font-mono text-sm font-semibold tracking-widest text-[var(--text-primary)]">{budget.InviteCode}</p>
            </div>
            <button
              onClick={copyInviteCode}
              className="flex items-center gap-1.5 rounded-lg bg-[var(--surface-raised)] px-3 py-1.5 text-sm font-medium text-[var(--text-primary)] shadow-sm"
            >
              {copied ? <IconCheck size={15} className="text-mint-500" /> : <IconCopy size={15} />}
              {copied ? 'Copied' : 'Copy'}
            </button>
          </div>

          <div className="flex flex-wrap gap-2">
            {budget.Members.map((m) => (
              <span
                key={m.Id}
                className="flex items-center gap-1.5 rounded-full bg-[var(--surface-sunken)] py-1 pl-1 pr-3 text-xs font-medium text-[var(--text-secondary)]"
              >
                <span className="flex h-5 w-5 items-center justify-center rounded-full bg-brand-100 text-[10px] font-semibold text-brand-700 dark:bg-brand-500/20 dark:text-brand-300">
                  {initials(m.FirstName, m.LastName)}
                </span>
                {m.FullName}
                {m.IsOwner && <span className="text-[var(--text-muted)]">· Owner</span>}
              </span>
            ))}
          </div>

          <Button variant="secondary" onClick={handleLeave} loading={leaveBudget.isPending}>
            <IconDoorExit size={16} /> Leave budget
          </Button>
        </Card>
      )}

      <Card className="flex flex-col gap-1 p-4">
        <h2 className="mb-2 font-semibold text-[var(--text-primary)]">Notifications</h2>
        <div className="flex items-center justify-between py-2">
          <span className="text-sm text-[var(--text-secondary)]">Push notifications</span>
          <Switch checked={push.enabled} onChange={push.toggle} disabled={!push.supported || push.busy} />
        </div>
        {preference &&
          PREFERENCE_TOGGLES.map(({ key, label }) => (
            <div key={key} className="flex items-center justify-between py-2">
              <span className="text-sm text-[var(--text-secondary)]">{label}</span>
              <Switch checked={preference[key]} onChange={(v) => togglePreference(key, v)} />
            </div>
          ))}
      </Card>

      <Button variant="danger" onClick={logout}>
        <IconLogout size={16} /> Log out
      </Button>
    </div>
  )
}
