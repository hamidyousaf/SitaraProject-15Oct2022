using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APGRNItem
    {
        [Key]
        public int Id { get; set; }

        //public int ReturnPurchaseNo { get; set; }
        //[Display(Name = "Return InvoiceItem Id")] public int ReturnInvoiceItemId { get; set; }
        //public int PurchaseOrderItemId { get; set; }
        //public int PurchaseId { get; set; }


        public int ItemId { get; set; }
        public int BrandId { get; set; }
        [Display(Name = "HS Code")] public string HSCode { get; set; }

        [Display(Name = "GRN Qty")] public decimal GRNQty { get; set; }
        [Display(Name = "Balance Qty")] public decimal BalanceQty { get; set; }

        [Display(Name = "Rejected Qty")] public decimal RejectedQty { get; set; }
        [Display(Name = "Accepted Qty")] public decimal AcceptedQty { get; set; }
        [Display(Name = "Purchase Qty")] public decimal PurchaseQty { get; set; }

        [Display(Name = "Rate")] public decimal Rate { get; set; }
        [Display(Name = "Total")] public decimal Total_ { get; set; }
        [Display(Name = "FCValue")] public decimal FCValue { get; set; }
        [Display(Name = "Expense")] public decimal Expense { get; set; }
        [Display(Name = "PKR Value")] public decimal PKRValue { get; set; }
        [Display(Name = "PKR Rate(Per Item)")] public decimal PKRRate { get; set; }

        //[Display(Name = "Line Total")] [Column(TypeName = "numeric(18,2)")] public decimal LineTotal { get; set; }
        public int DetailCostCenter { get; set; }


        //more fields



        [MaxLength(450)] public string CreatedBy { get; set; }
        [MaxLength(450)] public string UpdatedBy { get; set; }

        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }

        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }

        [DataType(DataType.Date)] public DateTime ExpiryDate { get; set; }
        public bool IsDeleted { get; set; }


        public int GRNID { get; set; }
        public int IRNItemId { get; set; }

        //navigation property
        public APPurchase Purchase { get; set; }
        public int CompanyId { get; set; }
        public int PRDetailId { get; set; }
        public int LocationId { get; set; }
        public int SaleTax { get; set; }
        public decimal SaleTaxAmount { get; set; }
        public decimal TotalValue { get; set; }

        //Expense Popup

        public APGRN GRN { get; set; }
        public InvItem Item { get; set; }
        public GLAccount ServiceAccount { get; set; }

        //public APPurchaseItem()
        //{
        //    ServiceAccountId = 0;
        //    PurchaseOrderItemId = 0;
        //}
    }
}
