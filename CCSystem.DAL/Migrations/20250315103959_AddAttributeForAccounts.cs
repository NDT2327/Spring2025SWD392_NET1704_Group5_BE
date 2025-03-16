using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CCSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAttributeForAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bankAccountNumber",
                table: "Accounts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bankName",
                table: "Accounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bankAccountNumber",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "bankName",
                table: "Accounts");
        }
    }
}
