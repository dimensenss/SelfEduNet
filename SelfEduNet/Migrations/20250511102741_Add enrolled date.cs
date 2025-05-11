using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfEduNet.Migrations
{
    /// <inheritdoc />
    public partial class Addenrolleddate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EnrolledAt",
                table: "UserCourses",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnrolledAt",
                table: "UserCourses");
        }
    }
}
