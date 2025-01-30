using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class GradingType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Assignments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GradingFormat_GradingType",
                table: "Assignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GradingFormat_MaxPoints",
                table: "Assignments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Mandatory",
                table: "Assignments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "GradingFormat_GradingType",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "GradingFormat_MaxPoints",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Mandatory",
                table: "Assignments");
        }
    }
}
