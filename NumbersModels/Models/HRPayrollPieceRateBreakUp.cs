using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRPayrollPieceRateBreakUp
    {
        public int Id { get; set; }
        //public int PayrollEmployeeBreakUpId { get; set; }
        //public int PayrollId { get; set; }
        //public int EmployeeId { get; set; }
        public int TranAmount { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string PeriodId { get; set; }
        public string TranType { get; set; }
        public string TranCode { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Half { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
        public HRPayroll Payroll { get; set; }
        public HRPayrollEmployeeBreakUp PayrollEmployeeBreakUp { get; set; }

    }
}
