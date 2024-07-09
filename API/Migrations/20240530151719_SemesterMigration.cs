using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAPI.Migrations
{
    public partial class SemesterMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Grades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Attendances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Semesters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semesters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Grades_SemesterId",
                table: "Grades",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SemesterId",
                table: "Attendances",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Semesters_SemesterId",
                table: "Attendances",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Semesters_SemesterId",
                table: "Grades",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Semesters_SemesterId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Semesters_SemesterId",
                table: "Grades");

            migrationBuilder.DropTable(
                name: "Semesters");

            migrationBuilder.DropIndex(
                name: "IX_Grades_SemesterId",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_SemesterId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Attendances");
        }
    }
}
