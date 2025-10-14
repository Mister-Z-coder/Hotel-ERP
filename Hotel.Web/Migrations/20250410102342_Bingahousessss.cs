using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingahousessss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$liXeExK5c5a3s.eNko89FulhANys.RLPAmAKq92AyzSIJdaRrM1xu");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$X6418YUnJNVRxa8w8MoOU.4..EcEHaGtRdCGcyy8rqco4HLDda8s.");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$aiD4IOBec./PoFJjRci53eKUXgUDQFTHnVaLdnHEH.fkS9TvO/ldS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$g5bbdJZ8Igo2lQWfj6.58eTO2FeWMHMCO7iMtqCMGbG8dW.fjDmc2");
        }
    }
}
