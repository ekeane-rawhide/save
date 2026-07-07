import { useState } from 'react'
import { IconBuildingBank, IconPlus, IconTrash } from '@tabler/icons-react'
import { useAuth } from '@/context/AuthContext'
import { useExchangePublicToken, useLinkToken, useLinkedAccounts, useUnlinkAccount } from '@/hooks/useAccounts'
import { useToast } from '@/context/ToastContext'
import { Card } from '@/components/ui/Card'
import { Button } from '@/components/ui/Button'
import { Modal } from '@/components/ui/Modal'
import { EmptyState } from '@/components/ui/EmptyState'
import { FullPageSpinner } from '@/components/ui/Spinner'
import { formatCurrency, formatDate } from '@/lib/format'

const MOCK_INSTITUTIONS = [
  { id: 'ins_chase', name: 'Chase' },
  { id: 'ins_boa', name: 'Bank of America' },
  { id: 'ins_wells', name: 'Wells Fargo' },
  { id: 'ins_ally', name: 'Ally Bank' },
  { id: 'ins_chime', name: 'Chime' },
]

export function AccountsPage() {
  const { user } = useAuth()
  const { data: accounts, isLoading } = useLinkedAccounts(user?.SharedBudgetId)
  const getLinkToken = useLinkToken(user?.Id)
  const exchangeToken = useExchangePublicToken()
  const unlinkAccount = useUnlinkAccount()
  const { show } = useToast()

  const [open, setOpen] = useState(false)
  const [connectingId, setConnectingId] = useState<string | null>(null)

  const connect = async (institution: { id: string; name: string }) => {
    if (!user?.SharedBudgetId) return
    setConnectingId(institution.id)
    try {
      await getLinkToken.mutateAsync()
      await exchangeToken.mutateAsync({
        sharedBudgetId: user.SharedBudgetId,
        userId: user.Id,
        exchange: {
          PublicToken: `public-sandbox-${crypto.randomUUID()}`,
          InstitutionId: institution.id,
          InstitutionName: institution.name,
          InstitutionLogoUrl: '',
          SelectedAccountIds: [crypto.randomUUID()],
        },
      })
      show(`${institution.name} connected`, 'success')
      setOpen(false)
    } catch {
      show('Could not connect account', 'error')
    } finally {
      setConnectingId(null)
    }
  }

  const unlink = async (id: string) => {
    try {
      await unlinkAccount.mutateAsync(id)
      show('Account removed', 'success')
    } catch {
      show('Could not remove account', 'error')
    }
  }

  if (isLoading) return <FullPageSpinner />

  return (
    <div className="flex flex-col gap-5 pb-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-2xl font-semibold tracking-tight text-[var(--text-primary)]">Linked Accounts</h1>
        <Button onClick={() => setOpen(true)}>
          <IconPlus size={16} /> Link account
        </Button>
      </div>

      {!accounts || accounts.length === 0 ? (
        <EmptyState
          icon={<IconBuildingBank size={22} />}
          title="No accounts linked"
          description="Connect a bank to start syncing transactions automatically."
          action={
            <Button onClick={() => setOpen(true)}>
              <IconPlus size={16} /> Link your first account
            </Button>
          }
        />
      ) : (
        <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
          {accounts.map((a) => (
            <Card key={a.Id} className="flex items-start justify-between gap-3 p-4">
              <div className="flex gap-3">
                <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-brand-100 text-brand-700 dark:bg-brand-500/15 dark:text-brand-300">
                  <IconBuildingBank size={19} />
                </div>
                <div>
                  <p className="text-sm font-semibold text-[var(--text-primary)]">{a.InstitutionName}</p>
                  <p className="text-xs text-[var(--text-muted)]">
                    {a.AccountName} •••• {a.Mask}
                  </p>
                  <p className="mt-1.5 text-lg font-semibold text-[var(--text-primary)]">{formatCurrency(a.CurrentBalance)}</p>
                  <p className="text-xs text-[var(--text-muted)]">Synced {formatDate(a.LastSynced)}</p>
                </div>
              </div>
              <button
                onClick={() => unlink(a.Id)}
                className="rounded-lg p-2 text-[var(--text-muted)] hover:bg-[var(--surface-sunken)] hover:text-red-500"
                aria-label="Remove account"
              >
                <IconTrash size={16} />
              </button>
            </Card>
          ))}
        </div>
      )}

      <Modal open={open} onClose={() => setOpen(false)} title="Link a bank">
        <p className="mb-4 text-sm text-[var(--text-secondary)]">
          Choose your institution to securely connect an account.
        </p>
        <div className="flex flex-col gap-1.5">
          {MOCK_INSTITUTIONS.map((inst) => (
            <button
              key={inst.id}
              onClick={() => connect(inst)}
              disabled={connectingId !== null}
              className="flex items-center gap-3 rounded-xl px-3 py-2.5 text-left transition-colors hover:bg-[var(--surface-sunken)] disabled:opacity-50"
            >
              <div className="flex h-9 w-9 items-center justify-center rounded-full bg-[var(--surface-sunken)] text-[var(--text-secondary)]">
                <IconBuildingBank size={17} />
              </div>
              <span className="text-sm font-medium text-[var(--text-primary)]">{inst.name}</span>
              {connectingId === inst.id && <span className="ml-auto text-xs text-[var(--text-muted)]">Connecting…</span>}
            </button>
          ))}
        </div>
      </Modal>
    </div>
  )
}
