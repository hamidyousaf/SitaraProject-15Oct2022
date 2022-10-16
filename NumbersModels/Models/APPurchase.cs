using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APPurchase
    {
        public int Id { get; set; }
        [Display(Name ="Purchase No")] public int PurchaseNo { get; set; }
        public int PeriodId { get; set; }
        [Display(Name = "Supplier")] public int SupplierId { get; set; }
        [Display(Name = "Ware House")] public int WareHouseId { get; set; }
        public int VoucherId { get; set; }
        public int CompanyId { get; set; }

        [Display(Name = "Sales Tax")] [Column(TypeName = "numeric(18,2)")] public decimal TotalSalesTaxAmount { get; set; }
        [Display(Name = "Excise Tax")] [Column(TypeName = "numeric(18,2)")] public decimal TotalExciseTaxAmount { get; set; }
        [Display(Name = "Total Discount Amount")] [Column(TypeName = "numeric(18,2)")] public decimal TotalDiscountAmount { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }
        [Display(Name = "Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "Payment Total")] [Column(TypeName = "numeric(18,2)")] public decimal PaymentTotal { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name ="Transaction Type")] [MaxLength(3)] public string TransactionType { get; set; }
       
        [Display(Name ="Supplier Invoice No")] [MaxLength(50)] public string SupplierInvoiceNo { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name ="IGP No")] [MaxLength(50)] public string IGPNo { get; set; }
        [Display(Name ="Reference No")] [MaxLength(50)] public string ReferenceNo { get; set; }
        [Display(Name = "Status")] [MaxLength(10)] public string Status { get; set; }
        [Display(Name ="Supplier Invoice Date")] [DataType(DataType.Date)] public DateTime SupplierInvoiceDate { get; set; }
        [Display(Name ="Purchase Date")] [DataType(DataType.Date)] public DateTime PurchaseDate { get; set; }

        [MaxLength(450)] public string CreatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [MaxLength(450)] public string UpdatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime? ApprovedDate { get; set; }

        public bool IsDeleted { get; set; }
        public int DepartmentId { get; set; }
        public int OperationId { get; set; }
        public int Resp_ID { get; set; }

        public APSupplier Supplier { get; set; }
        public AppCompanyConfig WareHouse { get; set; }

        public APPurchase()
        {
            Currency = "PKR";
            CurrencyExchangeRate = 1;
        }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
    }
}
