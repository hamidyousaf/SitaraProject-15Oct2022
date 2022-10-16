using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ItemDetailViewModel
    {
        public int PONo { get; set; }
        public decimal POQty { get; set; }
        public int GRNNo { get; set; }
        public string GRNDate { get; set; }
        public string Category { get; set; }
        public string Vendor { get; set; }
        public decimal GRNQty { get; set; }
        public decimal GRNRate { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public decimal Stock { get; set; }
        public decimal Average { get; set; }
        public decimal Consumption { get; set; }
        public string PODate { get; set; }
        public int InvoiceId { get; set; }
        public int InvoiceItemId { get; set; }
        public int ItemId { get; set; }
        public string ItemDescription { get; set; }
        public string Brand { get; set; }
        public string UOM { get; set; }
    }
}
