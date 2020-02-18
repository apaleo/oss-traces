using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Traces.Data.Migrations
{
    public partial class CreateFileTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TraceFile",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo),
                    CreatedUtc = table.Column<Instant>(nullable: false),
                    UpdatedUtc = table.Column<Instant>(nullable: false),
                    TenantId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    MimeType = table.Column<string>(nullable: false),
                    Size = table.Column<int>(nullable: false),
                    PublicId = table.Column<Guid>(nullable: false),
                    Path = table.Column<string>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: false),
                    TraceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraceFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraceFile_Trace_TraceId",
                        column: x => x.TraceId,
                        principalTable: "Trace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TraceFile_TraceId",
                table: "TraceFile",
                column: "TraceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TraceFile");
        }
    }
}
