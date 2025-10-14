using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingahousefinalapp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mouvements",
                columns: table => new
                {
                    MouvementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    TypeMouvementId = table.Column<int>(type: "int", nullable: false),
                    DateMouvement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureMouvement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NatureMouvement = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mouvements", x => x.MouvementId);
                    table.ForeignKey(
                        name: "FK_Mouvements_TypeMouvements_TypeMouvementId",
                        column: x => x.TypeMouvementId,
                        principalTable: "TypeMouvements",
                        principalColumn: "TypeMouvementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Decaissements",
                columns: table => new
                {
                    MouvementId = table.Column<int>(type: "int", nullable: false),
                    MotifDecaissement = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decaissements", x => x.MouvementId);
                    table.ForeignKey(
                        name: "FK_Decaissements_Mouvements_MouvementId",
                        column: x => x.MouvementId,
                        principalTable: "Mouvements",
                        principalColumn: "MouvementId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Livraisons",
                columns: table => new
                {
                    MouvementId = table.Column<int>(type: "int", nullable: false),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    ApprovisionnementDemandeId = table.Column<int>(type: "int", nullable: true),
                    FournisseurId = table.Column<int>(type: "int", nullable: false),
                    MontantLivraison = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateLivraison = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureLivraison = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livraisons", x => x.MouvementId);
                    table.ForeignKey(
                        name: "FK_Livraisons_Approvisionnements_ApprovisionnementDemandeId",
                        column: x => x.ApprovisionnementDemandeId,
                        principalTable: "Approvisionnements",
                        principalColumn: "DemandeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Livraisons_Fournisseurs_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Fournisseurs",
                        principalColumn: "FournisseurId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Livraisons_Mouvements_MouvementId",
                        column: x => x.MouvementId,
                        principalTable: "Mouvements",
                        principalColumn: "MouvementId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SortiePourAjouts",
                columns: table => new
                {
                    MouvementId = table.Column<int>(type: "int", nullable: false),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    MotifAjout = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortiePourAjouts", x => x.MouvementId);
                    table.ForeignKey(
                        name: "FK_SortiePourAjouts_Demandes_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "Demandes",
                        principalColumn: "DemandeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SortiePourAjouts_Mouvements_MouvementId",
                        column: x => x.MouvementId,
                        principalTable: "Mouvements",
                        principalColumn: "MouvementId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$AgTCN7Z2MyJkNOpiAglk6uv4djoB4x5MfBBHo26py8dF38GpmDMaq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$4xQCRBAPLMHUB3ZxxMcuyeFuFLe8rm5aDolaUksBLUqt1Hqpe.OTW");

            migrationBuilder.CreateIndex(
                name: "IX_Livraisons_ApprovisionnementDemandeId",
                table: "Livraisons",
                column: "ApprovisionnementDemandeId");

            migrationBuilder.CreateIndex(
                name: "IX_Livraisons_FournisseurId",
                table: "Livraisons",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Mouvements_TypeMouvementId",
                table: "Mouvements",
                column: "TypeMouvementId");

            migrationBuilder.CreateIndex(
                name: "IX_SortiePourAjouts_DemandeId",
                table: "SortiePourAjouts",
                column: "DemandeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Decaissements");

            migrationBuilder.DropTable(
                name: "Livraisons");

            migrationBuilder.DropTable(
                name: "SortiePourAjouts");

            migrationBuilder.DropTable(
                name: "Mouvements");

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
    }
}
