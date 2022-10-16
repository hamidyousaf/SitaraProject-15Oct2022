using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Numbers.Entity.Models;

namespace Numbers.Entity.ViewModels
{
   public class ItemListViewModel
    {
        public string UOM { get; set; }
        public string BrandName { get; set; }
        public string ICode { get; set; }
        public string IBarcode { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string DilveryDate { get; set; }
        public string type { get; set; }
        public InvItem InvItems { get; set; }
        public APPurchaseOrderItem orderItem { get; set; }
        public APShipmentDetail shipmentDetail { get; set; }
        public APIGPDetails APIGPDetails { get; set; }
        public APIRNDetails APIRNDetails { get; set; }
        public int BuilityNo { get; set; }
        public int POId { get; set; }
        public string PackUnit { get; set; }
        public string LastPODate { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal LastPOQty { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal Comsumption { get; set; }
        public decimal TotalRecieved { get; set; }
        public string RequiredDate { get; set; }
        public string Brand { get; set; }
    }
}
