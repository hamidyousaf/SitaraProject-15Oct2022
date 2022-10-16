using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APPurchaseReturnItem
    {
        public int Id { get; set; }
        [Display(Name = "Purchase Return Id")] public int PurchaseReturnId { get; set; }
        [Display(Name = "Sequence No")] public Int16 SequenceNo { get; set; }
        [Display(Name = "Item")] public int ItemId { get; set; }
        [Required] [Display(Name = "Qty")] public decimal Qty { get; set; }
        [Required] [Display(Name = "Invoice Qty")] public decimal InvoiceQty { get; set; }
        [Required] [Display(Name = "Rate")] public decimal Rate { get; set; }
        [Display(Name = "Total")] public decimal Total { get; set; }
        [Display(Name = "Disc.%")] [Column(TypeName = "numeric(5, 2)")] public decimal DiscountPercentage { get; set; }
        [Display(Name = "Discount Amount")] public decimal DiscountAmount { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }

        [Display(Name = "Created By")] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        //navigation property
        public APPurchaseReturn PurchaseReturn { get; set; }
        public InvItem Item { get; set; }
        public int ORG_ID { get; set; }
        public int DEPT_ID { get; set; }
        public int AREA_ID { get; set; }
    }
}
