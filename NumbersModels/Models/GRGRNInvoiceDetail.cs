using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity
{
    public class GRGRNInvoiceDetail
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal RatePerMeter { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPenaltyAmount { get; set; }
        public decimal NetPenaltyAMount { get; set; }
        public decimal LessYarnPrice { get; set; }
        public decimal NetPayableAmount { get; set; }
        public int GRNId { get; set; }
        public InvItem Item { get; set; }
    }
}
