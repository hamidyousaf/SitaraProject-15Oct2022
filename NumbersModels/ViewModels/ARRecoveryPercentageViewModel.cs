using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ARRecoveryPercentageViewModel
    {
        public ARRecoveryPercentage ARRecoveryPercentage { get; set; }
        public List<ARRecoveryPercentageItem> ARRecoveryPercentageItem { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public string ItemCategory { get; set; }
    }
}
