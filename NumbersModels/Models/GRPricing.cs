using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRPricing
    {
        public int Id { get; set; }
        public int TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public int GreigeQualityId { get; set; }
        public string Description { get; set; }
        public decimal SaleRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsClosed { get; set; }
        public int CompanyId { get; set; }


    }
}
