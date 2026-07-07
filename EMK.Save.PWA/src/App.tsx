import type { ReactNode } from 'react'
import { Navigate, Route, Routes, useLocation } from 'react-router-dom'
import { useAuth } from '@/context/AuthContext'
import { FullPageSpinner } from '@/components/ui/Spinner'
import { AppShell } from '@/components/layout/AppShell'
import { LoginPage } from '@/pages/auth/LoginPage'
import { SignupPage } from '@/pages/auth/SignupPage'
import { OnboardingPage } from '@/pages/auth/OnboardingPage'
import { DashboardPage } from '@/pages/DashboardPage'
import { TransactionsPage } from '@/pages/TransactionsPage'
import { BudgetsPage } from '@/pages/BudgetsPage'
import { CashFlowPage } from '@/pages/CashFlowPage'
import { InsightsPage } from '@/pages/InsightsPage'
import { AccountsPage } from '@/pages/AccountsPage'
import { NotificationsPage } from '@/pages/NotificationsPage'
import { SettingsPage } from '@/pages/SettingsPage'

function RequireAuth({ children }: { children: ReactNode }) {
  const { status } = useAuth()
  const location = useLocation()
  if (status === 'loading') return <FullPageSpinner />
  if (status === 'unauthenticated')
    return <Navigate to="/login" replace state={{ from: location.pathname }} />
  return <>{children}</>
}

function RequireBudget({ children }: { children: ReactNode }) {
  const { user } = useAuth()
  if (!user?.SharedBudgetId) return <Navigate to="/onboarding" replace />
  return <>{children}</>
}

function RequireOnboarding({ children }: { children: ReactNode }) {
  const { user } = useAuth()
  if (user?.SharedBudgetId) return <Navigate to="/" replace />
  return <>{children}</>
}

function RequireGuest({ children }: { children: ReactNode }) {
  const { status, user } = useAuth()
  if (status === 'loading') return <FullPageSpinner />
  if (status === 'authenticated') return <Navigate to={user?.SharedBudgetId ? '/' : '/onboarding'} replace />
  return <>{children}</>
}

function App() {
  return (
    <Routes>
      <Route path="/login" element={<RequireGuest><LoginPage /></RequireGuest>} />
      <Route path="/signup" element={<RequireGuest><SignupPage /></RequireGuest>} />
      <Route
        path="/onboarding"
        element={
          <RequireAuth>
            <RequireOnboarding>
              <OnboardingPage />
            </RequireOnboarding>
          </RequireAuth>
        }
      />
      <Route
        element={
          <RequireAuth>
            <RequireBudget>
              <AppShell />
            </RequireBudget>
          </RequireAuth>
        }
      >
        <Route path="/" element={<DashboardPage />} />
        <Route path="/transactions" element={<TransactionsPage />} />
        <Route path="/budgets" element={<BudgetsPage />} />
        <Route path="/cashflow" element={<CashFlowPage />} />
        <Route path="/insights" element={<InsightsPage />} />
        <Route path="/accounts" element={<AccountsPage />} />
        <Route path="/notifications" element={<NotificationsPage />} />
        <Route path="/settings" element={<SettingsPage />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

export default App
