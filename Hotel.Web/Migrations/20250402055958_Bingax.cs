using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetailsReglements",
                columns: table => new
                {
                    DetailsReglementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModeReglement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Devise = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeReglement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Agent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Caisse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateReglement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureReglement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontantReglement = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MotifReglement = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailsReglements", x => x.DetailsReglementId);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$bVC1urfmx5lSgtFRzaQy.OoMGHwQza.ZPDHI0ZjiqDig3enC0JDzC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$cxjNS0UFqdtGJPfPt/7DvehVPvgTxcLLAdu90ph.ut6fp5JsDJAsi");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailsReglements");

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
    }
}
