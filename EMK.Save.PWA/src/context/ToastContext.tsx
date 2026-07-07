import { createContext, useCallback, useContext, useState, type ReactNode } from 'react'
import { createPortal } from 'react-dom'
import { IconCheck, IconX, IconAlertTriangle, IconInfoCircle } from '@tabler/icons-react'

type ToastTone = 'success' | 'error' | 'info'

interface Toast {
  id: number
  message: string
  tone: ToastTone
}

interface ToastContextValue {
  show: (message: string, tone?: ToastTone) => void
}

const ToastContext = createContext<ToastContextValue | null>(null)

const icons: Record<ToastTone, ReactNode> = {
  success: <IconCheck size={18} className="text-mint-500" />,
  error: <IconAlertTriangle size={18} className="text-red-500" />,
  info: <IconInfoCircle size={18} className="text-brand-500" />,
}

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<Toast[]>([])

  const show = useCallback((message: string, tone: ToastTone = 'info') => {
    const id = Date.now() + Math.random()
    setToasts((prev) => [...prev, { id, message, tone }])
    setTimeout(() => setToasts((prev) => prev.filter((t) => t.id !== id)), 4000)
  }, [])

  return (
    <ToastContext.Provider value={{ show }}>
      {children}
      {createPortal(
        <div className="fixed inset-x-0 top-0 z-[100] flex flex-col items-center gap-2 p-4 safe-top">
          {toasts.map((t) => (
            <div
              key={t.id}
              className="flex animate-slide-up items-center gap-2 rounded-xl border border-[var(--border-subtle)] bg-[var(--surface-raised)] px-4 py-3 text-sm font-medium text-[var(--text-primary)] shadow-lg"
            >
              {icons[t.tone]}
              {t.message}
              <button
                onClick={() => setToasts((prev) => prev.filter((x) => x.id !== t.id))}
                className="ml-1 text-[var(--text-muted)]"
              >
                <IconX size={14} />
              </button>
            </div>
          ))}
        </div>,
        document.body,
      )}
    </ToastContext.Provider>
  )
}

export function useToast() {
  const ctx = useContext(ToastContext)
  if (!ctx) throw new Error('useToast must be used within ToastProvider')
  return ctx
}
