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
                    Password = table.Column<string>(type: "varchar(28)", unicode: false, maxLength: 28, nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_tblPushNotification_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUser",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "tblSharedBudget",
                columns: new[] { "Id", "DateCreated", "Description", "InviteCode", "IsActive", "MaxMembers", "Name", "OwnerId" },
                values: new object[] { new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shared household budget", "XEESFJ", true, 10, "Demo Family Budget", new Guid("926b898f-0450-4b4f-94ce-30c902b87205") });

            migrationBuilder.InsertData(
                table: "tblUser",
                columns: new[] { "Id", "BudgetRole", "CurrencyCode", "DateRegistered", "Email", "FirstName", "LastLogin", "LastName", "Password", "SharedBudgetId", "TimeZone", "UserId" },
                values: new object[] { new Guid("77503dc9-8293-48da-ba51-8d03b62ce5e7"), null, "USD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "taylor@emksave.com", "Taylor", null, "New", "PPz2fFi+bBSpHkNMZLKJkW7lB0Q=", null, "America/Chicago", "solo" });

            migrationBuilder.InsertData(
                table: "tblBudgetCategory",
                columns: new[] { "Id", "CategoryType", "Color", "Icon", "IsActive", "Name", "SharedBudgetId", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("099994e2-6406-4d89-ba5c-7838a979a775"), 2, "#008300", "ti-piggy-bank", true, "Savings", new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 7 },
                    { new Guid("20b1aa0f-f1e5-4d8b-8ffb-1a4240f06fb3"), 1, "#1baf7a", "ti-shopping-cart", true, "Groceries", new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 2 },
                    { new Guid("30ace2f8-7ae2-4ecb-afb2-046865a905a6"), 0, "#1baf7a", "ti-wallet", true, "Income", new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 0 },
                    { new Guid("79be1811-cf0f-428f-9f9c-31f0194c47ac"), 1, "#4a3aa7", "ti-car", true, "Transport", new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 4 },
                    { new Guid("7b0e162c-cd9f-4540-b9e1-9ff5a18e18ad"), 1, "#e34948", "ti-device-tv", true, "Entertainment", new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 5 },
                    { new Guid("94b0f9e0-87cd-4146-af66-add240f76988"), 1, "#e87ba4", "ti-heart", true, "Healthcare", new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 6 },
                    { new Guid("e15cf25b-e085-43b2-a35a-6f328f4a5b62"), 1, "#eda100", "ti-tools-kitchen", true, "Dining Out", new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 3 },
                    { new Guid("ed9cb15e-29af-4e5a-bca3-e7a136517fb3"), 1, "#2a78d6", "ti-home", true, "Housing", new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 1 }
                });

            migrationBuilder.InsertData(
                table: "tblCashFlowEntry",
                columns: new[] { "Id", "DayExpenses", "DayIncome", "EntryDate", "ProjectedBalance", "RunningBalance", "SharedBudgetId", "TransactionCount" },
                values: new object[,]
                {
                    { new Guid("07b18098-d525-4cde-ae15-3e8d62159166"), 62m, 0m, new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 3297m, 3297m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 1 },
                    { new Guid("0de5086d-6fc8-4508-a55c-7928761f34f6"), 14m, 0m, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 3359m, 3359m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 1 },
                    { new Guid("2ce7c3a8-bcba-4088-aae0-b409e31d55f0"), 18m, 0m, new DateTime(2026, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 2800m, 3279m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 1 },
                    { new Guid("966265d0-e59d-470b-92fe-9b3ed1ef43c3"), 1500m, 5000m, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3500m, 3500m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 2 },
                    { new Guid("ec6e5661-f6d3-49de-815c-ca5569336095"), 127m, 0m, new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 3373m, 3373m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 1 }
                });

            migrationBuilder.InsertData(
                table: "tblMonthlySnapshot",
                columns: new[] { "Id", "Month", "OverBudgetCategoryCount", "SharedBudgetId", "SnapshotDate", "TotalBudgeted", "TotalExpenses", "TotalIncome", "TotalSavings", "TransactionCount", "Year" },
                values: new object[,]
                {
                    { new Guid("9a734a99-5ad3-4b05-a302-cbdc78ae96d3"), 5, 1, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3100m, 2650m, 5000m, 500m, 42, 2025 },
                    { new Guid("f51a306b-8962-4fb5-bd55-1d66373c85dd"), 6, 2, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3100m, 2920m, 5000m, 500m, 38, 2025 }
                });

            migrationBuilder.InsertData(
                table: "tblTrackingInsight",
                columns: new[] { "Id", "Amount", "CategoryId", "ChangePercent", "GeneratedOn", "InsightType", "IsDismissed", "IsRead", "Message", "Month", "Severity", "SharedBudgetId", "Title", "Year" },
                values: new object[] { new Guid("382603b3-0af2-4a37-a411-7fd227736395"), null, null, null, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), 7, false, false, "You have 2 transactions that haven't been assigned to a budget category.", 7, 1, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), "2 unassigned transactions", 2026 });

            migrationBuilder.InsertData(
                table: "tblUser",
                columns: new[] { "Id", "BudgetRole", "CurrencyCode", "DateRegistered", "Email", "FirstName", "LastLogin", "LastName", "Password", "SharedBudgetId", "TimeZone", "UserId" },
                values: new object[,]
                {
                    { new Guid("40b129a5-e9e9-4e77-8ec3-2553cb1245d7"), 1, "USD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jamie@emksave.com", "Jamie", null, "Smith", "1bqJSAdM2/NtF/Ycjz8HclbgT7M=", new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), "America/Chicago", "member" },
                    { new Guid("926b898f-0450-4b4f-94ce-30c902b87205"), 0, "USD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "alex@emksave.com", "Alex", null, "Demo", "Y7g4XITs1TDz/DlSKl9C+8XWPD8=", new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), "America/Chicago", "owner" }
                });

            migrationBuilder.InsertData(
                table: "tblBudget",
                columns: new[] { "Id", "CategoryId", "Month", "Notes", "PlannedAmount", "RolloverAmount", "SharedBudgetId", "Year" },
                values: new object[,]
                {
                    { new Guid("15325532-a32a-46e8-8269-6def7847cea5"), new Guid("94b0f9e0-87cd-4146-af66-add240f76988"), 7, "", 150m, 0m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 2026 },
                    { new Guid("427c8836-123d-42e4-b0c1-815fc1b45522"), new Guid("7b0e162c-cd9f-4540-b9e1-9ff5a18e18ad"), 7, "", 100m, 0m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 2026 },
                    { new Guid("5d5a9776-a0d2-43b3-8e0e-0402971596e0"), new Guid("e15cf25b-e085-43b2-a35a-6f328f4a5b62"), 7, "", 200m, 0m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 2026 },
                    { new Guid("8f6b8e11-0cf3-43ab-8c3b-a71afe4e81fc"), new Guid("099994e2-6406-4d89-ba5c-7838a979a775"), 7, "", 500m, 0m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 2026 },
                    { new Guid("8f7f6894-ad6e-438d-bfd8-1366e7ea15a5"), new Guid("ed9cb15e-29af-4e5a-bca3-e7a136517fb3"), 7, "", 1500m, 0m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 2026 },
                    { new Guid("8f80773d-0e2b-4b72-bb6b-4055de1e2123"), new Guid("79be1811-cf0f-428f-9f9c-31f0194c47ac"), 7, "", 250m, 0m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 2026 },
                    { new Guid("dc893dba-5026-4716-9bcb-f73ef44b4bdf"), new Guid("20b1aa0f-f1e5-4d8b-8ffb-1a4240f06fb3"), 7, "", 400m, 0m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 2026 },
                    { new Guid("f3b6f565-9cdd-407e-a750-63c14598cf31"), new Guid("30ace2f8-7ae2-4ecb-afb2-046865a905a6"), 7, "Monthly salary", 5000m, 0m, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 2026 }
                });

            migrationBuilder.InsertData(
                table: "tblCategorySummary",
                columns: new[] { "Id", "ActualAmount", "CategoryId", "PlannedAmount", "SnapshotId", "TransactionCount" },
                values: new object[,]
                {
                    { new Guid("4347ce9e-9573-4cdd-a866-e42fc1869653"), 260m, new Guid("e15cf25b-e085-43b2-a35a-6f328f4a5b62"), 200m, new Guid("f51a306b-8962-4fb5-bd55-1d66373c85dd"), 8 },
                    { new Guid("5bf540ea-94d7-41f9-a623-6949354ba864"), 1500m, new Guid("ed9cb15e-29af-4e5a-bca3-e7a136517fb3"), 1500m, new Guid("f51a306b-8962-4fb5-bd55-1d66373c85dd"), 1 },
                    { new Guid("708d4016-e9ae-4da1-a503-4dffd6678237"), 290m, new Guid("79be1811-cf0f-428f-9f9c-31f0194c47ac"), 250m, new Guid("f51a306b-8962-4fb5-bd55-1d66373c85dd"), 6 },
                    { new Guid("7697bbd3-adfc-435d-a632-f18f32729668"), 5000m, new Guid("30ace2f8-7ae2-4ecb-afb2-046865a905a6"), 5000m, new Guid("f51a306b-8962-4fb5-bd55-1d66373c85dd"), 1 },
                    { new Guid("7f8e0721-8885-40b9-9101-a21baee493b3"), 300m, new Guid("099994e2-6406-4d89-ba5c-7838a979a775"), 500m, new Guid("f51a306b-8962-4fb5-bd55-1d66373c85dd"), 2 },
                    { new Guid("ea90d390-1366-459d-bfbc-d2caedce4854"), 105m, new Guid("94b0f9e0-87cd-4146-af66-add240f76988"), 150m, new Guid("f51a306b-8962-4fb5-bd55-1d66373c85dd"), 4 },
                    { new Guid("fc4695fc-da19-4000-b6c5-5385e3712321"), 380m, new Guid("20b1aa0f-f1e5-4d8b-8ffb-1a4240f06fb3"), 400m, new Guid("f51a306b-8962-4fb5-bd55-1d66373c85dd"), 5 },
                    { new Guid("fd3b4512-f342-4222-acfe-f30d57e71b9c"), 85m, new Guid("7b0e162c-cd9f-4540-b9e1-9ff5a18e18ad"), 100m, new Guid("f51a306b-8962-4fb5-bd55-1d66373c85dd"), 3 }
                });

            migrationBuilder.InsertData(
                table: "tblNotificationPreference",
                columns: new[] { "Id", "AuthKey", "IsPushEnabled", "LargeTransactionThreshold", "LastUpdated", "NotifyMonthlySummary", "NotifyOnBudgetOverage", "NotifyOnBudgetWarning", "NotifyOnLargeTransaction", "NotifyOnNewTransaction", "NotifyOnSyncError", "NotifyWeeklySummary", "P256dhKey", "PushEndpoint", "QuietHoursEnd", "QuietHoursStart", "UserId" },
                values: new object[,]
                {
                    { new Guid("6ca2071e-9c1c-4861-ad8d-1c4c963cc3e9"), "", false, 100m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, true, true, true, false, "", "", new TimeOnly(7, 0, 0), new TimeOnly(22, 0, 0), new Guid("926b898f-0450-4b4f-94ce-30c902b87205") },
                    { new Guid("e09b69d6-a88e-4b46-a733-daa391126ece"), "", false, 200m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, true, false, false, true, true, false, "", "", new TimeOnly(7, 0, 0), new TimeOnly(22, 0, 0), new Guid("40b129a5-e9e9-4e77-8ec3-2553cb1245d7") }
                });

            migrationBuilder.InsertData(
                table: "tblPlaidAccount",
                columns: new[] { "Id", "AccessTokenEncrypted", "AccountName", "AccountSubtype", "AccountType", "AvailableBalance", "CurrentBalance", "DateLinked", "InstitutionLogoUrl", "InstitutionName", "IsActive", "IsoCurrencyCode", "LastSynced", "Mask", "PlaidAccountId", "PlaidItemId", "SharedBudgetId", "UserId" },
                values: new object[,]
                {
                    { new Guid("18616a35-8b4e-47e9-9701-9345757f800b"), "DEMO_ENCRYPTED_TOKEN", "Checking", "checking", "depository", 3150.00m, 3200.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "First National Bank", true, "USD", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "1234", "demo_chk_001", "demo_item_001", new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new Guid("926b898f-0450-4b4f-94ce-30c902b87205") },
                    { new Guid("42120fc4-7069-47d6-847b-15314ce50133"), "DEMO_ENCRYPTED_TOKEN", "Savings", "savings", "depository", 8500.00m, 8500.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "First National Bank", true, "USD", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "5678", "demo_sav_001", "demo_item_001", new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new Guid("926b898f-0450-4b4f-94ce-30c902b87205") }
                });

            migrationBuilder.InsertData(
                table: "tblPushNotification",
                columns: new[] { "Id", "ActionUrl", "Amount", "Body", "CategoryId", "ErrorMessage", "Icon", "IsRead", "NotificationType", "PushEndpoint", "ScheduledFor", "SentOn", "SharedBudgetId", "Status", "Title", "TransactionId", "UserId" },
                values: new object[,]
                {
                    { new Guid("4df72c8d-1eca-47c8-b036-e4cae2aee49a"), "/budget", 60m, "You've exceeded your Dining Out budget by $60.", new Guid("e15cf25b-e085-43b2-a35a-6f328f4a5b62"), "", "/icons/icon-192.png", false, 1, "", new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 1, "Dining Out over budget!", null, new Guid("926b898f-0450-4b4f-94ce-30c902b87205") },
                    { new Guid("d1a25878-399a-4aa5-aa90-40c342ceba95"), "/budget", 200m, "You've used 80% of your Transport budget.", new Guid("79be1811-cf0f-428f-9f9c-31f0194c47ac"), "", "/icons/icon-192.png", true, 2, "", new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 1, "Transport at 80%", null, new Guid("926b898f-0450-4b4f-94ce-30c902b87205") }
                });

            migrationBuilder.InsertData(
                table: "tblTrackingInsight",
                columns: new[] { "Id", "Amount", "CategoryId", "ChangePercent", "GeneratedOn", "InsightType", "IsDismissed", "IsRead", "Message", "Month", "Severity", "SharedBudgetId", "Title", "Year" },
                values: new object[,]
                {
                    { new Guid("33692c8a-e37e-4c3d-8ef7-5af0bd3a5aa5"), 60m, new Guid("e15cf25b-e085-43b2-a35a-6f328f4a5b62"), 30.0, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), 0, false, false, "You've spent $60 more than planned on dining this month.", 7, 2, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), "Dining Out over budget", 2026 },
                    { new Guid("e71e9968-1e00-4a9f-9495-e3c12880e201"), null, new Guid("20b1aa0f-f1e5-4d8b-8ffb-1a4240f06fb3"), -5.0, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), 1, false, false, "You're spending less on groceries than last month — great job!", 7, 1, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), "Groceries on track", 2026 }
                });

            migrationBuilder.InsertData(
                table: "tblTransaction",
                columns: new[] { "Id", "Amount", "CategoryId", "Description", "IsExcluded", "IsPending", "IsReviewed", "IsoCurrencyCode", "MerchantName", "Notes", "PlaidAccountId", "PlaidCategory", "PlaidSubcategory", "PlaidTransactionId", "PostedDate", "SharedBudgetId", "TransactionDate", "tblBudgetId" },
                values: new object[,]
                {
                    { new Guid("16efd5a7-00df-4d63-a11c-bf190dd92989"), -38m, new Guid("94b0f9e0-87cd-4146-af66-add240f76988"), "Pharmacy", false, false, false, "USD", "CVS Pharmacy", "", new Guid("18616a35-8b4e-47e9-9701-9345757f800b"), "Medical", "Pharmacies", "txn_010", null, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("2e6b29c7-cf95-4ff4-9744-a14034271288"), -14m, new Guid("e15cf25b-e085-43b2-a35a-6f328f4a5b62"), "Lunch", false, false, true, "USD", "Chipotle", "", new Guid("18616a35-8b4e-47e9-9701-9345757f800b"), "Food and Drink", "Restaurants", "txn_003", null, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("622cc722-dd5a-40f3-acd7-74ccd66ea2fd"), -1500m, new Guid("ed9cb15e-29af-4e5a-bca3-e7a136517fb3"), "Monthly rent", false, false, true, "USD", "Rent Payment", "", new Guid("18616a35-8b4e-47e9-9701-9345757f800b"), "Payment", "Rent", "txn_001", null, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("75d8a246-cb28-4c09-a303-08d2f53820e8"), -7m, null, "Coffee", false, true, false, "USD", "Starbucks", "", new Guid("18616a35-8b4e-47e9-9701-9345757f800b"), "Food and Drink", "Coffee Shop", "txn_009", null, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("769d240d-87d9-4a55-8194-d3d493b38b69"), -127m, new Guid("20b1aa0f-f1e5-4d8b-8ffb-1a4240f06fb3"), "Groceries", false, false, true, "USD", "Whole Foods", "", new Guid("18616a35-8b4e-47e9-9701-9345757f800b"), "Food and Drink", "Supermarkets", "txn_002", null, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("baa6ecb4-38ab-4d83-987c-d8f5e8c35b65"), -18m, new Guid("7b0e162c-cd9f-4540-b9e1-9ff5a18e18ad"), "Streaming sub", false, false, true, "USD", "Netflix", "", new Guid("18616a35-8b4e-47e9-9701-9345757f800b"), "Service", "Subscription", "txn_005", null, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2026, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("c4642102-5a7a-4c92-8eb8-b41da17f1096"), -62m, new Guid("79be1811-cf0f-428f-9f9c-31f0194c47ac"), "Fuel", false, false, true, "USD", "Shell Gas Station", "", new Guid("18616a35-8b4e-47e9-9701-9345757f800b"), "Travel", "Gas Stations", "txn_004", null, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("cb4d659f-b2bd-4994-a227-cf7a434c1c42"), -43m, null, "Online purchase", false, false, false, "USD", "Amazon", "", new Guid("18616a35-8b4e-47e9-9701-9345757f800b"), "Shopping", "Online", "txn_008", null, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("ccac5cd3-8edf-4714-b7b1-247d4b54523a"), 5000m, new Guid("30ace2f8-7ae2-4ecb-afb2-046865a905a6"), "Payroll", false, false, true, "USD", "Employer Direct Dep", "", new Guid("18616a35-8b4e-47e9-9701-9345757f800b"), "Payroll", "Payroll", "txn_006", null, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("d6551933-7a67-41c8-8c14-0d02903f1a87"), -89m, new Guid("20b1aa0f-f1e5-4d8b-8ffb-1a4240f06fb3"), "Groceries", false, false, false, "USD", "Trader Joe's", "", new Guid("18616a35-8b4e-47e9-9701-9345757f800b"), "Food and Drink", "Supermarkets", "txn_007", null, new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null }
                });

            migrationBuilder.InsertData(
                table: "tblPushNotification",
                columns: new[] { "Id", "ActionUrl", "Amount", "Body", "CategoryId", "ErrorMessage", "Icon", "IsRead", "NotificationType", "PushEndpoint", "ScheduledFor", "SentOn", "SharedBudgetId", "Status", "Title", "TransactionId", "UserId" },
                values: new object[] { new Guid("c3171dae-f0c4-406d-8d97-d66f162865b9"), "/transactions", 7m, "A $7.00 charge at Starbucks is pending review.", null, "", "/icons/icon-192.png", false, 0, "", new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new Guid("2d11e4f4-3263-44ba-8d32-604272b4d779"), 1, "New transaction: Starbucks", new Guid("75d8a246-cb28-4c09-a303-08d2f53820e8"), new Guid("926b898f-0450-4b4f-94ce-30c902b87205") });

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
