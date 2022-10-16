using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREncashment
    {
        public int Id { get; set; }
        public int WopDuringYear { get; set; }
        public int GLVoucherId { get; set; }
        public int No { get; set; }
       // public int EmployeeId { get; set; }
        public int BankId { get; set; }     
        public int AbsentDuringYear { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [MaxLength(200)] public string Remarks { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        public string CompanyId { get; set; }
        public string BranchId { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Salary { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Amount { get; set; }

        public DateTime UpdatedDate { get; set; }
        public DateTime Date { get; set; }
        public DateTime Doj { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ApprovedDate { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
    }
}
