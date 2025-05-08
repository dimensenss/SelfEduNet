using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfEduNet.Migrations
{
    /// <inheritdoc />
    public partial class AddIsEnrolledtoUserCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnrolled",
                table: "UserCourses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnrolled",
                table: "UserCourses");
        }
    }
}
