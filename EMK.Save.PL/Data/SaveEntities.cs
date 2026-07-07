using EMK.Save.PL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace EMK.Save.PL.Data
{
    public class SaveEntities : DbContext
    {
        // ── Seed ID pools ─────────────────────────────────────────────────────
        Guid[] userId = new Guid[3];
        Guid[] sharedBudgetId = new Guid[1];
        Guid[] categoryId = new Guid[8];
        Guid[] budgetId = new Guid[8];
        Guid[] plaidAccountId = new Guid[2];
        Guid[] transactionId = new Guid[10];
        Guid[] snapshotId = new Guid[2];
        Guid[] summaryId = new Guid[8];
        Guid[] cashFlowId = new Guid[5];
        Guid[] insightId = new Guid[3];
        Guid[] notifId = new Guid[3];
        Guid[] prefId = new Guid[2];

        // ── DbSets ────────────────────────────────────────────────────────────
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblSharedBudget> tblSharedBudgets { get; set; }
        public virtual DbSet<tblBudgetCategory> tblBudgetCategories { get; set; }
        public virtual DbSet<tblBudget> tblBudgets { get; set; }
        public virtual DbSet<tblPlaidAccount> tblPlaidAccounts { get; set; }
        public virtual DbSet<tblTransaction> tblTransactions { get; set; }
        public virtual DbSet<tblMonthlySnapshot> tblMonthlySnapshots { get; set; }
        public virtual DbSet<tblCategorySummary> tblCategorySummaries { get; set; }
        public virtual DbSet<tblCashFlowEntry> tblCashFlowEntries { get; set; }
        public virtual DbSet<tblTrackingInsight> tblTrackingInsights { get; set; }
        public virtual DbSet<tblPushNotification> tblPushNotifications { get; set; }
        public virtual DbSet<tblNotificationPreference> tblNotificationPreferences { get; set; }

        public SaveEntities(DbContextOptions<SaveEntities> options) : base(options) { }

        public SaveEntities() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Order matters — FK parents must be seeded before their children
            CreateSharedBudgets(modelBuilder);
            CreateUsers(modelBuilder);
            CreateBudgetCategories(modelBuilder);
            CreateBudgets(modelBuilder);
            CreatePlaidAccounts(modelBuilder);
            CreateTransactions(modelBuilder);
            CreateMonthlySnapshots(modelBuilder);
            CreateCategorySummaries(modelBuilder);
            CreateCashFlowEntries(modelBuilder);
            CreateTrackingInsights(modelBuilder);
            CreatePushNotifications(modelBuilder);
            CreateNotificationPreferences(modelBuilder);
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private static string GetHash(string password)
        {
            using var hasher = SHA1.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(hasher.ComputeHash(bytes));
        }

        private static string MakeInviteCode()
        {
            // Deterministic seed so migrations are stable
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rng = new Random(42);
            return new string(Enumerable.Range(0, 6)
                .Select(_ => chars[rng.Next(chars.Length)])
                .ToArray());
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblSharedBudget  — seeded first; tblUser has FK into it
        // ─────────────────────────────────────────────────────────────────────
        private void CreateSharedBudgets(ModelBuilder modelBuilder)
        {
            for (int i = 0; i < sharedBudgetId.Length; i++)
                sharedBudgetId[i] = Guid.NewGuid();

            // Pre-generate user IDs here so OwnerId can reference userId[0]
            for (int i = 0; i < userId.Length; i++)
                userId[i] = Guid.NewGuid();

            modelBuilder.Entity<tblSharedBudget>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblSharedBudget_Id");

                entity.ToTable("tblSharedBudget");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.InviteCode)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.HasIndex(e => e.InviteCode)
                    .IsUnique()
                    .HasDatabaseName("UX_tblSharedBudget_InviteCode");
            });

            List<tblSharedBudget> sharedBudgets = new List<tblSharedBudget>
            {
                new tblSharedBudget
                {
                    Id          = sharedBudgetId[0],
                    Name        = "Demo Family Budget",
                    Description = "Shared household budget",
                    OwnerId     = userId[0],
                    InviteCode  = MakeInviteCode(),
                    IsActive    = true,
                    DateCreated = new DateTime(2025, 1, 1),
                    MaxMembers  = 10
                }
            };

            modelBuilder.Entity<tblSharedBudget>().HasData(sharedBudgets);
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblUser
        // ─────────────────────────────────────────────────────────────────────
        private void CreateUsers(ModelBuilder modelBuilder)
        {
            // userId[] was already allocated in CreateSharedBudgets

            modelBuilder.Entity<tblUser>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblUser_Id");

                entity.ToTable("tblUser");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(28)
                    .IsUnicode(false);

                entity.Property(e => e.TimeZone)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencyCode)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.DateRegistered).HasColumnType("datetime");

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.HasOne(d => d.SharedBudget)
                    .WithMany(p => p.Members)
                    .HasForeignKey(d => d.SharedBudgetId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_tblUser_SharedBudgetId");
            });

            List<tblUser> users = new List<tblUser>
            {
                new tblUser
                {
                    Id = userId[0], UserId = "owner", FirstName = "Alex",   LastName = "Demo",
                    Email = "alex@emksave.com",   Password = GetHash("owner123"),
                    TimeZone = "America/Chicago",  CurrencyCode = "USD",
                    DateRegistered = new DateTime(2025, 1, 1),
                    SharedBudgetId = sharedBudgetId[0], BudgetRole = 0
                },
                new tblUser
                {
                    Id = userId[1], UserId = "member", FirstName = "Jamie", LastName = "Smith",
                    Email = "jamie@emksave.com",  Password = GetHash("member123"),
                    TimeZone = "America/Chicago",  CurrencyCode = "USD",
                    DateRegistered = new DateTime(2025, 1, 1),
                    SharedBudgetId = sharedBudgetId[0], BudgetRole = 1
                },
                new tblUser
                {
                    Id = userId[2], UserId = "solo", FirstName = "Taylor", LastName = "New",
                    Email = "taylor@emksave.com", Password = GetHash("solo123"),
                    TimeZone = "America/Chicago",  CurrencyCode = "USD",
                    DateRegistered = new DateTime(2025, 1, 1),
                    SharedBudgetId = null, BudgetRole = null
                }
            };

            modelBuilder.Entity<tblUser>().HasData(users);
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblBudgetCategory
        // ─────────────────────────────────────────────────────────────────────
        private void CreateBudgetCategories(ModelBuilder modelBuilder)
        {
            for (int i = 0; i < categoryId.Length; i++)
                categoryId[i] = Guid.NewGuid();

            modelBuilder.Entity<tblBudgetCategory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblBudgetCategory_Id");

                entity.ToTable("tblBudgetCategory");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Icon)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.SharedBudget)
                    .WithMany(p => p.BudgetCategories)
                    .HasForeignKey(d => d.SharedBudgetId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblBudgetCategory_SharedBudgetId");
            });

            List<tblBudgetCategory> categories = new List<tblBudgetCategory>
            {
                new tblBudgetCategory { Id = categoryId[0], SharedBudgetId = sharedBudgetId[0], Name = "Housing",       Icon = "ti-home",          Color = "#2a78d6", CategoryType = 1, SortOrder = 1, IsActive = true },
                new tblBudgetCategory { Id = categoryId[1], SharedBudgetId = sharedBudgetId[0], Name = "Groceries",     Icon = "ti-shopping-cart", Color = "#1baf7a", CategoryType = 1, SortOrder = 2, IsActive = true },
                new tblBudgetCategory { Id = categoryId[2], SharedBudgetId = sharedBudgetId[0], Name = "Dining Out",    Icon = "ti-tools-kitchen", Color = "#eda100", CategoryType = 1, SortOrder = 3, IsActive = true },
                new tblBudgetCategory { Id = categoryId[3], SharedBudgetId = sharedBudgetId[0], Name = "Transport",     Icon = "ti-car",           Color = "#4a3aa7", CategoryType = 1, SortOrder = 4, IsActive = true },
                new tblBudgetCategory { Id = categoryId[4], SharedBudgetId = sharedBudgetId[0], Name = "Entertainment", Icon = "ti-device-tv",     Color = "#e34948", CategoryType = 1, SortOrder = 5, IsActive = true },
                new tblBudgetCategory { Id = categoryId[5], SharedBudgetId = sharedBudgetId[0], Name = "Healthcare",    Icon = "ti-heart",         Color = "#e87ba4", CategoryType = 1, SortOrder = 6, IsActive = true },
                new tblBudgetCategory { Id = categoryId[6], SharedBudgetId = sharedBudgetId[0], Name = "Savings",       Icon = "ti-piggy-bank",    Color = "#008300", CategoryType = 2, SortOrder = 7, IsActive = true },
                new tblBudgetCategory { Id = categoryId[7], SharedBudgetId = sharedBudgetId[0], Name = "Income",        Icon = "ti-wallet",        Color = "#1baf7a", CategoryType = 0, SortOrder = 0, IsActive = true }
            };

            modelBuilder.Entity<tblBudgetCategory>().HasData(categories);
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblBudget  (monthly spending plan per category)
        // ─────────────────────────────────────────────────────────────────────
        private void CreateBudgets(ModelBuilder modelBuilder)
        {
            for (int i = 0; i < budgetId.Length; i++)
                budgetId[i] = Guid.NewGuid();

            int month = DateTime.Today.Month;
            int year = DateTime.Today.Year;

            modelBuilder.Entity<tblBudget>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblBudget_Id");

                entity.ToTable("tblBudget");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PlannedAmount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.RolloverAmount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Notes)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.SharedBudget)
                    .WithMany(p => p.Budgets)
                    .HasForeignKey(d => d.SharedBudgetId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblBudget_SharedBudgetId");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Budgets)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_tblBudget_CategoryId");
            });

            List<tblBudget> budgets = new List<tblBudget>
            {
                new tblBudget { Id = budgetId[0], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[0], Month = month, Year = year, PlannedAmount = 1500m, RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = budgetId[1], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[1], Month = month, Year = year, PlannedAmount = 400m,  RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = budgetId[2], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[2], Month = month, Year = year, PlannedAmount = 200m,  RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = budgetId[3], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[3], Month = month, Year = year, PlannedAmount = 250m,  RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = budgetId[4], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[4], Month = month, Year = year, PlannedAmount = 100m,  RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = budgetId[5], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[5], Month = month, Year = year, PlannedAmount = 150m,  RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = budgetId[6], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[6], Month = month, Year = year, PlannedAmount = 500m,  RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = budgetId[7], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[7], Month = month, Year = year, PlannedAmount = 5000m, RolloverAmount = 0m, Notes = "Monthly salary" }
            };

            modelBuilder.Entity<tblBudget>().HasData(budgets);
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblPlaidAccount
        // ─────────────────────────────────────────────────────────────────────
        private void CreatePlaidAccounts(ModelBuilder modelBuilder)
        {
            for (int i = 0; i < plaidAccountId.Length; i++)
                plaidAccountId[i] = Guid.NewGuid();

            modelBuilder.Entity<tblPlaidAccount>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblPlaidAccount_Id");

                entity.ToTable("tblPlaidAccount");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PlaidAccountId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PlaidItemId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AccessTokenEncrypted)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.InstitutionName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.InstitutionLogoUrl)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.AccountName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Mask)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.AccountType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AccountSubtype)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsoCurrencyCode)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.CurrentBalance).HasColumnType("decimal(18,2)");

                entity.Property(e => e.AvailableBalance).HasColumnType("decimal(18,2)");

                entity.Property(e => e.LastSynced).HasColumnType("datetime");

                entity.Property(e => e.DateLinked).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PlaidAccounts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblPlaidAccount_UserId");

                entity.HasOne(d => d.SharedBudget)
                    .WithMany(p => p.PlaidAccounts)
                    .HasForeignKey(d => d.SharedBudgetId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblPlaidAccount_SharedBudgetId");
            });

            List<tblPlaidAccount> accounts = new List<tblPlaidAccount>
            {
                new tblPlaidAccount
                {
                    Id = plaidAccountId[0], UserId = userId[0], SharedBudgetId = sharedBudgetId[0],
                    PlaidAccountId = "demo_chk_001", PlaidItemId = "demo_item_001",
                    AccessTokenEncrypted = "DEMO_ENCRYPTED_TOKEN",
                    InstitutionName = "First National Bank", InstitutionLogoUrl = "",
                    AccountName = "Checking", Mask = "1234",
                    AccountType = "depository", AccountSubtype = "checking",
                    CurrentBalance = 3200.00m, AvailableBalance = 3150.00m,
                    IsoCurrencyCode = "USD", IsActive = true,
                    LastSynced = new DateTime(2025, 7, 1),
                    DateLinked = new DateTime(2025, 1, 1)
                },
                new tblPlaidAccount
                {
                    Id = plaidAccountId[1], UserId = userId[0], SharedBudgetId = sharedBudgetId[0],
                    PlaidAccountId = "demo_sav_001", PlaidItemId = "demo_item_001",
                    AccessTokenEncrypted = "DEMO_ENCRYPTED_TOKEN",
                    InstitutionName = "First National Bank", InstitutionLogoUrl = "",
                    AccountName = "Savings", Mask = "5678",
                    AccountType = "depository", AccountSubtype = "savings",
                    CurrentBalance = 8500.00m, AvailableBalance = 8500.00m,
                    IsoCurrencyCode = "USD", IsActive = true,
                    LastSynced = new DateTime(2025, 7, 1),
                    DateLinked = new DateTime(2025, 1, 1)
                }
            };

            modelBuilder.Entity<tblPlaidAccount>().HasData(accounts);
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblTransaction
        // ─────────────────────────────────────────────────────────────────────
        private void CreateTransactions(ModelBuilder modelBuilder)
        {
            for (int i = 0; i < transactionId.Length; i++)
                transactionId[i] = Guid.NewGuid();

            DateTime d = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            modelBuilder.Entity<tblTransaction>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblTransaction_Id");

                entity.ToTable("tblTransaction");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasIndex(e => e.PlaidTransactionId)
                    .IsUnique()
                    .HasDatabaseName("UX_tblTransaction_PlaidTransactionId");

                entity.Property(e => e.PlaidTransactionId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MerchantName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.IsoCurrencyCode)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.PlaidCategory)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PlaidSubcategory)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Notes)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDate).HasColumnType("date");

                entity.Property(e => e.PostedDate).HasColumnType("date");

                entity.HasOne(d => d.PlaidAccount)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.PlaidAccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblTransaction_PlaidAccountId");

                entity.HasOne(d => d.SharedBudget)
                    .WithMany()
                    .HasForeignKey(d => d.SharedBudgetId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_tblTransaction_SharedBudgetId");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_tblTransaction_CategoryId");
            });

            List<tblTransaction> transactions = new List<tblTransaction>
            {
                new tblTransaction { Id = transactionId[0], PlaidAccountId = plaidAccountId[0], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[0], PlaidTransactionId = "txn_001", TransactionDate = d,              MerchantName = "Rent Payment",       Description = "Monthly rent",      Amount = -1500m, IsoCurrencyCode = "USD", PlaidCategory = "Payment",        PlaidSubcategory = "Rent",         Notes = "", IsExcluded = false, IsPending = false, IsReviewed = true  },
                new tblTransaction { Id = transactionId[1], PlaidAccountId = plaidAccountId[0], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[1], PlaidTransactionId = "txn_002", TransactionDate = d.AddDays(2),   MerchantName = "Whole Foods",        Description = "Groceries",         Amount = -127m,  IsoCurrencyCode = "USD", PlaidCategory = "Food and Drink", PlaidSubcategory = "Supermarkets", Notes = "", IsExcluded = false, IsPending = false, IsReviewed = true  },
                new tblTransaction { Id = transactionId[2], PlaidAccountId = plaidAccountId[0], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[2], PlaidTransactionId = "txn_003", TransactionDate = d.AddDays(3),   MerchantName = "Chipotle",           Description = "Lunch",             Amount = -14m,   IsoCurrencyCode = "USD", PlaidCategory = "Food and Drink", PlaidSubcategory = "Restaurants",  Notes = "", IsExcluded = false, IsPending = false, IsReviewed = true  },
                new tblTransaction { Id = transactionId[3], PlaidAccountId = plaidAccountId[0], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[3], PlaidTransactionId = "txn_004", TransactionDate = d.AddDays(4),   MerchantName = "Shell Gas Station",  Description = "Fuel",              Amount = -62m,   IsoCurrencyCode = "USD", PlaidCategory = "Travel",         PlaidSubcategory = "Gas Stations", Notes = "", IsExcluded = false, IsPending = false, IsReviewed = true  },
                new tblTransaction { Id = transactionId[4], PlaidAccountId = plaidAccountId[0], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[4], PlaidTransactionId = "txn_005", TransactionDate = d.AddDays(5),   MerchantName = "Netflix",            Description = "Streaming sub",     Amount = -18m,   IsoCurrencyCode = "USD", PlaidCategory = "Service",        PlaidSubcategory = "Subscription", Notes = "", IsExcluded = false, IsPending = false, IsReviewed = true  },
                new tblTransaction { Id = transactionId[5], PlaidAccountId = plaidAccountId[0], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[7], PlaidTransactionId = "txn_006", TransactionDate = d,              MerchantName = "Employer Direct Dep",Description = "Payroll",           Amount = 5000m,  IsoCurrencyCode = "USD", PlaidCategory = "Payroll",        PlaidSubcategory = "Payroll",      Notes = "", IsExcluded = false, IsPending = false, IsReviewed = true  },
                new tblTransaction { Id = transactionId[6], PlaidAccountId = plaidAccountId[0], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[1], PlaidTransactionId = "txn_007", TransactionDate = d.AddDays(7),   MerchantName = "Trader Joe's",       Description = "Groceries",         Amount = -89m,   IsoCurrencyCode = "USD", PlaidCategory = "Food and Drink", PlaidSubcategory = "Supermarkets", Notes = "", IsExcluded = false, IsPending = false, IsReviewed = false },
                new tblTransaction { Id = transactionId[7], PlaidAccountId = plaidAccountId[0], SharedBudgetId = sharedBudgetId[0], CategoryId = null,          PlaidTransactionId = "txn_008", TransactionDate = d.AddDays(8),   MerchantName = "Amazon",             Description = "Online purchase",   Amount = -43m,   IsoCurrencyCode = "USD", PlaidCategory = "Shopping",       PlaidSubcategory = "Online",       Notes = "", IsExcluded = false, IsPending = false, IsReviewed = false },
                new tblTransaction { Id = transactionId[8], PlaidAccountId = plaidAccountId[0], SharedBudgetId = sharedBudgetId[0], CategoryId = null,          PlaidTransactionId = "txn_009", TransactionDate = d.AddDays(9),   MerchantName = "Starbucks",          Description = "Coffee",            Amount = -7m,    IsoCurrencyCode = "USD", PlaidCategory = "Food and Drink", PlaidSubcategory = "Coffee Shop",  Notes = "", IsExcluded = false, IsPending = true,  IsReviewed = false },
                new tblTransaction { Id = transactionId[9], PlaidAccountId = plaidAccountId[0], SharedBudgetId = sharedBudgetId[0], CategoryId = categoryId[5], PlaidTransactionId = "txn_010", TransactionDate = d.AddDays(10),  MerchantName = "CVS Pharmacy",       Description = "Pharmacy",          Amount = -38m,   IsoCurrencyCode = "USD", PlaidCategory = "Medical",        PlaidSubcategory = "Pharmacies",   Notes = "", IsExcluded = false, IsPending = false, IsReviewed = false }
            };

            modelBuilder.Entity<tblTransaction>().HasData(transactions);
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblMonthlySnapshot
        // ─────────────────────────────────────────────────────────────────────
        private void CreateMonthlySnapshots(ModelBuilder modelBuilder)
        {
            for (int i = 0; i < snapshotId.Length; i++)
                snapshotId[i] = Guid.NewGuid();

            modelBuilder.Entity<tblMonthlySnapshot>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblMonthlySnapshot_Id");

                entity.ToTable("tblMonthlySnapshot");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.TotalIncome).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalExpenses).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalBudgeted).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalSavings).HasColumnType("decimal(18,2)");

                entity.Property(e => e.SnapshotDate).HasColumnType("datetime");

                entity.HasOne(d => d.SharedBudget)
                    .WithMany(p => p.MonthlySnapshots)
                    .HasForeignKey(d => d.SharedBudgetId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblMonthlySnapshot_SharedBudgetId");
            });

            List<tblMonthlySnapshot> snapshots = new List<tblMonthlySnapshot>
            {
                new tblMonthlySnapshot { Id = snapshotId[0], SharedBudgetId = sharedBudgetId[0], Month = 5, Year = 2025, TotalIncome = 5000m, TotalExpenses = 2650m, TotalBudgeted = 3100m, TotalSavings = 500m, TransactionCount = 42, OverBudgetCategoryCount = 1, SnapshotDate = new DateTime(2025, 6, 1) },
                new tblMonthlySnapshot { Id = snapshotId[1], SharedBudgetId = sharedBudgetId[0], Month = 6, Year = 2025, TotalIncome = 5000m, TotalExpenses = 2920m, TotalBudgeted = 3100m, TotalSavings = 500m, TransactionCount = 38, OverBudgetCategoryCount = 2, SnapshotDate = new DateTime(2025, 7, 1) }
            };

            modelBuilder.Entity<tblMonthlySnapshot>().HasData(snapshots);
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblCategorySummary
        // ─────────────────────────────────────────────────────────────────────
        private void CreateCategorySummaries(ModelBuilder modelBuilder)
        {
            for (int i = 0; i < summaryId.Length; i++)
                summaryId[i] = Guid.NewGuid();

            modelBuilder.Entity<tblCategorySummary>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblCategorySummary_Id");

                entity.ToTable("tblCategorySummary");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PlannedAmount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.ActualAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.Snapshot)
                    .WithMany(p => p.CategorySummaries)
                    .HasForeignKey(d => d.SnapshotId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblCategorySummary_SnapshotId");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CategorySummaries)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_tblCategorySummary_CategoryId");
            });

            List<tblCategorySummary> summaries = new List<tblCategorySummary>
            {
                new tblCategorySummary { Id = summaryId[0], SnapshotId = snapshotId[1], CategoryId = categoryId[0], PlannedAmount = 1500m, ActualAmount = 1500m, TransactionCount = 1 },
                new tblCategorySummary { Id = summaryId[1], SnapshotId = snapshotId[1], CategoryId = categoryId[1], PlannedAmount = 400m,  ActualAmount = 380m,  TransactionCount = 5 },
                new tblCategorySummary { Id = summaryId[2], SnapshotId = snapshotId[1], CategoryId = categoryId[2], PlannedAmount = 200m,  ActualAmount = 260m,  TransactionCount = 8 },
                new tblCategorySummary { Id = summaryId[3], SnapshotId = snapshotId[1], CategoryId = categoryId[3], PlannedAmount = 250m,  ActualAmount = 290m,  TransactionCount = 6 },
                new tblCategorySummary { Id = summaryId[4], SnapshotId = snapshotId[1], CategoryId = categoryId[4], PlannedAmount = 100m,  ActualAmount = 85m,   TransactionCount = 3 },
                new tblCategorySummary { Id = summaryId[5], SnapshotId = snapshotId[1], CategoryId = categoryId[5], PlannedAmount = 150m,  ActualAmount = 105m,  TransactionCount = 4 },
                new tblCategorySummary { Id = summaryId[6], SnapshotId = snapshotId[1], CategoryId = categoryId[6], PlannedAmount = 500m,  ActualAmount = 300m,  TransactionCount = 2 },
                new tblCategorySummary { Id = summaryId[7], SnapshotId = snapshotId[1], CategoryId = categoryId[7], PlannedAmount = 5000m, ActualAmount = 5000m, TransactionCount = 1 }
            };

            modelBuilder.Entity<tblCategorySummary>().HasData(summaries);
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblCashFlowEntry
        // ─────────────────────────────────────────────────────────────────────
        private void CreateCashFlowEntries(ModelBuilder modelBuilder)
        {
            for (int i = 0; i < cashFlowId.Length; i++)
                cashFlowId[i] = Guid.NewGuid();

            DateTime d = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            modelBuilder.Entity<tblCashFlowEntry>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblCashFlowEntry_Id");

                entity.ToTable("tblCashFlowEntry");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DayIncome).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DayExpenses).HasColumnType("decimal(18,2)");
                entity.Property(e => e.RunningBalance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ProjectedBalance).HasColumnType("decimal(18,2)");

                entity.Property(e => e.EntryDate).HasColumnType("date");

                entity.HasOne(d => d.SharedBudget)
                    .WithMany(p => p.CashFlowEntries)
                    .HasForeignKey(d => d.SharedBudgetId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblCashFlowEntry_SharedBudgetId");
            });

            List<tblCashFlowEntry> entries = new List<tblCashFlowEntry>
            {
                new tblCashFlowEntry { Id = cashFlowId[0], SharedBudgetId = sharedBudgetId[0], EntryDate = d,             DayIncome = 5000m, DayExpenses = 1500m, RunningBalance = 3500m, ProjectedBalance = 3500m, TransactionCount = 2 },
                new tblCashFlowEntry { Id = cashFlowId[1], SharedBudgetId = sharedBudgetId[0], EntryDate = d.AddDays(2),  DayIncome = 0m,    DayExpenses = 127m,  RunningBalance = 3373m, ProjectedBalance = 3373m, TransactionCount = 1 },
                new tblCashFlowEntry { Id = cashFlowId[2], SharedBudgetId = sharedBudgetId[0], EntryDate = d.AddDays(3),  DayIncome = 0m,    DayExpenses = 14m,   RunningBalance = 3359m, ProjectedBalance = 3359m, TransactionCount = 1 },
                new tblCashFlowEntry { Id = cashFlowId[3], SharedBudgetId = sharedBudgetId[0], EntryDate = d.AddDays(4),  DayIncome = 0m,    DayExpenses = 62m,   RunningBalance = 3297m, ProjectedBalance = 3297m, TransactionCount = 1 },
                new tblCashFlowEntry { Id = cashFlowId[4], SharedBudgetId = sharedBudgetId[0], EntryDate = d.AddDays(5),  DayIncome = 0m,    DayExpenses = 18m,   RunningBalance = 3279m, ProjectedBalance = 2800m, TransactionCount = 1 }
            };

            modelBuilder.Entity<tblCashFlowEntry>().HasData(entries);
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblTrackingInsight
        // ─────────────────────────────────────────────────────────────────────
        private void CreateTrackingInsights(ModelBuilder modelBuilder)
        {
            for (int i = 0; i < insightId.Length; i++)
                insightId[i] = Guid.NewGuid();

            int month = DateTime.Today.Month;
            int year = DateTime.Today.Year;

            modelBuilder.Entity<tblTrackingInsight>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblTrackingInsight_Id");

                entity.ToTable("tblTrackingInsight");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.GeneratedOn).HasColumnType("datetime");

                entity.HasOne(d => d.SharedBudget)
                    .WithMany(p => p.TrackingInsights)
                    .HasForeignKey(d => d.SharedBudgetId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblTrackingInsight_SharedBudgetId");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.TrackingInsights)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_tblTrackingInsight_CategoryId");
            });

            List<tblTrackingInsight> insights = new List<tblTrackingInsight>
            {
                new tblTrackingInsight { Id = insightId[0], SharedBudgetId = sharedBudgetId[0], Month = month, Year = year, InsightType = 0, Severity = 2, CategoryId = categoryId[2], Title = "Dining Out over budget",    Message = "You've spent $60 more than planned on dining this month.",                    Amount = 60m,   ChangePercent = 30.0,  GeneratedOn = DateTime.Today, IsDismissed = false, IsRead = false },
                new tblTrackingInsight { Id = insightId[1], SharedBudgetId = sharedBudgetId[0], Month = month, Year = year, InsightType = 7, Severity = 1, CategoryId = null,          Title = "2 unassigned transactions", Message = "You have 2 transactions that haven't been assigned to a budget category.", Amount = null,  ChangePercent = null,  GeneratedOn = DateTime.Today, IsDismissed = false, IsRead = false },
                new tblTrackingInsight { Id = insightId[2], SharedBudgetId = sharedBudgetId[0], Month = month, Year = year, InsightType = 1, Severity = 1, CategoryId = categoryId[1], Title = "Groceries on track",        Message = "You're spending less on groceries than last month — great job!",           Amount = null,  ChangePercent = -5.0,  GeneratedOn = DateTime.Today, IsDismissed = false, IsRead = false }
            };

            modelBuilder.Entity<tblTrackingInsight>().HasData(insights);
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblPushNotification
        // ─────────────────────────────────────────────────────────────────────
        private void CreatePushNotifications(ModelBuilder modelBuilder)
        {
            for (int i = 0; i < notifId.Length; i++)
                notifId[i] = Guid.NewGuid();

            modelBuilder.Entity<tblPushNotification>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblPushNotification_Id");

                entity.ToTable("tblPushNotification");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Body)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Icon)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ActionUrl)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PushEndpoint)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorMessage)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.SentOn).HasColumnType("datetime");

                entity.Property(e => e.ScheduledFor).HasColumnType("datetime");

                entity.HasOne(d => d.SharedBudget)
                    .WithMany(p => p.PushNotifications)
                    .HasForeignKey(d => d.SharedBudgetId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblPushNotification_SharedBudgetId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PushNotifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_tblPushNotification_UserId");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.PushNotifications)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_tblPushNotification_TransactionId");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.PushNotifications)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_tblPushNotification_CategoryId");
            });

            List<tblPushNotification> notifications = new List<tblPushNotification>
            {
                new tblPushNotification { Id = notifId[0], SharedBudgetId = sharedBudgetId[0], UserId = userId[0], NotificationType = 1, Title = "Dining Out over budget!", Body = "You've exceeded your Dining Out budget by $60.",    Icon = "/icons/icon-192.png", ActionUrl = "/budget",       PushEndpoint = "", TransactionId = null,            CategoryId = categoryId[2], Amount = 60m,  ScheduledFor = DateTime.Today, SentOn = DateTime.Today, Status = 1, IsRead = false, ErrorMessage = "" },
                new tblPushNotification { Id = notifId[1], SharedBudgetId = sharedBudgetId[0], UserId = userId[0], NotificationType = 0, Title = "New transaction: Starbucks", Body = "A $7.00 charge at Starbucks is pending review.", Icon = "/icons/icon-192.png", ActionUrl = "/transactions", PushEndpoint = "", TransactionId = transactionId[8], CategoryId = null,          Amount = 7m,   ScheduledFor = DateTime.Today, SentOn = DateTime.Today, Status = 1, IsRead = false, ErrorMessage = "" },
                new tblPushNotification { Id = notifId[2], SharedBudgetId = sharedBudgetId[0], UserId = userId[0], NotificationType = 2, Title = "Transport at 80%",           Body = "You've used 80% of your Transport budget.",      Icon = "/icons/icon-192.png", ActionUrl = "/budget",       PushEndpoint = "", TransactionId = null,            CategoryId = categoryId[3], Amount = 200m, ScheduledFor = DateTime.Today, SentOn = DateTime.Today, Status = 1, IsRead = true,  ErrorMessage = "" }
            };

            modelBuilder.Entity<tblPushNotification>().HasData(notifications);
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblNotificationPreference
        // ─────────────────────────────────────────────────────────────────────
        private void CreateNotificationPreferences(ModelBuilder modelBuilder)
        {
            for (int i = 0; i < prefId.Length; i++)
                prefId[i] = Guid.NewGuid();

            modelBuilder.Entity<tblNotificationPreference>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_tblNotificationPreference_Id");

                entity.ToTable("tblNotificationPreference");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasIndex(e => e.UserId)
                    .IsUnique()
                    .HasDatabaseName("UX_tblNotificationPreference_UserId");

                entity.Property(e => e.PushEndpoint)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.P256dhKey)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.AuthKey)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LargeTransactionThreshold).HasColumnType("decimal(18,2)");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.NotificationPreference)
                    .HasForeignKey<tblNotificationPreference>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblNotificationPreference_UserId");
            });

            List<tblNotificationPreference> prefs = new List<tblNotificationPreference>
            {
                new tblNotificationPreference
                {
                    Id = prefId[0], UserId = userId[0],
                    PushEndpoint = "", P256dhKey = "", AuthKey = "",
                    NotifyOnNewTransaction = true,  NotifyOnBudgetOverage = true,
                    NotifyOnBudgetWarning  = true,  NotifyOnLargeTransaction = true,
                    NotifyWeeklySummary    = false, NotifyMonthlySummary = true,
                    NotifyOnSyncError      = true,  LargeTransactionThreshold = 100m,
                    QuietHoursStart = new TimeOnly(22, 0), QuietHoursEnd = new TimeOnly(7, 0),
                    IsPushEnabled = false, LastUpdated = new DateTime(2025, 1, 1)
                },
                new tblNotificationPreference
                {
                    Id = prefId[1], UserId = userId[1],
                    PushEndpoint = "", P256dhKey = "", AuthKey = "",
                    NotifyOnNewTransaction = true,  NotifyOnBudgetOverage = true,
                    NotifyOnBudgetWarning  = false, NotifyOnLargeTransaction = false,
                    NotifyWeeklySummary    = false, NotifyMonthlySummary = false,
                    NotifyOnSyncError      = true,  LargeTransactionThreshold = 200m,
                    QuietHoursStart = new TimeOnly(22, 0), QuietHoursEnd = new TimeOnly(7, 0),
                    IsPushEnabled = false, LastUpdated = new DateTime(2025, 1, 1)
                }
            };

            modelBuilder.Entity<tblNotificationPreference>().HasData(prefs);
        }
    }
}