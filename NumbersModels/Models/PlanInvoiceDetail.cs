using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class PlanInvoiceDetail
    {
		[Key]
        public int Id { get; set; }
        public int PlanInvoiceId { get; set; }
        public int GreigeQualityId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}