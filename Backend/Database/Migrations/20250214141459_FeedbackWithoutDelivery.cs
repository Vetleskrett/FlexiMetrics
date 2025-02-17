using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackWithoutDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Deliveries_DeliveryId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_DeliveryId",
                table: "Feedbacks");

            migrationBuilder.RenameColumn(
                name: "DeliveryId",
                table: "Feedbacks",
                newName: "AssignmentId");

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "Feedbacks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "Feedbacks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_AssignmentId_StudentId_TeamId",
                table: "Feedbacks",
                columns: new[] { "AssignmentId", "StudentId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_StudentId",
                table: "Feedbacks",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_TeamId",
                table: "Feedbacks",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Assignments_AssignmentId",
                table: "Feedbacks",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Teams_TeamId",
                table: "Feedbacks",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_StudentId",
                table: "Feedbacks",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Assignments_AssignmentId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Teams_TeamId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_StudentId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_AssignmentId_StudentId_TeamId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_StudentId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_TeamId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Feedbacks");

            migrationBuilder.RenameColumn(
                name: "AssignmentId",
                table: "Feedbacks",
                newName: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_DeliveryId",
                table: "Feedbacks",
                column: "DeliveryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Deliveries_DeliveryId",
                table: "Feedbacks",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
