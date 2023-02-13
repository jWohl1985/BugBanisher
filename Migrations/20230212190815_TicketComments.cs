using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugBanisher.Migrations
{
    public partial class TicketComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "TicketComment",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "TicketAttachment",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Deadline" },
                values: new object[] { new DateTime(2023, 2, 12, 11, 8, 15, 10, DateTimeKind.Local).AddTicks(6758), new DateTime(2023, 8, 11, 11, 8, 15, 10, DateTimeKind.Local).AddTicks(6785) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Created",
                table: "TicketComment",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Created",
                table: "TicketAttachment",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Deadline" },
                values: new object[] { new DateTime(2023, 2, 9, 22, 3, 23, 781, DateTimeKind.Local).AddTicks(8422), new DateTime(2023, 8, 8, 22, 3, 23, 781, DateTimeKind.Local).AddTicks(8451) });
        }
    }
}
