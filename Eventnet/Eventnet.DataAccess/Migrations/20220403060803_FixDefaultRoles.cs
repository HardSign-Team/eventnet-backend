using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventnet.DataAccess.Migrations
{
    public partial class FixDefaultRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7fca417c-325e-4afe-8868-ce71f2ec0bb3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c231c80b-a51d-45b6-8492-c1158090f2f9");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "22427488-0f6b-414f-9595-113da4a2a0e5", "7fe26813-7ffb-45c6-86d5-6cd1d5b7085d", "Admin", "ADMIN" },
                    { "55cba6ee-fee7-4908-8ed9-f31a0f434885", "a3780143-07c2-48d9-bee4-8683737b229d", "User", "USER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "22427488-0f6b-414f-9595-113da4a2a0e5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "55cba6ee-fee7-4908-8ed9-f31a0f434885");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7fca417c-325e-4afe-8868-ce71f2ec0bb3", "709a65fc-c607-4283-b77e-d6ca459b44cd", "Admin", null },
                    { "c231c80b-a51d-45b6-8492-c1158090f2f9", "defc1496-3f69-4a0f-ba52-a07050d33f3c", "User", null }
                });
        }
    }
}
