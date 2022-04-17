using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventnet.DataAccess.Migrations
{
    public partial class AddforgottenSubscriptionEntitiesdbset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionEntity_Events_EventId",
                table: "SubscriptionEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionEntity",
                table: "SubscriptionEntity");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "73c3dccf-642d-4cab-94ae-519117c9a2d6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8e4bcfe4-f0c0-436e-96f4-60f1dc9e6ac1");

            migrationBuilder.RenameTable(
                name: "SubscriptionEntity",
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
                    { "54ba432c-8b6f-4b63-87f8-56a1fbc69b8b", "46bdf8e4-144f-42f6-bd0a-e6cd8c8c4267", "User", "USER" },
                    { "bd0dad39-4c51-403d-bdad-5d8ae17d7b53", "da788890-d26b-4d48-b296-597c6896bcea", "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionEntities_Events_EventId",
                table: "SubscriptionEntities",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
                keyValue: "54ba432c-8b6f-4b63-87f8-56a1fbc69b8b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd0dad39-4c51-403d-bdad-5d8ae17d7b53");

            migrationBuilder.RenameTable(
                name: "SubscriptionEntities",
                newName: "SubscriptionEntity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionEntity",
                table: "SubscriptionEntity",
                columns: new[] { "EventId", "UserId" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "73c3dccf-642d-4cab-94ae-519117c9a2d6", "7f2abfaf-3db5-42ca-a6cc-2bd3a685473a", "User", "USER" },
                    { "8e4bcfe4-f0c0-436e-96f4-60f1dc9e6ac1", "eaebdf5c-d3e9-401a-8dd8-0b2bf66d1e58", "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionEntity_Events_EventId",
                table: "SubscriptionEntity",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
