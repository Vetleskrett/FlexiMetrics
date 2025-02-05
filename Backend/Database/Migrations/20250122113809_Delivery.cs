using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Delivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deliveries_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deliveries_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deliveries_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentFieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryFields_AssignmentFields_AssignmentFieldId",
                        column: x => x.AssignmentFieldId,
                        principalTable: "AssignmentFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryFields_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_StudentId",
                table: "Deliveries",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_TeamId",
                table: "Deliveries",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryFields_AssignmentFieldId",
                table: "DeliveryFields",
                column: "AssignmentFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryFields_DeliveryId",
                table: "DeliveryFields",
                column: "DeliveryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryFields");

            migrationBuilder.DropTable(
                name: "Deliveries");
        }
    }
}
