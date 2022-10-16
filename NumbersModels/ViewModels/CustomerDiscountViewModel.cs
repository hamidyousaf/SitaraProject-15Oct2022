using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class CustomerDiscountViewModel
    {
        public int SalePersonId { get; set; }
        public int CommissionAgentId { get; set; }
        public string SalePersonText { get; set; }
        public string CommissionAgentText { get; set; }
    }
}
