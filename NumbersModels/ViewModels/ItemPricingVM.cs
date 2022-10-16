using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ItemPricingVM
    {
        public ItemPricings itemPricing { get; set; }
        public List<ItemPricingDetails> itemPricingDetails { get; set; }
        public List<PricingDetailVM> itemPricingList { get; set; }
    }
}
