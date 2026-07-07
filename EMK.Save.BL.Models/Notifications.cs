namespace EMK.Save.BL.Models
{
    public class TrackingInsight
    {
        public Guid Id { get; set; }

        [DisplayName("Shared Budget")] public Guid SharedBudgetId { get; set; }
        public int Month { get; set; }
        public int Year  { get; set; }

        public InsightType     InsightType { get; set; }
        public InsightSeverity Severity    { get; set; }

        public Guid?   CategoryId   { get; set; }
        public string  CategoryName { get; set; } = string.Empty;
        public string  Title        { get; set; } = string.Empty;
        public string  Message      { get; set; } = string.Empty;

        [DisplayFormat(DataFormatString = "{0:C}")] public decimal? Amount { get; set; }
        public double? ChangePercent { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime GeneratedOn { get; set; }

        public bool IsDismissed { get; set; }
        public bool IsRead      { get; set; }

        public string SeverityLabel => Severity.ToString();
        public string TypeLabel     => InsightType.ToString();
        public string MonthLabel    => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    }

    public class PushNotification
    {
        public Guid Id { get; set; }

        [DisplayName("Shared Budget")] public Guid SharedBudgetId { get; set; }
        public Guid UserId { get; set; }

        public NotificationType   NotificationType { get; set; }
        public NotificationStatus Status           { get; set; } = NotificationStatus.Pending;

        public string Title      { get; set; } = string.Empty;
        public string Body       { get; set; } = string.Empty;
        public string Icon       { get; set; } = string.Empty;
        public string ActionUrl  { get; set; } = string.Empty;
        public string PushEndpoint { get; set; } = string.Empty;

        public Guid?   TransactionId { get; set; }
        public Guid?   CategoryId    { get; set; }
        public string  CategoryName  { get; set; } = string.Empty;

        [DisplayFormat(DataFormatString = "{0:C}")] public decimal? Amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime? SentOn       { get; set; }
        public DateTime  ScheduledFor { get; set; }

        public bool   IsRead       { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public bool   WasSent   => Status == NotificationStatus.Delivered;
        public string TypeLabel => NotificationType.ToString();
    }

    public class NotificationPreference
    {
        public Guid Id     { get; set; }
        public Guid UserId { get; set; }

        public string PushEndpoint { get; set; } = string.Empty;
        public string P256dhKey    { get; set; } = string.Empty;
        public string AuthKey      { get; set; } = string.Empty;

        public bool NotifyOnNewTransaction   { get; set; } = true;
        public bool NotifyOnBudgetOverage    { get; set; } = true;
        public bool NotifyOnBudgetWarning    { get; set; } = true;
        public bool NotifyOnLargeTransaction { get; set; } = true;
        public bool NotifyWeeklySummary      { get; set; } = false;
        public bool NotifyMonthlySummary     { get; set; } = true;
        public bool NotifyOnSyncError        { get; set; } = true;

        public decimal  LargeTransactionThreshold { get; set; } = 100m;
        public TimeOnly QuietHoursStart { get; set; } = new TimeOnly(22, 0);
        public TimeOnly QuietHoursEnd   { get; set; } = new TimeOnly(7, 0);
        public bool     IsPushEnabled   { get; set; } = false;
        public DateTime LastUpdated     { get; set; }

        public bool HasValidSubscription =>
            IsPushEnabled
            && !string.IsNullOrWhiteSpace(PushEndpoint)
            && !string.IsNullOrWhiteSpace(P256dhKey)
            && !string.IsNullOrWhiteSpace(AuthKey);
    }
}
