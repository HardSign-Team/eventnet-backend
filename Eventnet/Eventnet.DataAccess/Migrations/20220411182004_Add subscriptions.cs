using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventnet.DataAccess.Migrations
{
    public partial class Addsubscriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "502530ed-f0b8-4a0c-9502-1bd24da4b0e7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "624cf409-425d-4186-b7b4-69101ed6a84d");

            migrationBuilder.CreateTable(
                name: "SubscriptionEntity",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    SubscriptionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionEntity", x => new { x.EventId, x.UserId });
                    table.ForeignKey(
                        name: "FK_SubscriptionEntity_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "73c3dccf-642d-4cab-94ae-519117c9a2d6", "7f2abfaf-3db5-42ca-a6cc-2bd3a685473a", "User", "USER" },
                    { "8e4bcfe4-f0c0-436e-96f4-60f1dc9e6ac1", "eaebdf5c-d3e9-401a-8dd8-0b2bf66d1e58", "Admin", "ADMIN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionEntity");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "73c3dccf-642d-4cab-94ae-519117c9a2d6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8e4bcfe4-f0c0-436e-96f4-60f1dc9e6ac1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "502530ed-f0b8-4a0c-9502-1bd24da4b0e7", "d4749609-3a73-461e-adac-1985c5d7cfa5", "User", "USER" },
                    { "624cf409-425d-4186-b7b4-69101ed6a84d", "9df8c421-2691-4848-979c-de71c3c0cc67", "Admin", "ADMIN" }
                });
        }
    }
}
