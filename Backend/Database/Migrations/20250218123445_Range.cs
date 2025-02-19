using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Range : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RangeMax",
                table: "AssignmentFields",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RangeMin",
                table: "AssignmentFields",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RangeMax",
                table: "AssignmentFields");

            migrationBuilder.DropColumn(
                name: "RangeMin",
                table: "AssignmentFields");
        }
    }
}
