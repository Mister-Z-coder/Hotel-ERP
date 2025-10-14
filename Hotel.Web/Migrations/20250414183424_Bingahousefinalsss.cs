using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingahousefinalsss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Montanttotal",
                table: "DetailsFactures",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$m3fxzckaqO2t7jQDb4XsD.VzTkx5ZlukZ9dLOITyZnMabxWkG4bsi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$7kmg2NLpB7FkvfcyNdYEjehFFsrLpoGzN0SYvJhhZX5Et0csnrG9i");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Montanttotal",
                table: "DetailsFactures");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$lVXa02EpnNO5DnpqKts2jO4BgzTIGV0Z4egYRp4Brwr.ddbwxx0Lm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$KG8HXbvzKepvUEl7molVBO/TguIpptE/GA0e2hspB7S8Ve/Ic8ZH.");
        }
    }
}
