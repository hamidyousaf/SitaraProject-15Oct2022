using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ItemsViewModel
    {
        public string ItemName { get; set; }
        public decimal ItemQty { get; set; }
        public decimal ItemValue { get; set; }
        public IEnumerable <int> InvoiceId { get; set; }
    }
}
