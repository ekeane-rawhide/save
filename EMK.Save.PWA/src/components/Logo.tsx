interface LogoProps {
  size?: number
  withWordmark?: boolean
  className?: string
}

export function Logo({ size = 32, withWordmark = false, className = '' }: LogoProps) {
  return (
    <div className={`inline-flex items-center gap-2.5 ${className}`}>
      <svg width={size} height={size} viewBox="0 0 512 512" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
        <defs>
          <linearGradient id="save-logo-bg" x1="0" y1="0" x2="512" y2="512" gradientUnits="userSpaceOnUse">
            <stop offset="0" stopColor="#22c55e" />
            <stop offset="1" stopColor="#4f46e5" />
          </linearGradient>
        </defs>
        <rect width="512" height="512" rx="115" fill="url(#save-logo-bg)" />
        <circle cx="180" cy="340" r="92" fill="#ffffff" fillOpacity="0.96" />
        <circle cx="180" cy="340" r="70" fill="none" stroke="url(#save-logo-bg)" strokeOpacity="0.28" strokeWidth="10" />
        <polygon
          points="155.86,355.86 346.36,165.36 321.6,140.6 410,130 399.4,218.4 374.64,193.64 184.14,384.14"
          fill="#ffffff"
        />
      </svg>
      {withWordmark && (
        <span className="text-lg font-semibold tracking-tight text-(--text-primary)">Save</span>
      )}
    </div>
  )
}
