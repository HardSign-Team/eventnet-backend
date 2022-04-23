using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventnet.DataAccess.Migrations
{
    public partial class AddEventMarkEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionEntities_Events_EventId",
                table: "SubscriptionEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionEntities",
                table: "SubscriptionEntities");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ad647df6-9508-4b51-914b-75a9658159f7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d4dc76eb-d765-4ce8-a25f-130ae302c141");

            migrationBuilder.RenameTable(
                name: "SubscriptionEntities",
                newName: "Subscriptions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                columns: new[] { "EventId", "UserId" });

            migrationBuilder.CreateTable(
                name: "Marks",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsLike = table.Column<bool>(type: "boolean", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marks", x => new { x.EventId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Marks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Marks_Events_EventId",
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
                    { "051a3aea-170c-4287-8b85-7bba7c68d9a1", "c958c32e-0b90-47e4-a20c-73a7718e814f", "Admin", "ADMIN" },
                    { "b819fb9d-7f48-4a9b-8e44-85c951fb70eb", "17e96438-a712-410d-a14f-a8ebe60cfa8b", "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_UserId",
                table: "Marks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_AspNetUsers_UserId",
                table: "Subscriptions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Events_EventId",
                table: "Subscriptions",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_AspNetUsers_UserId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Events_EventId",
                table: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Marks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "051a3aea-170c-4287-8b85-7bba7c68d9a1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b819fb9d-7f48-4a9b-8e44-85c951fb70eb");

            migrationBuilder.RenameTable(
                name: "Subscriptions",
                newName: "SubscriptionEntities");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionEntities",
                table: "SubscriptionEntities",
                columns: new[] { "EventId", "UserId" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "ad647df6-9508-4b51-914b-75a9658159f7", "519c48ce-8707-4432-9069-c1c33fc1af6d", "User", "USER" },
                    { "d4dc76eb-d765-4ce8-a25f-130ae302c141", "de09676c-e3f5-4cdf-b38b-5aed9a5ec689", "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionEntities_Events_EventId",
                table: "SubscriptionEntities",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
