using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CCSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    accountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    rating = table.Column<double>(type: "float", nullable: true),
                    experience = table.Column<int>(type: "int", nullable: true),
                    fullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    avatar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    dateOfBirth = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Accounts__F267251E7190B3F8", x => x.accountId);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    categoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoryName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__23CAF1D80F210779", x => x.categoryId);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    discountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    discountPercent = table.Column<double>(type: "float", nullable: true),
                    startDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    minOrderAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    maxDiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Promotio__357D4CF873D84042", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    serviceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoryId = table.Column<int>(type: "int", nullable: false),
                    serviceName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Services__455070DF445A6B42", x => x.serviceId);
                    table.ForeignKey(
                        name: "FK_Services_Categories",
                        column: x => x.categoryId,
                        principalTable: "Categories",
                        principalColumn: "categoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    bookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customerId = table.Column<int>(type: "int", nullable: false),
                    promotionCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    bookingDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    totalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    bookingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    paymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    paymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bookings__C6D03BCD8FCCFE43", x => x.bookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Accounts",
                        column: x => x.customerId,
                        principalTable: "Accounts",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Promotions",
                        column: x => x.promotionCode,
                        principalTable: "Promotions",
                        principalColumn: "code",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDetails",
                columns: table => new
                {
                    serviceDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    serviceId = table.Column<int>(type: "int", nullable: true),
                    optionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    optionType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    basePrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    duration = table.Column<int>(type: "int", nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDetails", x => x.serviceDetailId);
                    table.ForeignKey(
                        name: "FK_ServiceDetails_Services",
                        column: x => x.serviceId,
                        principalTable: "Services",
                        principalColumn: "serviceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    paymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customerId = table.Column<int>(type: "int", nullable: false),
                    bookingId = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    paymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    paymentDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payments__A0D9EFC6454F04A1", x => x.paymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Accounts",
                        column: x => x.customerId,
                        principalTable: "Accounts",
                        principalColumn: "accountId");
                    table.ForeignKey(
                        name: "FK_Payments_Bookings",
                        column: x => x.bookingId,
                        principalTable: "Bookings",
                        principalColumn: "bookingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingDetails",
                columns: table => new
                {
                    detailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bookingId = table.Column<int>(type: "int", nullable: false),
                    serviceId = table.Column<int>(type: "int", nullable: false),
                    scheduleDate = table.Column<DateOnly>(type: "date", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    scheduleTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    serviceDetailId = table.Column<int>(type: "int", nullable: true),
                    isAssign = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BookingD__830778592CFC65D8", x => x.detailId);
                    table.ForeignKey(
                        name: "FK_BookingDetails_Bookings",
                        column: x => x.bookingId,
                        principalTable: "Bookings",
                        principalColumn: "bookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingDetails_ServiceDetails",
                        column: x => x.serviceDetailId,
                        principalTable: "ServiceDetails",
                        principalColumn: "serviceDetailId");
                    table.ForeignKey(
                        name: "FK_BookingDetails_Services",
                        column: x => x.serviceId,
                        principalTable: "Services",
                        principalColumn: "serviceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    reviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customerId = table.Column<int>(type: "int", nullable: false),
                    detailId = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    reviewDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reviews__2ECD6E040E6613F9", x => x.reviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_Accounts",
                        column: x => x.customerId,
                        principalTable: "Accounts",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_BookingDetails",
                        column: x => x.detailId,
                        principalTable: "BookingDetails",
                        principalColumn: "detailId");
                });

            migrationBuilder.CreateTable(
                name: "ScheduleAssignments",
                columns: table => new
                {
                    assignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    housekeeperId = table.Column<int>(type: "int", nullable: false),
                    detailId = table.Column<int>(type: "int", nullable: false),
                    assignDate = table.Column<DateOnly>(type: "date", nullable: false),
                    startTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    endTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Schedule__52C218205FA00C0B", x => x.assignmentId);
                    table.ForeignKey(
                        name: "FK_ScheduleAssignments_Accounts",
                        column: x => x.housekeeperId,
                        principalTable: "Accounts",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleAssignments_BookingDetails",
                        column: x => x.detailId,
                        principalTable: "BookingDetails",
                        principalColumn: "detailId");
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    recordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    housekeeperId = table.Column<int>(type: "int", nullable: false),
                    assignId = table.Column<int>(type: "int", nullable: false),
                    workDate = table.Column<DateOnly>(type: "date", nullable: false),
                    startTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    endTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    totalHours = table.Column<double>(type: "float", nullable: false),
                    taskStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reports__D825195EFB393F9D", x => x.recordId);
                    table.ForeignKey(
                        name: "FK_Reports_Accounts",
                        column: x => x.housekeeperId,
                        principalTable: "Accounts",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_ScheduleAssignments",
                        column: x => x.assignId,
                        principalTable: "ScheduleAssignments",
                        principalColumn: "assignmentId");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Accounts__AB6E6164FFDE7117",
                table: "Accounts",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_bookingId",
                table: "BookingDetails",
                column: "bookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_serviceDetailId",
                table: "BookingDetails",
                column: "serviceDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_serviceId",
                table: "BookingDetails",
                column: "serviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_customerId",
                table: "Bookings",
                column: "customerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_promotionCode",
                table: "Bookings",
                column: "promotionCode");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_bookingId",
                table: "Payments",
                column: "bookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_customerId",
                table: "Payments",
                column: "customerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_assignId",
                table: "Reports",
                column: "assignId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_housekeeperId",
                table: "Reports",
                column: "housekeeperId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_customerId",
                table: "Reviews",
                column: "customerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_detailId",
                table: "Reviews",
                column: "detailId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleAssignments_detailId",
                table: "ScheduleAssignments",
                column: "detailId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleAssignments_housekeeperId",
                table: "ScheduleAssignments",
                column: "housekeeperId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetails_serviceId",
                table: "ServiceDetails",
                column: "serviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_categoryId",
                table: "Services",
                column: "categoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "ScheduleAssignments");

            migrationBuilder.DropTable(
                name: "BookingDetails");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "ServiceDetails");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
