using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventnet.DataAccess.Migrations
{
    public partial class AddDefaultRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7fca417c-325e-4afe-8868-ce71f2ec0bb3", "709a65fc-c607-4283-b77e-d6ca459b44cd", "Admin", null },
                    { "c231c80b-a51d-45b6-8492-c1158090f2f9", "defc1496-3f69-4a0f-ba52-a07050d33f3c", "User", null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7fca417c-325e-4afe-8868-ce71f2ec0bb3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c231c80b-a51d-45b6-8492-c1158090f2f9");
        }
    }
}
