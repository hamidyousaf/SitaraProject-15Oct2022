using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APPurchaseOrderItem
    {
        public int Id { get; set; }
        [ForeignKey("PO")]
        [Display(Name ="PO Id")] public int POId { get; set; }
        [Display(Name = "PR No")] public int PrNo { get; set; }
        [Display(Name = "Sequence No")] public Int16 SequenceNo { get; set; }
        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "ItemCode")] public string ItemCode { get; set; }
        [Display(Name = "ItemDescription")] public string ItemDescription { get; set; }
        [Display(Name = "UOM")] [ForeignKey("UOMName")] public int UOM { get; set; }
        [Display(Name = "IndentNo")] public int IndentNo { get; set; }
        [Required] [Display(Name = "Qty")] public decimal Qty { get; set; }
        [Required] [Display(Name = "Rate")] public decimal Rate { get; set; }
        [Display(Name = "Value")] public decimal Value { get; set; }
        [Display(Name = "Tax")] public int TaxId { get; set; }
        [Display(Name = "Brand")] public int BrandId { get; set; }
        [Display(Name = "Tax Amount")] public decimal TaxAmount { get; set; }
        [Display(Name = "Fed %")] public decimal FedPercentage { get; set; }
        [Display(Name = "Fed Amount")] public decimal FedAmount { get; set; }
        [Display(Name = "Total")] public decimal Total { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }

        [Display(Name = "Remarks")] public string Remarks { get; set; }
        [Display(Name = "Created By")] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Display(Name = "Delivery Date")] public DateTime DeliveryDate { get; set; }
        [Display(Name = "Purchase Quantity")] [Column(TypeName = "numeric(18,2)")] public decimal PurchaseQty { get; set; }

        public bool IsDeleted { get; set; }
        [Column(TypeName = "numeric(18,2)")]
        public decimal IGPRcd { get; set; }
        [Column(TypeName = "numeric(18,2)")]
        public decimal IGPBalc { get; set; }
        //more fields
        [Display(Name = "HS Code")] public int HSCode { get; set; }
        [Display(Name = "Net Totale")] public decimal NetTotal { get; set; }

        public int DetailCostCenter { get; set; }

        public bool IsClosed { get; set; }
        public bool IsIGP { get; set; }
        public int PrDetailId { get; set; }
        //navigation property
        public virtual APPurchaseOrder PO { get; set; }
        public AppCompanyConfig UOMName { get; set; }
        public InvItem Item { get; set; }
        public string PRReferenceNo { get; set; }
        public int Category { get; set; }
        public int Origin { get; set; }
        public decimal FCValue { get; set; }
        public bool IsImport { get; set; }
        [NotMapped]
        public string Date { get; set; }
        [Column(TypeName = "numeric(18,2)")]
        public decimal ShipmentBalc { get; set; }
        [Column(TypeName = "numeric(18,2)")]
        public decimal ShipmentRcd { get; set; }
        public APPurchaseOrderItem()
        {
            PurchaseQty = 0;
        }
    }
}
