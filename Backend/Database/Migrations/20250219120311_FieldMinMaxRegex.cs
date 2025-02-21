using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class FieldMinMaxRegex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RangeMin",
                table: "AssignmentFields",
                newName: "Min");

            migrationBuilder.RenameColumn(
                name: "RangeMax",
                table: "AssignmentFields",
                newName: "Max");

            migrationBuilder.AddColumn<string>(
                name: "Regex",
                table: "AssignmentFields",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Regex",
                table: "AssignmentFields");

            migrationBuilder.RenameColumn(
                name: "Min",
                table: "AssignmentFields",
                newName: "RangeMin");

            migrationBuilder.RenameColumn(
                name: "Max",
                table: "AssignmentFields",
                newName: "RangeMax");
        }
    }
}
