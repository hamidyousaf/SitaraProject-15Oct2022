using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRIncrementEmployee
    {
        public int Id { get; set; }
        [Display(Name = "Increment Id")] public int IncrementId { get; set; }
        [Display(Name = "Employee Id")] public int EmployeeId { get; set; }

        [Display(Name = "Promotion")] public string Promotion { get; set; }
        [Display(Name = "Old Grade")] public string OldGrade { get; set; }
        [Display(Name = "Old Designation")] public string OldDesignation { get; set; }
        [Display(Name = "New Grade")] public string NewGrade { get; set; }
        [Display(Name = "New Designation")] public string NewDesignation { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Increment %")] public decimal IncrementPercentage { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal CurrentPay { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Amount { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [Display(Name = "Increment No")] public int IncrementNo { get; set; }
        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }

        //navigation property
        public HRIncrement Increment { get; set; }
        public HREmployee Employee { get; set; }
    }
}
