import { NavLink } from 'react-router-dom'
import { NAV_ITEMS } from './nav'

export function BottomNav() {
  const items = NAV_ITEMS.filter((i) => i.mobile)

  return (
    <nav className="fixed inset-x-0 bottom-0 z-40 flex border-t border-[var(--border-subtle)] bg-[var(--surface-raised)]/95 backdrop-blur safe-bottom md:hidden">
      {items.map(({ to, label, icon: Icon }) => (
        <NavLink
          key={to}
          to={to}
          end={to === '/'}
          className={({ isActive }) =>
            `flex flex-1 flex-col items-center gap-1 py-2.5 text-[11px] font-medium transition-colors ${
              isActive ? 'text-brand-600' : 'text-[var(--text-muted)]'
            }`
          }
        >
          {({ isActive }) => (
            <>
              <Icon size={22} stroke={isActive ? 2.1 : 1.75} />
              {label}
            </>
          )}
        </NavLink>
      ))}
    </nav>
  )
}
