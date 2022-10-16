
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class ARCommissionPayment
    {
        public int Id { get; set; }
        public int TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CategoryId { get; set; }
        public int CityId { get; set; }
        [ForeignKey("Agent")]
        public int AgentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string Status { get; set; }
        public DateTime DeletedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string DeletedBy { get; set; }
        public string UnApprovedBy { get; set; }
        public string Updatedby { get; set; }
        public int VoucherId { get; set; }
        public int CompanyId { get; set; }
        public int Resp_Id { get; set; }

        public  ARCommissionAgent Agent { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }


    }
}
