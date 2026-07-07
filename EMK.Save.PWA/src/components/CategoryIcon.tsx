import {
  IconHome, IconShoppingCart, IconToolsKitchen, IconCar, IconDeviceTv, IconHeart,
  IconPigMoney, IconWallet, IconBolt, IconPlane, IconGift, IconBriefcase,
  IconShieldCheck, IconSchool, IconStethoscope, IconArrowsExchange, IconTag,
  IconRefresh, IconPaw, IconBabyCarriage, IconBarbell, IconMovie, IconCoffee,
  IconGasStation, IconBuildingBank, IconCreditCard, IconTargetArrow,
  type Icon, type IconProps,
} from '@tabler/icons-react'

const ICON_MAP: Record<string, Icon> = {
  'ti-home': IconHome,
  'ti-shopping-cart': IconShoppingCart,
  'ti-tools-kitchen': IconToolsKitchen,
  'ti-car': IconCar,
  'ti-device-tv': IconDeviceTv,
  'ti-heart': IconHeart,
  'ti-piggy-bank': IconPigMoney,
  'ti-wallet': IconWallet,
  'ti-bolt': IconBolt,
  'ti-plane': IconPlane,
  'ti-gift': IconGift,
  'ti-briefcase': IconBriefcase,
  'ti-shield-check': IconShieldCheck,
  'ti-school': IconSchool,
  'ti-stethoscope': IconStethoscope,
  'ti-arrows-exchange': IconArrowsExchange,
  'ti-refresh': IconRefresh,
  'ti-paw': IconPaw,
  'ti-baby-carriage': IconBabyCarriage,
  'ti-barbell': IconBarbell,
  'ti-movie': IconMovie,
  'ti-coffee': IconCoffee,
  'ti-gas-station': IconGasStation,
  'ti-building-bank': IconBuildingBank,
  'ti-credit-card': IconCreditCard,
  'ti-target-arrow': IconTargetArrow,
}

export const CATEGORY_ICON_CHOICES = Object.keys(ICON_MAP)

export function CategoryIcon({ icon, ...props }: { icon: string } & IconProps) {
  const Cmp = ICON_MAP[icon] ?? IconTag
  return <Cmp {...props} />
}
