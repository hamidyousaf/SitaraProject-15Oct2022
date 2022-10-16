using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Numbers.Migrations
{
    public partial class BkgReceiptPaymentTblUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditToAccount",
                table: "BkgReceipts");

            migrationBuilder.DropColumn(
                name: "DebitAccount",
                table: "BkgPayments");

            migrationBuilder.AddColumn<bool>(
                name: "AdvancePurchase",
                table: "BkgCashPurchaseSales",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BkgCashPurchaseSales",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvancePurchase",
                table: "BkgCashPurchaseSales");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BkgCashPurchaseSales");

            migrationBuilder.AddColumn<bool>(
                name: "CreditToAccount",
                table: "BkgReceipts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DebitAccount",
                table: "BkgPayments",
                nullable: false,
                defaultValue: false);
        }
    }
}
