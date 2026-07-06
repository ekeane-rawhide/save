namespace EMK.Save.BL.Models
{
    public enum CategoryType    { Income = 0, Expense = 1, Savings = 2, Transfer = 3 }
    public enum InsightType     { OverBudget = 0, UnderBudget = 1, SpendingSpike = 2, SpendingDrop = 3, SavingsGoalMet = 4, SavingsGoalMissed = 5, LargeTransaction = 6, UnassignedTransactions = 7, CashFlowWarning = 8, MonthlyRecap = 9 }
    public enum InsightSeverity { Info = 0, Success = 1, Warning = 2, Critical = 3 }
    public enum NotificationType   { NewTransaction = 0, BudgetOverage = 1, BudgetWarning = 2, WeeklySummary = 3, MonthlySummary = 4, LargeTransaction = 5, AccountSyncError = 6 }
    public enum NotificationStatus { Pending = 0, Delivered = 1, Failed = 2, Dismissed = 3 }
    public enum BudgetRole      { Owner = 0, Member = 1 }
}
