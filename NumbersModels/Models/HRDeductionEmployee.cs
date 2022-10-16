using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRDeductionEmployee
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int DeductionId { get; set; }
        public int CompanyId { get; set; }
        [MaxLength(200)] public string Remarks { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Amount { get; set; }

        public bool IsDeleted { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
        public HRDeduction Deduction { get; set; }
    }
}
