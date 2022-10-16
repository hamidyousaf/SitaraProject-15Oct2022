using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APPurchaseItem
    {
        public int Id { get; set; }

        public int ReturnPurchaseNo { get; set; }
        [Display(Name = "Return InvoiceItem Id")] public int ReturnInvoiceItemId { get; set; }
        //[Display(Name = "ItemCode")] public string ItemCode { get; set; }
        //[Display(Name = "ItemDescription")] public string ItemDescription { get; set; }
        public int PurchaseOrderItemId { get; set; }
        public int PurchaseId { get; set; }
        public int ItemId { get; set; }
        public int Sequence { get; set; }
        public int PurchaseItemId { get; set; }
        public int ServiceAccountId { get; set; }
        public int GRNItemId { get; set; }
        public int SubAccountId { get; set; }
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
        public int CostCneterId { get; set; }
        public decimal BalanceQty { get; set; }
        [Display(Name = "Invoice Qty")] [Column(TypeName = "numeric(18,4)")] public decimal InvoiceQty { get; set; }
        [Display(Name = "Qty")] [Column(TypeName = "numeric(18,4)")] public decimal Qty { get; set; }

        [Display(Name = "Rate")] [Column(TypeName = "numeric(18,4)")] public decimal Rate { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,4)")]  public decimal Total { get; set; }

        [Display(Name = "Tax")] public int TaxId { get; set; }
        public int PrDetailId { get; set; }
        public int BrandId { get; set; }

        [Display(Name = "Disc.%")] [Column(TypeName = "numeric(18,2)")] public decimal DiscountPercentage { get; set; }

        [Display(Name = "Disc. Amt")] [Column(TypeName = "numeric(18,2)")] public decimal DiscountAmount { get; set; }

        [Display(Name = "ST.%")] [Column(TypeName = "numeric(18,2)")] public decimal SalesTaxPercentage { get; set; }

        [Display(Name = "ST.Amt")]  [Column(TypeName = "numeric(18,2)")] public decimal SalesTaxAmount { get; set; }

        [Display(Name = "SED.%")] [Column(TypeName = "numeric(18,2)")] public decimal ExciseTaxPercentage { get; set; }
       
        [Display(Name = "SED.Amt")] [Column(TypeName = "numeric(18,2)")] public decimal ExciseTaxAmount { get; set; }

        [Display(Name = "Line Total")] [Column(TypeName = "numeric(18,4)")] public decimal LineTotal { get; set; }

       

        [MaxLength(200)] public string Remarks { get; set; }

        [MaxLength(450)] public string CreatedBy { get; set; }
        [MaxLength(450)] public string UpdatedBy { get; set; }

        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }

        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }

        [DataType(DataType.Date)] public DateTime ExpiryDate { get; set; }
        public bool IsDeleted { get; set; }

        //navigation property
        public APPurchase Purchase { get; set; }
        public InvItem Item { get; set; }
        public GLAccount ServiceAccount { get; set; }

        public APPurchaseItem()
        {
            ServiceAccountId = 0;
            PurchaseOrderItemId = 0;
        }
    }
}
