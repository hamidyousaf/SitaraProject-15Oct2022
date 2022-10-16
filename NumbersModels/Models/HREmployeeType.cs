using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREmployeeType
    {
        public int Id { get; set; }
        public int AdvanceSalaryAccountId { get; set; }

        [Display(Name = "Payroll Process")] [MaxLength(450)] public string PayrollProcess { get; set; }
        [Display(Name = "Short Description")] [MaxLength(100)] public string ShortDescription { get; set; }
        [Display(Name = "Name")] [MaxLength(50)]  public string Name { get; set; }
        [Display(Name = "Description")] [MaxLength(200)] public string Description { get; set; }

        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Active")] public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }

    }
}
