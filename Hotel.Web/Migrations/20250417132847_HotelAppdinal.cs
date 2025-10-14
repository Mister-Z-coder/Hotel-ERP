using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class HotelAppdinal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livraisons_Approvisionnements_ApprovisionnementDemandeId",
                table: "Livraisons");

            migrationBuilder.DropForeignKey(
                name: "FK_SortiePourAjouts_Demandes_DemandeId",
                table: "SortiePourAjouts");

            migrationBuilder.DropIndex(
                name: "IX_Livraisons_ApprovisionnementDemandeId",
                table: "Livraisons");

            migrationBuilder.DropColumn(
                name: "ApprovisionnementDemandeId",
                table: "Livraisons");

            migrationBuilder.RenameColumn(
                name: "DemandeId",
                table: "SortiePourAjouts",
                newName: "AjoutId");

            migrationBuilder.RenameIndex(
                name: "IX_SortiePourAjouts_DemandeId",
                table: "SortiePourAjouts",
                newName: "IX_SortiePourAjouts_AjoutId");

            migrationBuilder.RenameColumn(
                name: "DemandeId",
                table: "Livraisons",
                newName: "ApprovisionnementId");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$/t0SMDlVecWkgJ1xXz8RS.d2pULCV7gExKZI5oyMFcrqQDWxuwI/O");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$2uGbJzZvF17/VRFIj0GseeHhe.tNc13v51UwZ.rud1pdF51LsKnA.");

            migrationBuilder.CreateIndex(
                name: "IX_Livraisons_ApprovisionnementId",
                table: "Livraisons",
                column: "ApprovisionnementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Livraisons_Approvisionnements_ApprovisionnementId",
                table: "Livraisons",
                column: "ApprovisionnementId",
                principalTable: "Approvisionnements",
                principalColumn: "DemandeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SortiePourAjouts_Ajouts_AjoutId",
                table: "SortiePourAjouts",
                column: "AjoutId",
                principalTable: "Ajouts",
                principalColumn: "DemandeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livraisons_Approvisionnements_ApprovisionnementId",
                table: "Livraisons");

            migrationBuilder.DropForeignKey(
                name: "FK_SortiePourAjouts_Ajouts_AjoutId",
                table: "SortiePourAjouts");

            migrationBuilder.DropIndex(
                name: "IX_Livraisons_ApprovisionnementId",
                table: "Livraisons");

            migrationBuilder.RenameColumn(
                name: "AjoutId",
                table: "SortiePourAjouts",
                newName: "DemandeId");

            migrationBuilder.RenameIndex(
                name: "IX_SortiePourAjouts_AjoutId",
                table: "SortiePourAjouts",
                newName: "IX_SortiePourAjouts_DemandeId");

            migrationBuilder.RenameColumn(
                name: "ApprovisionnementId",
                table: "Livraisons",
                newName: "DemandeId");

            migrationBuilder.AddColumn<int>(
                name: "ApprovisionnementDemandeId",
                table: "Livraisons",
                type: "int",
                nullable: true);

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
                name: "IX_Livraisons_ApprovisionnementDemandeId",
                table: "Livraisons",
                column: "ApprovisionnementDemandeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Livraisons_Approvisionnements_ApprovisionnementDemandeId",
                table: "Livraisons",
                column: "ApprovisionnementDemandeId",
                principalTable: "Approvisionnements",
                principalColumn: "DemandeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SortiePourAjouts_Demandes_DemandeId",
                table: "SortiePourAjouts",
                column: "DemandeId",
                principalTable: "Demandes",
                principalColumn: "DemandeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
