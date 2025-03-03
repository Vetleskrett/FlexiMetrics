using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryAnalysisFieldModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryAnalysisFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    JsonValue = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    DeliveryAnalysisId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAnalysisFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryAnalysisFields_DeliveryAnalyses_DeliveryAnalysisId",
                        column: x => x.DeliveryAnalysisId,
                        principalTable: "DeliveryAnalyses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAnalysisFields_DeliveryAnalysisId",
                table: "DeliveryAnalysisFields",
                column: "DeliveryAnalysisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryAnalysisFields");
        }
    }
}
