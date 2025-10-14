using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingav : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$cDUTRmC1IO9exuiRPIpW5.kEmMZDrBItF.1/xYI1lSFqetjkJBnii");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$52sKtjkHt1OsZ8J4q.ULPew1b7DuSnvdMOTc5cNTqwjlnCYOWzr.O");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$ymt1HjInRK183f8BGGhWoOpEvUsBdaedcM.xzE0si/gjJpbyzRldu");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$QgVJ460uUfYDkTa/LsgsoOf0GvBnv8EO8iAUWK7yep3OHJz8UBX72");
        }
    }
}
