using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ARPerformaInvoiceViewModel
    {
        //ARPerformaInvoice
      

        //ARDeliveryChallanItems

        [Display(Name = "D.C. No")] public int DCId { get; set; }
        [Display(Name = "Sale Order No.")] public int SaleOrderNo { get; set; }

        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "PR Item")] public int PRItemId { get; set; }
        [Display(Name = "Qty")] [Column(TypeName = "numeric(18,2)")] public decimal Qty { get; set; }
        [Display(Name = "Sale Order Balance")] [Column(TypeName = "numeric(18,2)")] public decimal SaleOrderBalance { get; set; }
        [Display(Name = "Bonus")] [Column(TypeName = "numeric(18,2)")] public decimal Bonus { get; set; }
        [Display(Name = "DC Balance")] [Column(TypeName = "numeric(18,2)")] public decimal DCBalance { get; set; }
        [Display(Name = "Stock In Store")] public int StockInStore { get; set; }
        [Display(Name = "Company")] public string Company { get; set; }

        //navigation property
        public ARSaleOrder SaleOrder { get; set; }
        public InvItem Item { get; set; }
        public string UOM { get; set; }
        public int StoreDetail { get; set; }
        public decimal Total { get; set; }

        public List<ARDeliveryChallanItem> ARDeliveryChallanItemList { get; set; }
    }
}