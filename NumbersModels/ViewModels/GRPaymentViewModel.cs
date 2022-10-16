using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class GRPaymentViewModel
    {
        //APSupplierPayment
        public int Id { get; set; }
        [Display(Name = "Payment No")] public int PaymentNo { get; set; }
        [Display(Name = "Payment Date")] public DateTime PaymentDate { get; set; }
        [Display(Name = "Bank Cash")] public int BankCashAccountId { get; set; }
        [Display(Name = "Payment Mode")] public int PaymentModeId { get; set; }
        [Display(Name = "Document No")] public string DocumentNo { get; set; }
        [Display(Name = "Document Date")] public DateTime DocumentDate { get; set; }
        [Display(Name = "Supplier Name")] public int SupplierId { get; set; }
        [Display(Name = "Attachment")] public string Attachment { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal PaymentTotal { get; set; }
        [Display(Name = "Total Income Tax")] [Column(TypeName = "numeric(18,2)")] public decimal TotalTaxAmount { get; set; }
        [Display(Name = "Excise Tax Amount")] [Column(TypeName = "numeric(18,2)")] public decimal ExciseTaxAmount { get; set; }
        [Display(Name = "Total Payment Amount")] [Column(TypeName = "numeric(18,2)")] public decimal TotalPaidAmount { get; set; }
        [Display(Name = "Invoice Adjusted")] [Column(TypeName = "numeric(18,2)")] public decimal InvoiceAdjusted { get; set; }
        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }
        [Display(Name = "Currency Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "Remarks")] public string Remarks { get; set; }
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
        public GLBankCashAccount BankCashAccount { get; set; }

        //APPaymentInvoices
        [Display(Name = "Supplier Payment Id")] public int PaymentId { get; set; }
        [Display(Name = "Invoice Item Id")] public int PaymentItemId { get; set; }
        [Display(Name = "Sequence No")] public Int16 SequenceNo { get; set; }
        [Display(Name = "Invoice No")] public int InvoiceId { get; set; }
        [Display(Name = "Invoice No")] public int InvoiceNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [Display(Name = "Invoice Date")] [DataType(DataType.Date)] public DateTime InvoiceDate { get; set; }
        [Required] [Display(Name = "Invoice Amount")] public decimal InvoiceAmount { get; set; }
        [Display(Name = "Invoice Balance")] public decimal Balance { get; set; }
        [Required] [Display(Name = "Payment")] public decimal PaymentAmount { get; set; }
        [Display(Name = "Tax")] public int TaxId { get; set; }
        [Display(Name = "Income Tax Amt")] public decimal TaxAmount { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }

        //navigation property
        public GRPayment Payment { get; set; }
        public GRGRNInvoice Invoice { get; set; }
        public AppCompanyConfig PaymentMode { get; set; }
        public List<AppTax> TaxList { get; set; }
        public List<AppCurrency> Currencies { get; set; }
        public string SupplierName { get; set; }
        public string PayDate { get; set; }
    }
}
