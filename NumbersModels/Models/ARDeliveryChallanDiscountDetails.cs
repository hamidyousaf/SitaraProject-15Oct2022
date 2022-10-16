using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARDeliveryChallanDiscountDetails
    {
        public int Id { get; set; }
        [ForeignKey("DeliveryChallanItem")]
        public int DeliveryChallanItemId { get; set; }
        [ForeignKey("DiscountItem")]
        public int ARDiscountItemId { get; set; }

        public virtual ARDeliveryChallanItem DeliveryChallanItem { get; set; }
        public virtual ARDiscountItem DiscountItem { get; set; }
    }
}