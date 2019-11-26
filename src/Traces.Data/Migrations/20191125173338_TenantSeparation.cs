using Microsoft.EntityFrameworkCore.Migrations;

namespace Traces.Data.Migrations
{
    public partial class TenantSeparation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletedBy",
                table: "Trace",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Trace",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedBy",
                table: "Trace");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Trace");
        }
    }
}
