using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRFinalSettlement
    {
        public int Id { get; set; }
        public int No { get; set; }
        public int OtAmount { get; set; }
        public int NoticePayDays { get; set; }
        public int LeaveEncashmentDays { get; set; }
        public int ExcessLeaveDays { get; set; }
       // public int EmployeeId { get; set; }
        public int DaysWorked { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        [MaxLength(200)] public string OtherPaymentsRemarks { get; set; }
        [MaxLength(200)] public string OtherDeductionRemarks { get; set; }
        public string CompanyId { get; set; }
        public string BranchId { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Value { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal TotalReceivable { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal TotalPayable { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal OtHours { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal OtherAllowances { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal NoticePayReceivable { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal NoticePayPayable { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal MonthSalary { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal LeaveEncashmentAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal GrossPay { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Gratuity { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal ExcessLeaveAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal CplAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Cpl { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal BasicPay { get; set; }

        public DateTime Date { get; set; }
        public DateTime ResignDate { get; set; }
        public DateTime NoticePeriodTo { get; set; }
        public DateTime NoticePeriodFrom { get; set; }
        public DateTime UpdatedDated { get; set; }
        public DateTime Doj { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ApprovedDate { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
    }
}
