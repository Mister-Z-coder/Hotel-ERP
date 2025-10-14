using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingahousey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReglementId",
                table: "DetailsReglements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$Tr7crGJTjDRuQW6W4LdAseQQHbpzsh3h/ECDKw2VVfx3O1/Ec/bpG");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$bokigNk9ZX4M3/GHvxYlL.vPi0D5wLO6hb2qbbSuHdoZiUE0PyZka");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReglementId",
                table: "DetailsReglements");

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
    }
}
