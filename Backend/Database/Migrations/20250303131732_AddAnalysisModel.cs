using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AnalysisStatus = table.Column<int>(type: "integer", nullable: false),
                    AnalyzerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analyses_Analyzers_AnalyzerId",
                        column: x => x.AnalyzerId,
                        principalTable: "Analyzers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnalysisId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryAnalyses_Analyses_AnalysisId",
                        column: x => x.AnalysisId,
                        principalTable: "Analyses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeliveryAnalyses_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_AnalyzerId",
                table: "Analyses",
                column: "AnalyzerId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAnalyses_AnalysisId",
                table: "DeliveryAnalyses",
                column: "AnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAnalyses_DeliveryId",
                table: "DeliveryAnalyses",
                column: "DeliveryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryAnalyses");

            migrationBuilder.DropTable(
                name: "Analyses");
        }
    }
}
