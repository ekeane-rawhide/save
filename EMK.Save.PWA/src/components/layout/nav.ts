import {
  IconLayoutDashboard, IconArrowsExchange, IconChartPie, IconChartLine,
  IconBulb, IconBuildingBank, IconBell, IconSettings, type Icon,
} from '@tabler/icons-react'

export interface NavItem {
  to: string
  label: string
  icon: Icon
  mobile: boolean
}

export const NAV_ITEMS: NavItem[] = [
  { to: '/', label: 'Dashboard', icon: IconLayoutDashboard, mobile: true },
  { to: '/transactions', label: 'Transactions', icon: IconArrowsExchange, mobile: true },
  { to: '/budgets', label: 'Budgets', icon: IconChartPie, mobile: true },
  { to: '/cashflow', label: 'Cash Flow', icon: IconChartLine, mobile: false },
  { to: '/insights', label: 'Insights', icon: IconBulb, mobile: true },
  { to: '/accounts', label: 'Accounts', icon: IconBuildingBank, mobile: false },
  { to: '/notifications', label: 'Notifications', icon: IconBell, mobile: false },
  { to: '/settings', label: 'Settings', icon: IconSettings, mobile: true },
]
