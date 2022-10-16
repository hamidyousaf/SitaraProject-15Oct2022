using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class PricingDetailVM
    {
        public int ID { get; set; }
        public int TransactionID { get; set; }
        public int ItemPrice_Id { get; set; }
        public int Item_ThirdLevelId { get; set; }
        public string Item_ThirdLevel { get; set; }
        public int ItemId { get; set; }
        public string Item { get; set; }
        public int SeasonId { get; set; }
        public string Season { get; set; }
        public decimal Price { get; set; }
        public string Price_StartDate { get; set; }
        public string Price_EndDate { get; set; }
        public decimal Discount { get; set; }
        [Display(Name = "Disc. Amt")]
        public decimal DiscountAmount { get; set; }
        public string Dis_StartDate { get; set; }
        public string Dis_EndDate { get; set; }
        public string UOM1 { get; set; }
        public string UOM2 { get; set; }
        public bool IsDelete { get; set; }
        public string Status { get; set; }
        public ItemPricings ItemPricings { get; set; }
    }
}
