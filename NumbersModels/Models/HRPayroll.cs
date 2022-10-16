using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRPayroll
    {
        public int Id { get; set; }
       // public int EmployeeId { get; set; }
        public int GrossPay { get; set; }
        public int Incentive { get; set; }
        public int Deduction { get; set; }
        public int NetPay { get; set; }
        public int OrgId { get; set; }
        public int CompanyId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public string PeriodId { get; set; }
        public string Designation { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Absent { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Leaves { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal WorkingDays { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal AbsentDeduction { get; set; }

        public DateTime MonthEndDate { get; set; }
        public DateTime CreatedDate { get; set; }


        //navigation property
        public HREmployee Employee { get; set; }
    }
}
