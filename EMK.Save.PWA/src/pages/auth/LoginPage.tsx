import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { IconAt, IconLock, IconAlertCircle } from '@tabler/icons-react'
import { AuthLayout } from '@/components/layout/AuthLayout'
import { Input } from '@/components/ui/Input'
import { Button } from '@/components/ui/Button'
import { useAuth } from '@/context/AuthContext'
import { ApiError } from '@/lib/apiClient'

const schema = z.object({
  userId: z.string().min(1, 'Enter your username'),
  password: z.string().min(1, 'Enter your password'),
})
type FormValues = z.infer<typeof schema>

export function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  const onSubmit = async (values: FormValues) => {
    try {
      await login(values.userId, values.password)
      const redirectTo = (location.state as { from?: string } | null)?.from ?? '/'
      navigate(redirectTo, { replace: true })
    } catch (err) {
      setError('root', {
        message: err instanceof ApiError ? err.message : 'Something went wrong. Please try again.',
      })
    }
  }

  return (
    <AuthLayout
      title="Welcome back"
      subtitle="Log in to keep tabs on your shared budget."
      footer={
        <>
          New here?{' '}
          <Link to="/signup" className="font-medium text-brand-600 hover:underline">
            Create an account
          </Link>
        </>
      }
    >
      <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4" noValidate>
        {errors.root && (
          <div className="flex items-center gap-2 rounded-xl bg-red-50 px-3 py-2.5 text-sm text-red-600 dark:bg-red-500/10 dark:text-red-400">
            <IconAlertCircle size={16} className="shrink-0" />
            {errors.root.message}
          </div>
        )}
        <Input
          label="Username"
          autoComplete="username"
          leftSlot={<IconAt size={17} />}
          error={errors.userId?.message}
          {...register('userId')}
        />
        <Input
          label="Password"
          type="password"
          autoComplete="current-password"
          leftSlot={<IconLock size={17} />}
          error={errors.password?.message}
          {...register('password')}
        />
        <Button type="submit" fullWidth loading={isSubmitting} className="mt-1">
          Log in
        </Button>
      </form>
    </AuthLayout>
  )
}
