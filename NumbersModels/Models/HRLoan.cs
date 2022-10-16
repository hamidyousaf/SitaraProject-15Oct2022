using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRLoan
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "Loan No")] public int LoanNo { get; set; }
        [Display(Name = "Employee Name")] public int EmployeeId { get; set; }
        [Display(Name = "Deduction Type")] public int DeductionType { get; set; }
        [Display(Name = "Bank")] public int BankCashAccountId { get; set; }
        [Display(Name = "No. Of Installment")] public int NoOfInstallment { get; set; }
        [Display(Name = "Loan Amount")] public decimal LoanAmount { get; set; }
        [Display(Name ="Monthly Installment")]  public decimal PerMonthInstalment { get; set; }
        [Display(Name = "Loan Date")] public DateTime LoanDate { get; set; }
        [Display(Name = "Cheque No")]public int ChequeNo { get; set; }
        [Display(Name = "Status")] [MaxLength(10)] public string Status { get; set; }
        [Display(Name = "Mark Up%")] [Column(TypeName = "numeric(18,2)")] public decimal MarkUpPercentage { get; set; }
        [Display(Name = "Remarks")] [MaxLength(450)] public string Remarks { get; set; }
        [Display(Name = "Exemption Account")] [MaxLength(450)] public string ExemptionAccount { get; set; }
        [Display(Name = "Exemption Amount")] public decimal ExemptionAmount { get; set; }
        [Display(Name = "Exemp.Remarks")] [MaxLength(450)] public string ExemptionRemarks { get; set; }
        [Display(Name = "Accounting Date")] public DateTime AccountingDate { get; set; }
        [Display(Name = "Total Amount")] [Column(TypeName = "numeric(18,2)")] public decimal TotalAmount { get; set; }
        [Display(Name = "Loan Start Date")]public DateTime StartDate { get; set; }
        [Display(Name = "Attachments")] [MaxLength(450)] public string Attachments { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsDeleted { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
       // public List<GLBankCashAccount> BankAccounts { get; set; }
    }
}
