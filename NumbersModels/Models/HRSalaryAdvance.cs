using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRSalaryAdvance
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public int AccountId { get; set; }
        public int CompanyId { get; set; }
        public int GLVoucherId { get; set; }
        // public int EmployeeTypeId { get; set; }

        [MaxLength(200)] public string Remarks { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        public string BranchId { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string ApprovedBy { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal NO { get; set; }

        public DateTime AdvanceDate { get; set; }
        public DateTime ApprovalDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        //navigation property
        public HREmployeeType EmployeeType { get; set; }
    }
}
