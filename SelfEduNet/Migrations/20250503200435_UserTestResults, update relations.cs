using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SelfEduNet.Migrations
{
    /// <inheritdoc />
    public partial class UserTestResultsupdaterelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserSteps",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserTestResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StepTestId = table.Column<int>(type: "integer", nullable: false),
                    UserStepId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    StepId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    TotalScore = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AttemptsCount = table.Column<int>(type: "integer", nullable: false),
                    BiggestScore = table.Column<string>(type: "text", nullable: false),
                    IsPassed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTestResults_StepTests_StepTestId",
                        column: x => x.StepTestId,
                        principalTable: "StepTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTestResults_UserSteps_UserId_StepId",
                        columns: x => new { x.UserId, x.StepId },
                        principalTable: "UserSteps",
                        principalColumns: new[] { "UserId", "StepId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTestResults_StepTestId",
                table: "UserTestResults",
                column: "StepTestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTestResults_UserId_StepId",
                table: "UserTestResults",
                columns: new[] { "UserId", "StepId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTestResults");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserSteps");
        }
    }
}
