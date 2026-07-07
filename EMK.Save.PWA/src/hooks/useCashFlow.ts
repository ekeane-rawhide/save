import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/apiClient'
import type { CashFlowEntry } from '@/types/models'

export function useCashFlow(sharedBudgetId: string | null | undefined, month: number, year: number) {
  return useQuery({
    queryKey: ['cashflow', sharedBudgetId, month, year],
    queryFn: () => apiClient.get<CashFlowEntry[]>(`CashFlow/${sharedBudgetId}/${month}/${year}`),
    enabled: !!sharedBudgetId,
  })
}

export function useBuildCashFlow() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({
      sharedBudgetId,
      month,
      year,
    }: {
      sharedBudgetId: string
      month: number
      year: number
    }) => apiClient.post<CashFlowEntry[]>(`CashFlow/build/${sharedBudgetId}/${month}/${year}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['cashflow'] }),
  })
}
