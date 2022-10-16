using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ARSaleReturnViewModel
    {
        public ARSaleReturnViewModel()
        {
            CurrencyExchangeRate = 1;
        }
        //ARSaleReturn
        public int Id { get; set; }
        [Display(Name = "Return No.")] public int ReturnNo { get; set; }
        [Display(Name = "Return Date")] [DataType(DataType.Date)] public DateTime ReturnDate { get; set; }
        public int PeriodId { get; set; }
        public int VoucherId { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "Ware House")] public int WareHouseId { get; set; }
        [Display(Name = "Customer")] public int CustomerId { get; set; }
        [Display(Name = "Customer")] [MaxLength(10)] public string Status { get; set; }
        [Display(Name = "Reference No")] [MaxLength(25)] public string ReferenceNo { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
        //[Display(Name = "Total Discount")] [Column(TypeName = "numeric(18,2)")] public decimal TotalDiscountAmount { get; set; }
        [Display(Name = "Total Sales Tax")] [Column(TypeName = "numeric(18,2)")] public decimal TotalSalesTaxAmount { get; set; }
        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }
        public int CompanyId { get; set; }
        public int ResponsibilityId { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime ApprovedDate { get; set; }
        public bool IsDeleted { get; set; }

        //navigation property
        public ARCustomer Customer { get; set; }
        public AppPeriod Period { get; set; }

        //ARSaleReturnItems
        [Display(Name = "Inv. No.")] public int InvoiceNo { get; set; }
        [Display(Name = "Sale Return Id")] public int SaleReturnId { get; set; }
        [Display(Name = "Invoice Id")] public int InvoiceItemId { get; set; }
        [Display(Name = "SR Item")] public int SRItemId { get; set; }
        [Display(Name = "Sequence No")] public Int16 SequenceNo { get; set; }
        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "UOM")] public string ItemName { get; set; }
        [Display(Name = "UOM")] public string ItemCode { get; set; }
        [Display(Name = "UOM")] public string UOM { get; set; }
        [Display(Name = "Tax")] public int TaxId { get; set; }
        [Required] [Display(Name = "Qty")] public decimal Qty { get; set; }
        [Required] [Display(Name = "Stock")] public decimal Stock { get; set; }
        [Required] [Display(Name = "Inv. Qty")] public decimal InvoiceQty { get; set; }
        [Required] [Display(Name = "Rate")] public decimal Rate { get; set; }
        [Required] [Display(Name = "Rate")] public decimal Amount { get; set; }
        [Display(Name = "Total")] public decimal Total_ { get; set; }
        //[Display(Name = "Disc.%")] [Column(TypeName = "numeric(5, 2)")] public decimal DiscountPercentage { get; set; }
        //[Display(Name = "Discount Amount")] public decimal DiscountAmount { get; set; }
        [Display(Name = "S.Tax %")] [Column(TypeName = "numeric(18,2)")] public decimal SalesTaxPercentage { get; set; }
        [Display(Name = "S.Tax Amount")] public decimal SalesTaxAmount { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }
        [Display(Name = "Issue Rate")] [Column(TypeName = "numeric(18,6)")] public decimal IssueRate { get; set; }
        [Display(Name = "Cost of Sales")] [Column(TypeName = "numeric(18,4)")] public decimal CostofSales { get; set; }

        //navigation property
        public ARSaleReturn SaleReturn { get; set; }
        public InvItem Item { get; set; }
        public List<AppCurrency> Currencies { get; set; }
        public List<AppTax> TaxList { get; set; }
    }
}
