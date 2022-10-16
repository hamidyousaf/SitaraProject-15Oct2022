using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARDeliveryChallanComAgentPayGenDetails
    {
        public int Id { get; set; }
        [ForeignKey("DeliveryChallanItem")]
        public int DeliveryChallanItemId { get; set; }
        [ForeignKey("CommissionAgentPaymentGenerationDetails")]
        public int CommissionAgentPaymentGenDetailsId { get; set; }

        public virtual ARDeliveryChallanItem DeliveryChallanItem { get; set; }
        public virtual ARCommissionAgentPaymentGenerationDetails CommissionAgentPaymentGenerationDetails { get; set; }
    }
}