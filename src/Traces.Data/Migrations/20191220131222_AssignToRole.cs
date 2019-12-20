using Microsoft.EntityFrameworkCore.Migrations;

namespace Traces.Data.Migrations
{
    public partial class AssignToRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedRole",
                table: "Trace",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedRole",
                table: "Trace");
        }
    }
}
