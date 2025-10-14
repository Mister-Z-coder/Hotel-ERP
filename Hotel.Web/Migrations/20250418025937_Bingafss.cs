using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingafss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$S63MbXTnCE1ywUCFs06V1uzMHZ2KYB708IWEx2JnVi4OZA//d3HOy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$e53jGf3EZFiQH4YLSpfKn.8UhTFPMk0bpTbkmJcHiJUp8z5ZjGv8u");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$lpqjE4gDMKaLh7Dg3E90oOd.tymj9VdbW/KOMglzHSLYLiLQlfd02");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$wJkZTZFdQVNTGvvoSjXDqeVLPHc3val0qsbMnaU.DaESLrDw6xtnC");
        }
    }
}
