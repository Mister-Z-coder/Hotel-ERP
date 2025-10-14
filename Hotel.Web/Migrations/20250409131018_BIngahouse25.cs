using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class BIngahouse25 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_ReglementCommandes_CommandeId",
                table: "ReglementCommandes",
                column: "CommandeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReglementCommandes_Commandes_CommandeId",
                table: "ReglementCommandes",
                column: "CommandeId",
                principalTable: "Commandes",
                principalColumn: "CommandeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReglementCommandes_Commandes_CommandeId",
                table: "ReglementCommandes");

            migrationBuilder.DropIndex(
                name: "IX_ReglementCommandes_CommandeId",
                table: "ReglementCommandes");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$/RhdhdztA7DudF3N5Wj8U.gVoHeD0GyfhKhZPHG7puroDUcPT0eh.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$13PLE6dnnTzqybLn.zcou.MYBN3lGaCN5j6vreul4UJ2rZiIlhlI.");
        }
    }
}
