import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/apiClient'
import type { DashboardSummary } from '@/types/models'

export function useDashboard(
  sharedBudgetId: string | null | undefined,
  userId: string | null | undefined,
  month: number,
  year: number,
) {
  return useQuery({
    queryKey: ['dashboard', sharedBudgetId, userId, month, year],
    queryFn: () =>
      apiClient.get<DashboardSummary>(`Dashboard/${sharedBudgetId}/${userId}/${month}/${year}`),
    enabled: !!sharedBudgetId && !!userId,
  })
}
