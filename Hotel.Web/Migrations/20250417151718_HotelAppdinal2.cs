using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class HotelAppdinal2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetailsDemandes",
                columns: table => new
                {
                    DetailsDemandeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    Agent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateDemande = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TypeDemande = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EtatDemande = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProduitDemande = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QteDemande = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailsDemandes", x => x.DetailsDemandeId);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailsDemandes");

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
        }
    }
}
