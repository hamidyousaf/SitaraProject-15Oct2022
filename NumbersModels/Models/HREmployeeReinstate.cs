using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREmployeeReinstate
    {
        public int Id { get; set; }
       // public int EmployeeId { get; set; }
        public int CompanyId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string ApprovedBy { get; set; }
        [MaxLength(200)] public string Remarks { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        public string OrgId { get; set; }
        public string Designation { get; set; }
        public string BranchId { get; set; }

        public DateTime ResignDate { get; set; }
        public DateTime Date { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ApprovalDate { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
    }
}
