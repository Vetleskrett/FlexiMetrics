using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Feedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Teams_TeamId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Users_StudentId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_AssignmentId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_AssignmentId_StudentId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_AssignmentId_TeamId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Deliveries");

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    DeliveryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: true),
                    LetterGrade = table.Column<int>(type: "integer", nullable: true),
                    Points = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_AssignmentId_StudentId_TeamId",
                table: "Deliveries",
                columns: new[] { "AssignmentId", "StudentId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_DeliveryId",
                table: "Feedbacks",
                column: "DeliveryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Teams_TeamId",
                table: "Deliveries",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Users_StudentId",
                table: "Deliveries",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Teams_TeamId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Users_StudentId",
                table: "Deliveries");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_AssignmentId_StudentId_TeamId",
                table: "Deliveries");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Deliveries",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_AssignmentId",
                table: "Deliveries",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_AssignmentId_StudentId",
                table: "Deliveries",
                columns: new[] { "AssignmentId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_AssignmentId_TeamId",
                table: "Deliveries",
                columns: new[] { "AssignmentId", "TeamId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Teams_TeamId",
                table: "Deliveries",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Users_StudentId",
                table: "Deliveries",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
