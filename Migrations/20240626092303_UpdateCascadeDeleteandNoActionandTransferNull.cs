using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialManagementMVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeDeleteandNoActionandTransferNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_BankAccounts_FromAccountId",
                table: "Transfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_BankAccounts_ToAccountId",
                table: "Transfers");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_BankAccounts_FromAccountId",
                table: "Transfers",
                column: "FromAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_BankAccounts_ToAccountId",
                table: "Transfers",
                column: "ToAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_BankAccounts_FromAccountId",
                table: "Transfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_BankAccounts_ToAccountId",
                table: "Transfers");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_BankAccounts_FromAccountId",
                table: "Transfers",
                column: "FromAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_BankAccounts_ToAccountId",
                table: "Transfers",
                column: "ToAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
