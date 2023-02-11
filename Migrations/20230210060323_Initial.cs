using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugBanisher.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketPriorities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketPriorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketStatuses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PictureExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PictureData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ProjectManagerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PictureData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PictureExtension = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    NotificationTypeId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasBeenSeen = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUsers",
                columns: table => new
                {
                    ProjectsId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUsers", x => new { x.ProjectsId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_ProjectUsers_AspNetUsers_TeamId",
                        column: x => x.TeamId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    TicketTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketStatusId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketPriorityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeveloperId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    IsArchivedByProject = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_DeveloperId",
                        column: x => x.DeveloperId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tickets_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketPriorities_TicketPriorityId",
                        column: x => x.TicketPriorityId,
                        principalTable: "TicketPriorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketStatuses_TicketStatusId",
                        column: x => x.TicketStatusId,
                        principalTable: "TicketStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketAttachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketAttachment_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketAttachment_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketComment_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketComment_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Property = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketHistory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TicketHistory_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "JDW, Inc." });

            migrationBuilder.InsertData(
                table: "TicketPriorities",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { "high", "High" },
                    { "low", "Low" },
                    { "medium", "Medium" },
                    { "veryHigh", "Emergency" },
                    { "veryLow", "If time allows" }
                });

            migrationBuilder.InsertData(
                table: "TicketStatuses",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { "complete", "Complete" },
                    { "development", "In development" },
                    { "hold", "On hold" },
                    { "pending", "Pending accept" },
                    { "unassigned", "Unassigned" }
                });

            migrationBuilder.InsertData(
                table: "TicketTypes",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { "bug", "Bug fix" },
                    { "feature", "New feature" },
                    { "other", "Other" },
                    { "UI", "UI change" }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CompanyId", "Created", "Deadline", "Description", "IsArchived", "Name", "PictureData", "PictureExtension", "ProjectManagerId" },
                values: new object[] { 1, 1, new DateTime(2023, 2, 9, 22, 3, 23, 781, DateTimeKind.Local).AddTicks(8422), new DateTime(2023, 8, 8, 22, 3, 23, 781, DateTimeKind.Local).AddTicks(8451), "Build an enterprise application that can track, manage, and generate reports for WidgetCo's inventory.", false, "WidgetCo Inventory System", new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 202, 0, 0, 0, 201, 8, 3, 0, 0, 0, 85, 47, 93, 52, 0, 0, 0, 4, 103, 65, 77, 65, 0, 0, 177, 143, 11, 252, 97, 5, 0, 0, 2, 37, 80, 76, 84, 69, 24, 0, 82, 70, 70, 70, 71, 71, 71, 72, 72, 72, 73, 73, 73, 74, 74, 74, 75, 75, 75, 76, 76, 76, 77, 77, 77, 78, 78, 78, 79, 79, 79, 80, 80, 80, 81, 81, 81, 82, 82, 82, 83, 83, 83, 84, 84, 84, 85, 85, 85, 86, 86, 86, 87, 87, 87, 88, 88, 88, 89, 89, 89, 90, 90, 90, 91, 91, 91, 92, 92, 92, 93, 93, 93, 94, 94, 94, 95, 95, 95, 96, 96, 96, 97, 97, 97, 98, 98, 98, 99, 99, 99, 100, 100, 100, 101, 101, 101, 102, 102, 102, 103, 103, 103, 104, 104, 104, 105, 105, 105, 106, 106, 106, 107, 107, 107, 108, 108, 108, 109, 109, 109, 110, 110, 110, 111, 111, 111, 112, 112, 112, 113, 113, 113, 114, 114, 114, 115, 115, 115, 116, 116, 116, 117, 117, 117, 118, 118, 118, 119, 119, 119, 120, 120, 120, 121, 121, 121, 122, 122, 122, 123, 123, 123, 124, 124, 124, 125, 125, 125, 126, 126, 126, 127, 127, 127, 128, 128, 128, 129, 129, 129, 130, 130, 130, 131, 131, 131, 132, 132, 132, 133, 133, 133, 134, 134, 134, 135, 135, 135, 136, 136, 136, 137, 137, 137, 138, 138, 138, 139, 139, 139, 140, 140, 140, 141, 141, 141, 142, 142, 142, 143, 143, 143, 144, 144, 144, 145, 145, 145, 146, 146, 146, 148, 148, 148, 149, 149, 149, 150, 150, 150, 151, 151, 151, 152, 152, 152, 153, 153, 153, 154, 154, 154, 155, 155, 155, 156, 156, 156, 157, 157, 157, 158, 158, 158, 160, 160, 160, 161, 161, 161, 162, 162, 162, 163, 163, 163, 164, 164, 164, 165, 165, 165, 166, 166, 166, 167, 167, 167, 168, 168, 168, 169, 169, 169, 170, 170, 170, 171, 171, 171, 172, 172, 172, 173, 173, 173, 174, 174, 174, 175, 175, 175, 176, 176, 176, 177, 177, 177, 178, 178, 178, 180, 180, 180, 181, 181, 181, 182, 182, 182, 184, 184, 184, 185, 185, 185, 186, 186, 186, 187, 187, 187, 188, 188, 188, 189, 189, 189, 190, 190, 190, 191, 191, 191, 192, 192, 192, 193, 193, 193, 194, 194, 194, 195, 195, 195, 196, 196, 196, 197, 197, 197, 198, 198, 198, 199, 199, 199, 200, 200, 200, 201, 201, 201, 202, 202, 202, 203, 203, 203, 204, 204, 204, 205, 205, 205, 206, 206, 206, 207, 207, 207, 208, 208, 208, 209, 209, 209, 210, 210, 210, 211, 211, 211, 212, 212, 212, 213, 213, 213, 214, 214, 214, 215, 215, 215, 216, 216, 216, 217, 217, 217, 218, 218, 218, 219, 219, 219, 220, 220, 220, 221, 221, 221, 222, 222, 222, 223, 223, 223, 224, 224, 224, 225, 225, 225, 226, 226, 226, 227, 227, 227, 228, 228, 228, 229, 229, 229, 230, 230, 230, 231, 231, 231, 232, 232, 232, 233, 233, 233, 234, 234, 234, 235, 235, 235, 236, 236, 236, 237, 237, 237, 238, 238, 238, 239, 239, 239, 240, 240, 240, 241, 241, 241, 242, 242, 242, 243, 243, 243, 244, 244, 244, 245, 245, 245, 246, 246, 246, 247, 247, 247, 248, 248, 248, 249, 249, 249, 250, 250, 250, 251, 251, 251, 252, 252, 252, 253, 253, 253, 254, 254, 254, 255, 255, 255, 47, 212, 58, 7, 0, 0, 0, 9, 112, 72, 89, 115, 0, 0, 14, 196, 0, 0, 14, 196, 1, 149, 43, 14, 27, 0, 0, 8, 147, 73, 68, 65, 84, 120, 218, 237, 221, 139, 63, 84, 105, 24, 7, 112, 115, 230, 230, 50, 152, 50, 201, 45, 217, 81, 17, 91, 20, 37, 170, 173, 136, 149, 237, 166, 221, 116, 221, 182, 109, 85, 138, 86, 106, 55, 91, 68, 23, 41, 145, 138, 164, 92, 70, 22, 163, 52, 134, 49, 115, 206, 223, 183, 51, 69, 100, 230, 152, 243, 190, 207, 51, 251, 190, 249, 204, 239, 15, 56, 158, 175, 115, 123, 175, 103, 66, 76, 203, 36, 33, 33, 33, 38, 213, 178, 72, 144, 194, 99, 130, 20, 30, 19, 164, 240, 152, 32, 133, 199, 4, 41, 60, 38, 72, 225, 49, 65, 10, 143, 9, 82, 120, 76, 144, 194, 99, 130, 20, 30, 19, 164, 240, 152, 32, 69, 65, 4, 109, 76, 218, 238, 83, 181, 15, 58, 60, 105, 174, 61, 249, 67, 250, 42, 157, 240, 45, 82, 52, 201, 165, 13, 3, 54, 81, 154, 143, 104, 179, 220, 61, 184, 78, 251, 173, 81, 34, 242, 31, 188, 95, 200, 248, 146, 143, 79, 127, 92, 249, 45, 81, 162, 127, 122, 233, 144, 228, 226, 236, 63, 185, 58, 48, 215, 25, 62, 37, 108, 87, 167, 83, 90, 50, 253, 21, 171, 2, 129, 193, 166, 8, 169, 141, 211, 146, 191, 136, 61, 133, 161, 220, 83, 244, 135, 70, 253, 66, 60, 153, 105, 76, 230, 156, 18, 85, 229, 255, 148, 204, 158, 152, 190, 2, 53, 207, 148, 132, 123, 46, 133, 18, 119, 38, 42, 194, 185, 165, 8, 169, 189, 162, 114, 137, 251, 89, 86, 103, 228, 149, 146, 218, 67, 2, 241, 92, 100, 77, 168, 207, 78, 60, 74, 210, 11, 66, 137, 36, 185, 154, 86, 240, 72, 49, 62, 34, 150, 184, 207, 203, 93, 196, 107, 12, 139, 18, 86, 79, 112, 199, 47, 176, 220, 48, 240, 70, 209, 158, 112, 210, 72, 220, 247, 254, 69, 180, 6, 38, 18, 37, 123, 156, 78, 34, 73, 182, 66, 172, 70, 12, 14, 37, 178, 141, 86, 34, 73, 150, 181, 60, 81, 132, 10, 202, 203, 235, 83, 30, 70, 114, 68, 73, 25, 3, 72, 36, 215, 105, 156, 38, 12, 6, 69, 247, 23, 209, 91, 222, 43, 214, 12, 110, 40, 153, 74, 219, 144, 114, 105, 212, 113, 66, 209, 223, 4, 74, 36, 251, 14, 78, 40, 25, 31, 160, 20, 169, 21, 163, 145, 12, 167, 168, 255, 128, 221, 41, 158, 76, 231, 115, 65, 137, 238, 3, 75, 36, 233, 49, 194, 105, 129, 83, 10, 160, 55, 189, 39, 83, 91, 120, 160, 220, 66, 144, 72, 82, 61, 252, 221, 2, 166, 24, 70, 80, 40, 99, 102, 246, 148, 60, 7, 220, 225, 73, 57, 115, 138, 250, 2, 142, 68, 122, 10, 110, 236, 67, 41, 97, 228, 221, 96, 223, 25, 79, 97, 77, 89, 61, 137, 68, 113, 149, 178, 166, 228, 206, 32, 81, 196, 6, 104, 23, 12, 74, 57, 4, 127, 213, 207, 102, 0, 58, 98, 1, 164, 104, 42, 177, 36, 146, 109, 61, 91, 138, 190, 30, 141, 34, 237, 101, 75, 137, 104, 199, 163, 84, 1, 95, 248, 64, 74, 36, 233, 224, 234, 18, 105, 209, 48, 165, 68, 43, 155, 78, 81, 148, 94, 224, 112, 5, 144, 98, 4, 13, 80, 124, 157, 225, 4, 166, 148, 24, 43, 30, 229, 195, 58, 166, 148, 164, 247, 120, 20, 123, 38, 83, 138, 17, 241, 172, 136, 185, 108, 41, 136, 247, 138, 180, 141, 41, 37, 250, 223, 101, 67, 49, 116, 47, 27, 74, 56, 205, 92, 23, 159, 20, 248, 200, 36, 55, 20, 225, 28, 158, 68, 204, 97, 74, 81, 149, 81, 77, 65, 250, 204, 36, 112, 64, 31, 74, 201, 198, 234, 69, 74, 146, 21, 216, 187, 135, 82, 86, 126, 68, 163, 88, 98, 217, 82, 66, 1, 179, 144, 139, 210, 13, 92, 88, 197, 205, 56, 152, 36, 221, 97, 219, 245, 82, 169, 182, 33, 141, 78, 74, 210, 105, 224, 144, 11, 152, 18, 129, 213, 10, 115, 1, 91, 147, 8, 35, 249, 117, 72, 20, 107, 18, 115, 202, 110, 164, 43, 172, 11, 186, 156, 18, 78, 137, 177, 160, 72, 196, 74, 214, 163, 147, 238, 103, 88, 13, 10, 101, 42, 11, 40, 193, 152, 33, 222, 138, 50, 2, 222, 183, 130, 3, 138, 225, 9, 6, 165, 26, 42, 193, 160, 8, 197, 144, 197, 58, 179, 177, 111, 229, 129, 162, 50, 116, 193, 41, 109, 97, 92, 80, 4, 248, 243, 216, 185, 15, 44, 193, 89, 68, 165, 111, 133, 82, 94, 70, 115, 66, 81, 101, 77, 192, 36, 174, 253, 8, 171, 14, 113, 40, 154, 43, 176, 201, 175, 103, 24, 235, 90, 145, 150, 129, 38, 188, 129, 72, 38, 49, 86, 235, 96, 81, 132, 221, 118, 0, 229, 111, 61, 71, 20, 149, 246, 34, 253, 120, 197, 48, 124, 81, 8, 38, 69, 21, 209, 66, 123, 187, 56, 246, 163, 72, 16, 183, 23, 36, 80, 174, 171, 112, 94, 66, 185, 188, 80, 55, 125, 100, 12, 210, 72, 196, 102, 164, 101, 198, 168, 91, 113, 242, 105, 6, 146, 222, 64, 59, 143, 129, 160, 184, 45, 86, 98, 201, 208, 102, 44, 9, 238, 94, 47, 33, 135, 244, 245, 210, 151, 137, 183, 67, 18, 119, 7, 158, 144, 222, 69, 242, 28, 19, 187, 55, 32, 238, 245, 196, 222, 226, 25, 247, 80, 185, 69, 108, 79, 68, 252, 203, 248, 27, 111, 195, 206, 43, 109, 90, 206, 212, 225, 238, 140, 198, 223, 67, 172, 201, 237, 80, 50, 186, 47, 246, 149, 192, 123, 91, 1, 166, 168, 4, 99, 197, 176, 223, 86, 140, 173, 46, 25, 123, 75, 116, 64, 246, 219, 11, 241, 23, 109, 75, 66, 92, 207, 115, 241, 191, 33, 16, 160, 175, 32, 168, 19, 171, 135, 101, 7, 47, 166, 90, 247, 34, 181, 85, 254, 15, 138, 251, 150, 137, 45, 109, 24, 245, 94, 227, 46, 218, 94, 93, 222, 140, 188, 121, 56, 208, 20, 119, 244, 241, 133, 213, 237, 86, 187, 227, 243, 233, 17, 157, 142, 73, 75, 243, 185, 236, 40, 236, 29, 221, 72, 20, 195, 166, 146, 242, 242, 195, 57, 178, 255, 102, 65, 23, 153, 182, 239, 216, 165, 203, 238, 92, 56, 186, 43, 41, 76, 254, 14, 137, 218, 113, 180, 188, 188, 40, 29, 48, 6, 14, 160, 232, 18, 75, 27, 198, 167, 61, 207, 42, 199, 192, 65, 96, 231, 220, 120, 252, 157, 231, 9, 238, 156, 26, 187, 85, 24, 75, 249, 68, 160, 165, 104, 227, 138, 155, 198, 230, 111, 108, 199, 189, 20, 192, 179, 85, 189, 233, 249, 252, 161, 102, 134, 235, 247, 154, 104, 214, 30, 210, 81, 244, 25, 151, 135, 22, 189, 7, 7, 119, 80, 223, 3, 154, 162, 69, 187, 45, 28, 253, 191, 154, 201, 119, 178, 209, 80, 244, 57, 247, 166, 188, 31, 177, 31, 203, 40, 151, 113, 234, 142, 123, 79, 5, 136, 182, 219, 155, 72, 15, 71, 76, 17, 12, 197, 157, 190, 247, 17, 217, 127, 163, 122, 200, 174, 168, 245, 221, 206, 177, 183, 20, 132, 17, 93, 180, 132, 20, 205, 234, 35, 221, 178, 3, 196, 51, 119, 205, 196, 23, 153, 58, 163, 77, 182, 145, 51, 213, 121, 32, 134, 224, 128, 68, 20, 193, 116, 214, 178, 100, 235, 106, 176, 144, 240, 18, 15, 61, 52, 188, 212, 241, 102, 222, 28, 141, 86, 124, 102, 72, 40, 186, 162, 33, 127, 157, 145, 233, 171, 68, 227, 216, 113, 119, 252, 77, 205, 184, 222, 22, 40, 189, 103, 8, 40, 107, 110, 43, 152, 123, 112, 189, 206, 83, 252, 90, 208, 23, 90, 20, 244, 211, 166, 174, 41, 172, 79, 49, 69, 179, 93, 97, 191, 125, 178, 214, 172, 232, 255, 168, 203, 168, 159, 82, 116, 64, 87, 199, 70, 69, 119, 140, 82, 138, 166, 68, 241, 188, 131, 56, 250, 123, 162, 223, 11, 92, 109, 174, 81, 62, 145, 49, 182, 83, 137, 69, 33, 69, 93, 74, 50, 13, 44, 142, 156, 143, 91, 242, 112, 66, 114, 149, 149, 224, 120, 210, 135, 124, 5, 55, 191, 66, 74, 62, 225, 42, 111, 113, 228, 202, 70, 217, 135, 89, 104, 246, 205, 113, 194, 1, 230, 161, 116, 44, 74, 202, 0, 217, 95, 246, 100, 226, 201, 47, 235, 35, 53, 139, 254, 155, 130, 38, 58, 243, 76, 135, 141, 248, 96, 98, 135, 255, 33, 13, 69, 20, 195, 125, 114, 201, 39, 205, 171, 234, 194, 141, 177, 58, 97, 54, 250, 184, 239, 75, 254, 236, 165, 91, 176, 32, 94, 247, 251, 198, 82, 68, 41, 5, 172, 245, 180, 143, 244, 61, 185, 115, 169, 178, 178, 170, 177, 189, 127, 84, 217, 35, 203, 103, 156, 126, 103, 198, 148, 80, 76, 224, 173, 67, 162, 59, 208, 99, 60, 243, 247, 242, 85, 66, 57, 133, 182, 97, 16, 18, 231, 1, 56, 37, 6, 227, 219, 0, 8, 105, 139, 0, 83, 74, 17, 150, 176, 96, 196, 223, 50, 24, 255, 20, 253, 83, 214, 134, 217, 136, 215, 213, 64, 202, 6, 242, 183, 64, 128, 242, 46, 26, 70, 81, 159, 100, 45, 248, 18, 231, 30, 24, 37, 226, 57, 107, 193, 124, 154, 180, 32, 138, 25, 184, 18, 7, 51, 99, 38, 16, 165, 148, 117, 253, 11, 147, 7, 161, 8, 136, 91, 183, 225, 169, 17, 0, 20, 99, 47, 235, 242, 23, 230, 69, 36, 128, 146, 142, 245, 237, 9, 148, 76, 172, 5, 80, 14, 179, 174, 254, 171, 136, 251, 0, 148, 219, 172, 171, 255, 58, 85, 244, 20, 3, 104, 245, 29, 126, 90, 67, 169, 41, 223, 193, 63, 99, 134, 154, 65, 19, 53, 37, 31, 111, 175, 32, 74, 236, 27, 104, 41, 194, 25, 46, 122, 93, 243, 153, 41, 166, 166, 180, 176, 174, 125, 81, 196, 42, 90, 74, 4, 213, 194, 187, 64, 230, 190, 150, 146, 146, 104, 101, 93, 250, 226, 116, 71, 80, 82, 54, 163, 237, 20, 196, 202, 251, 24, 58, 138, 80, 204, 186, 114, 175, 56, 211, 232, 40, 26, 188, 173, 168, 88, 113, 149, 209, 81, 180, 205, 172, 43, 247, 138, 88, 45, 80, 81, 194, 95, 179, 174, 220, 59, 143, 180, 84, 148, 184, 119, 172, 11, 247, 78, 79, 36, 21, 101, 61, 71, 253, 250, 185, 140, 198, 82, 81, 182, 112, 214, 2, 243, 196, 153, 76, 69, 225, 106, 136, 98, 46, 121, 84, 20, 254, 158, 197, 238, 28, 163, 162, 252, 195, 186, 108, 95, 169, 161, 161, 104, 58, 89, 151, 237, 43, 143, 212, 20, 20, 227, 91, 214, 101, 251, 74, 87, 36, 5, 101, 13, 206, 247, 125, 145, 51, 20, 71, 65, 201, 4, 204, 130, 6, 46, 54, 51, 5, 101, 39, 235, 170, 125, 39, 139, 130, 242, 51, 235, 162, 125, 103, 63, 5, 229, 42, 235, 162, 125, 231, 52, 5, 133, 114, 13, 69, 160, 83, 75, 78, 17, 184, 26, 196, 159, 207, 67, 129, 152, 18, 53, 196, 186, 104, 223, 233, 12, 39, 166, 36, 96, 126, 136, 17, 49, 111, 87, 18, 83, 210, 184, 154, 90, 153, 207, 72, 2, 49, 37, 135, 195, 222, 138, 39, 118, 51, 49, 165, 152, 179, 241, 226, 185, 56, 182, 144, 82, 132, 51, 172, 107, 150, 137, 179, 152, 148, 162, 190, 193, 186, 102, 153, 184, 78, 145, 82, 52, 188, 44, 211, 89, 28, 177, 134, 148, 162, 229, 244, 181, 34, 73, 205, 2, 33, 5, 245, 251, 190, 168, 105, 215, 19, 82, 214, 112, 54, 13, 57, 159, 215, 81, 132, 20, 94, 223, 144, 238, 126, 100, 12, 33, 37, 135, 187, 185, 149, 185, 76, 196, 19, 82, 246, 176, 174, 88, 54, 174, 181, 100, 20, 225, 8, 235, 138, 101, 35, 243, 181, 96, 89, 138, 250, 44, 235, 138, 229, 41, 69, 100, 20, 237, 53, 214, 21, 203, 83, 78, 8, 68, 20, 93, 19, 235, 138, 229, 115, 149, 140, 162, 71, 252, 57, 18, 236, 212, 147, 81, 194, 184, 28, 101, 253, 156, 199, 106, 34, 74, 12, 183, 77, 48, 73, 122, 17, 78, 68, 137, 199, 252, 93, 2, 228, 244, 68, 17, 81, 82, 169, 127, 89, 56, 240, 177, 152, 136, 40, 25, 120, 223, 39, 71, 207, 88, 2, 17, 37, 139, 203, 97, 252, 207, 153, 72, 38, 162, 108, 199, 251, 0, 62, 122, 166, 205, 68, 148, 221, 172, 235, 93, 42, 105, 65, 10, 135, 145, 167, 248, 74, 25, 235, 114, 151, 202, 118, 159, 37, 135, 44, 167, 252, 7, 203, 84, 245, 79, 112, 215, 178, 249, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130 }, "png", null });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AppUserId",
                table: "Notifications",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CompanyId",
                table: "Notifications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CompanyId",
                table: "Projects",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_TeamId",
                table: "ProjectUsers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAttachment_AppUserId",
                table: "TicketAttachment",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAttachment_TicketId",
                table: "TicketAttachment",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComment_AppUserId",
                table: "TicketComment",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComment_TicketId",
                table: "TicketComment",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistory_TicketId",
                table: "TicketHistory",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistory_UserId",
                table: "TicketHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatorId",
                table: "Tickets",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_DeveloperId",
                table: "Tickets",
                column: "DeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ProjectId",
                table: "Tickets",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketPriorityId",
                table: "Tickets",
                column: "TicketPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketStatusId",
                table: "Tickets",
                column: "TicketStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketTypeId",
                table: "Tickets",
                column: "TicketTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "ProjectUsers");

            migrationBuilder.DropTable(
                name: "TicketAttachment");

            migrationBuilder.DropTable(
                name: "TicketComment");

            migrationBuilder.DropTable(
                name: "TicketHistory");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "TicketPriorities");

            migrationBuilder.DropTable(
                name: "TicketStatuses");

            migrationBuilder.DropTable(
                name: "TicketTypes");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
