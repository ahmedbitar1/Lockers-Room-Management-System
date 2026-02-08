using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerRoom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPOSCheckNoToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "POSCheckNo",
                table: "Bookings",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "POSCheckNo",
                table: "Bookings");
        }
    }
}
