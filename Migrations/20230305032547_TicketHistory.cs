using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugBanisher.Migrations
{
    public partial class TicketHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketHistories_AspNetUsers_UserId",
                table: "TicketHistories");

            migrationBuilder.DropIndex(
                name: "IX_TicketHistories_UserId",
                table: "TicketHistories");

            migrationBuilder.DeleteData(
                table: "TicketPriorities",
                keyColumn: "Id",
                keyValue: "veryHigh");

            migrationBuilder.DeleteData(
                table: "TicketPriorities",
                keyColumn: "Id",
                keyValue: "veryLow");

            migrationBuilder.DropColumn(
                name: "NewValue",
                table: "TicketHistories");

            migrationBuilder.DropColumn(
                name: "OldValue",
                table: "TicketHistories");

            migrationBuilder.DropColumn(
                name: "Property",
                table: "TicketHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TicketHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TicketHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "TicketHistories",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "TicketHistories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Deadline" },
                values: new object[] { new DateTime(2023, 3, 4, 19, 25, 47, 26, DateTimeKind.Local).AddTicks(5875), new DateTime(2023, 8, 31, 19, 25, 47, 26, DateTimeKind.Local).AddTicks(5904) });

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistories_AppUserId",
                table: "TicketHistories",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketHistories_AspNetUsers_AppUserId",
                table: "TicketHistories",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketHistories_AspNetUsers_AppUserId",
                table: "TicketHistories");

            migrationBuilder.DropIndex(
                name: "IX_TicketHistories_AppUserId",
                table: "TicketHistories");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "TicketHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TicketHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Created",
                table: "TicketHistories",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                table: "TicketHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValue",
                table: "TicketHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Property",
                table: "TicketHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TicketHistories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Deadline" },
                values: new object[] { new DateTime(2023, 2, 12, 22, 40, 55, 814, DateTimeKind.Local).AddTicks(3625), new DateTime(2023, 8, 11, 22, 40, 55, 814, DateTimeKind.Local).AddTicks(3651) });

            migrationBuilder.InsertData(
                table: "TicketPriorities",
                columns: new[] { "Id", "Description" },
                values: new object[] { "veryHigh", "Emergency" });

            migrationBuilder.InsertData(
                table: "TicketPriorities",
                columns: new[] { "Id", "Description" },
                values: new object[] { "veryLow", "If time allows" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistories_UserId",
                table: "TicketHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketHistories_AspNetUsers_UserId",
                table: "TicketHistories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
