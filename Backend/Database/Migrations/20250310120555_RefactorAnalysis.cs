using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAnalyses_Deliveries_DeliveryId",
                table: "DeliveryAnalyses");

            migrationBuilder.DropTable(
                name: "DeliveryAnalysisFields");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryAnalyses_DeliveryId",
                table: "DeliveryAnalyses");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "DeliveryAnalyses");

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "DeliveryAnalyses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "DeliveryAnalyses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AnalysisFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnalysisEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    JsonValue = table.Column<JsonDocument>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisFields_DeliveryAnalyses_AnalysisEntryId",
                        column: x => x.AnalysisEntryId,
                        principalTable: "DeliveryAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAnalyses_StudentId",
                table: "DeliveryAnalyses",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAnalyses_TeamId",
                table: "DeliveryAnalyses",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisFields_AnalysisEntryId",
                table: "AnalysisFields",
                column: "AnalysisEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAnalyses_Teams_TeamId",
                table: "DeliveryAnalyses",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAnalyses_Users_StudentId",
                table: "DeliveryAnalyses",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAnalyses_Teams_TeamId",
                table: "DeliveryAnalyses");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAnalyses_Users_StudentId",
                table: "DeliveryAnalyses");

            migrationBuilder.DropTable(
                name: "AnalysisFields");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryAnalyses_StudentId",
                table: "DeliveryAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryAnalyses_TeamId",
                table: "DeliveryAnalyses");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "DeliveryAnalyses");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "DeliveryAnalyses");

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryId",
                table: "DeliveryAnalyses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "DeliveryAnalysisFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    JsonValue = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAnalysisFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryAnalysisFields_DeliveryAnalyses_DeliveryAnalysisId",
                        column: x => x.DeliveryAnalysisId,
                        principalTable: "DeliveryAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAnalyses_DeliveryId",
                table: "DeliveryAnalyses",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAnalysisFields_DeliveryAnalysisId",
                table: "DeliveryAnalysisFields",
                column: "DeliveryAnalysisId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAnalyses_Deliveries_DeliveryId",
                table: "DeliveryAnalyses",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
