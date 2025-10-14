using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingahousefinal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$.iIkYSLVz8.3Q7YkW8H18ORWIG/FqKsqYcMMqgI/Xjd3OhzuDDXXe");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$vfos53d.tfwYHIZQte3h1.DQW1WM73Z5xh4zWMERDTYjvvJubTW.O");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$Tr7crGJTjDRuQW6W4LdAseQQHbpzsh3h/ECDKw2VVfx3O1/Ec/bpG");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$bokigNk9ZX4M3/GHvxYlL.vPi0D5wLO6hb2qbbSuHdoZiUE0PyZka");
        }
    }
}
