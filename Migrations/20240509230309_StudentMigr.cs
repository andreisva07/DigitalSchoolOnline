using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAPI.Migrations
{
    public partial class StudentMigr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Classes_ClassId",
                table: "Student");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "Student",
                newName: "ClassID");

            migrationBuilder.RenameIndex(
                name: "IX_Student_ClassId",
                table: "Student",
                newName: "IX_Student_ClassID");

            migrationBuilder.AlterColumn<int>(
                name: "ClassID",
                table: "Student",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Student",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Classes_ClassID",
                table: "Student",
                column: "ClassID",
                principalTable: "Classes",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Classes_ClassID",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Student");

            migrationBuilder.RenameColumn(
                name: "ClassID",
                table: "Student",
                newName: "ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_Student_ClassID",
                table: "Student",
                newName: "IX_Student_ClassId");

            migrationBuilder.AlterColumn<int>(
                name: "ClassId",
                table: "Student",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Classes_ClassId",
                table: "Student",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "ClassId");
        }
    }
}
