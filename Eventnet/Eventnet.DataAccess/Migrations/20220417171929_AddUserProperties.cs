using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventnet.DataAccess.Migrations
{
    public partial class AddUserProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventEntityTagEntity");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "54ba432c-8b6f-4b63-87f8-56a1fbc69b8b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd0dad39-4c51-403d-bdad-5d8ae17d7b53");

            migrationBuilder.AddColumn<Guid>(
                name: "EventEntityId",
                table: "Tags",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Gender",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1ad9a266-95fe-4a04-8cce-bd7335dbdfc8", "6d4f197d-1170-417e-a553-fa249a7db9de", "Admin", "ADMIN" },
                    { "cbd3ff27-aac6-4773-a551-8f90f02c8daf", "aa2967d1-b903-46ba-aaea-3b260e048cd4", "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_EventEntityId",
                table: "Tags",
                column: "EventEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Events_EventEntityId",
                table: "Tags",
                column: "EventEntityId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Events_EventEntityId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_EventEntityId",
                table: "Tags");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1ad9a266-95fe-4a04-8cce-bd7335dbdfc8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cbd3ff27-aac6-4773-a551-8f90f02c8daf");

            migrationBuilder.DropColumn(
                name: "EventEntityId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "EventEntityTagEntity",
                columns: table => new
                {
                    EventsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventEntityTagEntity", x => new { x.EventsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_EventEntityTagEntity_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventEntityTagEntity_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "54ba432c-8b6f-4b63-87f8-56a1fbc69b8b", "46bdf8e4-144f-42f6-bd0a-e6cd8c8c4267", "User", "USER" },
                    { "bd0dad39-4c51-403d-bdad-5d8ae17d7b53", "da788890-d26b-4d48-b296-597c6896bcea", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventEntityTagEntity_TagsId",
                table: "EventEntityTagEntity",
                column: "TagsId");
        }
    }
}
