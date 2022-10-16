using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class APPurchaseItemViewModel
    {
        public APPurchaseItemViewModel()
        {
            // CurrencyExchangeRate = 1;
        }
        //APPurchase
        public int Id { get; set; }
        [Display(Name = "Purchase No")] public int PurchaseNo { get; set; }
        public int PeriodId { get; set; }
        [Display(Name = "Supplier Name")] public int SupplierId { get; set; }
        [Display(Name = "Ware House")] public int WareHouseId { get; set; }
        public int BranchId { get; set; }
        public int VoucherId { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public int OperationId { get; set; }
        public int Resp_ID { get; set; }
        public int GRNItemId { get; set; }
        public int BalanceQty { get; set; }
        public string WareHose { get; set; }
        public string OperatingUnit { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal TotalSalesTaxAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal TotalExciseTaxAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal TotalDiscountAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }
        [Display(Name = "Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal PaymentTotal { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Transaction Type")] [MaxLength(3)] public string TransactionType { get; set; }
        [Display(Name = "Supplier Invoice No")] [MaxLength(50)] public string SupplierInvoiceNo { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "IGP No")] [MaxLength(50)] public string IGPNo { get; set; }
        [Display(Name = "Reference No")] [MaxLength(50)] public string ReferenceNo { get; set; }
        [Display(Name = "Status")] [MaxLength(10)] public string Status { get; set; }
        [Display(Name = "Supplier Invoice Date")] [DataType(DataType.Date)] public DateTime SupplierInvoiceDate { get; set; }
        [Display(Name = "Purchase Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime PurchaseDate { get; set; }
        public string PurchaseItems { get; set; }
        [MaxLength(450)] public string CreatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [MaxLength(450)] public string UpdatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime ApprovedDate { get; set; }

        public bool IsDeleted { get; set; }

        public APSupplier Supplier { get; set; }
        public AppCompanyConfig WareHouse { get; set; }

        public int SalesPersonId { get; set; }
        //APPurchaseItems
        public int PurchaseOrderItemId { get; set; }
        public int ServiceAccountId { get; set; }
        public int PurchaseId { get; set; }
        public int ItemId { get; set; }
        public int Sequence { get; set; }
        public int GRNNo { get; set; }
        public int PurchaseItemId { get; set; }


        [Display(Name = "Qty")] [Column(TypeName = "numeric(18,2)")] public decimal Qty { get; set; }

        [Display(Name = "Rate")] [Column(TypeName = "numeric(18,2)")] public decimal Rate { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,2)")] public decimal Total_ { get; set; }
        [Display(Name = "Tax")] public int TaxId { get; set; }

        [Display(Name = "Disc.%")] [Column(TypeName = "numeric(18,2)")] public decimal DiscountPercentage { get; set; }

        [Display(Name = "Disc. Amt")] [Column(TypeName = "numeric(18,2)")] public decimal DiscountAmount { get; set; }

        [Display(Name = "ST.%")] [Column(TypeName = "numeric(18,2)")] public decimal SalesTaxPercentage { get; set; }

        [Display(Name = "ST.Amt")] [Column(TypeName = "numeric(18,2)")] public decimal SalesTaxAmount { get; set; }

        [Display(Name = "SED.%")] [Column(TypeName = "numeric(18,2)")] public decimal ExciseTaxPercentage { get; set; }

        [Display(Name = "SED.Amt")] [Column(TypeName = "numeric(18,2)")] public decimal ExciseTaxAmount { get; set; }

        [Display(Name = "Line Total")] [Column(TypeName = "numeric(18,2)")] public decimal LineTotal { get; set; }
        public decimal Stock { get; set; }
        public decimal NetTotal { get; set; }
        public string PDate { get; set; }
        public string Suplier { get; set; }
        public int SubAccountId { get; set; }
        public int Department { get; set; }
        public int SubDepartment { get; set; }
        public int CostCenter { get; set; }
        [MaxLength(200)] public string ItemRemarks { get; set; }
        [DataType(DataType.Date)] public DateTime ExpiryDate { get; set; }


        public APPurchase Purchase { get; set; }
        // public APPurchaseItem APPurchaseItems { get; set; }
        public List<APPurchaseItem> APPurchaseItems { get; set; }
        public InvItem Item { get; set; }
        public List<AppTax> TaxList { get; set; }
        public List<AppCurrency> Currencies { get; set; }
    }
}
