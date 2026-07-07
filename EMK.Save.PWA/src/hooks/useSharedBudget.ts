import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/apiClient'
import type { SharedBudget } from '@/types/models'

export function useSharedBudget(id: string | null | undefined) {
  return useQuery({
    queryKey: ['sharedBudget', id],
    queryFn: () => apiClient.get<SharedBudget>(`SharedBudget/${id}`),
    enabled: !!id,
  })
}

export function usePreviewInviteCode() {
  return useMutation({
    mutationFn: (code: string) => apiClient.get<SharedBudget>(`SharedBudget/preview/${code}`),
  })
}

export function useCreateSharedBudget() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (fields: { OwnerId: string; Name: string; Description: string }) =>
      apiClient.post<SharedBudget>('SharedBudget/create', fields),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['sharedBudget'] }),
  })
}

export function useJoinSharedBudget() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (fields: { UserId: string; InviteCode: string }) =>
      apiClient.post<SharedBudget>('SharedBudget/join', fields),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['sharedBudget'] }),
  })
}

export function useLeaveSharedBudget() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: async (userId: string) => {
      const result = await apiClient.post<{ rowsaffected: string }>(`SharedBudget/leave/${userId}`)
      return parseInt(result.rowsaffected, 10)
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['sharedBudget'] }),
  })
}
