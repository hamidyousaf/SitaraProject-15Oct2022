using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ARInvoiceViewModel
    {
        public ARInvoiceViewModel()
        {
            CurrencyExchangeRate = 1;
        }
        //ARInvoice
        public int Id { get; set; }


         
        [Display(Name = "Invoice No")] public int InvoiceNo { get; set; }
        [Display(Name = "DC No")] public string DCNo { get; set; }
        [Display(Name = "Ware House")] public int WareHouseId { get; set; }
        [Display(Name = "Voucher Id")] public int VoucherId { get; set; }
        [Display(Name = "Period Id")] public int PeriodId { get; set; }
        [Display(Name = "Item Rate Id")] public int ItemRateId { get; set; }
        public int ServiceAccountId { get; set; }

        public string BaleType { get; set; }

        public int CompanyId { get; set; }
        [Display(Name = "Customer")] public int CustomerId { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalSaleSTaxAmount { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Cash { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Change { get; set; }
        [Display(Name = "Freight Amount")] [Column(TypeName = "numeric(18,2)")] public decimal FreightAmount { get; set; }
        [Display(Name = "Invoice Amount")] [Column(TypeName = "numeric(18,2)")] public decimal InvoiceAmount { get; set; }
        [Display(Name = "Commission Percentage")] [Column(TypeName = "numeric(18,2)")] public decimal CommissionPercentage { get; set; }
        [Display(Name = "Commission Amount")] [Column(TypeName = "numeric(18,2)")] public decimal CommissionAmount { get; set; }
        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
        [Display(Name = "Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Transaction Type")] [MaxLength(15)] public string TransactionType { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "OGP No")] [MaxLength(50)] public string OGPNo { get; set; }
        [Display(Name = "Reference No")] [MaxLength(30)] public string ReferenceNo { get; set; }
        [Display(Name = "Customer P.O No")] [MaxLength(50)] public string CustomerPONo { get; set; }
        [Display(Name = "Vehicle")] [MaxLength(50)] public string Vehicle { get; set; }
        [Display(Name = "Status")] [MaxLength(10)] public string Status { get; set; }
        [Display(Name = "Location")] [MaxLength(50)] public string Location { get; set; }
        [Display(Name = "Old No")] [MaxLength(50)] public string OldNo { get; set; }
        [Display(Name = "Booking No")] [MaxLength(50)] public string BookingNo { get; set; }
        [Display(Name = "Name")] [MaxLength(2000)] public string Name { get; set; }
        [Display(Name = "Customer Invoice Address")] [MaxLength(2000)] public string CustomerInvAddress { get; set; }
        [Display(Name = "Sales Man")] [MaxLength(450)] public string SalesMan { get; set; }
        [Display(Name = "Sale Man")] [MaxLength(2)] public string SaleMan { get; set; }
        [Display(Name = "Invoice Type")] [MaxLength(10)] public string InvoiceType { get; set; }

        [Display(Name = "Invoice Date")] [DataType(DataType.Date)] public DateTime InvoiceDate { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [DataType(DataType.Date)] public DateTime ApprovedDate { get; set; }
        [Display(Name = "Invoice Due Date")] [DataType(DataType.Date)] public DateTime InvoiceDueDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsClosed { get; set; }

        public ARCustomer Customer { get; set; }

        //ARInvoiceItems
        public int InvoiceId { get; set; }
        [Display(Name = "Item")] public int ItemId { get; set; }
        public int SalesOrderId { get; set; }
        public int SalesOrderItemId { get; set; }
        public int DCItemId { get; set; }
        [Display(Name = "Stock")] public decimal Stock { get; set; }
        public int UOM { get; set; }
        public string UOMName { get; set; }
        [Display(Name = "Tax")] public int TaxSlab { get; set; }
        public int InvoiceItemId { get; set; }

        [Display(Name = "Rate Ent")] [Column(TypeName = "numeric(18,4)")] public decimal RateEnt { get; set; }
        [Display(Name = "Rate")] [Column(TypeName = "numeric(18,4)")] public decimal Rate { get; set; }
        [Display(Name = "Qty")] [Column(TypeName = "numeric(18,2)")] public decimal Qty { get; set; }
        [Display(Name = "Lot No")] public string BaleNumber { get; set; }
        [Display(Name = "Issue Rate")] [Column(TypeName = "numeric(18,6)")] public decimal IssueRate { get; set; }
        [Display(Name = "Cost of Sales")] [Column(TypeName = "numeric(18,4)")] public decimal CostofSales { get; set; }
        [Display(Name = "ST.%")] [Column(TypeName = "numeric(18,2)")] public decimal SalesTaxPercentage { get; set; }
        [Display(Name = "ST. Amt")] [Column(TypeName = "numeric(18,2)")] public decimal SalesTaxAmount { get; set; }
        [Display(Name = "SED.%")] [Column(TypeName = "numeric(18,2)")] public decimal ExciseTaxPercentage { get; set; }
        [Display(Name = "SED. Amt")] [Column(TypeName = "numeric(18,2)")] public decimal ExciseTaxAmount { get; set; }
        [Display(Name = "Disc.%")] [Column(TypeName = "numeric(18,4)")] public decimal DiscountPercentage { get; set; }
        [Display(Name = "Disc. Amt")] [Column(TypeName = "numeric(18,2)")] public decimal DiscountAmount { get; set; }
        [Display(Name = "Line Total")] [Column(TypeName = "numeric(18,2)")] public decimal LineTotal { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,2)")] public decimal Total_ { get; set; }
        [Display(Name = "Sale Qty")] [Column(TypeName = "numeric(18,2)")] public decimal SaleQty { get; set; }
        [Display(Name = "Bonus")] [Column(TypeName = "numeric(18,2)")] public decimal Bonus { get; set; }
        [Display(Name = "Market Description")] [MaxLength(150)] public string MarketDescription { get; set; } 
        [Display(Name = "Remarks")] [MaxLength(100)] public string InvoiceItemRemarks { get; set; }
        public int DetailCostCenter { get; set; }


        public int CostCenter { get; set; }
        public string InvoiceItems { get; set; }
        public string ItemDescription { get; set; }
        public decimal SaleOrderQty { get; set; }
        public decimal Balance { get; set; }
        public string UnitName { get; set; }
        public DateTime DeliveryDate { get; set; }
        public InvItem Item { get; set; }
        public ARInvoice Invoice { get; set; }
        public AppTax Tax { get; set; }
        public List<BaleInformation> BaleInformation { get; set; }

        public List<AppTax> TaxList { get; set; }

        public List<CostCenter> CostCenterList { get; set; }
        public List<AppCurrency> Currencies { get; set; } 
        public string CustomerName { get; set; }
        public string InvDate { get; set; }
        public string ItemBrand { get; set; }
        public string DueDate { get; set; }
        public string SaleOrderDate { get; set; }
        public string ItemCode { get; set; }
        public int BaleId { get; set; }
        public int? AvailableStock { get; set; }
        public int? BalanceStock { get; set; }
        public int DeliveryChallanId { get; set; }
        [Display(Name = "Mtrs/Bale")]
        public decimal? Meters { get; set; }
        [Display(Name ="T. Mtr")]
        public decimal? TotalMeter { get; set; }
        [Display(Name ="T. Mtr Amt")]
        public decimal? TotalMeterAmount { get; set; }
    }
}