import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/apiClient'
import type { NotificationPreference, PushNotification } from '@/types/models'

export function useNotifications(userId: string | null | undefined) {
  return useQuery({
    queryKey: ['notifications', userId],
    queryFn: () => apiClient.get<PushNotification[]>(`PushNotification/${userId}`),
    enabled: !!userId,
  })
}

export function useUnreadNotifications(userId: string | null | undefined) {
  return useQuery({
    queryKey: ['notifications', 'unread', userId],
    queryFn: () => apiClient.get<PushNotification[]>(`PushNotification/unread/${userId}`),
    enabled: !!userId,
    refetchInterval: 60_000,
  })
}

export function useMarkNotificationRead() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => apiClient.put(`PushNotification/markread/${id}`, undefined),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['notifications'] }),
  })
}

export function useNotificationPreference(userId: string | null | undefined) {
  return useQuery({
    queryKey: ['notificationPreference', userId],
    queryFn: () => apiClient.get<NotificationPreference>(`NotificationPreference/${userId}`),
    enabled: !!userId,
  })
}

export function useUpdateNotificationPreference() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, pref }: { id: string; pref: NotificationPreference }) =>
      apiClient.put(`NotificationPreference/${id}`, pref),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['notificationPreference'] }),
  })
}

export function useSubscribePush() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({
      userId,
      subscription,
    }: {
      userId: string
      subscription: { Endpoint: string; P256dh: string; Auth: string }
    }) => apiClient.post(`NotificationPreference/subscribe/${userId}`, subscription),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['notificationPreference'] }),
  })
}

export function useUnsubscribePush() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (userId: string) => apiClient.post(`NotificationPreference/unsubscribe/${userId}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['notificationPreference'] }),
  })
}
