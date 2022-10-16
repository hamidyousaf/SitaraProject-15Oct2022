using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ItemPricingDetails
    {
        public int ID { get; set; }
        [ForeignKey("ItemPricing")]
        public int ItemPrice_Id { get; set; }
        [ForeignKey("InvItemThird")]
        public int ItemID_ThirdLevel { get; set; }
        [ForeignKey("InvItemFourth")]
        public int ItemID_FourthLevel { get; set; }
        [ForeignKey("SeasonName")]
        public int Season { get; set; }
        public decimal ItemPrice { get; set; }
        public DateTime Price_StartDate { get; set; }
        public DateTime Price_EndDate { get; set; }
        public decimal ItemDiscount { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime Dis_StartDate { get; set; }
        public DateTime Dis_EndDate { get; set; }
        public bool IsDelete { get; set; }

        public virtual ItemPricings ItemPricing { get; set; }
        public InvItemCategories InvItemThird { get; set; }
        public InvItemCategories InvItemFourth { get; set; }
        public AppCompanyConfig SeasonName { get; set; }
    }
}
