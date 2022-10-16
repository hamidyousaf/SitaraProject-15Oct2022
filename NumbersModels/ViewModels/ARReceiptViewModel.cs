using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ARReceiptViewModel
    {
        //ARReceipt
        public int Id { get; set; }
        [Display(Name = "Receipt No")] public int ReceiptNo { get; set; }
        [Display(Name = "Receipt Date")] public DateTime ReceiptDate { get; set; }
        [Display(Name = "Bank Cash")] public int BankCashAccountId { get; set; }
        [Display(Name = "Accounts")] public int FourthLevelAccountId { get; set; }
        [Display(Name = "Payment Mode")] public int PaymentModeId { get; set; }
        [Display(Name = "Document No")] [MaxLength(30)] [DisplayFormat(ConvertEmptyStringToNull = false)] public string DocumentNo { get; set; }
        [Display(Name = "Document Date")] public DateTime DocumentDate { get; set; }
        [Display(Name = "Customer Name")] public int CustomerId { get; set; }
        [Display(Name = "Attachment")] public string Attachment { get; set; }
        [Display(Name = "Total Income Tax")] [Column(TypeName = "numeric(18,2)")] public decimal TotalTaxAmount { get; set; }
        [Display(Name = "Total Received")] [Column(TypeName = "numeric(18,2)")] public decimal TotalReceivedAmount { get; set; }
        [Display(Name = "Invoice Adjusted")] [Column(TypeName = "numeric(18,2)")] public decimal InvoiceAdjusted { get; set; }
        [Column(TypeName = "numeric(18,2)")]public decimal ReceiptTotal { get; set; }
        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }
        [Display(Name = "Remarks")] public string Remarks { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        public int CompanyId { get; set; }
        public string SalePerson { get; set; }
        public int ItemCategoryId { get; set; }
         //public int ReceiptAmount { get; set; }
        public int CityId { get; set; }
        public int ResponsibilityId { get; set; }
        [Display(Name = "Voucher Id")] public int VoucherId { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Currency Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime ApprovedDate { get; set; }

        public ARCustomer Customer { get; set; }
        public GLBankCashAccount BankCashAccount { get; set; }

        //ARReceiptInvoices
        [Display(Name = "Receipt Id")] public int ReceiptId { get; set; }
        [Display(Name = "Receipt Item Id")] public int ReceiptItemId { get; set; }
        [Display(Name = "Sequence No")] public Int16 SequenceNo { get; set; }
        [Display(Name = "Invoice Id")] public int InvoiceId { get; set; }
        [Display(Name = "Invoice No")] public int InvoiceNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [Display(Name = "Invoice Date")] public DateTime InvoiceDate { get; set; }
        [Required] [Display(Name = "Inv Amt")] public decimal InvoiceAmount { get; set; }
        [Display(Name = "Inv Balance")] public decimal Balance { get; set; }
        [Required] [Display(Name = "Received")] public decimal ReceiptAmount { get; set; }
        [Display(Name = "Tax")] public int TaxId { get; set; }
        [Display(Name = "Income Tax Amt")] public decimal TaxAmount { get; set; }
        [Display(Name = "Excise Tax Amount")] public decimal ExciseTaxAmount { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }

        //navigation property
        public ARReceipt Receipt { get; set; }
        public APPurchase Invoice { get; set; }
        public AppCompanyConfig PaymentMode { get; set; }
        public List<AppTax> TaxList { get; set; }
        public List<AppCurrency> Currencies { get; set; }
        public string CustomerName { get; set; }
        public string PaymentType { get; set; }
        public string RecDate { get; set; }
        public SelectList FourthLevelAccountLOV { get; set; }
    }
}
