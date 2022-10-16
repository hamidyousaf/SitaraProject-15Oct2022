using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARCommissionAgentPaymentDetails
    {
        public int Id { get; set; }
        [Display(Name = "Trans.#")]
        public int TransactionNo { get; set; }
        [ForeignKey("CommissionAgentPayment")]
        public int CommissionAgentPaymentId { get; set; }
        [ForeignKey("CommissionAgentPaymentGeneration")]
        public int CommissionAgentPaymentGenerationId { get; set; }
        public string CreatedBy { get; set; }
        public string DeletedBy { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? CommissionBalance { get; set; }
        public int? VoucherId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int Resp_Id { get; set; }
        public virtual ARCommissionAgentPayment CommissionAgentPayment { get; set; }
        public virtual ARCommissionAgentPaymentGeneration CommissionAgentPaymentGeneration { get; set; }
    }
}
