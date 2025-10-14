using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingafs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetailsLivraisons",
                columns: table => new
                {
                    DetailsLivraisonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LivraisonId = table.Column<int>(type: "int", nullable: false),
                    Agent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeMouvement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateMouvement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureMouvement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NatureMouvement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovisionnementId = table.Column<int>(type: "int", nullable: false),
                    Fournisseur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MontantLivraison = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateLivraison = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureLivraison = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProduitLivre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QteLivre = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailsLivraisons", x => x.DetailsLivraisonId);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailsLivraisons");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$DfKSld0VSQqJkj71fqcEQOFUXeU8ssbTtbs8Yrc6bYnQyJDQnDpXS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$0jocGs8LHTU0qkyj5iQdrOTZFOt/BxkipYHFDF5uKbaWY.W8Uj1My");
        }
    }
}
