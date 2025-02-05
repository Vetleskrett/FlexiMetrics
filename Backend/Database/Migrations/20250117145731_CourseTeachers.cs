using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class CourseTeachers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseUser_Users_TeachersId",
                table: "CourseUser");

            migrationBuilder.DropTable(
                name: "CourseUser1");

            migrationBuilder.RenameColumn(
                name: "TeachersId",
                table: "CourseUser",
                newName: "StudentsId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseUser_TeachersId",
                table: "CourseUser",
                newName: "IX_CourseUser_StudentsId");

            migrationBuilder.CreateTable(
                name: "CourseTeachers",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTeachers", x => new { x.CourseId, x.TeacherId });
                    table.ForeignKey(
                        name: "FK_CourseTeachers_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseTeachers_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachers_TeacherId",
                table: "CourseTeachers",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUser_Users_StudentsId",
                table: "CourseUser",
                column: "StudentsId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseUser_Users_StudentsId",
                table: "CourseUser");

            migrationBuilder.DropTable(
                name: "CourseTeachers");

            migrationBuilder.RenameColumn(
                name: "StudentsId",
                table: "CourseUser",
                newName: "TeachersId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseUser_StudentsId",
                table: "CourseUser",
                newName: "IX_CourseUser_TeachersId");

            migrationBuilder.CreateTable(
                name: "CourseUser1",
                columns: table => new
                {
                    Course1Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseUser1", x => new { x.Course1Id, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_CourseUser1_Courses_Course1Id",
                        column: x => x.Course1Id,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseUser1_Users_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseUser1_StudentsId",
                table: "CourseUser1",
                column: "StudentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUser_Users_TeachersId",
                table: "CourseUser",
                column: "TeachersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
