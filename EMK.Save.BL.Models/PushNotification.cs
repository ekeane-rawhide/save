
using System.ComponentModel.DataAnnotations;

namespace EMK.Save.BL.Models
{
    /// <summary>
    /// Records a push notification sent (or queued) to the user's device.
    /// Triggered by: new transactions, budget overages, and weekly/monthly summaries.
    /// Maps to tblPushNotification in the PL.
    /// </summary>
    public class PushNotification
    {
        public Guid Id { get; set; }

        [DisplayName("User")]
        public Guid UserId { get; set; }

        [DisplayName("Notification Type")]
        public NotificationType NotificationType { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; } = string.Empty;

        [DisplayName("Body")]
        public string Body { get; set; } = string.Empty;

        [DisplayName("Icon")]
        public string Icon { get; set; } = string.Empty;

        /// <summary>Deep-link URL opened when the user taps the notification.</summary>
        [DisplayName("Action URL")]
        public string ActionUrl { get; set; } = string.Empty;

        /// <summary>Web Push subscription endpoint for this device.</summary>
        [DisplayName("Push Endpoint")]
        public string PushEndpoint { get; set; } = string.Empty;

        [DisplayName("Related Transaction")]
        public Guid? TransactionId { get; set; }

        [DisplayName("Related Category")]
        public Guid? CategoryId { get; set; }

        [DisplayName("Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [DisplayName("Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? Amount { get; set; }

        [DisplayName("Sent On")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime? SentOn { get; set; }

        [DisplayName("Scheduled For")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime ScheduledFor { get; set; }

        [DisplayName("Status")]
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        [DisplayName("Is Read")]
        public bool IsRead { get; set; }

        [DisplayName("Error Message")]
        public string ErrorMessage { get; set; } = string.Empty;

        // ── Computed ─────────────────────────────────────────────────────────
        [DisplayName("Status Label")]
        public string StatusLabel => Status.ToString();

        [DisplayName("Type Label")]
        public string TypeLabel => NotificationType.ToString();

        [DisplayName("Was Sent")]
        public bool WasSent => Status == NotificationStatus.Delivered;
    }

    public enum NotificationType
    {
        NewTransaction = 0,
        BudgetOverage = 1,
        BudgetWarning = 2,       // at 80% of budget
        WeeklySummary = 3,
        MonthlySummary = 4,
        LargeTransaction = 5,
        AccountSyncError = 6
    }

    public enum NotificationStatus
    {
        Pending = 0,
        Delivered = 1,
        Failed = 2,
        Dismissed = 3
    }
}
