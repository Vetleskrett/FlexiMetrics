using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class CourseYearSemester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Courses",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "Semester",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Semester",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Courses",
                newName: "Title");
        }
    }
}
