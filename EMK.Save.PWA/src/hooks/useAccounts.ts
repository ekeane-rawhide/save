import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/apiClient'
import type { PlaidAccount, PlaidLinkToken } from '@/types/models'

export function useLinkedAccounts(sharedBudgetId: string | null | undefined) {
  return useQuery({
    queryKey: ['accounts', sharedBudgetId],
    queryFn: () => apiClient.get<PlaidAccount[]>(`PlaidAccount/${sharedBudgetId}`),
    enabled: !!sharedBudgetId,
  })
}

export function useLinkToken(userId: string | null | undefined) {
  return useMutation({
    mutationFn: () => apiClient.get<PlaidLinkToken>(`PlaidAccount/linktoken/${userId}`),
  })
}

export function useExchangePublicToken() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({
      sharedBudgetId,
      userId,
      exchange,
    }: {
      sharedBudgetId: string
      userId: string
      exchange: {
        PublicToken: string
        InstitutionId: string
        InstitutionName: string
        InstitutionLogoUrl: string
        SelectedAccountIds: string[]
      }
    }) => apiClient.getInsertedId(`PlaidAccount/exchange/${sharedBudgetId}/${userId}`, exchange),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['accounts'] }),
  })
}

export function useUnlinkAccount() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`PlaidAccount/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['accounts'] }),
  })
}
