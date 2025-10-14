using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingahousejleu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UniteAliment",
                table: "Aliments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$BVXMypE3fEe7TKy1SDamt.m/eubC6/YOFWqvAWJ6eE4u36Z1MnY42");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$mtLJat6l8kuWVvx8OpgRyeKHfsFR4aaGN.BY4VEQs2FtIxCmuR9ay");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UniteAliment",
                table: "Aliments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$bwfgVEKTCg/wxE6/KNJ48.4VKxpQclSe1slM71RG2hnK5moc6zeNW");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$arPoibeNg6bWwo9M2RNt4uoC5IaKXaDg0Gn82l6aTkvSa6HMCv8fi");
        }
    }
}
