using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfEduNet.Migrations
{
    /// <inheritdoc />
    public partial class Addcompetedtousercourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "UserCourses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "UserCourses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "UserCourses");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "UserCourses");
        }
    }
}
