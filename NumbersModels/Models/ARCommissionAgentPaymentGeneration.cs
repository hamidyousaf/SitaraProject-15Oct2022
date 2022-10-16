using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARCommissionAgentPaymentGeneration
    {
        public int Id { get; set; }
        public int TransactionNo { get; set; }
        [ForeignKey("ARCommissionAgent")]
        public int? CommissionAgentId { get; set; }
        public int? ProductTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? GrandTotal { get; set; } = 0;
        public decimal? UtilizedAmount { get; set; } = 0;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? UnapprovedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsClosed { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsPending { get; set; }
        public string DeletedBy { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string UnapprovedBy { get; set; }
        public int? Resp_Id { get; set; }
        public int? CompanyId { get; set; }
        public int? VoucherId { get; set; }
        public virtual ARCommissionAgent ARCommissionAgent { get; set; }
        public virtual ARCommissionAgentPaymentDetails CommissionAgentPaymentDetails { get; set; }
        public virtual ICollection<ARCommissionAgentPaymentGenerationDetails> ARCommissionAgentPaymentGenerationDetails { get; set; }
    }
}
