using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREmployeeAllowance
    {
        public int Id { get; set; }

        [Display(Name = "Allowance Name")] [MaxLength(100)] public string Name { get; set; }
        [Display(Name = "Increment Percentage")] public decimal IncrementPercentAge { get; set; }
        [Display(Name = "Account Description")] [MaxLength(200)] public string AccountDescription { get; set; }
        [Display(Name = "Created by")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated by")] [MaxLength(450)] public string UpdatedBy { get; set; }

        [Display(Name = "Active")] public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
    }
}
