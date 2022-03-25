using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Predictly_Api.Migrations
{
    public partial class addpredictedresulttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PredictedResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    A = table.Column<double>(type: "float", nullable: false),
                    B = table.Column<double>(type: "float", nullable: false),
                    C = table.Column<double>(type: "float", nullable: false),
                    S = table.Column<double>(type: "float", nullable: false),
                    W = table.Column<double>(type: "float", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictedResults", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PredictedResults");
        }
    }
}
