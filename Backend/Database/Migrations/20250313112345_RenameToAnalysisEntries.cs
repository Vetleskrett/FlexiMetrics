using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameToAnalysisEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisFields_DeliveryAnalyses_AnalysisEntryId",
                table: "AnalysisFields");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAnalyses_Analyses_AnalysisId",
                table: "DeliveryAnalyses");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAnalyses_Teams_TeamId",
                table: "DeliveryAnalyses");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAnalyses_Users_StudentId",
                table: "DeliveryAnalyses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryAnalyses",
                table: "DeliveryAnalyses");

            migrationBuilder.RenameTable(
                name: "DeliveryAnalyses",
                newName: "AnalysisEntries");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAnalyses_TeamId",
                table: "AnalysisEntries",
                newName: "IX_AnalysisEntries_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAnalyses_StudentId",
                table: "AnalysisEntries",
                newName: "IX_AnalysisEntries_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAnalyses_AnalysisId",
                table: "AnalysisEntries",
                newName: "IX_AnalysisEntries_AnalysisId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnalysisEntries",
                table: "AnalysisEntries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisEntries_Analyses_AnalysisId",
                table: "AnalysisEntries",
                column: "AnalysisId",
                principalTable: "Analyses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisEntries_Teams_TeamId",
                table: "AnalysisEntries",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisEntries_Users_StudentId",
                table: "AnalysisEntries",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisFields_AnalysisEntries_AnalysisEntryId",
                table: "AnalysisFields",
                column: "AnalysisEntryId",
                principalTable: "AnalysisEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisEntries_Analyses_AnalysisId",
                table: "AnalysisEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisEntries_Teams_TeamId",
                table: "AnalysisEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisEntries_Users_StudentId",
                table: "AnalysisEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisFields_AnalysisEntries_AnalysisEntryId",
                table: "AnalysisFields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnalysisEntries",
                table: "AnalysisEntries");

            migrationBuilder.RenameTable(
                name: "AnalysisEntries",
                newName: "DeliveryAnalyses");

            migrationBuilder.RenameIndex(
                name: "IX_AnalysisEntries_TeamId",
                table: "DeliveryAnalyses",
                newName: "IX_DeliveryAnalyses_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_AnalysisEntries_StudentId",
                table: "DeliveryAnalyses",
                newName: "IX_DeliveryAnalyses_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_AnalysisEntries_AnalysisId",
                table: "DeliveryAnalyses",
                newName: "IX_DeliveryAnalyses_AnalysisId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryAnalyses",
                table: "DeliveryAnalyses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisFields_DeliveryAnalyses_AnalysisEntryId",
                table: "AnalysisFields",
                column: "AnalysisEntryId",
                principalTable: "DeliveryAnalyses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAnalyses_Analyses_AnalysisId",
                table: "DeliveryAnalyses",
                column: "AnalysisId",
                principalTable: "Analyses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
