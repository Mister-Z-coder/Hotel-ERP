using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class BingahouseFactureConcept : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$rulvuymGrbwpUMgxifziN.q8bYuE7/wiY5CHyMtmWpnMIhj2K2H1O");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$7rRkCaCRqTqCro/Xx.NuGO1IH5sqr14B66NvPldDMREGnHsA2/x56");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$tM/tnREddxF4gkROHCwWZ.P.sqWEYXjyY0MUqIevgWgK/vUR8u9cW");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$DrIEyroPWjDSKwSjOw4bauk9zcbeyE9KBxNfdp4lIMfJq2ahTxlLS");
        }
    }
}
