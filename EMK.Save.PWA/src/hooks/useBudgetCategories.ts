import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/apiClient'
import type { BudgetCategory } from '@/types/models'

export function useBudgetCategories(sharedBudgetId: string | null | undefined) {
  return useQuery({
    queryKey: ['categories', sharedBudgetId],
    queryFn: () => apiClient.get<BudgetCategory[]>(`BudgetCategory/${sharedBudgetId}`),
    enabled: !!sharedBudgetId,
  })
}

export function useCreateBudgetCategory() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (category: Omit<BudgetCategory, 'Id' | 'CategoryTypeLabel'>) =>
      apiClient.getInsertedId('BudgetCategory', category),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['categories'] }),
  })
}

export function useUpdateBudgetCategory() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, category }: { id: string; category: BudgetCategory }) =>
      apiClient.put(`BudgetCategory/${id}`, category),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] })
      queryClient.invalidateQueries({ queryKey: ['budgets'] })
    },
  })
}

export function useDeleteBudgetCategory() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`BudgetCategory/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['categories'] }),
  })
}
