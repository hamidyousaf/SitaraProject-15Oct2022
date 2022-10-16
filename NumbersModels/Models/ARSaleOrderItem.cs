using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARSaleOrderItem
    {
        public int Id { get; set; }
        public int SaleOrderId { get; set; }

        //public string BaleType { get; set; }
        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "Rate")] public decimal Rate { get; set; }
        [Display(Name = "Conversion")] public decimal? Conversion { get; set; }
        [Display(Name = "Price")] public decimal? Price { get; set; }
        [Display(Name = "UOM Qty")] public decimal? UOMQty { get; set; }
        public decimal? PricingRate { get; set; }
        [Display(Name = "Qty")] public int Qty { get; set; }
        [Display(Name = "Sale Qty")] public decimal SaleQty { get; set; }
        [Display(Name = "Total")] public decimal Total { get; set; }
        [Display(Name = "Tax Amount")] public decimal TaxAmount { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }
        [Display(Name = "Tax")] public int TaxId { get; set; }
        [Display(Name = "Bale No")] public string BaleNumber { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsExpired { get; set; }

        //navigation property
        public ARSaleOrder SaleOrder { get; set; }
        public InvItem Item { get; set; }
        public BaleInformation? Bale { get; set; }

        public int DetailCostCenter { get; set; }
        public ARSaleOrderItem()
        {
            SaleQty = 0;
        }
        [NotMapped]
        public string Unit { get; set; }
        [NotMapped]
        public string PackUnit { get; set; }
        [NotMapped]
        public string TaxName { get; set; }
        public string? BaleType { get; set; }
        public int? AvailableStock { get; set; }
        public int? BookedStock { get; set; }
        public int? BalanceStock { get; set; }
        public decimal? Meters { get; set; }
        [Display(Name = "Bale Id")] public int? BaleId { get; set; }
        public int DCQty { get; set; }
        public decimal? TotalMeter { get; set; }
        public decimal? TotalMeterAmount { get; set; }
    }
}
