using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingahousez : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EtatDemande",
                table: "Demandes");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$aiD4IOBec./PoFJjRci53eKUXgUDQFTHnVaLdnHEH.fkS9TvO/ldS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$g5bbdJZ8Igo2lQWfj6.58eTO2FeWMHMCO7iMtqCMGbG8dW.fjDmc2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EtatDemande",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$cnL9Pe6I3T.MmxPLSFZCxeYnddIdzRmx6HNClxy2LarnwLTWr5J/K");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$au5IMjHzO2f6qZSqkVQ87O0P.2liZaGPKkGobnH6cC66BiL/9XORW");
        }
    }
}
