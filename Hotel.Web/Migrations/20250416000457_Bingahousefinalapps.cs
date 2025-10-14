using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingahousefinalapps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Decaissements");

            migrationBuilder.CreateTable(
                name: "Declassements",
                columns: table => new
                {
                    MouvementId = table.Column<int>(type: "int", nullable: false),
                    MotifDecaissement = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Declassements", x => x.MouvementId);
                    table.ForeignKey(
                        name: "FK_Declassements_Mouvements_MouvementId",
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
                value: "$2a$12$wwYbfCb8YWD0hrgGVSNoKe61y92wxYH2hJ6Ad60jZDXATqFmg5UmC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$.uES6K2dcPOP95elUL7/TOD5k.oawnd79txJiX7BnI4cl8EElreN2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Declassements");

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
        }
    }
}
