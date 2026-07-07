import { useState } from 'react'
import { subscribeToPush, unsubscribeFromPush, isPushSupported } from '@/lib/push'
import { useNotificationPreference, useSubscribePush, useUnsubscribePush } from '@/hooks/useNotifications'
import { useToast } from '@/context/ToastContext'

export function usePushSubscription(userId: string | null | undefined) {
  const { data: preference, isLoading } = useNotificationPreference(userId)
  const subscribePush = useSubscribePush()
  const unsubscribePush = useUnsubscribePush()
  const { show } = useToast()
  const [busy, setBusy] = useState(false)

  const enabled = !!preference?.IsPushEnabled
  const supported = isPushSupported()

  const toggle = async (next: boolean) => {
    if (!userId) return
    setBusy(true)
    try {
      if (next) {
        const subscription = await subscribeToPush()
        if (!subscription) {
          show('Enable notifications in your browser settings first.', 'error')
          return
        }
        await subscribePush.mutateAsync({ userId, subscription })
        show('Push notifications enabled', 'success')
      } else {
        await unsubscribeFromPush()
        await unsubscribePush.mutateAsync(userId)
        show('Push notifications disabled', 'info')
      }
    } catch {
      show('Could not update push settings', 'error')
    } finally {
      setBusy(false)
    }
  }

  return { preference, enabled, supported, isLoading, busy, toggle }
}
