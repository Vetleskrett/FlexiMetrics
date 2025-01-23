using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryFieldJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "DeliveryFields");

            migrationBuilder.AddColumn<JsonDocument>(
                name: "JsonValue",
                table: "DeliveryFields",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonValue",
                table: "DeliveryFields");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "DeliveryFields",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
