using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARCommissionAgentPaymentGenerationDetails
    {
        public int Id { get; set; }
        [ForeignKey("InvItemCategories")]
        public int? CategoryId { get; set; }
        [ForeignKey("ARCommissionAgentPaymentGeneration")]
        public int? CommissionAgentPaymentId { get; set; }
        [ForeignKey("ARCustomers")]
        public int? CustomerId { get; set; }
        public int? Qty { get; set; }
        public decimal? Amount { get; set; }
        public decimal? CommissionPercentge { get; set; }
        public decimal? CommissionAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int? Resp_Id { get; set; }
        public int? CompanyId { get; set; }
        public virtual ARCommissionAgentPaymentGeneration ARCommissionAgentPaymentGeneration { get; set; }
        public virtual InvItemCategories InvItemCategories { get; set; }
        public virtual ARCustomer ARCustomers { get; set; }
        public virtual ICollection<ARDeliveryChallanComAgentPayGenDetails> DeliveryChallanComAgentPayGenDetails { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }
    }
}
