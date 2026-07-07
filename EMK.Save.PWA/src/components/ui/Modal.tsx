import { useEffect, type ReactNode } from 'react'
import { createPortal } from 'react-dom'
import { IconX } from '@tabler/icons-react'

interface ModalProps {
  open: boolean
  onClose: () => void
  title?: string
  children: ReactNode
}

export function Modal({ open, onClose, title, children }: ModalProps) {
  useEffect(() => {
    if (!open) return
    const onKey = (e: KeyboardEvent) => e.key === 'Escape' && onClose()
    document.addEventListener('keydown', onKey)
    document.body.style.overflow = 'hidden'
    return () => {
      document.removeEventListener('keydown', onKey)
      document.body.style.overflow = ''
    }
  }, [open, onClose])

  if (!open) return null

  return createPortal(
    <div className="fixed inset-0 z-50 flex items-end justify-center sm:items-center animate-fade-in">
      <div className="absolute inset-0 bg-black/50 backdrop-blur-[2px]" onClick={onClose} />
      <div className="relative z-10 max-h-[90vh] w-full max-w-lg animate-slide-up overflow-y-auto rounded-t-3xl border border-[var(--border-subtle)] bg-[var(--surface-raised)] p-5 shadow-xl sm:rounded-3xl sm:p-6">
        {title && (
          <div className="mb-4 flex items-center justify-between">
            <h2 className="text-lg font-semibold text-[var(--text-primary)]">{title}</h2>
            <button
              onClick={onClose}
              className="rounded-full p-1.5 text-[var(--text-muted)] hover:bg-[var(--surface-sunken)]"
              aria-label="Close"
            >
              <IconX size={18} />
            </button>
          </div>
        )}
        {children}
      </div>
    </div>,
    document.body,
  )
}
