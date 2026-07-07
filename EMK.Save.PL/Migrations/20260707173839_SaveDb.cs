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
                values: new object[] { new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shared household budget", "XEESFJ", true, 10, "Demo Family Budget", new Guid("f3a19404-554e-4297-a043-189d327532c6") });

            migrationBuilder.InsertData(
                table: "tblUser",
                columns: new[] { "Id", "BudgetRole", "CurrencyCode", "DateRegistered", "Email", "FirstName", "LastLogin", "LastName", "Password", "SharedBudgetId", "TimeZone", "UserId" },
                values: new object[] { new Guid("1cb9f7cf-89cd-4577-b2db-14708eedac8a"), null, "USD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "taylor@emksave.com", "Taylor", null, "New", "210000.bSJrhIp7rjY84FV7PVRhYQ==.47xyRn9TSKbqrYZODyfwsKyx2P05DFidHEaOzN32H+0=", null, "America/Chicago", "solo" });

            migrationBuilder.InsertData(
                table: "tblBudgetCategory",
                columns: new[] { "Id", "CategoryType", "Color", "Icon", "IsActive", "Name", "SharedBudgetId", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("088af96a-f35e-4ecf-a7bb-dcfe18e4491f"), 1, "#eda100", "ti-tools-kitchen", true, "Dining Out", new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 3 },
                    { new Guid("273366f8-4856-4b80-aabd-0685e83c484c"), 1, "#e34948", "ti-device-tv", true, "Entertainment", new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 5 },
                    { new Guid("4802be14-6e75-4f32-9ef4-3219fb2d1381"), 1, "#2a78d6", "ti-home", true, "Housing", new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 1 },
                    { new Guid("74dd7e95-eb7c-4032-ab14-80c624b4f7c3"), 1, "#4a3aa7", "ti-car", true, "Transport", new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 4 },
                    { new Guid("99763208-7007-4ced-947f-c87cc5a87ee8"), 0, "#1baf7a", "ti-wallet", true, "Income", new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 0 },
                    { new Guid("b65d18dd-d97a-460d-9d66-c5c1c8369757"), 1, "#e87ba4", "ti-heart", true, "Healthcare", new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 6 },
                    { new Guid("cd79f3e9-b67d-4a88-978f-6dc315b3b688"), 2, "#008300", "ti-piggy-bank", true, "Savings", new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 7 },
                    { new Guid("ff9a4543-7ce6-4dca-b999-df4b089369b2"), 1, "#1baf7a", "ti-shopping-cart", true, "Groceries", new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 2 }
                });

            migrationBuilder.InsertData(
                table: "tblCashFlowEntry",
                columns: new[] { "Id", "DayExpenses", "DayIncome", "EntryDate", "ProjectedBalance", "RunningBalance", "SharedBudgetId", "TransactionCount" },
                values: new object[,]
                {
                    { new Guid("0b7f4b18-acb3-4ab7-905c-e9492f848375"), 18m, 0m, new DateTime(2026, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 2800m, 3279m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 1 },
                    { new Guid("14113ed4-1cff-4792-b8df-eb9398f10519"), 62m, 0m, new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 3297m, 3297m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 1 },
                    { new Guid("7e6c26bc-9b50-47a4-94a5-046b9374e63b"), 127m, 0m, new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 3373m, 3373m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 1 },
                    { new Guid("9466fc40-06c7-4f1a-9dad-412a43116262"), 14m, 0m, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 3359m, 3359m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 1 },
                    { new Guid("a9264fdb-70e9-4058-a035-f55ce4889074"), 1500m, 5000m, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3500m, 3500m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 2 }
                });

            migrationBuilder.InsertData(
                table: "tblMonthlySnapshot",
                columns: new[] { "Id", "Month", "OverBudgetCategoryCount", "SharedBudgetId", "SnapshotDate", "TotalBudgeted", "TotalExpenses", "TotalIncome", "TotalSavings", "TransactionCount", "Year" },
                values: new object[,]
                {
                    { new Guid("0a2704d9-fd7a-40bd-82f9-66fd8bbd65f9"), 6, 2, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3100m, 2920m, 5000m, 500m, 38, 2025 },
                    { new Guid("51f24e9f-da2c-44fb-bfa6-ed26fcef8fb1"), 5, 1, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3100m, 2650m, 5000m, 500m, 42, 2025 }
                });

            migrationBuilder.InsertData(
                table: "tblTrackingInsight",
                columns: new[] { "Id", "Amount", "CategoryId", "ChangePercent", "GeneratedOn", "InsightType", "IsDismissed", "IsRead", "Message", "Month", "Severity", "SharedBudgetId", "Title", "Year" },
                values: new object[] { new Guid("9ac2c910-6758-4de3-a82c-c81e71773e71"), null, null, null, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), 7, false, false, "You have 2 transactions that haven't been assigned to a budget category.", 7, 1, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), "2 unassigned transactions", 2026 });

            migrationBuilder.InsertData(
                table: "tblUser",
                columns: new[] { "Id", "BudgetRole", "CurrencyCode", "DateRegistered", "Email", "FirstName", "LastLogin", "LastName", "Password", "SharedBudgetId", "TimeZone", "UserId" },
                values: new object[,]
                {
                    { new Guid("5ddab21a-0717-4cfa-a40c-ea59da059d0e"), 1, "USD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jamie@emksave.com", "Jamie", null, "Smith", "210000.X5yQzVjG+AyJg8dN1S88fw==.I/2k8kA+8VFRva9LwLxACnH0/ud3YIDynODIKqYQobY=", new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), "America/Chicago", "member" },
                    { new Guid("f3a19404-554e-4297-a043-189d327532c6"), 0, "USD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "alex@emksave.com", "Alex", null, "Demo", "210000.r27gfn+F0KYK/X3FURCZrg==.q56PYreQxT4JsNtHBdOB4LCfEZbJARPmH193+wdBV6M=", new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), "America/Chicago", "owner" }
                });

            migrationBuilder.InsertData(
                table: "tblBudget",
                columns: new[] { "Id", "CategoryId", "Month", "Notes", "PlannedAmount", "RolloverAmount", "SharedBudgetId", "Year" },
                values: new object[,]
                {
                    { new Guid("1414712e-5ffe-46b0-9928-d47937766ccf"), new Guid("ff9a4543-7ce6-4dca-b999-df4b089369b2"), 7, "", 400m, 0m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 2026 },
                    { new Guid("1d488bb4-8c78-4e2c-a3c3-104c8d1bbcd8"), new Guid("cd79f3e9-b67d-4a88-978f-6dc315b3b688"), 7, "", 500m, 0m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 2026 },
                    { new Guid("664a6f15-114f-4989-9115-7e7fd2f1adc9"), new Guid("99763208-7007-4ced-947f-c87cc5a87ee8"), 7, "Monthly salary", 5000m, 0m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 2026 },
                    { new Guid("6b308c14-59f2-483e-a1c6-9cf5695a1055"), new Guid("4802be14-6e75-4f32-9ef4-3219fb2d1381"), 7, "", 1500m, 0m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 2026 },
                    { new Guid("cef43024-460f-42ad-b2a1-8d2c8fe591be"), new Guid("088af96a-f35e-4ecf-a7bb-dcfe18e4491f"), 7, "", 200m, 0m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 2026 },
                    { new Guid("d4fdf597-8fda-4def-8a46-b2f405fca028"), new Guid("74dd7e95-eb7c-4032-ab14-80c624b4f7c3"), 7, "", 250m, 0m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 2026 },
                    { new Guid("eb7b5e5b-ace1-47db-b3bb-1274f0a6c6f2"), new Guid("b65d18dd-d97a-460d-9d66-c5c1c8369757"), 7, "", 150m, 0m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 2026 },
                    { new Guid("ee753c14-52ac-477d-b056-f7b62befaa00"), new Guid("273366f8-4856-4b80-aabd-0685e83c484c"), 7, "", 100m, 0m, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 2026 }
                });

            migrationBuilder.InsertData(
                table: "tblCategorySummary",
                columns: new[] { "Id", "ActualAmount", "CategoryId", "PlannedAmount", "SnapshotId", "TransactionCount" },
                values: new object[,]
                {
                    { new Guid("00814d89-e4ec-4483-adc5-1777f4d6b4b8"), 260m, new Guid("088af96a-f35e-4ecf-a7bb-dcfe18e4491f"), 200m, new Guid("0a2704d9-fd7a-40bd-82f9-66fd8bbd65f9"), 8 },
                    { new Guid("41dbcac4-5b17-4d48-a1dd-8ad82901d6dc"), 105m, new Guid("b65d18dd-d97a-460d-9d66-c5c1c8369757"), 150m, new Guid("0a2704d9-fd7a-40bd-82f9-66fd8bbd65f9"), 4 },
                    { new Guid("60190e01-32da-4a2e-89e7-0a17dfb6fba5"), 5000m, new Guid("99763208-7007-4ced-947f-c87cc5a87ee8"), 5000m, new Guid("0a2704d9-fd7a-40bd-82f9-66fd8bbd65f9"), 1 },
                    { new Guid("64ed7a0a-5de8-47bb-8a40-300a8afea265"), 300m, new Guid("cd79f3e9-b67d-4a88-978f-6dc315b3b688"), 500m, new Guid("0a2704d9-fd7a-40bd-82f9-66fd8bbd65f9"), 2 },
                    { new Guid("6aac8402-bc73-4a75-8b2e-abf08600a24b"), 85m, new Guid("273366f8-4856-4b80-aabd-0685e83c484c"), 100m, new Guid("0a2704d9-fd7a-40bd-82f9-66fd8bbd65f9"), 3 },
                    { new Guid("7f51b9a8-a594-4635-87bb-272569a11d96"), 290m, new Guid("74dd7e95-eb7c-4032-ab14-80c624b4f7c3"), 250m, new Guid("0a2704d9-fd7a-40bd-82f9-66fd8bbd65f9"), 6 },
                    { new Guid("808ef97f-b4c2-4f91-ac18-3a8ed618ad8a"), 380m, new Guid("ff9a4543-7ce6-4dca-b999-df4b089369b2"), 400m, new Guid("0a2704d9-fd7a-40bd-82f9-66fd8bbd65f9"), 5 },
                    { new Guid("f76f60e5-9c89-4f7f-a5bd-421c23e20d64"), 1500m, new Guid("4802be14-6e75-4f32-9ef4-3219fb2d1381"), 1500m, new Guid("0a2704d9-fd7a-40bd-82f9-66fd8bbd65f9"), 1 }
                });

            migrationBuilder.InsertData(
                table: "tblNotificationPreference",
                columns: new[] { "Id", "AuthKey", "IsPushEnabled", "LargeTransactionThreshold", "LastUpdated", "NotifyMonthlySummary", "NotifyOnBudgetOverage", "NotifyOnBudgetWarning", "NotifyOnLargeTransaction", "NotifyOnNewTransaction", "NotifyOnSyncError", "NotifyWeeklySummary", "P256dhKey", "PushEndpoint", "QuietHoursEnd", "QuietHoursStart", "UserId" },
                values: new object[,]
                {
                    { new Guid("41f9fa79-4380-4e3c-b99f-ca087846c6fe"), "", false, 200m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, true, false, false, true, true, false, "", "", new TimeOnly(7, 0, 0), new TimeOnly(22, 0, 0), new Guid("5ddab21a-0717-4cfa-a40c-ea59da059d0e") },
                    { new Guid("983f79ed-60f6-44b4-858b-74aec12fc14b"), "", false, 100m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, true, true, true, false, "", "", new TimeOnly(7, 0, 0), new TimeOnly(22, 0, 0), new Guid("f3a19404-554e-4297-a043-189d327532c6") }
                });

            migrationBuilder.InsertData(
                table: "tblPlaidAccount",
                columns: new[] { "Id", "AccessTokenEncrypted", "AccountName", "AccountSubtype", "AccountType", "AvailableBalance", "CurrentBalance", "DateLinked", "InstitutionLogoUrl", "InstitutionName", "IsActive", "IsoCurrencyCode", "LastSynced", "Mask", "PlaidAccountId", "PlaidItemId", "SharedBudgetId", "UserId" },
                values: new object[,]
                {
                    { new Guid("1f749ecc-0210-4cd6-8ade-a6f5151045aa"), "DEMO_ENCRYPTED_TOKEN", "Checking", "checking", "depository", 3150.00m, 3200.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "First National Bank", true, "USD", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "1234", "demo_chk_001", "demo_item_001", new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new Guid("f3a19404-554e-4297-a043-189d327532c6") },
                    { new Guid("38502936-10b3-476f-85cb-11659e6a076b"), "DEMO_ENCRYPTED_TOKEN", "Savings", "savings", "depository", 8500.00m, 8500.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "First National Bank", true, "USD", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "5678", "demo_sav_001", "demo_item_001", new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new Guid("f3a19404-554e-4297-a043-189d327532c6") }
                });

            migrationBuilder.InsertData(
                table: "tblPushNotification",
                columns: new[] { "Id", "ActionUrl", "Amount", "Body", "CategoryId", "ErrorMessage", "Icon", "IsRead", "NotificationType", "PushEndpoint", "ScheduledFor", "SentOn", "SharedBudgetId", "Status", "Title", "TransactionId", "UserId" },
                values: new object[,]
                {
                    { new Guid("56bb51db-c194-40ec-96cf-56da2e7bc3cf"), "/budget", 60m, "You've exceeded your Dining Out budget by $60.", new Guid("088af96a-f35e-4ecf-a7bb-dcfe18e4491f"), "", "/icons/icon-192.png", false, 1, "", new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 1, "Dining Out over budget!", null, new Guid("f3a19404-554e-4297-a043-189d327532c6") },
                    { new Guid("e4dd7062-3df5-4b8c-9874-293db0365446"), "/budget", 200m, "You've used 80% of your Transport budget.", new Guid("74dd7e95-eb7c-4032-ab14-80c624b4f7c3"), "", "/icons/icon-192.png", true, 2, "", new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 1, "Transport at 80%", null, new Guid("f3a19404-554e-4297-a043-189d327532c6") }
                });

            migrationBuilder.InsertData(
                table: "tblTrackingInsight",
                columns: new[] { "Id", "Amount", "CategoryId", "ChangePercent", "GeneratedOn", "InsightType", "IsDismissed", "IsRead", "Message", "Month", "Severity", "SharedBudgetId", "Title", "Year" },
                values: new object[,]
                {
                    { new Guid("5ec45efa-9211-4a7d-b1cb-cf455080fab5"), null, new Guid("ff9a4543-7ce6-4dca-b999-df4b089369b2"), -5.0, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), 1, false, false, "You're spending less on groceries than last month — great job!", 7, 1, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), "Groceries on track", 2026 },
                    { new Guid("e2457fd5-4f6d-46e2-b0a5-8ac1a71fdeaf"), 60m, new Guid("088af96a-f35e-4ecf-a7bb-dcfe18e4491f"), 30.0, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), 0, false, false, "You've spent $60 more than planned on dining this month.", 7, 2, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), "Dining Out over budget", 2026 }
                });

            migrationBuilder.InsertData(
                table: "tblTransaction",
                columns: new[] { "Id", "Amount", "CategoryId", "Description", "IsExcluded", "IsPending", "IsReviewed", "IsoCurrencyCode", "MerchantName", "Notes", "PlaidAccountId", "PlaidCategory", "PlaidSubcategory", "PlaidTransactionId", "PostedDate", "SharedBudgetId", "TransactionDate", "tblBudgetId" },
                values: new object[,]
                {
                    { new Guid("1dd49adb-9111-4143-9ddc-66be00ec2c89"), -127m, new Guid("ff9a4543-7ce6-4dca-b999-df4b089369b2"), "Groceries", false, false, true, "USD", "Whole Foods", "", new Guid("1f749ecc-0210-4cd6-8ade-a6f5151045aa"), "Food and Drink", "Supermarkets", "txn_002", null, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("2cf7c40b-90c4-4bd0-8bba-4ad5e4ac7260"), 5000m, new Guid("99763208-7007-4ced-947f-c87cc5a87ee8"), "Payroll", false, false, true, "USD", "Employer Direct Dep", "", new Guid("1f749ecc-0210-4cd6-8ade-a6f5151045aa"), "Payroll", "Payroll", "txn_006", null, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("3c06d38a-68f6-4309-9f7a-031eb6c0290b"), -18m, new Guid("273366f8-4856-4b80-aabd-0685e83c484c"), "Streaming sub", false, false, true, "USD", "Netflix", "", new Guid("1f749ecc-0210-4cd6-8ade-a6f5151045aa"), "Service", "Subscription", "txn_005", null, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2026, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("4d29c646-7305-45e7-bad4-fdfadb90bc60"), -89m, new Guid("ff9a4543-7ce6-4dca-b999-df4b089369b2"), "Groceries", false, false, false, "USD", "Trader Joe's", "", new Guid("1f749ecc-0210-4cd6-8ade-a6f5151045aa"), "Food and Drink", "Supermarkets", "txn_007", null, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("6af45b9d-c431-4fd4-aa6f-9c9568540085"), -1500m, new Guid("4802be14-6e75-4f32-9ef4-3219fb2d1381"), "Monthly rent", false, false, true, "USD", "Rent Payment", "", new Guid("1f749ecc-0210-4cd6-8ade-a6f5151045aa"), "Payment", "Rent", "txn_001", null, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("899a0a35-5edf-45d0-8830-49a5145f16ec"), -62m, new Guid("74dd7e95-eb7c-4032-ab14-80c624b4f7c3"), "Fuel", false, false, true, "USD", "Shell Gas Station", "", new Guid("1f749ecc-0210-4cd6-8ade-a6f5151045aa"), "Travel", "Gas Stations", "txn_004", null, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("8fd372eb-c8ce-4df0-8dcb-5107c408a00c"), -7m, null, "Coffee", false, true, false, "USD", "Starbucks", "", new Guid("1f749ecc-0210-4cd6-8ade-a6f5151045aa"), "Food and Drink", "Coffee Shop", "txn_009", null, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("b952159c-ca31-436b-b8e0-643181606487"), -38m, new Guid("b65d18dd-d97a-460d-9d66-c5c1c8369757"), "Pharmacy", false, false, false, "USD", "CVS Pharmacy", "", new Guid("1f749ecc-0210-4cd6-8ade-a6f5151045aa"), "Medical", "Pharmacies", "txn_010", null, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("bd4b9933-4ab3-4e43-bdb9-31a998689b80"), -43m, null, "Online purchase", false, false, false, "USD", "Amazon", "", new Guid("1f749ecc-0210-4cd6-8ade-a6f5151045aa"), "Shopping", "Online", "txn_008", null, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("cefe3de1-eaef-4e77-9a20-d53447c1df74"), -14m, new Guid("088af96a-f35e-4ecf-a7bb-dcfe18e4491f"), "Lunch", false, false, true, "USD", "Chipotle", "", new Guid("1f749ecc-0210-4cd6-8ade-a6f5151045aa"), "Food and Drink", "Restaurants", "txn_003", null, new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null }
                });

            migrationBuilder.InsertData(
                table: "tblPushNotification",
                columns: new[] { "Id", "ActionUrl", "Amount", "Body", "CategoryId", "ErrorMessage", "Icon", "IsRead", "NotificationType", "PushEndpoint", "ScheduledFor", "SentOn", "SharedBudgetId", "Status", "Title", "TransactionId", "UserId" },
                values: new object[] { new Guid("6b511dfd-878e-4df9-b5f9-d1ac6d1deab0"), "/transactions", 7m, "A $7.00 charge at Starbucks is pending review.", null, "", "/icons/icon-192.png", false, 0, "", new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Local), new Guid("e525f430-337e-4147-ae42-07fd8fdce182"), 1, "New transaction: Starbucks", new Guid("8fd372eb-c8ce-4df0-8dcb-5107c408a00c"), new Guid("f3a19404-554e-4297-a043-189d327532c6") });

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
