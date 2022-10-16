using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRSalaryAdvanceEmployee
    {
        public int Id { get; set; }
        // public int SalaryAdvanceId { get; set; }
        //public int EmployeeId { get; set; }

        public string DesignationNo { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Amount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal DeptId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        //navigation property
        public HRSalaryAdvance SalaryAdvance { get; set; }
        public HREmployee Employee { get; set; }
    }
}
