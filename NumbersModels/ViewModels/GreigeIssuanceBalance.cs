using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class GreigeIssuanceBalance
    {
        public int Id { get; set; }
        public int QualityId { get; set; }
        public string description { get; set; }
        public decimal availableStock { get; set; }
        public decimal RequiredQty { get; set; }
    }
}
