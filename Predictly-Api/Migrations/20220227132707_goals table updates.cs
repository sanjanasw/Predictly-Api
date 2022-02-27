using Microsoft.EntityFrameworkCore.Migrations;

namespace Predictly_Api.Migrations
{
    public partial class goalstableupdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Goals",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Sub1Goal",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Sub2Goal",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Sub3Goal",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Sub4Goal",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Sub5Goal",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Sub6Goal",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Sub7Goal",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Sub8Goal",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Sub9Goal",
                table: "Goals");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Goals",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Goals",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Goal",
                table: "Goals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Goals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Goals",
                table: "Goals",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Goals",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Goal",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Goals");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Goals",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Sub1Goal",
                table: "Goals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sub2Goal",
                table: "Goals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sub3Goal",
                table: "Goals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sub4Goal",
                table: "Goals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sub5Goal",
                table: "Goals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sub6Goal",
                table: "Goals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sub7Goal",
                table: "Goals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sub8Goal",
                table: "Goals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sub9Goal",
                table: "Goals",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Goals",
                table: "Goals",
                column: "UserId");
        }
    }
}
