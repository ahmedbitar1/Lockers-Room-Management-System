using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerRoom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateLockerLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.CreateTable(
                name: "LockerLogs",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LockerID = table.Column<int>(type: "int", nullable: true),
                    BookingID = table.Column<int>(type: "int", nullable: true),
                    BraceletCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyID = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousIsActive = table.Column<bool>(type: "bit", nullable: true),
                    PreviousEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockerLogs", x => x.LogID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LockerLogs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Bookings");
        }
    }
}
