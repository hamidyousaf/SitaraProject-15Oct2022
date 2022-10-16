using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ShipmentVM
    {
        public string Vendor { get; set; }
        public string ItemCode { get; set; }
        public int PrDetailId { get; set; }
        public int PoDetailId { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public int UOMId { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string Origin { get; set; }
        public int OriginId { get; set; }
        public int HSCode{ get; set; }
        public int ItemId{ get; set; }
        public decimal POQty { get; set; }
        public decimal Rate { get; set; }
        public decimal FCValue { get; set; }
        public decimal Value { get; set; }
        public string Remarks { get; set; }
        public string Currency { get; set; }
        public decimal Balance { get; set; }
        public decimal CurrencyExchangeRate { get; set; }
    }
}
