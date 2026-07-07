import { Suspense, lazy, type ReactNode } from 'react'
import { Navigate, Route, Routes, useLocation } from 'react-router-dom'
import { useAuth } from '@/context/AuthContext'
import { FullPageSpinner } from '@/components/ui/Spinner'
import { AppShell } from '@/components/layout/AppShell'

const LoginPage = lazy(() => import('@/pages/auth/LoginPage').then((m) => ({ default: m.LoginPage })))
const SignupPage = lazy(() => import('@/pages/auth/SignupPage').then((m) => ({ default: m.SignupPage })))
const OnboardingPage = lazy(() => import('@/pages/auth/OnboardingPage').then((m) => ({ default: m.OnboardingPage })))
const DashboardPage = lazy(() => import('@/pages/DashboardPage').then((m) => ({ default: m.DashboardPage })))
const TransactionsPage = lazy(() => import('@/pages/TransactionsPage').then((m) => ({ default: m.TransactionsPage })))
const BudgetsPage = lazy(() => import('@/pages/BudgetsPage').then((m) => ({ default: m.BudgetsPage })))
const CashFlowPage = lazy(() => import('@/pages/CashFlowPage').then((m) => ({ default: m.CashFlowPage })))
const InsightsPage = lazy(() => import('@/pages/InsightsPage').then((m) => ({ default: m.InsightsPage })))
const AccountsPage = lazy(() => import('@/pages/AccountsPage').then((m) => ({ default: m.AccountsPage })))
const NotificationsPage = lazy(() => import('@/pages/NotificationsPage').then((m) => ({ default: m.NotificationsPage })))
const SettingsPage = lazy(() => import('@/pages/SettingsPage').then((m) => ({ default: m.SettingsPage })))

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
    <Suspense fallback={<FullPageSpinner />}>
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
    </Suspense>
  )
}

export default App
