using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class ARInvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string DCNo { get; set; }
        public int ReturnInvoiceItemId { get; set; }
        public int ServiceAccountId { get; set; }
        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "Return Invoice No.")] public int ReturnInvoiceNo { get; set; }
        public int SalesOrderItemId { get; set; }
        [ForeignKey("ARDeliveryChallanItem")]
        public int DCItemId { get; set; }
        [Display(Name = "Stock")] public decimal Stock { get; set; }
        [Display(Name = "Tax")] public int TaxSlab { get; set; }

        [Display(Name = "Rate Ent")] [Column(TypeName = "numeric(18,4)")] public decimal RateEnt { get; set; }
        [Display(Name = "Rate")] [Column(TypeName = "numeric(18,4)")] public decimal Rate { get; set; }
        [Display(Name = "Qty")] [Column(TypeName = "numeric(18,2)")] public decimal Qty { get; set; }
        [Display(Name = "Invoice Qty")] [Column(TypeName = "numeric(18,2)")] public decimal InvoiceQty { get; set; }
        [Display(Name = "Issue Rate")] [Column(TypeName = "numeric(18,6)")] public decimal IssueRate { get; set; }
        [Display(Name = "Cost of Sales")] [Column(TypeName = "numeric(18,4)")] public decimal CostofSales { get; set; }
        [Display(Name = "ST.%")] [Column(TypeName = "numeric(18,2)")] public decimal SalesTaxPercentage { get; set; }
        [Display(Name = "ST. Amt")] [Column(TypeName = "numeric(18,2)")] public decimal SalesTaxAmount { get; set; }
        [Display(Name = "SED.%")] [Column(TypeName = "numeric(18,2)")] public decimal ExciseTaxPercentage { get; set; }
        [Display(Name = "SED. Amt")] [Column(TypeName = "numeric(18,2)")] public decimal ExciseTaxAmount { get; set; }
        [Display(Name = "Disc.%")] [Column(TypeName = "numeric(18,4)")] public decimal DiscountPercentage { get; set; }
        [Display(Name = "Disc. Amt")] [Column(TypeName = "numeric(18,2)")] public decimal DiscountAmount { get; set; }
        [Display(Name = "Line Total")] [Column(TypeName = "numeric(18,2)")] public decimal LineTotal { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
        [Display(Name = "SaleQty")] [Column(TypeName = "numeric(18,2)")] public decimal SaleQty { get; set; }
        [Display(Name = "Bonus")] [Column(TypeName = "numeric(18,2)")] public decimal Bonus { get; set; }
        [Display(Name = "Average Rate")] [Column(TypeName = "numeric(18,4)")] public decimal AvgRate { get; set; }

        [Display(Name="Market Description")] [MaxLength(150)] public string MarketDescription { get; set; }
        [Display(Name = "Remarks")] [MaxLength(100)] public string Remarks { get; set; }

        public int DetailCostCenter { get; set; }

        public bool IsDeleted { get; set; }

        public InvItem Item { get; set; }
        public ARInvoice Invoice { get; set; }
        public GLAccount ServiceAccount { get; set; }
        public virtual ARDeliveryChallanItem ARDeliveryChallanItem { get; set; }
        public decimal? Meters { get; set; }
        public decimal? TotalMeter { get; set; }
        public decimal? TotalMeterAmount{ get; set; }
    }
}
