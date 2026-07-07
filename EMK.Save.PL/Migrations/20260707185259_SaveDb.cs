using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EMK.Save.PL.Migrations
{
    /// <inheritdoc />
    public partial class SaveDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblSharedBudget",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InviteCode = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    MaxMembers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSharedBudget_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblBudgetCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SharedBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CategoryType = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBudgetCategory_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblBudgetCategory_SharedBudgetId",
                        column: x => x.SharedBudgetId,
                        principalTable: "tblSharedBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblCashFlowEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SharedBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "date", nullable: false),
                    DayIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DayExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RunningBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProjectedBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCashFlowEntry_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblCashFlowEntry_SharedBudgetId",
                        column: x => x.SharedBudgetId,
                        principalTable: "tblSharedBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblMonthlySnapshot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SharedBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    TotalIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalBudgeted = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalSavings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionCount = table.Column<int>(type: "int", nullable: false),
                    OverBudgetCategoryCount = table.Column<int>(type: "int", nullable: false),
                    SnapshotDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblMonthlySnapshot_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblMonthlySnapshot_SharedBudgetId",
                        column: x => x.SharedBudgetId,
                        principalTable: "tblSharedBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    TimeZone = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CurrencyCode = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    DateRegistered = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    SharedBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BudgetRole = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUser_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblUser_SharedBudgetId",
                        column: x => x.SharedBudgetId,
                        principalTable: "tblSharedBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "tblBudget",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SharedBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    PlannedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RolloverAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBudget_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblBudget_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "tblBudgetCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblBudget_SharedBudgetId",
                        column: x => x.SharedBudgetId,
                        principalTable: "tblSharedBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblTrackingInsight",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SharedBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    InsightType = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChangePercent = table.Column<double>(type: "float", nullable: true),
                    GeneratedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDismissed = table.Column<bool>(type: "bit", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTrackingInsight_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblTrackingInsight_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "tblBudgetCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblTrackingInsight_SharedBudgetId",
                        column: x => x.SharedBudgetId,
                        principalTable: "tblSharedBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblCategorySummary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlannedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCategorySummary_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblCategorySummary_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "tblBudgetCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblCategorySummary_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "tblMonthlySnapshot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblNotificationPreference",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PushEndpoint = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    P256dhKey = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    AuthKey = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    NotifyOnNewTransaction = table.Column<bool>(type: "bit", nullable: false),
                    NotifyOnBudgetOverage = table.Column<bool>(type: "bit", nullable: false),
                    NotifyOnBudgetWarning = table.Column<bool>(type: "bit", nullable: false),
                    NotifyOnLargeTransaction = table.Column<bool>(type: "bit", nullable: false),
                    NotifyWeeklySummary = table.Column<bool>(type: "bit", nullable: false),
                    NotifyMonthlySummary = table.Column<bool>(type: "bit", nullable: false),
                    NotifyOnSyncError = table.Column<bool>(type: "bit", nullable: false),
                    LargeTransactionThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuietHoursStart = table.Column<TimeOnly>(type: "time", nullable: false),
                    QuietHoursEnd = table.Column<TimeOnly>(type: "time", nullable: false),
                    IsPushEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblNotificationPreference_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblNotificationPreference_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblPlaidAccount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SharedBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaidAccountId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    PlaidItemId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    AccessTokenEncrypted = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    SyncCursor = table.Column<string>(type: "varchar(2000)", unicode: false, maxLength: 2000, nullable: true),
                    InstitutionName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    InstitutionLogoUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    AccountName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Mask = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: false),
                    AccountType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    AccountSubtype = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvailableBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsoCurrencyCode = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    LastSynced = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateLinked = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPlaidAccount_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblPlaidAccount_SharedBudgetId",
                        column: x => x.SharedBudgetId,
                        principalTable: "tblSharedBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblPlaidAccount_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaidAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SharedBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlaidTransactionId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "date", nullable: false),
                    PostedDate = table.Column<DateTime>(type: "date", nullable: true),
                    MerchantName = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsoCurrencyCode = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    PlaidCategory = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    PlaidSubcategory = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    IsExcluded = table.Column<bool>(type: "bit", nullable: false),
                    IsPending = table.Column<bool>(type: "bit", nullable: false),
                    IsReviewed = table.Column<bool>(type: "bit", nullable: false),
                    tblBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTransaction_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblTransaction_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "tblBudgetCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblTransaction_PlaidAccountId",
                        column: x => x.PlaidAccountId,
                        principalTable: "tblPlaidAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblTransaction_SharedBudgetId",
                        column: x => x.SharedBudgetId,
                        principalTable: "tblSharedBudget",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblTransaction_tblBudget_tblBudgetId",
                        column: x => x.tblBudgetId,
                        principalTable: "tblBudget",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tblPushNotification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SharedBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    Icon = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    ActionUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    PushEndpoint = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SentOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ScheduledFor = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPushNotification_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblPushNotification_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "tblBudgetCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblPushNotification_SharedBudgetId",
                        column: x => x.SharedBudgetId,
                        principalTable: "tblSharedBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblPushNotification_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "tblTransaction",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblPushNotification_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUser",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "tblSharedBudget",
                columns: new[] { "Id", "DateCreated", "Description", "InviteCode", "IsActive", "MaxMembers", "Name", "OwnerId" },
                values: new object[] { new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shared household budget", "XEESFJ", true, 10, "Demo Family Budget", new Guid("9f9f6c88-5c09-41ff-8b09-0ee67426fd15") });

            migrationBuilder.InsertData(
                table: "tblUser",
                columns: new[] { "Id", "BudgetRole", "CurrencyCode", "DateRegistered", "Email", "FirstName", "LastLogin", "LastName", "Password", "SharedBudgetId", "TimeZone", "UserId" },
                values: new object[] { new Guid("36a2d704-30af-484d-b73c-bfbb60e5e4fa"), null, "USD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "taylor@emksave.com", "Taylor", null, "New", "210000.Emgqctwvtp+1ZEtOw0L20g==.dJfRRDjOvcbwSl5/O078gSiNVmdl/u9/I2dKU30yrDE=", null, "America/Chicago", "solo" });

            migrationBuilder.InsertData(
                table: "tblBudgetCategory",
                columns: new[] { "Id", "CategoryType", "Color", "Icon", "IsActive", "Name", "SharedBudgetId", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("16154f06-48c2-47c3-8aee-728a1bbfd358"), 0, "#1baf7a", "ti-wallet", true, "Income", new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 0 },
                    { new Guid("18df092b-ac09-42e4-9cfc-63d94a947ceb"), 1, "#e34948", "ti-device-tv", true, "Entertainment", new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 5 },
                    { new Guid("28d938ae-5d46-4ebe-bd6e-1cfe711e7886"), 1, "#eda100", "ti-tools-kitchen", true, "Dining Out", new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 3 },
                    { new Guid("2e07d55a-15f5-4b6c-9d60-6818f3e3f93f"), 2, "#008300", "ti-piggy-bank", true, "Savings", new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 7 },
                    { new Guid("865d387b-a568-42d2-94f0-ce8abcf0af83"), 1, "#1baf7a", "ti-shopping-cart", true, "Groceries", new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 2 },
                    { new Guid("a330c6ee-742f-474a-86bc-9a3435f4404d"), 1, "#4a3aa7", "ti-car", true, "Transport", new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 4 },
                    { new Guid("a85f43a1-9796-4d2e-b722-5eaa6fedca0c"), 1, "#e87ba4", "ti-heart", true, "Healthcare", new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 6 },
                    { new Guid("f213e9aa-454f-4d45-9c7a-45f81fb6e5bd"), 1, "#2a78d6", "ti-home", true, "Housing", new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 1 }
                });

            migrationBuilder.InsertData(
                table: "tblCashFlowEntry",
                columns: new[] { "Id", "DayExpenses", "DayIncome", "EntryDate", "ProjectedBalance", "RunningBalance", "SharedBudgetId", "TransactionCount" },
                values: new object[,]
                {
                    { new Guid("33dd9864-1b71-48d8-a358-7bdfc1d0ea9c"), 18m, 0m, new DateTime(2026, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 2800m, 3279m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 1 },
                    { new Guid("8b50f359-bc80-4dfb-9614-2c2e4c7bcaa7"), 127m, 0m, new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 3373m, 3373m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 1 },
                    { new Guid("94c10ce3-1d3f-4bef-89a2-570f55f94e0d"), 14m, 0m, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 3359m, 3359m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 1 },
                    { new Guid("97e7607c-5198-4021-ab72-f35ceaf2625e"), 1500m, 5000m, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3500m, 3500m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 2 },
                    { new Guid("fb17517b-48b1-440a-b0d2-34bfbb169187"), 62m, 0m, new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 3297m, 3297m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 1 }
                });

            migrationBuilder.InsertData(
                table: "tblMonthlySnapshot",
                columns: new[] { "Id", "Month", "OverBudgetCategoryCount", "SharedBudgetId", "SnapshotDate", "TotalBudgeted", "TotalExpenses", "TotalIncome", "TotalSavings", "TransactionCount", "Year" },
                values: new object[,]
                {
                    { new Guid("81d4cb8c-424c-4d6a-9a85-c5f5c6a85875"), 5, 1, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3100m, 2650m, 5000m, 500m, 42, 2025 },
                    { new Guid("9f6bdc8d-ec49-4621-aefa-c2a94662097b"), 6, 2, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3100m, 2920m, 5000m, 500m, 38, 2025 }
                });

            migrationBuilder.InsertData(
                table: "tblTrackingInsight",
                columns: new[] { "Id", "Amount", "CategoryId", "ChangePercent", "GeneratedOn", "InsightType", "IsDismissed", "IsRead", "Message", "Month", "Severity", "SharedBudgetId", "Title", "Year" },
                values: new object[] { new Guid("df23a5f8-6804-4212-9407-24a014791a74"), null, null, null, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), 7, false, false, "You have 2 transactions that haven't been assigned to a budget category.", 7, 1, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), "2 unassigned transactions", 2026 });

            migrationBuilder.InsertData(
                table: "tblUser",
                columns: new[] { "Id", "BudgetRole", "CurrencyCode", "DateRegistered", "Email", "FirstName", "LastLogin", "LastName", "Password", "SharedBudgetId", "TimeZone", "UserId" },
                values: new object[,]
                {
                    { new Guid("2addd71c-50c8-4910-8934-587dfddaf4e0"), 1, "USD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jamie@emksave.com", "Jamie", null, "Smith", "210000.PVOSX26R03MJMAT79eVt+w==.G2w9U1/DNw88fBiKKGgC3vif8oVaTDD/Lsa+g+JLlOc=", new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), "America/Chicago", "member" },
                    { new Guid("9f9f6c88-5c09-41ff-8b09-0ee67426fd15"), 0, "USD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "alex@emksave.com", "Alex", null, "Demo", "210000.KQQBHUCITNCsf7vHNITa/g==.ipIC718UjA+o5oGnWMV47cOxqeAf2pd/Tekj3A2iJxE=", new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), "America/Chicago", "owner" }
                });

            migrationBuilder.InsertData(
                table: "tblBudget",
                columns: new[] { "Id", "CategoryId", "Month", "Notes", "PlannedAmount", "RolloverAmount", "SharedBudgetId", "Year" },
                values: new object[,]
                {
                    { new Guid("10610c70-55e9-408d-9d56-bdc11cf04db6"), new Guid("a85f43a1-9796-4d2e-b722-5eaa6fedca0c"), 7, "", 150m, 0m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 2026 },
                    { new Guid("182199b0-9047-4c5f-993f-f7797ff05ac0"), new Guid("18df092b-ac09-42e4-9cfc-63d94a947ceb"), 7, "", 100m, 0m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 2026 },
                    { new Guid("20d8135a-7f35-4759-97d0-9d81f389ce05"), new Guid("a330c6ee-742f-474a-86bc-9a3435f4404d"), 7, "", 250m, 0m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 2026 },
                    { new Guid("8236e33e-8659-45df-864f-2df47b429bb1"), new Guid("f213e9aa-454f-4d45-9c7a-45f81fb6e5bd"), 7, "", 1500m, 0m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 2026 },
                    { new Guid("9683508f-f172-4eb0-a9fa-4fc32419ac27"), new Guid("16154f06-48c2-47c3-8aee-728a1bbfd358"), 7, "Monthly salary", 5000m, 0m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 2026 },
                    { new Guid("c8ac551e-1242-4f12-bc94-9270cec1089d"), new Guid("865d387b-a568-42d2-94f0-ce8abcf0af83"), 7, "", 400m, 0m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 2026 },
                    { new Guid("f0ea80d2-2d92-4e5a-9e3c-d014fcf57f56"), new Guid("28d938ae-5d46-4ebe-bd6e-1cfe711e7886"), 7, "", 200m, 0m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 2026 },
                    { new Guid("fad8e459-770d-4abe-9d46-c2fa24802fbf"), new Guid("2e07d55a-15f5-4b6c-9d60-6818f3e3f93f"), 7, "", 500m, 0m, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 2026 }
                });

            migrationBuilder.InsertData(
                table: "tblCategorySummary",
                columns: new[] { "Id", "ActualAmount", "CategoryId", "PlannedAmount", "SnapshotId", "TransactionCount" },
                values: new object[,]
                {
                    { new Guid("37e2ac2e-5ae2-4764-81a5-35cc14322957"), 260m, new Guid("28d938ae-5d46-4ebe-bd6e-1cfe711e7886"), 200m, new Guid("9f6bdc8d-ec49-4621-aefa-c2a94662097b"), 8 },
                    { new Guid("5f165f3a-02dc-4bdd-a804-d5b85cf26c1a"), 85m, new Guid("18df092b-ac09-42e4-9cfc-63d94a947ceb"), 100m, new Guid("9f6bdc8d-ec49-4621-aefa-c2a94662097b"), 3 },
                    { new Guid("8978cb6a-6e56-4a46-aa77-fcd440bafbaf"), 1500m, new Guid("f213e9aa-454f-4d45-9c7a-45f81fb6e5bd"), 1500m, new Guid("9f6bdc8d-ec49-4621-aefa-c2a94662097b"), 1 },
                    { new Guid("a79b401d-a9f8-40a4-b91d-20d807ab0632"), 290m, new Guid("a330c6ee-742f-474a-86bc-9a3435f4404d"), 250m, new Guid("9f6bdc8d-ec49-4621-aefa-c2a94662097b"), 6 },
                    { new Guid("c8592abb-e307-4d2c-b069-1410cf07623b"), 300m, new Guid("2e07d55a-15f5-4b6c-9d60-6818f3e3f93f"), 500m, new Guid("9f6bdc8d-ec49-4621-aefa-c2a94662097b"), 2 },
                    { new Guid("deba07d3-a38a-484f-81ec-36d323569c60"), 5000m, new Guid("16154f06-48c2-47c3-8aee-728a1bbfd358"), 5000m, new Guid("9f6bdc8d-ec49-4621-aefa-c2a94662097b"), 1 },
                    { new Guid("efd17d60-c19f-403b-aa31-82c81d9a1459"), 380m, new Guid("865d387b-a568-42d2-94f0-ce8abcf0af83"), 400m, new Guid("9f6bdc8d-ec49-4621-aefa-c2a94662097b"), 5 },
                    { new Guid("f43e9309-b14a-4135-9a8e-8530aea6c596"), 105m, new Guid("a85f43a1-9796-4d2e-b722-5eaa6fedca0c"), 150m, new Guid("9f6bdc8d-ec49-4621-aefa-c2a94662097b"), 4 }
                });

            migrationBuilder.InsertData(
                table: "tblNotificationPreference",
                columns: new[] { "Id", "AuthKey", "IsPushEnabled", "LargeTransactionThreshold", "LastUpdated", "NotifyMonthlySummary", "NotifyOnBudgetOverage", "NotifyOnBudgetWarning", "NotifyOnLargeTransaction", "NotifyOnNewTransaction", "NotifyOnSyncError", "NotifyWeeklySummary", "P256dhKey", "PushEndpoint", "QuietHoursEnd", "QuietHoursStart", "UserId" },
                values: new object[,]
                {
                    { new Guid("12cddeeb-9452-4312-9d86-a9599bd956b5"), "", false, 100m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, true, true, true, false, "", "", new TimeOnly(7, 0, 0), new TimeOnly(22, 0, 0), new Guid("9f9f6c88-5c09-41ff-8b09-0ee67426fd15") },
                    { new Guid("422a5432-edd8-4ff4-a609-3c4f53d47a0f"), "", false, 200m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, true, false, false, true, true, false, "", "", new TimeOnly(7, 0, 0), new TimeOnly(22, 0, 0), new Guid("2addd71c-50c8-4910-8934-587dfddaf4e0") }
                });

            migrationBuilder.InsertData(
                table: "tblPlaidAccount",
                columns: new[] { "Id", "AccessTokenEncrypted", "AccountName", "AccountSubtype", "AccountType", "AvailableBalance", "CurrentBalance", "DateLinked", "InstitutionLogoUrl", "InstitutionName", "IsActive", "IsoCurrencyCode", "LastSynced", "Mask", "PlaidAccountId", "PlaidItemId", "SharedBudgetId", "SyncCursor", "UserId" },
                values: new object[,]
                {
                    { new Guid("6c6eee68-42e1-4175-8bb9-db17d1678823"), "DEMO_ENCRYPTED_TOKEN", "Checking", "checking", "depository", 3150.00m, 3200.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "First National Bank", true, "USD", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "1234", "demo_chk_001", "demo_item_001", new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), null, new Guid("9f9f6c88-5c09-41ff-8b09-0ee67426fd15") },
                    { new Guid("aa167159-7f7c-46e9-9d42-333959be8357"), "DEMO_ENCRYPTED_TOKEN", "Savings", "savings", "depository", 8500.00m, 8500.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "First National Bank", true, "USD", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "5678", "demo_sav_001", "demo_item_001", new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), null, new Guid("9f9f6c88-5c09-41ff-8b09-0ee67426fd15") }
                });

            migrationBuilder.InsertData(
                table: "tblPushNotification",
                columns: new[] { "Id", "ActionUrl", "Amount", "Body", "CategoryId", "ErrorMessage", "Icon", "IsRead", "NotificationType", "PushEndpoint", "ScheduledFor", "SentOn", "SharedBudgetId", "Status", "Title", "TransactionId", "UserId" },
                values: new object[,]
                {
                    { new Guid("c4e6a911-fe26-4ab4-ad5d-00224602a66a"), "/budget", 200m, "You've used 80% of your Transport budget.", new Guid("a330c6ee-742f-474a-86bc-9a3435f4404d"), "", "/icons/icon-192.png", true, 2, "", new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 1, "Transport at 80%", null, new Guid("9f9f6c88-5c09-41ff-8b09-0ee67426fd15") },
                    { new Guid("c8630244-e86f-4f1b-9285-ed07a0d79e1b"), "/budget", 60m, "You've exceeded your Dining Out budget by $60.", new Guid("28d938ae-5d46-4ebe-bd6e-1cfe711e7886"), "", "/icons/icon-192.png", false, 1, "", new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 1, "Dining Out over budget!", null, new Guid("9f9f6c88-5c09-41ff-8b09-0ee67426fd15") }
                });

            migrationBuilder.InsertData(
                table: "tblTrackingInsight",
                columns: new[] { "Id", "Amount", "CategoryId", "ChangePercent", "GeneratedOn", "InsightType", "IsDismissed", "IsRead", "Message", "Month", "Severity", "SharedBudgetId", "Title", "Year" },
                values: new object[,]
                {
                    { new Guid("24cb8fcd-69b0-4f6c-b8f7-d5998acf8879"), null, new Guid("865d387b-a568-42d2-94f0-ce8abcf0af83"), -5.0, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), 1, false, false, "You're spending less on groceries than last month — great job!", 7, 1, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), "Groceries on track", 2026 },
                    { new Guid("fb9e4f08-48ec-40c5-9b03-202d559bb4eb"), 60m, new Guid("28d938ae-5d46-4ebe-bd6e-1cfe711e7886"), 30.0, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), 0, false, false, "You've spent $60 more than planned on dining this month.", 7, 2, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), "Dining Out over budget", 2026 }
                });

            migrationBuilder.InsertData(
                table: "tblTransaction",
                columns: new[] { "Id", "Amount", "CategoryId", "Description", "IsExcluded", "IsPending", "IsReviewed", "IsoCurrencyCode", "MerchantName", "Notes", "PlaidAccountId", "PlaidCategory", "PlaidSubcategory", "PlaidTransactionId", "PostedDate", "SharedBudgetId", "TransactionDate", "tblBudgetId" },
                values: new object[,]
                {
                    { new Guid("0bff46a3-5308-419e-b2ea-45dab1ae635b"), -62m, new Guid("a330c6ee-742f-474a-86bc-9a3435f4404d"), "Fuel", false, false, true, "USD", "Shell Gas Station", "", new Guid("6c6eee68-42e1-4175-8bb9-db17d1678823"), "Travel", "Gas Stations", "txn_004", null, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("6233a16e-a489-4259-a25e-680a1284d1ed"), -18m, new Guid("18df092b-ac09-42e4-9cfc-63d94a947ceb"), "Streaming sub", false, false, true, "USD", "Netflix", "", new Guid("6c6eee68-42e1-4175-8bb9-db17d1678823"), "Service", "Subscription", "txn_005", null, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2026, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("66d6ff4f-81a7-4b29-b20a-bb8e931dd6ba"), -14m, new Guid("28d938ae-5d46-4ebe-bd6e-1cfe711e7886"), "Lunch", false, false, true, "USD", "Chipotle", "", new Guid("6c6eee68-42e1-4175-8bb9-db17d1678823"), "Food and Drink", "Restaurants", "txn_003", null, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("69b27686-647a-4d48-901d-a92dd0371035"), -7m, null, "Coffee", false, true, false, "USD", "Starbucks", "", new Guid("6c6eee68-42e1-4175-8bb9-db17d1678823"), "Food and Drink", "Coffee Shop", "txn_009", null, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("91075ad3-670f-48a8-afb3-38924167ca9b"), -127m, new Guid("865d387b-a568-42d2-94f0-ce8abcf0af83"), "Groceries", false, false, true, "USD", "Whole Foods", "", new Guid("6c6eee68-42e1-4175-8bb9-db17d1678823"), "Food and Drink", "Supermarkets", "txn_002", null, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("94804a3e-047d-4e0a-9666-910fb7d48344"), -38m, new Guid("a85f43a1-9796-4d2e-b722-5eaa6fedca0c"), "Pharmacy", false, false, false, "USD", "CVS Pharmacy", "", new Guid("6c6eee68-42e1-4175-8bb9-db17d1678823"), "Medical", "Pharmacies", "txn_010", null, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("bce65089-5c9c-49cd-9a0e-e4c057d6f5d2"), 5000m, new Guid("16154f06-48c2-47c3-8aee-728a1bbfd358"), "Payroll", false, false, true, "USD", "Employer Direct Dep", "", new Guid("6c6eee68-42e1-4175-8bb9-db17d1678823"), "Payroll", "Payroll", "txn_006", null, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("d29a7a52-876c-46fd-8541-e10bd4173bdf"), -1500m, new Guid("f213e9aa-454f-4d45-9c7a-45f81fb6e5bd"), "Monthly rent", false, false, true, "USD", "Rent Payment", "", new Guid("6c6eee68-42e1-4175-8bb9-db17d1678823"), "Payment", "Rent", "txn_001", null, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("d7bbe1ac-5629-427e-8fec-389539efb37f"), -89m, new Guid("865d387b-a568-42d2-94f0-ce8abcf0af83"), "Groceries", false, false, false, "USD", "Trader Joe's", "", new Guid("6c6eee68-42e1-4175-8bb9-db17d1678823"), "Food and Drink", "Supermarkets", "txn_007", null, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("da683557-8fa7-4757-bcd2-96caa2f404f4"), -43m, null, "Online purchase", false, false, false, "USD", "Amazon", "", new Guid("6c6eee68-42e1-4175-8bb9-db17d1678823"), "Shopping", "Online", "txn_008", null, new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null }
                });

            migrationBuilder.InsertData(
                table: "tblPushNotification",
                columns: new[] { "Id", "ActionUrl", "Amount", "Body", "CategoryId", "ErrorMessage", "Icon", "IsRead", "NotificationType", "PushEndpoint", "ScheduledFor", "SentOn", "SharedBudgetId", "Status", "Title", "TransactionId", "UserId" },
                values: new object[] { new Guid("203b2b4d-c5ba-489e-8fb1-2c3127bdcf38"), "/transactions", 7m, "A $7.00 charge at Starbucks is pending review.", null, "", "/icons/icon-192.png", false, 0, "", new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new Guid("41c394db-a679-405e-995f-b48ed42dab0c"), 1, "New transaction: Starbucks", new Guid("69b27686-647a-4d48-901d-a92dd0371035"), new Guid("9f9f6c88-5c09-41ff-8b09-0ee67426fd15") });

            migrationBuilder.CreateIndex(
                name: "IX_tblBudget_CategoryId",
                table: "tblBudget",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblBudget_SharedBudgetId",
                table: "tblBudget",
                column: "SharedBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_tblBudgetCategory_SharedBudgetId",
                table: "tblBudgetCategory",
                column: "SharedBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_tblCashFlowEntry_SharedBudgetId",
                table: "tblCashFlowEntry",
                column: "SharedBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_tblCategorySummary_CategoryId",
                table: "tblCategorySummary",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblCategorySummary_SnapshotId",
                table: "tblCategorySummary",
                column: "SnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_tblMonthlySnapshot_SharedBudgetId",
                table: "tblMonthlySnapshot",
                column: "SharedBudgetId");

            migrationBuilder.CreateIndex(
                name: "UX_tblNotificationPreference_UserId",
                table: "tblNotificationPreference",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblPlaidAccount_SharedBudgetId",
                table: "tblPlaidAccount",
                column: "SharedBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPlaidAccount_UserId",
                table: "tblPlaidAccount",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPushNotification_CategoryId",
                table: "tblPushNotification",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPushNotification_SharedBudgetId",
                table: "tblPushNotification",
                column: "SharedBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPushNotification_TransactionId",
                table: "tblPushNotification",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPushNotification_UserId",
                table: "tblPushNotification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_tblSharedBudget_InviteCode",
                table: "tblSharedBudget",
                column: "InviteCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblTrackingInsight_CategoryId",
                table: "tblTrackingInsight",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblTrackingInsight_SharedBudgetId",
                table: "tblTrackingInsight",
                column: "SharedBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_tblTransaction_CategoryId",
                table: "tblTransaction",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblTransaction_PlaidAccountId",
                table: "tblTransaction",
                column: "PlaidAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_tblTransaction_SharedBudgetId",
                table: "tblTransaction",
                column: "SharedBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_tblTransaction_tblBudgetId",
                table: "tblTransaction",
                column: "tblBudgetId");

            migrationBuilder.CreateIndex(
                name: "UX_tblTransaction_PlaidTransactionId",
                table: "tblTransaction",
                column: "PlaidTransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblUser_SharedBudgetId",
                table: "tblUser",
                column: "SharedBudgetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblCashFlowEntry");

            migrationBuilder.DropTable(
                name: "tblCategorySummary");

            migrationBuilder.DropTable(
                name: "tblNotificationPreference");

            migrationBuilder.DropTable(
                name: "tblPushNotification");

            migrationBuilder.DropTable(
                name: "tblTrackingInsight");

            migrationBuilder.DropTable(
                name: "tblMonthlySnapshot");

            migrationBuilder.DropTable(
                name: "tblTransaction");

            migrationBuilder.DropTable(
                name: "tblPlaidAccount");

            migrationBuilder.DropTable(
                name: "tblBudget");

            migrationBuilder.DropTable(
                name: "tblUser");

            migrationBuilder.DropTable(
                name: "tblBudgetCategory");

            migrationBuilder.DropTable(
                name: "tblSharedBudget");
        }
    }
}
