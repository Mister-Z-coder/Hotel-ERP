using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingahousesdl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$lVXa02EpnNO5DnpqKts2jO4BgzTIGV0Z4egYRp4Brwr.ddbwxx0Lm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$KG8HXbvzKepvUEl7molVBO/TguIpptE/GA0e2hspB7S8Ve/Ic8ZH.");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
