using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovedOwnedGradeFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GradingFormat_MaxPoints",
                table: "Assignments",
                newName: "MaxPoints");

            migrationBuilder.RenameColumn(
                name: "GradingFormat_GradingType",
                table: "Assignments",
                newName: "GradingType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxPoints",
                table: "Assignments",
                newName: "GradingFormat_MaxPoints");

            migrationBuilder.RenameColumn(
                name: "GradingType",
                table: "Assignments",
                newName: "GradingFormat_GradingType");
        }
    }
}
