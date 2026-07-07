import { Link } from 'react-router-dom'
import { IconBell, IconSun, IconMoon, IconDeviceDesktop } from '@tabler/icons-react'
import { Logo } from '@/components/Logo'
import { useAuth } from '@/context/AuthContext'
import { useTheme } from '@/context/ThemeContext'
import { useUnreadNotifications } from '@/hooks/useNotifications'

const THEME_CYCLE = ['system', 'light', 'dark'] as const

export function TopBar() {
  const { user } = useAuth()
  const { mode, setMode } = useTheme()
  const { data: unread } = useUnreadNotifications(user?.Id)

  const cycleTheme = () => {
    const idx = THEME_CYCLE.indexOf(mode)
    setMode(THEME_CYCLE[(idx + 1) % THEME_CYCLE.length])
  }

  const ThemeIcon = mode === 'light' ? IconSun : mode === 'dark' ? IconMoon : IconDeviceDesktop

  return (
    <header className="safe-top sticky top-0 z-30 flex h-16 items-center justify-between border-b border-[var(--border-subtle)] bg-[var(--surface-raised)]/90 px-4 backdrop-blur md:px-6">
      <div className="md:hidden">
        <Logo size={26} withWordmark />
      </div>
      <div className="hidden md:block" />

      <div className="flex items-center gap-1.5">
        <button
          onClick={cycleTheme}
          className="rounded-lg p-2 text-[var(--text-muted)] hover:bg-[var(--surface-sunken)] hover:text-[var(--text-primary)]"
          aria-label="Toggle theme"
          title={`Theme: ${mode}`}
        >
          <ThemeIcon size={19} stroke={1.75} />
        </button>
        <Link
          to="/notifications"
          className="relative rounded-lg p-2 text-[var(--text-muted)] hover:bg-[var(--surface-sunken)] hover:text-[var(--text-primary)]"
          aria-label="Notifications"
        >
          <IconBell size={19} stroke={1.75} />
          {!!unread?.length && (
            <span className="absolute right-1 top-1 flex h-4 min-w-4 items-center justify-center rounded-full bg-red-500 px-1 text-[10px] font-semibold text-white">
              {unread.length > 9 ? '9+' : unread.length}
            </span>
          )}
        </Link>
      </div>
    </header>
  )
}
