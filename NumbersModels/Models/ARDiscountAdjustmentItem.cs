using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARDiscountAdjustmentItem
    {
        public int Id { get; set; }
        [ForeignKey("InvItemCategories")]
        public int ItemCategory { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ActualDiscountRate { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal ReserveRate { get; set; }
        public decimal ReserveAmount { get; set; }
        [ForeignKey("DiscountAdjustment")]
        public int ARDiscountAdjustmentId { get; set; }
        public InvItemCategories InvItemCategories { get; set; }
        [NotMapped]
        public List<int> InvoiceId { get; set; }

        public virtual ARDiscountAdjustment DiscountAdjustment { get; set; }
        //public virtual ICollection<ARDeliveryChallanDiscountDetails> DeliveryChallanDiscountDetails { get; set; }

    }
}
