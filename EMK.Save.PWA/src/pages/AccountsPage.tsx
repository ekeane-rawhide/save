import { useCallback, useEffect, useState } from 'react'
import { usePlaidLink, type PlaidLinkOnSuccessMetadata } from 'react-plaid-link'
import { IconBuildingBank, IconPlus, IconRefresh, IconTrash } from '@tabler/icons-react'
import { useAuth } from '@/context/AuthContext'
import {
  useExchangePublicToken, useLinkToken, useLinkedAccounts, useSyncAccount, useUnlinkAccount,
} from '@/hooks/useAccounts'
import { useToast } from '@/context/ToastContext'
import { Card } from '@/components/ui/Card'
import { Button } from '@/components/ui/Button'
import { EmptyState } from '@/components/ui/EmptyState'
import { FullPageSpinner, Spinner } from '@/components/ui/Spinner'
import { formatCurrency, formatDate } from '@/lib/format'

function ConnectBankButton({ onConnected }: { onConnected: () => void }) {
  const { user } = useAuth()
  const getLinkToken = useLinkToken(user?.Id)
  const exchangeToken = useExchangePublicToken()
  const { show } = useToast()
  const [linkToken, setLinkToken] = useState<string | null>(null)

  const onSuccess = useCallback(
    async (publicToken: string, metadata: PlaidLinkOnSuccessMetadata) => {
      if (!user?.SharedBudgetId) return
      try {
        await exchangeToken.mutateAsync({
          sharedBudgetId: user.SharedBudgetId,
          userId: user.Id,
          exchange: {
            PublicToken: publicToken,
            InstitutionId: metadata.institution?.institution_id ?? '',
            InstitutionName: metadata.institution?.name ?? 'Bank',
            InstitutionLogoUrl: '',
            SelectedAccountIds: metadata.accounts.map((a) => a.id),
          },
        })
        show(`${metadata.institution?.name ?? 'Account'} connected`, 'success')
        onConnected()
      } catch {
        show('Could not finish connecting account', 'error')
      } finally {
        setLinkToken(null)
      }
    },
    [user, exchangeToken, show, onConnected],
  )

  const { open, ready } = usePlaidLink({ token: linkToken, onSuccess })

  useEffect(() => {
    if (linkToken && ready) open()
  }, [linkToken, ready, open])

  const startLink = async () => {
    try {
      const token = await getLinkToken.mutateAsync()
      setLinkToken(token.LinkToken)
    } catch {
      show('Could not start Plaid Link. Have Plaid sandbox credentials been configured?', 'error')
    }
  }

  return (
    <Button onClick={startLink} loading={getLinkToken.isPending || (!!linkToken && !ready)}>
      <IconPlus size={16} /> Link account
    </Button>
  )
}

export function AccountsPage() {
  const { user } = useAuth()
  const { data: accounts, isLoading } = useLinkedAccounts(user?.SharedBudgetId)
  const unlinkAccount = useUnlinkAccount()
  const syncAccount = useSyncAccount()
  const { show } = useToast()
  const [syncingId, setSyncingId] = useState<string | null>(null)

  const unlink = async (id: string) => {
    try {
      await unlinkAccount.mutateAsync(id)
      show('Account removed', 'success')
    } catch {
      show('Could not remove account', 'error')
    }
  }

  const sync = async (id: string) => {
    setSyncingId(id)
    try {
      const newCount = await syncAccount.mutateAsync(id)
      show(newCount > 0 ? `${newCount} new transaction(s) synced` : 'Already up to date', 'success')
    } catch {
      show('Could not sync this account', 'error')
    } finally {
      setSyncingId(null)
    }
  }

  if (isLoading) return <FullPageSpinner />

  return (
    <div className="flex flex-col gap-5 pb-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-2xl font-semibold tracking-tight text-[var(--text-primary)]">Linked Accounts</h1>
        <ConnectBankButton onConnected={() => {}} />
      </div>

      {!accounts || accounts.length === 0 ? (
        <EmptyState
          icon={<IconBuildingBank size={22} />}
          title="No accounts linked"
          description="Connect a bank with Plaid to start syncing transactions automatically."
          action={<ConnectBankButton onConnected={() => {}} />}
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
              <div className="flex flex-col items-end gap-1">
                <button
                  onClick={() => unlink(a.Id)}
                  className="rounded-lg p-2 text-[var(--text-muted)] hover:bg-[var(--surface-sunken)] hover:text-red-500"
                  aria-label="Remove account"
                >
                  <IconTrash size={16} />
                </button>
                <button
                  onClick={() => sync(a.Id)}
                  disabled={syncingId === a.Id}
                  className="rounded-lg p-2 text-[var(--text-muted)] hover:bg-[var(--surface-sunken)] hover:text-brand-600"
                  aria-label="Sync now"
                  title="Sync transactions now"
                >
                  {syncingId === a.Id ? <Spinner size={16} /> : <IconRefresh size={16} />}
                </button>
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  )
}
