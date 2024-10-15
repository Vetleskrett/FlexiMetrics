using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Assignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teams_TeamId",
                table: "Teams");

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentVariables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentVariables", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TeamId_CourseId",
                table: "Teams",
                columns: new[] { "TeamId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_Name_CourseId",
                table: "Assignments",
                columns: new[] { "Name", "CourseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "AssignmentVariables");

            migrationBuilder.DropIndex(
                name: "IX_Teams_TeamId_CourseId",
                table: "Teams");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TeamId",
                table: "Teams",
                column: "TeamId",
                unique: true);
        }
    }
}
