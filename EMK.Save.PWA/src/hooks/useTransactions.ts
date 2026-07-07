import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/apiClient'
import type { Transaction } from '@/types/models'

export function useTransactions(sharedBudgetId: string | null | undefined, month: number, year: number) {
  return useQuery({
    queryKey: ['transactions', sharedBudgetId, month, year],
    queryFn: () => apiClient.get<Transaction[]>(`Transaction/${sharedBudgetId}/${month}/${year}`),
    enabled: !!sharedBudgetId,
  })
}

export function useUnassignedTransactions(
  sharedBudgetId: string | null | undefined,
  month: number,
  year: number,
) {
  return useQuery({
    queryKey: ['transactions', 'unassigned', sharedBudgetId, month, year],
    queryFn: () =>
      apiClient.get<Transaction[]>(`Transaction/unassigned/${sharedBudgetId}/${month}/${year}`),
    enabled: !!sharedBudgetId,
  })
}

export function useAssignCategory() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ transactionId, categoryId }: { transactionId: string; categoryId: string | null }) =>
      apiClient.put(`Transaction/assign/${transactionId}`, { CategoryId: categoryId }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard'] })
      queryClient.invalidateQueries({ queryKey: ['budgets'] })
    },
  })
}

export function useUpdateTransaction() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, transaction }: { id: string; transaction: Transaction }) =>
      apiClient.put(`Transaction/${id}`, transaction),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard'] })
    },
  })
}
