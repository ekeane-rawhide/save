import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useNavigate } from 'react-router-dom'
import { IconAlertCircle, IconTicket, IconWallet } from '@tabler/icons-react'
import { AuthLayout } from '@/components/layout/AuthLayout'
import { Input } from '@/components/ui/Input'
import { Button } from '@/components/ui/Button'
import { useAuth } from '@/context/AuthContext'
import { useCreateSharedBudget, useJoinSharedBudget } from '@/hooks/useSharedBudget'
import { BudgetRole } from '@/types/models'
import { ApiError } from '@/lib/apiClient'

const createSchema = z.object({
  name: z.string().min(1, 'Give your budget a name'),
  description: z.string().optional(),
})
type CreateValues = z.infer<typeof createSchema>

const joinSchema = z.object({
  inviteCode: z.string().min(1, 'Enter the invite code'),
})
type JoinValues = z.infer<typeof joinSchema>

export function OnboardingPage() {
  const [tab, setTab] = useState<'create' | 'join'>('create')
  const { user, updateUser, logout } = useAuth()
  const navigate = useNavigate()
  const createBudget = useCreateSharedBudget()
  const joinBudget = useJoinSharedBudget()

  const createForm = useForm<CreateValues>({ resolver: zodResolver(createSchema) })
  const joinForm = useForm<JoinValues>({ resolver: zodResolver(joinSchema) })

  const onCreate = async (values: CreateValues) => {
    if (!user) return
    try {
      const budget = await createBudget.mutateAsync({
        OwnerId: user.Id,
        Name: values.name,
        Description: values.description ?? '',
      })
      updateUser({ SharedBudgetId: budget.Id, BudgetRole: BudgetRole.Owner })
      navigate('/', { replace: true })
    } catch (err) {
      createForm.setError('root', {
        message: err instanceof ApiError ? err.message : 'Could not create budget.',
      })
    }
  }

  const onJoin = async (values: JoinValues) => {
    if (!user) return
    try {
      const budget = await joinBudget.mutateAsync({ UserId: user.Id, InviteCode: values.inviteCode })
      updateUser({ SharedBudgetId: budget.Id, BudgetRole: BudgetRole.Member })
      navigate('/', { replace: true })
    } catch (err) {
      joinForm.setError('root', {
        message: err instanceof ApiError ? err.message : 'Could not join budget.',
      })
    }
  }

  return (
    <AuthLayout
      title={`Hey ${user?.FirstName ?? ''} 👋`}
      subtitle="Create a shared budget or join one with an invite code."
      footer={
        <button onClick={logout} className="font-medium text-[var(--text-muted)] hover:underline">
          Log out
        </button>
      }
    >
      <div className="mb-5 flex rounded-xl bg-[var(--surface-sunken)] p-1">
        <button
          onClick={() => setTab('create')}
          className={`flex flex-1 items-center justify-center gap-1.5 rounded-lg py-2 text-sm font-medium transition-colors ${
            tab === 'create' ? 'bg-[var(--surface-raised)] text-[var(--text-primary)] shadow-sm' : 'text-[var(--text-muted)]'
          }`}
        >
          <IconWallet size={16} /> Create
        </button>
        <button
          onClick={() => setTab('join')}
          className={`flex flex-1 items-center justify-center gap-1.5 rounded-lg py-2 text-sm font-medium transition-colors ${
            tab === 'join' ? 'bg-[var(--surface-raised)] text-[var(--text-primary)] shadow-sm' : 'text-[var(--text-muted)]'
          }`}
        >
          <IconTicket size={16} /> Join
        </button>
      </div>

      {tab === 'create' ? (
        <form onSubmit={createForm.handleSubmit(onCreate)} className="flex flex-col gap-4" noValidate>
          {createForm.formState.errors.root && (
            <div className="flex items-center gap-2 rounded-xl bg-red-50 px-3 py-2.5 text-sm text-red-600 dark:bg-red-500/10 dark:text-red-400">
              <IconAlertCircle size={16} className="shrink-0" />
              {createForm.formState.errors.root.message}
            </div>
          )}
          <Input
            label="Budget name"
            placeholder="The Smith Household"
            error={createForm.formState.errors.name?.message}
            {...createForm.register('name')}
          />
          <Input
            label="Description (optional)"
            placeholder="Shared monthly expenses"
            {...createForm.register('description')}
          />
          <Button type="submit" fullWidth loading={createForm.formState.isSubmitting} className="mt-1">
            Create budget
          </Button>
        </form>
      ) : (
        <form onSubmit={joinForm.handleSubmit(onJoin)} className="flex flex-col gap-4" noValidate>
          {joinForm.formState.errors.root && (
            <div className="flex items-center gap-2 rounded-xl bg-red-50 px-3 py-2.5 text-sm text-red-600 dark:bg-red-500/10 dark:text-red-400">
              <IconAlertCircle size={16} className="shrink-0" />
              {joinForm.formState.errors.root.message}
            </div>
          )}
          <Input
            label="Invite code"
            placeholder="ABC123"
            className="uppercase tracking-widest"
            error={joinForm.formState.errors.inviteCode?.message}
            {...joinForm.register('inviteCode')}
          />
          <Button type="submit" fullWidth loading={joinForm.formState.isSubmitting} className="mt-1">
            Join budget
          </Button>
        </form>
      )}
    </AuthLayout>
  )
}
