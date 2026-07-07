// Mirrors EMK.Save.BL.Models — property names match the API's PascalCase JSON exactly
// (Program.cs sets PropertyNamingPolicy = null so no camelCase conversion happens).

export const CategoryType = { Income: 0, Expense: 1, Savings: 2, Transfer: 3 } as const
export type CategoryType = (typeof CategoryType)[keyof typeof CategoryType]

export const InsightType = {
  OverBudget: 0, UnderBudget: 1, SpendingSpike: 2, SpendingDrop: 3, SavingsGoalMet: 4,
  SavingsGoalMissed: 5, LargeTransaction: 6, UnassignedTransactions: 7, CashFlowWarning: 8, MonthlyRecap: 9,
} as const
export type InsightType = (typeof InsightType)[keyof typeof InsightType]

export const InsightSeverity = { Info: 0, Success: 1, Warning: 2, Critical: 3 } as const
export type InsightSeverity = (typeof InsightSeverity)[keyof typeof InsightSeverity]

export const NotificationType = {
  NewTransaction: 0, BudgetOverage: 1, BudgetWarning: 2, WeeklySummary: 3,
  MonthlySummary: 4, LargeTransaction: 5, AccountSyncError: 6,
} as const
export type NotificationType = (typeof NotificationType)[keyof typeof NotificationType]

export const NotificationStatus = { Pending: 0, Delivered: 1, Failed: 2, Dismissed: 3 } as const
export type NotificationStatus = (typeof NotificationStatus)[keyof typeof NotificationStatus]

export const BudgetRole = { Owner: 0, Member: 1 } as const
export type BudgetRole = (typeof BudgetRole)[keyof typeof BudgetRole]

export interface User {
  Id: string
  UserId: string
  FirstName: string
  LastName: string
  Email: string
  TimeZone: string
  CurrencyCode: string
  DateRegistered: string
  LastLogin: string | null
  SharedBudgetId: string | null
  BudgetRole: BudgetRole | null
  FullName: string
  Initials: string
  HasBudget: boolean
  IsOwner: boolean
}

export interface AuthenticateResponse {
  Id: string
  FirstName: string
  LastName: string
  UserId: string
  Email: string
  SharedBudgetId: string | null
  BudgetRole: BudgetRole | null
  Token: string
}

export interface SharedBudget {
  Id: string
  Name: string
  Description: string
  OwnerId: string
  OwnerFullName: string
  InviteCode: string
  IsActive: boolean
  DateCreated: string
  MaxMembers: number
  Members: User[]
  MemberCount: number
  IsFull: boolean
  CanJoin: boolean
}

export interface BudgetCategory {
  Id: string
  SharedBudgetId: string
  Name: string
  Icon: string
  Color: string
  CategoryType: CategoryType
  SortOrder: number
  IsActive: boolean
  CategoryTypeLabel: string
}

export interface Transaction {
  Id: string
  PlaidAccountId: string
  AccountDisplayName: string
  SharedBudgetId: string
  PlaidTransactionId: string
  TransactionDate: string
  PostedDate: string | null
  MerchantName: string
  Description: string
  Amount: number
  IsoCurrencyCode: string
  PlaidCategory: string
  PlaidSubcategory: string
  CategoryId: string | null
  CategoryName: string
  CategoryIcon: string
  CategoryColor: string
  Notes: string
  IsExcluded: boolean
  IsPending: boolean
  IsReviewed: boolean
  IsExpense: boolean
  IsIncome: boolean
  DisplayAmount: number
  DisplayName: string
  IsAssigned: boolean
  Month: number
  Year: number
}

export interface Budget {
  Id: string
  SharedBudgetId: string
  CategoryId: string
  CategoryName: string
  CategoryIcon: string
  CategoryColor: string
  CategoryType: CategoryType
  Month: number
  Year: number
  PlannedAmount: number
  RolloverAmount: number
  Notes: string
  Transactions: Transaction[]
  AmountSpent: number
  AmountRemaining: number
  PercentUsed: number
  IsOverBudget: boolean
  MonthLabel: string
}

