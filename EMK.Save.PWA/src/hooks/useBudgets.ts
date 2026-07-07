import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/apiClient'
import type { Budget } from '@/types/models'

export function useBudgets(sharedBudgetId: string | null | undefined, month: number, year: number) {
  return useQuery({
    queryKey: ['budgets', sharedBudgetId, month, year],
    queryFn: () => apiClient.get<Budget[]>(`Budget/${sharedBudgetId}/${month}/${year}`),
    enabled: !!sharedBudgetId,
  })
}

export function useCreateBudget() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (budget: Partial<Budget>) => apiClient.getInsertedId('Budget', budget),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['budgets'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard'] })
    },
  })
}

export function useUpdateBudget() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, budget }: { id: string; budget: Partial<Budget> }) =>
      apiClient.put(`Budget/${id}`, budget),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['budgets'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard'] })
    },
  })
}

export function useDeleteBudget() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`Budget/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['budgets'] }),
  })
}
