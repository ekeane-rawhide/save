import { NavLink } from 'react-router-dom'
import { IconLogout } from '@tabler/icons-react'
import { Logo } from '@/components/Logo'
import { NAV_ITEMS } from './nav'
import { useAuth } from '@/context/AuthContext'
import { useRealtime } from '@/context/RealtimeContext'

export function Sidebar() {
  const { user, logout } = useAuth()
  const { connected } = useRealtime()

  return (
    <aside className="hidden w-64 shrink-0 flex-col border-r border-[var(--border-subtle)] bg-[var(--surface-raised)] md:flex">
      <div className="flex h-16 items-center gap-2.5 px-5">
        <Logo size={30} withWordmark />
      </div>

      <nav className="flex-1 space-y-1 px-3 py-2">
        {NAV_ITEMS.map(({ to, label, icon: Icon }) => (
          <NavLink
            key={to}
            to={to}
            end={to === '/'}
            className={({ isActive }) =>
              `flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium transition-colors ${
                isActive
                  ? 'bg-brand-600 text-white shadow-sm shadow-brand-600/25'
                  : 'text-[var(--text-secondary)] hover:bg-[var(--surface-sunken)] hover:text-[var(--text-primary)]'
              }`
            }
          >
            <Icon size={19} stroke={1.75} />
            {label}
          </NavLink>
        ))}
      </nav>

      <div className="border-t border-[var(--border-subtle)] p-3">
        <div className="flex items-center gap-2.5 rounded-xl px-2 py-2">
          <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-brand-100 text-sm font-semibold text-brand-700 dark:bg-brand-500/20 dark:text-brand-300">
            {user?.FirstName?.[0]}
            {user?.LastName?.[0]}
          </div>
          <div className="min-w-0 flex-1">
            <p className="truncate text-sm font-medium text-[var(--text-primary)]">
              {user?.FirstName} {user?.LastName}
            </p>
            <p className="flex items-center gap-1.5 text-xs text-[var(--text-muted)]">
              <span className={`h-1.5 w-1.5 rounded-full ${connected ? 'bg-mint-500' : 'bg-[var(--text-muted)]'}`} />
              {connected ? 'Live' : 'Offline'}
            </p>
          </div>
          <button
            onClick={logout}
            className="rounded-lg p-2 text-[var(--text-muted)] hover:bg-[var(--surface-sunken)] hover:text-red-500"
            aria-label="Log out"
            title="Log out"
          >
            <IconLogout size={18} />
          </button>
        </div>
      </div>
    </aside>
  )
}
