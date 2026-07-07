import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Link, useNavigate } from 'react-router-dom'
import { IconLock, IconUser, IconMail, IconAlertCircle } from '@tabler/icons-react'
import { AuthLayout } from '@/components/layout/AuthLayout'
import { Input } from '@/components/ui/Input'
import { Button } from '@/components/ui/Button'
import { useAuth } from '@/context/AuthContext'
import { ApiError } from '@/lib/apiClient'

const schema = z
  .object({
    firstName: z.string().min(1, 'Required'),
    lastName: z.string().min(1, 'Required'),
    userId: z.string().min(3, 'At least 3 characters').regex(/^\S+$/, 'No spaces allowed'),
    email: z.string().email('Enter a valid email'),
    password: z.string().min(8, 'At least 8 characters'),
    confirmPassword: z.string(),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords don't match",
    path: ['confirmPassword'],
  })
type FormValues = z.infer<typeof schema>

export function SignupPage() {
  const { register: registerUser } = useAuth()
  const navigate = useNavigate()
  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  const onSubmit = async (values: FormValues) => {
    try {
      await registerUser({
        UserId: values.userId,
        Password: values.password,
        FirstName: values.firstName,
        LastName: values.lastName,
        Email: values.email,
      })
      navigate('/onboarding', { replace: true })
    } catch (err) {
      setError('root', {
        message: err instanceof ApiError ? err.message : 'Something went wrong. Please try again.',
      })
    }
  }

  return (
    <AuthLayout
      title="Create your account"
      subtitle="Start tracking spending together in minutes."
      footer={
        <>
          Already have an account?{' '}
          <Link to="/login" className="font-medium text-brand-600 hover:underline">
            Log in
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
        <div className="grid grid-cols-2 gap-3">
          <Input label="First name" error={errors.firstName?.message} {...register('firstName')} />
          <Input label="Last name" error={errors.lastName?.message} {...register('lastName')} />
        </div>
        <Input
          label="Username"
          leftSlot={<IconUser size={17} />}
          error={errors.userId?.message}
          {...register('userId')}
        />
        <Input
          label="Email"
          type="email"
          autoComplete="email"
          leftSlot={<IconMail size={17} />}
          error={errors.email?.message}
          {...register('email')}
        />
        <Input
          label="Password"
          type="password"
          autoComplete="new-password"
          leftSlot={<IconLock size={17} />}
          error={errors.password?.message}
          {...register('password')}
        />
        <Input
          label="Confirm password"
          type="password"
          autoComplete="new-password"
          leftSlot={<IconLock size={17} />}
          error={errors.confirmPassword?.message}
          {...register('confirmPassword')}
        />
        <Button type="submit" fullWidth loading={isSubmitting} className="mt-1">
          Create account
        </Button>
      </form>
    </AuthLayout>
  )
}
