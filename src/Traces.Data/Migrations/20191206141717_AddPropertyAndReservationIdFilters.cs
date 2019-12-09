using Microsoft.EntityFrameworkCore.Migrations;

namespace Traces.Data.Migrations
{
    public partial class AddPropertyAndReservationIdFilters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"Trace\"");

            migrationBuilder.AddColumn<string>(
                name: "PropertyId",
                table: "Trace",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ReservationId",
                table: "Trace",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "Trace");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                table: "Trace");
        }
    }
}
