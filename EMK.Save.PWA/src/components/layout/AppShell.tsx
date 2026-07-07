import { Outlet } from 'react-router-dom'
import { useCallback } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { Sidebar } from './Sidebar'
import { BottomNav } from './BottomNav'
import { TopBar } from './TopBar'
import { useRealtimeEvent } from '@/context/RealtimeContext'
import { useToast } from '@/context/ToastContext'
import { PeriodProvider } from '@/context/PeriodContext'
import { showLocalNotification } from '@/lib/push'
import type { PushNotification, Transaction, TrackingInsight } from '@/types/models'

export function AppShell() {
  const queryClient = useQueryClient()
  const { show } = useToast()

  const onTransactionsSynced = useCallback(
    (payload: { sharedBudgetId: string; newCount: number }) => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard'] })
      if (payload.newCount > 0) show(`${payload.newCount} new transaction(s) synced`, 'info')
    },
    [queryClient, show],
  )

  const onTransactionUpdated = useCallback(
    (_transaction: Transaction) => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard'] })
      queryClient.invalidateQueries({ queryKey: ['budgets'] })
    },
    [queryClient],
  )

  const onNotificationReceived = useCallback(
    (notification: PushNotification) => {
      queryClient.invalidateQueries({ queryKey: ['notifications'] })
      if (document.hidden) {
        showLocalNotification(notification.Title || 'Save', notification.Body, notification.ActionUrl)
      } else {
        show(notification.Title || 'New notification', 'info')
      }
    },
    [queryClient, show],
  )

  const onInsightsGenerated = useCallback(
    (_insights: TrackingInsight[]) => {
      queryClient.invalidateQueries({ queryKey: ['insights'] })
    },
    [queryClient],
  )

  useRealtimeEvent('TransactionsSynced', onTransactionsSynced)
  useRealtimeEvent('TransactionUpdated', onTransactionUpdated)
  useRealtimeEvent('NotificationReceived', onNotificationReceived)
  useRealtimeEvent('InsightsGenerated', onInsightsGenerated)

  return (
    <div className="flex h-full min-h-screen">
      <Sidebar />
      <div className="flex min-w-0 flex-1 flex-col">
        <TopBar />
        <main className="flex-1 px-4 pb-20 pt-4 md:px-8 md:pb-8 md:pt-6">
          <div className="mx-auto w-full max-w-5xl animate-fade-in">
            <PeriodProvider>
              <Outlet />
            </PeriodProvider>
          </div>
        </main>
      </div>
      <BottomNav />
    </div>
  )
}
