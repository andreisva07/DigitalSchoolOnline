using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAPI.Migrations
{
    public partial class ClassMigr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Class_ClassId",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClass_Class_ClassId",
                table: "TeacherClass");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClass_Teachers_TeacherId",
                table: "TeacherClass");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherClass",
                table: "TeacherClass");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Class",
                table: "Class");

            migrationBuilder.RenameTable(
                name: "TeacherClass",
                newName: "TeacherClasses");

            migrationBuilder.RenameTable(
                name: "Class",
                newName: "Classes");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherClass_ClassId",
                table: "TeacherClasses",
                newName: "IX_TeacherClasses_ClassId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Classes",
                newName: "Series");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherClasses",
                table: "TeacherClasses",
                columns: new[] { "TeacherId", "ClassId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classes",
                table: "Classes",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Classes_ClassId",
                table: "Student",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClasses_Classes_ClassId",
                table: "TeacherClasses",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClasses_Teachers_TeacherId",
                table: "TeacherClasses",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Classes_ClassId",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClasses_Classes_ClassId",
                table: "TeacherClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClasses_Teachers_TeacherId",
                table: "TeacherClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherClasses",
                table: "TeacherClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classes",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Classes");

            migrationBuilder.RenameTable(
                name: "TeacherClasses",
                newName: "TeacherClass");

            migrationBuilder.RenameTable(
                name: "Classes",
                newName: "Class");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherClasses_ClassId",
                table: "TeacherClass",
                newName: "IX_TeacherClass_ClassId");

            migrationBuilder.RenameColumn(
                name: "Series",
                table: "Class",
                newName: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherClass",
                table: "TeacherClass",
                columns: new[] { "TeacherId", "ClassId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Class",
                table: "Class",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Class_ClassId",
                table: "Student",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClass_Class_ClassId",
                table: "TeacherClass",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClass_Teachers_TeacherId",
                table: "TeacherClass",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
