using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Hsjdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetailsSorties",
                columns: table => new
                {
                    DetailsSortieId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SortieId = table.Column<int>(type: "int", nullable: false),
                    Agent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeMouvement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateMouvement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureMouvement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NatureMouvement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AjoutId = table.Column<int>(type: "int", nullable: false),
                    MotifAjout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProduitLivre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QteLivre = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailsSorties", x => x.DetailsSortieId);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$XnQbrbAqdKyShEUuzcTB6OajYKLHcRK5GM80jUKk4f4b3Zrecb7Ru");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$uP7ujv2EbK3Rtl6W6aGYk.Oa7bX/rHn3jEZKerU/TcNcfPOrv4l72");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailsSorties");

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
    }
}
