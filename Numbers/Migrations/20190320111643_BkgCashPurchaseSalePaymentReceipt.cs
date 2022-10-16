using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Numbers.Migrations
{
    public partial class BkgCashPurchaseSalePaymentReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Atif added below script
    //        migrationBuilder.CreateTable(
    //name: "ARInvoices",
    //columns: table => new
    //{
    //    Id = table.Column<int>(nullable: false)
    //        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
    //    InvoiceNo = table.Column<int>(nullable: false),
    //    WareHouse = table.Column<int>(nullable: false),
    //    VoucherId = table.Column<int>(nullable: false),
    //    PeriodId = table.Column<int>(nullable: false),
    //    ItemRateId = table.Column<int>(nullable: false),
    //    CompanyId = table.Column<int>(nullable: false),
    //    CustomerId = table.Column<int>(nullable: false),
    //    SalesTaxPercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    SalesTaxAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    ExciseTaxPercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    ExciseTaxAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    Cash = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    Change = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    FreightAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    InvoiceAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    CommissionPercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    CommissionAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    DiscountPercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    CurrencyExchangeRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
    //    UpdatedBy = table.Column<string>(maxLength: 450, nullable: true),
    //    ApprovedBy = table.Column<string>(maxLength: 450, nullable: true),
    //    Remarks = table.Column<string>(maxLength: 200, nullable: true),
    //    TransactionType = table.Column<string>(maxLength: 3, nullable: true),
    //    Currency = table.Column<string>(maxLength: 3, nullable: true),
    //    OGPNo = table.Column<string>(maxLength: 50, nullable: true),
    //    ReferenceNo = table.Column<string>(maxLength: 30, nullable: true),
    //    CustomerPONo = table.Column<string>(maxLength: 50, nullable: true),
    //    Vehicle = table.Column<string>(maxLength: 50, nullable: true),
    //    Status = table.Column<string>(maxLength: 10, nullable: true),
    //    Location = table.Column<string>(maxLength: 50, nullable: true),
    //    OldNo = table.Column<string>(maxLength: 50, nullable: true),
    //    BookingNo = table.Column<string>(maxLength: 50, nullable: true),
    //    Name = table.Column<string>(maxLength: 2000, nullable: true),
    //    CustomerInvAddress = table.Column<string>(maxLength: 2000, nullable: true),
    //    SalesMan = table.Column<string>(maxLength: 450, nullable: true),
    //    SaleMan = table.Column<string>(maxLength: 2, nullable: true),
    //    InvoiceType = table.Column<string>(maxLength: 10, nullable: true),
    //    InvoiceDate = table.Column<DateTime>(nullable: false),
    //    CreatedDate = table.Column<DateTime>(nullable: false),
    //    UpdatedDate = table.Column<DateTime>(nullable: false),
    //    ApprovedDate = table.Column<DateTime>(nullable: false),
    //    InvoiceDueDate = table.Column<DateTime>(nullable: false),
    //    IsDeleted = table.Column<bool>(nullable: false),
    //    IsClosed = table.Column<bool>(nullable: false)
    //},
    //constraints: table =>
    //{
    //    table.PrimaryKey("PK_ARInvoices", x => x.Id);
    //    table.ForeignKey(
    //        name: "FK_ARInvoices_ARCustomers_CustomerId",
    //        column: x => x.CustomerId,
    //        principalTable: "ARCustomers",
    //        principalColumn: "Id",
    //        onDelete: ReferentialAction.Cascade);
    //});
    //        //Atif added below
    //        migrationBuilder.CreateTable(
    //name: "ARInvoiceItems",
    //columns: table => new
    //{
    //    Id = table.Column<int>(nullable: false)
    //        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
    //    InvoiceId = table.Column<int>(nullable: false),
    //    ItemId = table.Column<int>(nullable: false),
    //    SalesOrderItemId = table.Column<int>(nullable: false),
    //    Stock = table.Column<int>(nullable: false),
    //    UOM = table.Column<int>(nullable: false),
    //    TaxSlab = table.Column<int>(nullable: false),
    //    RateEnt = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
    //    Rate = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
    //    Qty = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    IssueRate = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
    //    CostofSales = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
    //    SalesTaxPercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    SalesTaxAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    ExciseTaxPercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    ExciseTaxAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    DiscountPercentage = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
    //    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    NetValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    SaleQty = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    Bonus = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
    //    MarketDescription = table.Column<string>(maxLength: 150, nullable: true),
    //    Remarks = table.Column<string>(maxLength: 100, nullable: true)
    //},
    //constraints: table =>
    //{
    //    table.PrimaryKey("PK_ARInvoiceItems", x => x.Id);
    //    table.ForeignKey(
    //        name: "FK_ARInvoiceItems_ARInvoices_InvoiceId",
    //        column: x => x.InvoiceId,
    //        principalTable: "ARInvoices",
    //        principalColumn: "Id",
    //        onDelete: ReferentialAction.Cascade);
    //    table.ForeignKey(
    //        name: "FK_ARInvoiceItems_InvItems_ItemId",
    //        column: x => x.ItemId,
    //        principalTable: "InvItems",
    //        principalColumn: "Id",
    //        onDelete: ReferentialAction.Cascade);
    //});
            migrationBuilder.CreateTable(
                name: "BkgCashPurchaseSalePayments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    PaymentAmount = table.Column<decimal>(nullable: false),
                    Reference = table.Column<string>(maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(maxLength: 500, nullable: true),
                    GLAccountId = table.Column<int>(nullable: false),
                    BkgCashPurchaseSaleId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BkgCashPurchaseSalePayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BkgCashPurchaseSaleReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReceiptDate = table.Column<DateTime>(nullable: false),
                    ReceiptAmount = table.Column<decimal>(nullable: false),
                    Reference = table.Column<string>(maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(maxLength: 500, nullable: true),
                    GLAccountId = table.Column<int>(nullable: false),
                    BkgCashPurchaseSaleId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BkgCashPurchaseSaleReceipts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BkgCashPurchaseSalePayments");

            migrationBuilder.DropTable(
                name: "BkgCashPurchaseSaleReceipts");
        }
    }
}
