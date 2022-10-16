using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRLoanSchedule
    {
        public int Id { get; set; }
       // public int LoanId { get; set; }
        public int Amount { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string PeriodId { get; set; }
        public string Posted { get; set; }
        public string Deducted { get; set; }
        public string InCash { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal MarkUpAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal InstallmentAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Balance { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime InstallmentDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        //navigation property
        public HRLoan Loan { get; set; }
    }
}
