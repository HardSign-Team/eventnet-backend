using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventnet.DataAccess.Migrations
{
    public partial class Adduniqueindextotagname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "051a3aea-170c-4287-8b85-7bba7c68d9a1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b819fb9d-7f48-4a9b-8e44-85c951fb70eb");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "ba1ecd9c-0e1c-4a3d-a08f-89896a03a452", "bc70577a-01dc-4733-a874-ac3ea747bb12", "User", "USER" },
                    { "da2e7979-2df8-4686-b80d-b466b18f1a05", "91734f54-f050-4644-8610-aad734ae4e3b", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba1ecd9c-0e1c-4a3d-a08f-89896a03a452");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "da2e7979-2df8-4686-b80d-b466b18f1a05");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "051a3aea-170c-4287-8b85-7bba7c68d9a1", "c958c32e-0b90-47e4-a20c-73a7718e814f", "Admin", "ADMIN" },
                    { "b819fb9d-7f48-4a9b-8e44-85c951fb70eb", "17e96438-a712-410d-a14f-a8ebe60cfa8b", "User", "USER" }
                });
        }
    }
}
