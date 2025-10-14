using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class BingaStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Demandes",
                columns: table => new
                {
                    DemandeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    DateDemande = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TypeDemande = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demandes", x => x.DemandeId);
                    table.ForeignKey(
                        name: "FK_Demandes_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ajouts",
                columns: table => new
                {
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    EtatDemandeAjout = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ajouts", x => x.DemandeId);
                    table.ForeignKey(
                        name: "FK_Ajouts_Demandes_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "Demandes",
                        principalColumn: "DemandeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Approvisionnements",
                columns: table => new
                {
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    EtatDemandeAppro = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approvisionnements", x => x.DemandeId);
                    table.ForeignKey(
                        name: "FK_Approvisionnements_Demandes_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "Demandes",
                        principalColumn: "DemandeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comporters",
                columns: table => new
                {
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    AlimentId = table.Column<int>(type: "int", nullable: false),
                    QteOrdone = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comporters", x => new { x.DemandeId, x.AlimentId });
                    table.ForeignKey(
                        name: "FK_Comporters_Aliments_AlimentId",
                        column: x => x.AlimentId,
                        principalTable: "Aliments",
                        principalColumn: "AlimentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comporters_Demandes_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "Demandes",
                        principalColumn: "DemandeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comprendres",
                columns: table => new
                {
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    BoissonId = table.Column<int>(type: "int", nullable: false),
                    QteDemande = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comprendres", x => new { x.DemandeId, x.BoissonId });
                    table.ForeignKey(
                        name: "FK_Comprendres_Boissons_BoissonId",
                        column: x => x.BoissonId,
                        principalTable: "Boissons",
                        principalColumn: "BoissonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comprendres_Demandes_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "Demandes",
                        principalColumn: "DemandeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$Key/W6BLKUITllz5.B8dxOJ9K2ukuapMDgy0Mxhl/BmkLEuEXIlGi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$z1s8as2hA0GH2nBmA6SYXeoTwdN9jfgRan5vXuerQI6ec4fPGauGi");

            migrationBuilder.CreateIndex(
                name: "IX_Comporters_AlimentId",
                table: "Comporters",
                column: "AlimentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comprendres_BoissonId",
                table: "Comprendres",
                column: "BoissonId");

            migrationBuilder.CreateIndex(
                name: "IX_Demandes_AgentId",
                table: "Demandes",
                column: "AgentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ajouts");

            migrationBuilder.DropTable(
                name: "Approvisionnements");

            migrationBuilder.DropTable(
                name: "Comporters");

            migrationBuilder.DropTable(
                name: "Comprendres");

            migrationBuilder.DropTable(
                name: "Demandes");

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
    }
}
