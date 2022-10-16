using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREmployeeSalaryBreakUp
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public int EmployeeAllowanceId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
        public HREmployeeAllowance EmployeeAllowance { get; set; }
    }
}
