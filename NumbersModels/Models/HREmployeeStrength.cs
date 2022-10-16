using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREmployeeStrength
    {
        public int Id { get; set; }
        public int Strength { get; set; }
        public int OrgId { get; set; }
        public int CompanyId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string ApprovedBy { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        public string EmployeeDesignationId { get; set; }
        public string BranchId { get; set; }

        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ApprovedDate { get; set; }
    }
}
