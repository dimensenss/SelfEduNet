using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfEduNet.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseInfoAuthorsRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CourseInfos_CourseInfoId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CourseInfoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CourseInfoId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "CourseInfoAuthor",
                columns: table => new
                {
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseInfoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseInfoAuthor", x => new { x.AuthorId, x.CourseInfoId });
                    table.ForeignKey(
                        name: "FK_CourseInfoAuthor_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseInfoAuthor_CourseInfos_CourseInfoId",
                        column: x => x.CourseInfoId,
                        principalTable: "CourseInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseInfoAuthor_CourseInfoId",
                table: "CourseInfoAuthor",
                column: "CourseInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseInfoAuthor");

            migrationBuilder.AddColumn<int>(
                name: "CourseInfoId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CourseInfoId",
                table: "AspNetUsers",
                column: "CourseInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CourseInfos_CourseInfoId",
                table: "AspNetUsers",
                column: "CourseInfoId",
                principalTable: "CourseInfos",
                principalColumn: "Id");
        }
    }
}
