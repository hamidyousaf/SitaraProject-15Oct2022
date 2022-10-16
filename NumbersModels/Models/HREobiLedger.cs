using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREobiLedger
    {
        public int Id { get; set; }
        //public int EmployeeId { get; set; }
        public int CompanyId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public string PeriodId { get; set; }
        public string Narration { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal EmployerAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal EmployeeAmount { get; set; }

        public DateTime TransactionDate { get; set; }
        public DateTime CreatedDate { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
    }
}
