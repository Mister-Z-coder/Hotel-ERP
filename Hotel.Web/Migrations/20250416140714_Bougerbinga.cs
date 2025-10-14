using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bougerbinga : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bougers",
                columns: table => new
                {
                    MouvementId = table.Column<int>(type: "int", nullable: false),
                    BoissonId = table.Column<int>(type: "int", nullable: false),
                    QteBouger = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bougers", x => new { x.MouvementId, x.BoissonId });
                    table.ForeignKey(
                        name: "FK_Bougers_Boissons_BoissonId",
                        column: x => x.BoissonId,
                        principalTable: "Boissons",
                        principalColumn: "BoissonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bougers_Mouvements_MouvementId",
                        column: x => x.MouvementId,
                        principalTable: "Mouvements",
                        principalColumn: "MouvementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mouvoirs",
                columns: table => new
                {
                    MouvementId = table.Column<int>(type: "int", nullable: false),
                    AlimentId = table.Column<int>(type: "int", nullable: false),
                    QteMouv = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mouvoirs", x => new { x.MouvementId, x.AlimentId });
                    table.ForeignKey(
                        name: "FK_Mouvoirs_Aliments_AlimentId",
                        column: x => x.AlimentId,
                        principalTable: "Aliments",
                        principalColumn: "AlimentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mouvoirs_Mouvements_MouvementId",
                        column: x => x.MouvementId,
                        principalTable: "Mouvements",
                        principalColumn: "MouvementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$b4IvD6n6jDfScYUrdWiL1eeVWxP51.6O/1eMbqBpgSt1nfbzS4hxi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$Og6EldPnA6p.AbzLipQH2.ZPbjMtDF6z4Pwi5sNxVo4ic0Vy90B/a");

            migrationBuilder.CreateIndex(
                name: "IX_Mouvements_AgentId",
                table: "Mouvements",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bougers_BoissonId",
                table: "Bougers",
                column: "BoissonId");

            migrationBuilder.CreateIndex(
                name: "IX_Mouvoirs_AlimentId",
                table: "Mouvoirs",
                column: "AlimentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mouvements_Agents_AgentId",
                table: "Mouvements",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "AgentId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mouvements_Agents_AgentId",
                table: "Mouvements");

            migrationBuilder.DropTable(
                name: "Bougers");

            migrationBuilder.DropTable(
                name: "Mouvoirs");

            migrationBuilder.DropIndex(
                name: "IX_Mouvements_AgentId",
                table: "Mouvements");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$wwYbfCb8YWD0hrgGVSNoKe61y92wxYH2hJ6Ad60jZDXATqFmg5UmC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$.uES6K2dcPOP95elUL7/TOD5k.oawnd79txJiX7BnI4cl8EElreN2");
        }
    }
}
