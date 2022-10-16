using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARDeliveryChallanItem
    {
        public int Id { get; set; }
        [Display(Name = "D.C. No")] public int DCId { get; set; }
        [ForeignKey("ARSaleOrderItem")]
        [Display(Name = "Sale Order No.")] public int SaleOrderId { get; set; }
        //[Display(Name = "Sale Order No.")] public int SaleOrderItemId { get; set; }
        [Display(Name = "Item")] public int ItemId { get; set; }

        [Display(Name = "Item Description")] public string ItemDescription { get; set; }
        public string UOM { get; set; }

        
        [Display(Name = "PR Item")] public int PRItemId { get; set; }
        [Display(Name = "Expiry Date")] public DateTime ExpiryDate { get; set; }
        [Display(Name = "Qty")] public int Qty { get; set; }
        [Display(Name = "Qty")] public decimal SaleQty { get; set; }
        [Display(Name = "Sale Order Balance")] public decimal SaleOrderBalance { get; set; }
        [Display(Name = "Item")] public decimal Bonus { get; set; }
        [Display(Name = "Sale Order No")] public decimal SaleOrderNo { get; set; }
        [Display(Name = "Store")] public int StoreDetail { get; set; }

        [Display(Name = "DC Balance")] public decimal DCBalance { get; set; }
        [Display(Name = "Stock In Store")] public int StockInStore { get; set; }
        [Display(Name = "Company")] public int CompanyId { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }



        //navigation property
        public virtual ARSaleOrderItem ARSaleOrderItem { get; set; }
        public InvItem Item { get; set; }
        public ARDeliveryChallan DC { get; set; }

        public decimal SalesOrderQty { get; set; }


        public string UnitName { get; set; }
        [ForeignKey("DC")]
        public int DeliveryChallanId { get; set; }

        public virtual ARInvoiceItem ARInvoiceItem { get; set; }


        public List<ARDeliveryChallanItem> ARDeliveryChallanItemList { get; set; }
        [NotMapped]
        public string ItemBrand { get; set; }
        public string BaleNo { get; set; }
        public int? BaleId { get; set; }
        public int? AvailableStock { get; set; }
        [NotMapped]
        public SelectList BalesLOV { get; set; }
        // navigational property
        public BaleInformation Bale { get; set; }
    }
}
