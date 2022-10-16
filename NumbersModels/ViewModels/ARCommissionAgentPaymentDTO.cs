using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ARCommissionAgentPaymentDTO
    {
        public int Id { get; set; }
        [Display(Name = "Trans.#")]
        public int TransactionNo { get; set; }
        public int TransactionType { get; set; }
        public string CreatedBy { get; set; }
        public string DeletedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime DeletedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime UnApprovedDate { get; set; }
        public string UnApprovedBy { get; set; }
        public int CompanyId { get; set; }
        public int Resp_Id { get; set; }
        public bool IsApproved { get; set; }
        public bool IsClosed { get; set; }
        public string Status { get; set; }
        public SelectList TransactionTypeList { get; set; }
        public virtual List<ARCommissionAgentPaymentDetails> CommissionAgentPaymentDetails { get; set; }
        public List<ARCommissionAgentPaymentGeneration> CommissionAgentPaymentGeneration { get; set; }
    }
}
