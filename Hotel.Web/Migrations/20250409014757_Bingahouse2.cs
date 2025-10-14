using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingahouse2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UniteBoisson",
                table: "Boissons",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$/RhdhdztA7DudF3N5Wj8U.gVoHeD0GyfhKhZPHG7puroDUcPT0eh.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$13PLE6dnnTzqybLn.zcou.MYBN3lGaCN5j6vreul4UJ2rZiIlhlI.");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UniteBoisson",
                table: "Boissons",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
    }
}
