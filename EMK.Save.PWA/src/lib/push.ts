function urlBase64ToUint8Array(base64String: string): BufferSource {
  const padding = '='.repeat((4 - (base64String.length % 4)) % 4)
  const base64 = (base64String + padding).replace(/-/g, '+').replace(/_/g, '/')
  const rawData = atob(base64)
  return Uint8Array.from([...rawData].map((c) => c.charCodeAt(0))) as BufferSource
}

export function isPushSupported(): boolean {
  return 'serviceWorker' in navigator && 'PushManager' in window && 'Notification' in window
}

export async function requestNotificationPermission(): Promise<NotificationPermission> {
  if (!('Notification' in window)) return 'denied'
  if (Notification.permission !== 'default') return Notification.permission
  return Notification.requestPermission()
}

export interface PushSubscriptionDto {
  Endpoint: string
  P256dh: string
  Auth: string
}

function toDto(subscription: PushSubscription): PushSubscriptionDto {
  const json = subscription.toJSON()
  return {
    Endpoint: json.endpoint ?? '',
    P256dh: json.keys?.p256dh ?? '',
    Auth: json.keys?.auth ?? '',
  }
}

export async function subscribeToPush(): Promise<PushSubscriptionDto | null> {
  if (!isPushSupported()) return null

  const permission = await requestNotificationPermission()
  if (permission !== 'granted') return null

  const registration = await navigator.serviceWorker.ready
  let subscription = await registration.pushManager.getSubscription()

  if (!subscription) {
    subscription = await registration.pushManager.subscribe({
      userVisibleOnly: true,
      applicationServerKey: urlBase64ToUint8Array(import.meta.env.VITE_VAPID_PUBLIC_KEY),
    })
  }

  return toDto(subscription)
}

export async function unsubscribeFromPush(): Promise<void> {
  if (!isPushSupported()) return
  const registration = await navigator.serviceWorker.ready
  const subscription = await registration.pushManager.getSubscription()
  await subscription?.unsubscribe()
}

export function showLocalNotification(title: string, body: string, url?: string) {
  if (Notification.permission !== 'granted') return
  navigator.serviceWorker.ready.then((registration) => {
    registration.showNotification(title, {
      body,
      icon: '/icons/icon-192.png',
      data: { url: url ?? '/' },
    })
  })
}
