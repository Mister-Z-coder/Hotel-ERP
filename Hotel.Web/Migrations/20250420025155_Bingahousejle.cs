using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingahousejle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetailsMouvements",
                columns: table => new
                {
                    DetailsMouvementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MouvementId = table.Column<int>(type: "int", nullable: false),
                    Agent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeMouvement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateMouvement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureMouvement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NatureMouvement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProduitMouvement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QteMouvement = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailsMouvements", x => x.DetailsMouvementId);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailsMouvements");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$eHBO7Rs/o2lbVgjqQMxQfu6NuCmNqfKzbl/a.j80Ff2Jwr6gG/LrW");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$f4c4SLULr44WzPotpIMkbOzKgmtC1rxHMYn/bLTZHsmCwXavEMnF2");
        }
    }
}