export interface CategorySummary {
  Id: string
  SnapshotId: string
  CategoryId: string
  CategoryName: string
  CategoryIcon: string
  CategoryColor: string
  CategoryType: CategoryType
  PlannedAmount: number
  ActualAmount: number
  TransactionCount: number
  Variance: number
  PercentUsed: number
  IsOverBudget: boolean
  VarianceLabel: string
}

export interface CashFlowEntry {
  Id: string
  SharedBudgetId: string
  EntryDate: string
  DayIncome: number
  DayExpenses: number
  RunningBalance: number
  ProjectedBalance: number
  TransactionCount: number
  DayNet: number
  IsFuture: boolean
  DayLabel: string
  Month: number
  Year: number
}

export interface MonthlySnapshot {
  Id: string
  SharedBudgetId: string
  Month: number
  Year: number
  TotalIncome: number
  TotalExpenses: number
  TotalBudgeted: number
  TotalSavings: number
  TransactionCount: number
  OverBudgetCategoryCount: number
  SnapshotDate: string
  CategorySummaries: CategorySummary[]
  NetCashFlow: number
  BudgetVariance: number
  SavingsRate: number
  IsUnderBudget: boolean
  MonthLabel: string
}

export interface TrackingInsight {
  Id: string
  SharedBudgetId: string
  Month: number
  Year: number
  InsightType: InsightType
  Severity: InsightSeverity
  CategoryId: string | null
  CategoryName: string
  Title: string
  Message: string
  Amount: number | null
  ChangePercent: number | null
  GeneratedOn: string
  IsDismissed: boolean
  IsRead: boolean
  SeverityLabel: string
  TypeLabel: string
  MonthLabel: string
}

export interface PushNotification {
  Id: string
  SharedBudgetId: string
  UserId: string
  NotificationType: NotificationType
  Status: NotificationStatus
  Title: string
  Body: string
  Icon: string
  ActionUrl: string
  PushEndpoint: string
  TransactionId: string | null
  CategoryId: string | null
  CategoryName: string
  Amount: number | null
  SentOn: string | null
  ScheduledFor: string
  IsRead: boolean
  ErrorMessage: string
  WasSent: boolean
  TypeLabel: string
}

export interface NotificationPreference {
  Id: string
  UserId: string
  PushEndpoint: string
  P256dhKey: string
  AuthKey: string
  NotifyOnNewTransaction: boolean
  NotifyOnBudgetOverage: boolean
  NotifyOnBudgetWarning: boolean
  NotifyOnLargeTransaction: boolean
  NotifyWeeklySummary: boolean
  NotifyMonthlySummary: boolean
  NotifyOnSyncError: boolean
  LargeTransactionThreshold: number
  QuietHoursStart: string
  QuietHoursEnd: string
  IsPushEnabled: boolean
  LastUpdated: string
  HasValidSubscription: boolean
}

export interface PlaidAccount {
  Id: string
  UserId: string
  SharedBudgetId: string
  PlaidAccountId: string
  PlaidItemId: string
  InstitutionName: string
  InstitutionLogoUrl: string
  AccountName: string
  Mask: string
  AccountType: string
  AccountSubtype: string
  CurrentBalance: number
  AvailableBalance: number
  IsoCurrencyCode: string
  LastSynced: string
  IsActive: boolean
  DateLinked: string
  DisplayName: string
}

export interface PlaidLinkToken {
  LinkToken: string
  Expiration: string
  RequestId: string
}

export interface DashboardSummary {
  SharedBudgetId: string
  Month: number
  Year: number
  TotalIncome: number
  TotalExpenses: number
  TotalBudgeted: number
  NetCashFlow: number
  BudgetRemaining: number
  SavingsRate: number
  Budgets: Budget[]
  RecentTransactions: Transaction[]
  UnassignedTransactions: Transaction[]
  CategorySummaries: CategorySummary[]
  CashFlow: CashFlowEntry[]
  Insights: TrackingInsight[]
  LinkedAccounts: PlaidAccount[]
  UnreadNotifications: PushNotification[]
  SharedBudget: SharedBudget | null
  HasOverBudgetCategories: boolean
  UnassignedCount: number
  MonthLabel: string
  OverBudgetCategories: CategorySummary[]
}
