import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/apiClient'
import type { TrackingInsight } from '@/types/models'

export function useInsights(sharedBudgetId: string | null | undefined, month: number, year: number) {
  return useQuery({
    queryKey: ['insights', sharedBudgetId, month, year],
    queryFn: () => apiClient.get<TrackingInsight[]>(`TrackingInsight/${sharedBudgetId}/${month}/${year}`),
    enabled: !!sharedBudgetId,
  })
}

export function useGenerateInsights() {
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
    }) => apiClient.post<TrackingInsight[]>(`TrackingInsight/generate/${sharedBudgetId}/${month}/${year}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['insights'] }),
  })
}

export function useDismissInsight() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => apiClient.put(`TrackingInsight/dismiss/${id}`, undefined),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['insights'] }),
  })
}
