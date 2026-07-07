using EMK.Save.PL.Data;

namespace EMK.Save.PL.Data
{
    public class SaveEntities : DbContext
    {
        // ── Seed ID pools ─────────────────────────────────────────────────────
        private Guid[] userId      = new Guid[3];
        private Guid[] budgetId    = new Guid[1];   // one SharedBudget
        private Guid[] categoryId  = new Guid[8];
        private Guid[] planId      = new Guid[8];   // tblBudget rows
        private Guid[] accountId   = new Guid[2];
        private Guid[] txId        = new Guid[10];
        private Guid[] snapId      = new Guid[2];
        private Guid[] summaryId   = new Guid[8];
        private Guid[] cashId      = new Guid[5];
        private Guid[] insightId   = new Guid[3];
        private Guid[] notifId     = new Guid[3];
        private Guid[] prefId      = new Guid[2];

        // ── DbSets ────────────────────────────────────────────────────────────
        public virtual DbSet<tblUser>                   tblUsers                   { get; set; }
        public virtual DbSet<tblSharedBudget>           tblSharedBudgets           { get; set; }
        public virtual DbSet<tblBudgetCategory>         tblBudgetCategories        { get; set; }
        public virtual DbSet<tblBudget>                 tblBudgets                 { get; set; }
        public virtual DbSet<tblPlaidAccount>           tblPlaidAccounts           { get; set; }
        public virtual DbSet<tblTransaction>            tblTransactions            { get; set; }
        public virtual DbSet<tblMonthlySnapshot>        tblMonthlySnapshots        { get; set; }
        public virtual DbSet<tblCategorySummary>        tblCategorySummaries       { get; set; }
        public virtual DbSet<tblCashFlowEntry>          tblCashFlowEntries         { get; set; }
        public virtual DbSet<tblTrackingInsight>        tblTrackingInsights        { get; set; }
        public virtual DbSet<tblPushNotification>       tblPushNotifications       { get; set; }
        public virtual DbSet<tblNotificationPreference> tblNotificationPreferences { get; set; }

        public SaveEntities(DbContextOptions<SaveEntities> options) : base(options) { }
        public SaveEntities() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            CreateSharedBudgets(modelBuilder);
            CreateUsers(modelBuilder);
            CreateBudgetCategories(modelBuilder);
            CreateBudgetPlans(modelBuilder);
            CreatePlaidAccounts(modelBuilder);
            CreateTransactions(modelBuilder);
            CreateMonthlySnapshots(modelBuilder);
            CreateCategorySummaries(modelBuilder);
            CreateCashFlowEntries(modelBuilder);
            CreateTrackingInsights(modelBuilder);
            CreatePushNotifications(modelBuilder);
            CreateNotificationPreferences(modelBuilder);
        }

        private static string Hash(string pw)
        {
            using var h = SHA1.Create();
            return Convert.ToBase64String(h.ComputeHash(Encoding.UTF8.GetBytes(pw)));
        }

