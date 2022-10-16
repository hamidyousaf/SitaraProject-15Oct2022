using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRPayment
    {
        public int Id  {get; set;}
        [Display(Name = "Payment No")] public int PaymentNo  {get; set;}
        [Display(Name = "Payment Date")] [DataType(DataType.Date)] public DateTime PaymentDate  {get; set;}
        [Display(Name = "Bank Cash")] public int BankCashAccountId { get; set; }
        [Display(Name = "Payment Mode")] public int PaymentModeId  {get; set;}
        [Display(Name = "Document No")] public string DocumentNo {get; set;}
        [Display(Name = "Document Date")] public DateTime DocumentDate  {get; set;}
        [Display(Name = "Supplier Name")] public int SupplierId { get; set; }
        [Display(Name="Attachment")]public string Attachment  {get; set;}
        [Display(Name = "Total Income Tax")] [Column(TypeName = "numeric(18,2)")] public decimal TotalTaxAmount { get; set;}
        [Display(Name = "Total Payment Amount")] [Column(TypeName = "numeric(18,2)")] public decimal TotalPaidAmount  {get; set;}
        [Display(Name = "Invoice Adjusted")] [Column(TypeName = "numeric(18,2)")] public decimal InvoiceAdjusted  {get; set;}
        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal {get; set;}
        [Display(Name = "Currency Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "Remarks")] public string Remarks {get; set;}
        [MaxLength(10)] public string Status { get; set; }
        public int CompanyId { get; set; }
        public int VoucherId { get; set; }
        public bool IsDeleted { get; set; }
        public int DepartmentId { get; set; }
        public int OperationId { get; set; }
        public int Resp_ID { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime ApprovedDate { get; set; }

        public APSupplier Supplier { get; set; }
        public AppCompanyConfig PaymentMode { get; set; }
        public GLBankCashAccount BankCashAccount { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
    }
}
