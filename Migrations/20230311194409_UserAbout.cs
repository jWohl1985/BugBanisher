using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugBanisher.Migrations
{
    public partial class UserAbout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Deadline" },
                values: new object[] { new DateTime(2023, 3, 11, 11, 44, 8, 876, DateTimeKind.Local).AddTicks(7283), new DateTime(2023, 9, 7, 11, 44, 8, 876, DateTimeKind.Local).AddTicks(7308) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "About",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Deadline" },
                values: new object[] { new DateTime(2023, 3, 4, 19, 25, 47, 26, DateTimeKind.Local).AddTicks(5875), new DateTime(2023, 8, 31, 19, 25, 47, 26, DateTimeKind.Local).AddTicks(5904) });
        }
    }
}
