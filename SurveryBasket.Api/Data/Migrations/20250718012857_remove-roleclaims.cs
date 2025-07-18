using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveryBasket.Api.data.Migrations
{
    /// <inheritdoc />
    public partial class removeroleclaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "polls:read", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 2, "permissions", "polls:add", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 3, "permissions", "polls:update", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 4, "permissions", "polls:delete", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 5, "permissions", "questions:read", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 6, "permissions", "questions:add", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 7, "permissions", "questions:update", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 8, "permissions", "users:read", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 9, "permissions", "users:add", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 10, "permissions", "users:update", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 11, "permissions", "roles:read", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 12, "permissions", "roles:add", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 13, "permissions", "roles:update", "090703ff-7006-4f71-ac7f-c9b4446ff074" },
                    { 14, "permissions", "results:read", "090703ff-7006-4f71-ac7f-c9b4446ff074" }
                });
        }
    }
}
