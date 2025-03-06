using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CCSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddBookDetailStatusToBookingDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bookdetailStatus",
                table: "BookingDetails",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bookdetailStatus",
                table: "BookingDetails");
        }
    }
}
