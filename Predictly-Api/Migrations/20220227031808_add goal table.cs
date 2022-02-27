using Microsoft.EntityFrameworkCore.Migrations;

namespace Predictly_Api.Migrations
{
    public partial class addgoaltable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sub1Goal = table.Column<int>(type: "int", nullable: false),
                    Sub2Goal = table.Column<int>(type: "int", nullable: false),
                    Sub3Goal = table.Column<int>(type: "int", nullable: false),
                    Sub4Goal = table.Column<int>(type: "int", nullable: false),
                    Sub5Goal = table.Column<int>(type: "int", nullable: false),
                    Sub6Goal = table.Column<int>(type: "int", nullable: false),
                    Sub7Goal = table.Column<int>(type: "int", nullable: false),
                    Sub8Goal = table.Column<int>(type: "int", nullable: false),
                    Sub9Goal = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goals");
        }
    }
}
