using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAnalyzerState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogError",
                table: "AnalysisEntries");

            migrationBuilder.DropColumn(
                name: "LogInformation",
                table: "AnalysisEntries");

            migrationBuilder.DropColumn(
                name: "TotalNumEntries",
                table: "Analyses");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Analyzers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "AnalysisEntries",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Analyzers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "AnalysisEntries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogError",
                table: "AnalysisEntries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LogInformation",
                table: "AnalysisEntries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalNumEntries",
                table: "Analyses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
