using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARDiscountItem
    {
        public int Id { get; set; }
        [ForeignKey("InvItemCategories")]
        public int ItemCategory { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        [ForeignKey("Discount")]
        public int ARDiscount_Id { get; set; }
        public InvItemCategories InvItemCategories { get; set; }
        [NotMapped]
        public List<int> InvoiceId { get; set; }

        public virtual ARDiscount Discount { get; set; }
        public virtual ICollection<ARDeliveryChallanDiscountDetails> DeliveryChallanDiscountDetails { get; set; }

    }
}
