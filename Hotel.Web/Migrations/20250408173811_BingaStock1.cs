using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class BingaStock1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EtatDemande",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$e.daCaokk6pm8DZelPDn6.ppo7IDUdoOqhjE8EJKpBT4pSKw2vKYq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$CpanaLE.lMvJrRqw7g4CUu/M7A8xx0/QbSgipJpXYMrhhyNjEvw2y");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EtatDemande",
                table: "Demandes");

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
        }
    }
}
