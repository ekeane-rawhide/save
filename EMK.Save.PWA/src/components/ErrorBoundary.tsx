import { Component, type ErrorInfo, type ReactNode } from 'react'
import { IconAlertTriangle, IconRefresh } from '@tabler/icons-react'
import { Logo } from '@/components/Logo'
import { Button } from '@/components/ui/Button'

interface Props {
  children: ReactNode
}

interface State {
  hasError: boolean
}

export class ErrorBoundary extends Component<Props, State> {
  state: State = { hasError: false }

  static getDerivedStateFromError(): State {
    return { hasError: true }
  }

  componentDidCatch(error: Error, info: ErrorInfo) {
    // eslint-disable-next-line no-console
    console.error('Unhandled render error', error, info.componentStack)
  }

  render() {
    if (!this.state.hasError) return this.props.children

    return (
      <div className="flex min-h-screen flex-col items-center justify-center gap-5 bg-[var(--surface)] px-6 text-center">
        <Logo size={48} />
        <div className="flex flex-col items-center gap-2">
          <div className="flex h-12 w-12 items-center justify-center rounded-full bg-red-100 text-red-600 dark:bg-red-500/15 dark:text-red-400">
            <IconAlertTriangle size={24} />
          </div>
          <h1 className="text-lg font-semibold text-[var(--text-primary)]">Something went wrong</h1>
          <p className="max-w-sm text-sm text-[var(--text-secondary)]">
            An unexpected error occurred. Reloading usually fixes it — if it keeps happening, please let us know.
          </p>
        </div>
        <Button onClick={() => window.location.reload()}>
          <IconRefresh size={16} /> Reload
        </Button>
      </div>
    )
  }
}
