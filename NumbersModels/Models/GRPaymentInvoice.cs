using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRPaymentInvoice
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "Payment Id")] public int PaymentId { get; set; }
        [Display(Name = "Sequence No")] public Int16 SequenceNo { get; set; }
        [Display(Name = "Invoice No")] public int InvoiceId { get; set; }
        [Display(Name = "Invoice No")] public int InvoiceNo { get; set; }
        [Display(Name = "Invoice Date")] public DateTime InvoiceDate { get; set; }
        [Required] [Display(Name = "Invoice Amount")] public decimal InvoiceAmount { get; set; }
        [Display(Name = "Invoice Balance")] public decimal Balance { get; set; }
        [Required] [Display(Name = "Payment")] public decimal PaymentAmount { get; set; }
        [Display(Name = "Tax")] public int TaxId { get; set; }
        [Display(Name = "Income Tax Amt")] public decimal TaxAmount { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }

        [Display(Name = "Created By")] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        //navigation property
        public GRPayment Payment { get; set; }
        public APPurchase Invoice { get; set; }
    }
}
