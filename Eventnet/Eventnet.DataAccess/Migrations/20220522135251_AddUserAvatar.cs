using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventnet.DataAccess.Migrations
{
    public partial class AddUserAvatar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1053eba5-3f1f-46a2-8a8a-11c868d7c8a3"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3f6abaa6-0e0a-4ece-9b64-9a70d79e1fba"));

            migrationBuilder.AddColumn<Guid>(
                name: "AvatarId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("58dd79a8-453f-43f2-8892-14c36491db65"), "2596de2a-6078-4b75-83c5-98984ecd9f46", "Admin", "ADMIN" },
                    { new Guid("6bd1e763-dc07-46e8-8d66-6ab2271e4f3a"), "58a3e73a-7499-4832-9b69-81af263ea85e", "User", "USER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("58dd79a8-453f-43f2-8892-14c36491db65"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("6bd1e763-dc07-46e8-8d66-6ab2271e4f3a"));

            migrationBuilder.DropColumn(
                name: "AvatarId",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("1053eba5-3f1f-46a2-8a8a-11c868d7c8a3"), "07312cc5-a009-4624-9751-c1e3c112dbb8", "Admin", "ADMIN" },
                    { new Guid("3f6abaa6-0e0a-4ece-9b64-9a70d79e1fba"), "dfb43980-667b-470c-a883-4cc080a4763c", "User", "USER" }
                });
        }
    }
}
