using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAnalyses_Analyses_AnalysisId",
                table: "DeliveryAnalyses");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAnalysisFields_DeliveryAnalyses_DeliveryAnalysisId",
                table: "DeliveryAnalysisFields");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeliveryAnalysisId",
                table: "DeliveryAnalysisFields",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AnalysisId",
                table: "DeliveryAnalyses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAnalyses_Analyses_AnalysisId",
                table: "DeliveryAnalyses",
                column: "AnalysisId",
                principalTable: "Analyses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAnalysisFields_DeliveryAnalyses_DeliveryAnalysisId",
                table: "DeliveryAnalysisFields",
                column: "DeliveryAnalysisId",
                principalTable: "DeliveryAnalyses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAnalyses_Analyses_AnalysisId",
                table: "DeliveryAnalyses");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAnalysisFields_DeliveryAnalyses_DeliveryAnalysisId",
                table: "DeliveryAnalysisFields");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeliveryAnalysisId",
                table: "DeliveryAnalysisFields",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "AnalysisId",
                table: "DeliveryAnalyses",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAnalyses_Analyses_AnalysisId",
                table: "DeliveryAnalyses",
                column: "AnalysisId",
                principalTable: "Analyses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAnalysisFields_DeliveryAnalyses_DeliveryAnalysisId",
                table: "DeliveryAnalysisFields",
                column: "DeliveryAnalysisId",
                principalTable: "DeliveryAnalyses",
                principalColumn: "Id");
        }
    }
}
