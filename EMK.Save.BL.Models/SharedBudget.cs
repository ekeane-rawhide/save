namespace EMK.Save.BL.Models
{
    /// <summary>
    /// Represents a shared budget workspace.
    /// One user creates it (Owner) and others join via InviteCode.
    /// A user may only belong to ONE SharedBudget — either as owner or member, never both.
    /// </summary>
    public class SharedBudget
    {
        public Guid Id { get; set; }

        [DisplayName("Budget Name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Description")]
        public string Description { get; set; } = string.Empty;

        [DisplayName("Owner")]
        public Guid OwnerId { get; set; }

        [DisplayName("Owner Name")]
        public string OwnerFullName { get; set; } = string.Empty;

        /// <summary>6-character alphanumeric code users enter to join.</summary>
        [DisplayName("Invite Code")]
        public string InviteCode { get; set; } = string.Empty;

        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;

        [DisplayName("Date Created")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Max Members")]
        public int MaxMembers { get; set; } = 10;

        // ── Populated by manager ──────────────────────────────────────────────
        [DisplayName("Members")]
        public List<User> Members { get; set; } = new List<User>();

        // ── Computed ─────────────────────────────────────────────────────────
        [DisplayName("Member Count")]
        public int MemberCount => Members.Count;

        [DisplayName("Is Full")]
        public bool IsFull => MemberCount >= MaxMembers;

        [DisplayName("Can Join")]
        public bool CanJoin => IsActive && !IsFull;
    }
}