        private static string MakeCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rng   = new Random(42);   // deterministic for seed
            return new string(Enumerable.Range(0, 6).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblSharedBudget  (must be seeded before tblUser due to FK)
        // ─────────────────────────────────────────────────────────────────────
        private void CreateSharedBudgets(ModelBuilder mb)
        {
            for (int i = 0; i < budgetId.Length; i++) budgetId[i] = Guid.NewGuid();
            // OwnerId is userId[0] — seeded in CreateUsers
            for (int i = 0; i < userId.Length;  i++) userId[i]  = Guid.NewGuid();

            mb.Entity<tblSharedBudget>(e =>
            {
                e.HasKey(x => x.Id).HasName("PK_tblSharedBudget_Id");
                e.ToTable("tblSharedBudget");
                e.Property(x => x.Id).ValueGeneratedNever();
                e.Property(x => x.Name).IsRequired().HasMaxLength(100).IsUnicode(false);
                e.Property(x => x.Description).IsRequired().HasMaxLength(500).IsUnicode(false);
                e.Property(x => x.InviteCode).IsRequired().HasMaxLength(6).IsUnicode(false);
                e.Property(x => x.DateCreated).HasColumnType("datetime");
                e.HasIndex(x => x.InviteCode).IsUnique().HasDatabaseName("UX_tblSharedBudget_InviteCode");
            });

            mb.Entity<tblSharedBudget>().HasData(new tblSharedBudget
            {
                Id          = budgetId[0],
                Name        = "Demo Family Budget",
                Description = "Shared household budget",
                OwnerId     = userId[0],
                InviteCode  = MakeCode(),
                IsActive    = true,
                DateCreated = new DateTime(2025, 1, 1),
                MaxMembers  = 10
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblUser
        // ─────────────────────────────────────────────────────────────────────
        private void CreateUsers(ModelBuilder mb)
        {
            mb.Entity<tblUser>(e =>
            {
                e.HasKey(x => x.Id).HasName("PK_tblUser_Id");
                e.ToTable("tblUser");
                e.Property(x => x.Id).ValueGeneratedNever();
                e.Property(x => x.UserId).IsRequired().HasMaxLength(50).IsUnicode(false);
                e.Property(x => x.FirstName).IsRequired().HasMaxLength(50).IsUnicode(false);
                e.Property(x => x.LastName).IsRequired().HasMaxLength(50).IsUnicode(false);
                e.Property(x => x.Email).IsRequired().HasMaxLength(150).IsUnicode(false);
                e.Property(x => x.Password).IsRequired().HasMaxLength(28).IsUnicode(false);
                e.Property(x => x.TimeZone).IsRequired().HasMaxLength(50).IsUnicode(false);
                e.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(3).IsUnicode(false);
                e.Property(x => x.DateRegistered).HasColumnType("datetime");
                e.Property(x => x.LastLogin).HasColumnType("datetime");

                // FK → tblSharedBudget (nullable — user may not belong to any budget)
                e.HasOne(x => x.SharedBudget)
                 .WithMany(x => x.Members)
                 .HasForeignKey(x => x.SharedBudgetId)
                 .OnDelete(DeleteBehavior.SetNull)
                 .HasConstraintName("FK_tblUser_SharedBudgetId");
            });

            mb.Entity<tblUser>().HasData(
                new tblUser { Id = userId[0], UserId = "owner",  FirstName = "Alex",  LastName = "Demo",  Email = "alex@emksave.com",   Password = Hash("owner123"),  TimeZone = "America/Chicago", CurrencyCode = "USD", DateRegistered = new DateTime(2025,1,1), SharedBudgetId = budgetId[0], BudgetRole = 0 },
                new tblUser { Id = userId[1], UserId = "member", FirstName = "Jamie", LastName = "Smith", Email = "jamie@emksave.com",  Password = Hash("member123"), TimeZone = "America/Chicago", CurrencyCode = "USD", DateRegistered = new DateTime(2025,1,1), SharedBudgetId = budgetId[0], BudgetRole = 1 },
                new tblUser { Id = userId[2], UserId = "solo",   FirstName = "Taylor",LastName = "New",   Email = "taylor@emksave.com", Password = Hash("solo123"),   TimeZone = "America/Chicago", CurrencyCode = "USD", DateRegistered = new DateTime(2025,1,1), SharedBudgetId = null,        BudgetRole = null }
            );
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblBudgetCategory
        // ─────────────────────────────────────────────────────────────────────
        private void CreateBudgetCategories(ModelBuilder mb)
        {
            for (int i = 0; i < categoryId.Length; i++) categoryId[i] = Guid.NewGuid();

            mb.Entity<tblBudgetCategory>(e =>
            {
                e.HasKey(x => x.Id).HasName("PK_tblBudgetCategory_Id");
                e.ToTable("tblBudgetCategory");
                e.Property(x => x.Id).ValueGeneratedNever();
                e.Property(x => x.Name).IsRequired().HasMaxLength(100).IsUnicode(false);
                e.Property(x => x.Icon).IsRequired().HasMaxLength(50).IsUnicode(false);
                e.Property(x => x.Color).IsRequired().HasMaxLength(20).IsUnicode(false);

                e.HasOne(x => x.SharedBudget)
                 .WithMany(x => x.BudgetCategories)
                 .HasForeignKey(x => x.SharedBudgetId)
                 .OnDelete(DeleteBehavior.Cascade)
                 .HasConstraintName("FK_tblBudgetCategory_SharedBudgetId");
            });

            mb.Entity<tblBudgetCategory>().HasData(
                new tblBudgetCategory { Id = categoryId[0], SharedBudgetId = budgetId[0], Name = "Housing",       Icon = "ti-home",          Color = "#2a78d6", CategoryType = 1, SortOrder = 1, IsActive = true },
                new tblBudgetCategory { Id = categoryId[1], SharedBudgetId = budgetId[0], Name = "Groceries",     Icon = "ti-shopping-cart", Color = "#1baf7a", CategoryType = 1, SortOrder = 2, IsActive = true },
                new tblBudgetCategory { Id = categoryId[2], SharedBudgetId = budgetId[0], Name = "Dining Out",    Icon = "ti-tools-kitchen", Color = "#eda100", CategoryType = 1, SortOrder = 3, IsActive = true },
                new tblBudgetCategory { Id = categoryId[3], SharedBudgetId = budgetId[0], Name = "Transport",     Icon = "ti-car",           Color = "#4a3aa7", CategoryType = 1, SortOrder = 4, IsActive = true },
                new tblBudgetCategory { Id = categoryId[4], SharedBudgetId = budgetId[0], Name = "Entertainment", Icon = "ti-device-tv",     Color = "#e34948", CategoryType = 1, SortOrder = 5, IsActive = true },
                new tblBudgetCategory { Id = categoryId[5], SharedBudgetId = budgetId[0], Name = "Healthcare",    Icon = "ti-heart",         Color = "#e87ba4", CategoryType = 1, SortOrder = 6, IsActive = true },
                new tblBudgetCategory { Id = categoryId[6], SharedBudgetId = budgetId[0], Name = "Savings",       Icon = "ti-piggy-bank",    Color = "#008300", CategoryType = 2, SortOrder = 7, IsActive = true },
                new tblBudgetCategory { Id = categoryId[7], SharedBudgetId = budgetId[0], Name = "Income",        Icon = "ti-wallet",        Color = "#1baf7a", CategoryType = 0, SortOrder = 0, IsActive = true }
            );
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblBudget (monthly plan)
        // ─────────────────────────────────────────────────────────────────────
        private void CreateBudgetPlans(ModelBuilder mb)
        {
            for (int i = 0; i < planId.Length; i++) planId[i] = Guid.NewGuid();
            int m = DateTime.Today.Month, y = DateTime.Today.Year;

            mb.Entity<tblBudget>(e =>
            {
                e.HasKey(x => x.Id).HasName("PK_tblBudget_Id");
                e.ToTable("tblBudget");
                e.Property(x => x.Id).ValueGeneratedNever();
                e.Property(x => x.PlannedAmount).HasColumnType("decimal(18,2)");
                e.Property(x => x.RolloverAmount).HasColumnType("decimal(18,2)");
                e.Property(x => x.Notes).IsRequired().HasMaxLength(500).IsUnicode(false);

                e.HasOne(x => x.SharedBudget).WithMany(x => x.Budgets)
                 .HasForeignKey(x => x.SharedBudgetId).OnDelete(DeleteBehavior.Cascade)
                 .HasConstraintName("FK_tblBudget_SharedBudgetId");

                e.HasOne(x => x.Category).WithMany(x => x.Budgets)
                 .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.NoAction)
                 .HasConstraintName("FK_tblBudget_CategoryId");
            });

            mb.Entity<tblBudget>().HasData(
                new tblBudget { Id = planId[0], SharedBudgetId = budgetId[0], CategoryId = categoryId[0], Month = m, Year = y, PlannedAmount = 1500m, RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = planId[1], SharedBudgetId = budgetId[0], CategoryId = categoryId[1], Month = m, Year = y, PlannedAmount = 400m,  RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = planId[2], SharedBudgetId = budgetId[0], CategoryId = categoryId[2], Month = m, Year = y, PlannedAmount = 200m,  RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = planId[3], SharedBudgetId = budgetId[0], CategoryId = categoryId[3], Month = m, Year = y, PlannedAmount = 250m,  RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = planId[4], SharedBudgetId = budgetId[0], CategoryId = categoryId[4], Month = m, Year = y, PlannedAmount = 100m,  RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = planId[5], SharedBudgetId = budgetId[0], CategoryId = categoryId[5], Month = m, Year = y, PlannedAmount = 150m,  RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = planId[6], SharedBudgetId = budgetId[0], CategoryId = categoryId[6], Month = m, Year = y, PlannedAmount = 500m,  RolloverAmount = 0m, Notes = "" },
                new tblBudget { Id = planId[7], SharedBudgetId = budgetId[0], CategoryId = categoryId[7], Month = m, Year = y, PlannedAmount = 5000m, RolloverAmount = 0m, Notes = "Monthly salary" }
            );
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblPlaidAccount
        // ─────────────────────────────────────────────────────────────────────
        private void CreatePlaidAccounts(ModelBuilder mb)
        {
            for (int i = 0; i < accountId.Length; i++) accountId[i] = Guid.NewGuid();

            mb.Entity<tblPlaidAccount>(e =>
            {
                e.HasKey(x => x.Id).HasName("PK_tblPlaidAccount_Id");
                e.ToTable("tblPlaidAccount");
                e.Property(x => x.Id).ValueGeneratedNever();
                e.Property(x => x.PlaidAccountId).IsRequired().HasMaxLength(100).IsUnicode(false);
                e.Property(x => x.PlaidItemId).IsRequired().HasMaxLength(100).IsUnicode(false);
                e.Property(x => x.AccessTokenEncrypted).IsRequired().HasMaxLength(500).IsUnicode(false);
                e.Property(x => x.InstitutionName).IsRequired().HasMaxLength(100).IsUnicode(false);
                e.Property(x => x.InstitutionLogoUrl).IsRequired().HasMaxLength(500).IsUnicode(false);
                e.Property(x => x.AccountName).IsRequired().HasMaxLength(100).IsUnicode(false);
                e.Property(x => x.Mask).IsRequired().HasMaxLength(4).IsUnicode(false);
                e.Property(x => x.AccountType).IsRequired().HasMaxLength(50).IsUnicode(false);
                e.Property(x => x.AccountSubtype).IsRequired().HasMaxLength(50).IsUnicode(false);
                e.Property(x => x.IsoCurrencyCode).IsRequired().HasMaxLength(3).IsUnicode(false);
                e.Property(x => x.CurrentBalance).HasColumnType("decimal(18,2)");
                e.Property(x => x.AvailableBalance).HasColumnType("decimal(18,2)");
                e.Property(x => x.LastSynced).HasColumnType("datetime");
                e.Property(x => x.DateLinked).HasColumnType("datetime");

                e.HasOne(x => x.User).WithMany(x => x.PlaidAccounts)
                 .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_tblPlaidAccount_UserId");

                e.HasOne(x => x.SharedBudget).WithMany(x => x.PlaidAccounts)
                 .HasForeignKey(x => x.SharedBudgetId).OnDelete(DeleteBehavior.Cascade)
                 .HasConstraintName("FK_tblPlaidAccount_SharedBudgetId");
            });

            mb.Entity<tblPlaidAccount>().HasData(
                new tblPlaidAccount { Id = accountId[0], UserId = userId[0], SharedBudgetId = budgetId[0], PlaidAccountId = "demo_chk_001", PlaidItemId = "demo_item_001", AccessTokenEncrypted = "DEMO_TOKEN", InstitutionName = "First National", InstitutionLogoUrl = "", AccountName = "Checking", Mask = "1234", AccountType = "depository", AccountSubtype = "checking", CurrentBalance = 3200m, AvailableBalance = 3150m, IsoCurrencyCode = "USD", IsActive = true, LastSynced = new DateTime(2025,7,1), DateLinked = new DateTime(2025,1,1) },
                new tblPlaidAccount { Id = accountId[1], UserId = userId[0], SharedBudgetId = budgetId[0], PlaidAccountId = "demo_sav_001", PlaidItemId = "demo_item_001", AccessTokenEncrypted = "DEMO_TOKEN", InstitutionName = "First National", InstitutionLogoUrl = "", AccountName = "Savings",  Mask = "5678", AccountType = "depository", AccountSubtype = "savings",  CurrentBalance = 8500m, AvailableBalance = 8500m, IsoCurrencyCode = "USD", IsActive = true, LastSynced = new DateTime(2025,7,1), DateLinked = new DateTime(2025,1,1) }
            );
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblTransaction
        // ─────────────────────────────────────────────────────────────────────
        private void CreateTransactions(ModelBuilder mb)
        {
            for (int i = 0; i < txId.Length; i++) txId[i] = Guid.NewGuid();
            var d = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            mb.Entity<tblTransaction>(e =>
            {
                e.HasKey(x => x.Id).HasName("PK_tblTransaction_Id");
                e.ToTable("tblTransaction");
                e.Property(x => x.Id).ValueGeneratedNever();
                e.HasIndex(x => x.PlaidTransactionId).IsUnique().HasDatabaseName("UX_tblTransaction_PlaidTransactionId");
                e.Property(x => x.PlaidTransactionId).IsRequired().HasMaxLength(100).IsUnicode(false);
                e.Property(x => x.MerchantName).IsRequired().HasMaxLength(200).IsUnicode(false);
                e.Property(x => x.Description).IsRequired().HasMaxLength(500).IsUnicode(false);
                e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                e.Property(x => x.IsoCurrencyCode).IsRequired().HasMaxLength(3).IsUnicode(false);
                e.Property(x => x.PlaidCategory).IsRequired().HasMaxLength(100).IsUnicode(false);
                e.Property(x => x.PlaidSubcategory).IsRequired().HasMaxLength(100).IsUnicode(false);
                e.Property(x => x.Notes).IsRequired().HasMaxLength(500).IsUnicode(false);
                e.Property(x => x.TransactionDate).HasColumnType("date");
                e.Property(x => x.PostedDate).HasColumnType("date");

                e.HasOne(x => x.PlaidAccount).WithMany(x => x.Transactions)
                 .HasForeignKey(x => x.PlaidAccountId).OnDelete(DeleteBehavior.Cascade)
                 .HasConstraintName("FK_tblTransaction_PlaidAccountId");

                e.HasOne(x => x.SharedBudget).WithMany()
                 .HasForeignKey(x => x.SharedBudgetId).OnDelete(DeleteBehavior.NoAction)
                 .HasConstraintName("FK_tblTransaction_SharedBudgetId");

                e.HasOne(x => x.Category).WithMany(x => x.Transactions)
                 .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull)
                 .HasConstraintName("FK_tblTransaction_CategoryId");
            });

            mb.Entity<tblTransaction>().HasData(
                new tblTransaction { Id=txId[0], PlaidAccountId=accountId[0], SharedBudgetId=budgetId[0], CategoryId=categoryId[0], PlaidTransactionId="txn_001", TransactionDate=d,            MerchantName="Rent Payment",        Description="Monthly rent",       Amount=-1500m, IsoCurrencyCode="USD", PlaidCategory="Payment",        PlaidSubcategory="Rent",         Notes="", IsExcluded=false, IsPending=false, IsReviewed=true  },
                new tblTransaction { Id=txId[1], PlaidAccountId=accountId[0], SharedBudgetId=budgetId[0], CategoryId=categoryId[1], PlaidTransactionId="txn_002", TransactionDate=d.AddDays(2),  MerchantName="Whole Foods",          Description="Groceries",          Amount=-127m,  IsoCurrencyCode="USD", PlaidCategory="Food and Drink", PlaidSubcategory="Supermarkets", Notes="", IsExcluded=false, IsPending=false, IsReviewed=true  },
                new tblTransaction { Id=txId[2], PlaidAccountId=accountId[0], SharedBudgetId=budgetId[0], CategoryId=categoryId[2], PlaidTransactionId="txn_003", TransactionDate=d.AddDays(3),  MerchantName="Chipotle",             Description="Lunch",              Amount=-14m,   IsoCurrencyCode="USD", PlaidCategory="Food and Drink", PlaidSubcategory="Restaurants",  Notes="", IsExcluded=false, IsPending=false, IsReviewed=true  },
                new tblTransaction { Id=txId[3], PlaidAccountId=accountId[0], SharedBudgetId=budgetId[0], CategoryId=categoryId[3], PlaidTransactionId="txn_004", TransactionDate=d.AddDays(4),  MerchantName="Shell Gas Station",    Description="Fuel",               Amount=-62m,   IsoCurrencyCode="USD", PlaidCategory="Travel",         PlaidSubcategory="Gas Stations", Notes="", IsExcluded=false, IsPending=false, IsReviewed=true  },
                new tblTransaction { Id=txId[4], PlaidAccountId=accountId[0], SharedBudgetId=budgetId[0], CategoryId=categoryId[4], PlaidTransactionId="txn_005", TransactionDate=d.AddDays(5),  MerchantName="Netflix",              Description="Streaming sub",      Amount=-18m,   IsoCurrencyCode="USD", PlaidCategory="Service",        PlaidSubcategory="Subscription", Notes="", IsExcluded=false, IsPending=false, IsReviewed=true  },
                new tblTransaction { Id=txId[5], PlaidAccountId=accountId[0], SharedBudgetId=budgetId[0], CategoryId=categoryId[7], PlaidTransactionId="txn_006", TransactionDate=d,             MerchantName="Employer Direct Dep",  Description="Payroll",            Amount=5000m,  IsoCurrencyCode="USD", PlaidCategory="Payroll",        PlaidSubcategory="Payroll",      Notes="", IsExcluded=false, IsPending=false, IsReviewed=true  },
                new tblTransaction { Id=txId[6], PlaidAccountId=accountId[0], SharedBudgetId=budgetId[0], CategoryId=categoryId[1], PlaidTransactionId="txn_007", TransactionDate=d.AddDays(7),  MerchantName="Trader Joe's",         Description="Groceries",          Amount=-89m,   IsoCurrencyCode="USD", PlaidCategory="Food and Drink", PlaidSubcategory="Supermarkets", Notes="", IsExcluded=false, IsPending=false, IsReviewed=false },
                new tblTransaction { Id=txId[7], PlaidAccountId=accountId[0], SharedBudgetId=budgetId[0], CategoryId=null,          PlaidTransactionId="txn_008", TransactionDate=d.AddDays(8),  MerchantName="Amazon",               Description="Online purchase",    Amount=-43m,   IsoCurrencyCode="USD", PlaidCategory="Shopping",       PlaidSubcategory="Online",       Notes="", IsExcluded=false, IsPending=false, IsReviewed=false },
                new tblTransaction { Id=txId[8], PlaidAccountId=accountId[0], SharedBudgetId=budgetId[0], CategoryId=null,          PlaidTransactionId="txn_009", TransactionDate=d.AddDays(9),  MerchantName="Starbucks",            Description="Coffee",             Amount=-7m,    IsoCurrencyCode="USD", PlaidCategory="Food and Drink", PlaidSubcategory="Coffee Shop",  Notes="", IsExcluded=false, IsPending=true,  IsReviewed=false },
                new tblTransaction { Id=txId[9], PlaidAccountId=accountId[0], SharedBudgetId=budgetId[0], CategoryId=categoryId[5], PlaidTransactionId="txn_010", TransactionDate=d.AddDays(10), MerchantName="CVS Pharmacy",         Description="Pharmacy",           Amount=-38m,   IsoCurrencyCode="USD", PlaidCategory="Medical",        PlaidSubcategory="Pharmacies",   Notes="", IsExcluded=false, IsPending=false, IsReviewed=false }
            );
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblMonthlySnapshot + tblCategorySummary
        // ─────────────────────────────────────────────────────────────────────
        private void CreateMonthlySnapshots(ModelBuilder mb)
        {
            for (int i = 0; i < snapId.Length; i++) snapId[i] = Guid.NewGuid();

            mb.Entity<tblMonthlySnapshot>(e =>
            {
                e.HasKey(x => x.Id).HasName("PK_tblMonthlySnapshot_Id");
                e.ToTable("tblMonthlySnapshot");
                e.Property(x => x.Id).ValueGeneratedNever();
                foreach (var col in new[]{ "TotalIncome","TotalExpenses","TotalBudgeted","TotalSavings" })
                    e.Property(col).HasColumnType("decimal(18,2)");
                e.Property(x => x.SnapshotDate).HasColumnType("datetime");

                e.HasOne(x => x.SharedBudget).WithMany(x => x.MonthlySnapshots)
                 .HasForeignKey(x => x.SharedBudgetId).OnDelete(DeleteBehavior.Cascade)
                 .HasConstraintName("FK_tblMonthlySnapshot_SharedBudgetId");
            });

            mb.Entity<tblMonthlySnapshot>().HasData(
                new tblMonthlySnapshot { Id=snapId[0], SharedBudgetId=budgetId[0], Month=5, Year=2025, TotalIncome=5000m, TotalExpenses=2650m, TotalBudgeted=3100m, TotalSavings=500m, TransactionCount=42, OverBudgetCategoryCount=1, SnapshotDate=new DateTime(2025,6,1) },
                new tblMonthlySnapshot { Id=snapId[1], SharedBudgetId=budgetId[0], Month=6, Year=2025, TotalIncome=5000m, TotalExpenses=2920m, TotalBudgeted=3100m, TotalSavings=500m, TransactionCount=38, OverBudgetCategoryCount=2, SnapshotDate=new DateTime(2025,7,1) }
            );
        }

        private void CreateCategorySummaries(ModelBuilder mb)
        {
            for (int i = 0; i < summaryId.Length; i++) summaryId[i] = Guid.NewGuid();

            mb.Entity<tblCategorySummary>(e =>
            {
                e.HasKey(x => x.Id).HasName("PK_tblCategorySummary_Id");
                e.ToTable("tblCategorySummary");
                e.Property(x => x.Id).ValueGeneratedNever();
                e.Property(x => x.PlannedAmount).HasColumnType("decimal(18,2)");
                e.Property(x => x.ActualAmount).HasColumnType("decimal(18,2)");

                e.HasOne(x => x.Snapshot).WithMany(x => x.CategorySummaries)
                 .HasForeignKey(x => x.SnapshotId).OnDelete(DeleteBehavior.Cascade)
                 .HasConstraintName("FK_tblCategorySummary_SnapshotId");

                e.HasOne(x => x.Category).WithMany(x => x.CategorySummaries)
                 .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.NoAction)
                 .HasConstraintName("FK_tblCategorySummary_CategoryId");
            });

            mb.Entity<tblCategorySummary>().HasData(
                new tblCategorySummary { Id=summaryId[0], SnapshotId=snapId[1], CategoryId=categoryId[0], PlannedAmount=1500m, ActualAmount=1500m, TransactionCount=1 },
                new tblCategorySummary { Id=summaryId[1], SnapshotId=snapId[1], CategoryId=categoryId[1], PlannedAmount=400m,  ActualAmount=380m,  TransactionCount=5 },
                new tblCategorySummary { Id=summaryId[2], SnapshotId=snapId[1], CategoryId=categoryId[2], PlannedAmount=200m,  ActualAmount=260m,  TransactionCount=8 },
                new tblCategorySummary { Id=summaryId[3], SnapshotId=snapId[1], CategoryId=categoryId[3], PlannedAmount=250m,  ActualAmount=290m,  TransactionCount=6 },
                new tblCategorySummary { Id=summaryId[4], SnapshotId=snapId[1], CategoryId=categoryId[4], PlannedAmount=100m,  ActualAmount=85m,   TransactionCount=3 },
                new tblCategorySummary { Id=summaryId[5], SnapshotId=snapId[1], CategoryId=categoryId[5], PlannedAmount=150m,  ActualAmount=105m,  TransactionCount=4 },
                new tblCategorySummary { Id=summaryId[6], SnapshotId=snapId[1], CategoryId=categoryId[6], PlannedAmount=500m,  ActualAmount=300m,  TransactionCount=2 },
                new tblCategorySummary { Id=summaryId[7], SnapshotId=snapId[1], CategoryId=categoryId[7], PlannedAmount=5000m, ActualAmount=5000m, TransactionCount=1 }
            );
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblCashFlowEntry
        // ─────────────────────────────────────────────────────────────────────
        private void CreateCashFlowEntries(ModelBuilder mb)
        {
            for (int i = 0; i < cashId.Length; i++) cashId[i] = Guid.NewGuid();
            var d = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            mb.Entity<tblCashFlowEntry>(e =>
            {
                e.HasKey(x => x.Id).HasName("PK_tblCashFlowEntry_Id");
                e.ToTable("tblCashFlowEntry");
                e.Property(x => x.Id).ValueGeneratedNever();
                foreach (var col in new[]{ "DayIncome","DayExpenses","RunningBalance","ProjectedBalance" })
                    e.Property(col).HasColumnType("decimal(18,2)");
                e.Property(x => x.EntryDate).HasColumnType("date");

                e.HasOne(x => x.SharedBudget).WithMany(x => x.CashFlowEntries)
                 .HasForeignKey(x => x.SharedBudgetId).OnDelete(DeleteBehavior.Cascade)
                 .HasConstraintName("FK_tblCashFlowEntry_SharedBudgetId");
            });

            mb.Entity<tblCashFlowEntry>().HasData(
                new tblCashFlowEntry { Id=cashId[0], SharedBudgetId=budgetId[0], EntryDate=d,           DayIncome=5000m, DayExpenses=1500m, RunningBalance=3500m, ProjectedBalance=3500m, TransactionCount=2 },
                new tblCashFlowEntry { Id=cashId[1], SharedBudgetId=budgetId[0], EntryDate=d.AddDays(2), DayIncome=0m,    DayExpenses=127m,  RunningBalance=3373m, ProjectedBalance=3373m, TransactionCount=1 },
                new tblCashFlowEntry { Id=cashId[2], SharedBudgetId=budgetId[0], EntryDate=d.AddDays(3), DayIncome=0m,    DayExpenses=14m,   RunningBalance=3359m, ProjectedBalance=3359m, TransactionCount=1 },
                new tblCashFlowEntry { Id=cashId[3], SharedBudgetId=budgetId[0], EntryDate=d.AddDays(4), DayIncome=0m,    DayExpenses=62m,   RunningBalance=3297m, ProjectedBalance=3297m, TransactionCount=1 },
                new tblCashFlowEntry { Id=cashId[4], SharedBudgetId=budgetId[0], EntryDate=d.AddDays(5), DayIncome=0m,    DayExpenses=18m,   RunningBalance=3279m, ProjectedBalance=2800m, TransactionCount=1 }
            );
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblTrackingInsight
        // ─────────────────────────────────────────────────────────────────────
        private void CreateTrackingInsights(ModelBuilder mb)
        {
            for (int i = 0; i < insightId.Length; i++) insightId[i] = Guid.NewGuid();
            int m = DateTime.Today.Month, y = DateTime.Today.Year;

            mb.Entity<tblTrackingInsight>(e =>
            {
                e.HasKey(x => x.Id).HasName("PK_tblTrackingInsight_Id");
                e.ToTable("tblTrackingInsight");
                e.Property(x => x.Id).ValueGeneratedNever();
                e.Property(x => x.Title).IsRequired().HasMaxLength(200).IsUnicode(false);
                e.Property(x => x.Message).IsRequired().HasMaxLength(1000).IsUnicode(false);
                e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                e.Property(x => x.GeneratedOn).HasColumnType("datetime");

                e.HasOne(x => x.SharedBudget).WithMany(x => x.TrackingInsights)
                 .HasForeignKey(x => x.SharedBudgetId).OnDelete(DeleteBehavior.Cascade)
                 .HasConstraintName("FK_tblTrackingInsight_SharedBudgetId");

                e.HasOne(x => x.Category).WithMany(x => x.TrackingInsights)
                 .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull)
                 .HasConstraintName("FK_tblTrackingInsight_CategoryId");
            });

            mb.Entity<tblTrackingInsight>().HasData(
                new tblTrackingInsight { Id=insightId[0], SharedBudgetId=budgetId[0], Month=m, Year=y, InsightType=0, Severity=2, CategoryId=categoryId[2], Title="Dining Out over budget",      Message="You've spent $60 more than planned on dining this month.",                      Amount=60m,   ChangePercent=30.0, GeneratedOn=DateTime.Today, IsDismissed=false, IsRead=false },
                new tblTrackingInsight { Id=insightId[1], SharedBudgetId=budgetId[0], Month=m, Year=y, InsightType=7, Severity=1, CategoryId=null,          Title="2 unassigned transactions",   Message="You have 2 transactions that haven't been assigned to a budget category.",     Amount=null,  ChangePercent=null, GeneratedOn=DateTime.Today, IsDismissed=false, IsRead=false },
                new tblTrackingInsight { Id=insightId[2], SharedBudgetId=budgetId[0], Month=m, Year=y, InsightType=1, Severity=1, CategoryId=categoryId[1], Title="Groceries on track",           Message="You're spending less on groceries than last month — nice work!",               Amount=null,  ChangePercent=-5.0, GeneratedOn=DateTime.Today, IsDismissed=false, IsRead=false }
            );
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblPushNotification
        // ─────────────────────────────────────────────────────────────────────
        private void CreatePushNotifications(ModelBuilder mb)
        {
            for (int i = 0; i < notifId.Length; i++) notifId[i] = Guid.NewGuid();

            mb.Entity<tblPushNotification>(e =>
            {
                e.HasKey(x => x.Id).HasName("PK_tblPushNotification_Id");
                e.ToTable("tblPushNotification");
                e.Property(x => x.Id).ValueGeneratedNever();
                e.Property(x => x.Title).IsRequired().HasMaxLength(200).IsUnicode(false);
                e.Property(x => x.Body).IsRequired().HasMaxLength(500).IsUnicode(false);
                e.Property(x => x.Icon).IsRequired().HasMaxLength(200).IsUnicode(false);
                e.Property(x => x.ActionUrl).IsRequired().HasMaxLength(500).IsUnicode(false);
                e.Property(x => x.PushEndpoint).IsRequired().HasMaxLength(500).IsUnicode(false);
                e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                e.Property(x => x.SentOn).HasColumnType("datetime");
                e.Property(x => x.ScheduledFor).HasColumnType("datetime");
                e.Property(x => x.ErrorMessage).IsRequired().HasMaxLength(500).IsUnicode(false);

                e.HasOne(x => x.SharedBudget).WithMany(x => x.PushNotifications)
                 .HasForeignKey(x => x.SharedBudgetId).OnDelete(DeleteBehavior.Cascade)
                 .HasConstraintName("FK_tblPushNotification_SharedBudgetId");

                e.HasOne(x => x.User).WithMany(x => x.PushNotifications)
                 .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction)
                 .HasConstraintName("FK_tblPushNotification_UserId");

                e.HasOne(x => x.Transaction).WithMany(x => x.PushNotifications)
                 .HasForeignKey(x => x.TransactionId).OnDelete(DeleteBehavior.SetNull)
                 .HasConstraintName("FK_tblPushNotification_TransactionId");

                e.HasOne(x => x.Category).WithMany(x => x.PushNotifications)
                 .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull)
                 .HasConstraintName("FK_tblPushNotification_CategoryId");
            });

            mb.Entity<tblPushNotification>().HasData(
                new tblPushNotification { Id=notifId[0], SharedBudgetId=budgetId[0], UserId=userId[0], NotificationType=1, Title="Dining Out over budget!", Body="You've exceeded your Dining Out budget by $60.",    Icon="/icons/icon-192.png", ActionUrl="/budget",       PushEndpoint="", TransactionId=null,    CategoryId=categoryId[2], Amount=60m, ScheduledFor=DateTime.Today, SentOn=DateTime.Today, Status=1, IsRead=false, ErrorMessage="" },
                new tblPushNotification { Id=notifId[1], SharedBudgetId=budgetId[0], UserId=userId[0], NotificationType=0, Title="New transaction: Starbucks", Body="A $7.00 charge at Starbucks is pending review.", Icon="/icons/icon-192.png", ActionUrl="/transactions", PushEndpoint="", TransactionId=txId[8], CategoryId=null,          Amount=7m,  ScheduledFor=DateTime.Today, SentOn=DateTime.Today, Status=1, IsRead=false, ErrorMessage="" },
                new tblPushNotification { Id=notifId[2], SharedBudgetId=budgetId[0], UserId=userId[0], NotificationType=2, Title="Transport at 80%",           Body="You've used 80% of your Transport budget.",      Icon="/icons/icon-192.png", ActionUrl="/budget",       PushEndpoint="", TransactionId=null,    CategoryId=categoryId[3], Amount=200m,ScheduledFor=DateTime.Today, SentOn=DateTime.Today, Status=1, IsRead=true,  ErrorMessage="" }
            );
        }

        // ─────────────────────────────────────────────────────────────────────
        // tblNotificationPreference
        // ─────────────────────────────────────────────────────────────────────
        private void CreateNotificationPreferences(ModelBuilder mb)
        {
            for (int i = 0; i < prefId.Length; i++) prefId[i] = Guid.NewGuid();

            mb.Entity<tblNotificationPreference>(e =>
            {
                e.HasKey(x => x.Id).HasName("PK_tblNotificationPreference_Id");
                e.ToTable("tblNotificationPreference");
                e.Property(x => x.Id).ValueGeneratedNever();
                e.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("UX_tblNotificationPreference_UserId");
                e.Property(x => x.PushEndpoint).IsRequired().HasMaxLength(500).IsUnicode(false);
                e.Property(x => x.P256dhKey).IsRequired().HasMaxLength(200).IsUnicode(false);
                e.Property(x => x.AuthKey).IsRequired().HasMaxLength(100).IsUnicode(false);
                e.Property(x => x.LargeTransactionThreshold).HasColumnType("decimal(18,2)");
                e.Property(x => x.LastUpdated).HasColumnType("datetime");

                e.HasOne(x => x.User).WithOne(x => x.NotificationPreference)
                 .HasForeignKey<tblNotificationPreference>(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade)
                 .HasConstraintName("FK_tblNotificationPreference_UserId");
            });

            mb.Entity<tblNotificationPreference>().HasData(
                new tblNotificationPreference { Id=prefId[0], UserId=userId[0], PushEndpoint="", P256dhKey="", AuthKey="", NotifyOnNewTransaction=true,  NotifyOnBudgetOverage=true,  NotifyOnBudgetWarning=true,  NotifyOnLargeTransaction=true,  NotifyWeeklySummary=false, NotifyMonthlySummary=true,  NotifyOnSyncError=true, LargeTransactionThreshold=100m, QuietHoursStart=new TimeOnly(22,0), QuietHoursEnd=new TimeOnly(7,0), IsPushEnabled=false, LastUpdated=new DateTime(2025,1,1) },
                new tblNotificationPreference { Id=prefId[1], UserId=userId[1], PushEndpoint="", P256dhKey="", AuthKey="", NotifyOnNewTransaction=true,  NotifyOnBudgetOverage=true,  NotifyOnBudgetWarning=false, NotifyOnLargeTransaction=false, NotifyWeeklySummary=false, NotifyMonthlySummary=false, NotifyOnSyncError=true, LargeTransactionThreshold=200m, QuietHoursStart=new TimeOnly(22,0), QuietHoursEnd=new TimeOnly(7,0), IsPushEnabled=false, LastUpdated=new DateTime(2025,1,1) }
            );
        }
    }
}
