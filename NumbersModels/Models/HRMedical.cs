using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRMedical
    {
        public int Id { get; set; }
        public int No { get; set; }
        // public int EmployeeId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; } [Display(Name = "Updated By")] [MaxLength(450)]
        public string UpdatedBy { get; set; }
        [MaxLength(200)] public string Remarks { get; set; }
        public string For { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        public string Closed { get; set; }
        public string BranchId { get; set; }
        public string CompanyId { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Amount { get; set; }

        public DateTime Date { get; set; }
        public DateTime ApprovalDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
    } 
}
