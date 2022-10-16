using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARReceiptInvoice
    {
        public int Id { get; set; }
        [Display(Name = "Payment Id")] public int ReceiptId { get; set; }
        [Display(Name = "Sequence No")] public Int16 SequenceNo { get; set; }
        [Display(Name = "Invoice Id")] public int InvoiceId { get; set; }
        [Display(Name = "Invoice No")] public int InvoiceNo { get; set; }
        [Display(Name = "Invoice Date")] public DateTime InvoiceDate { get; set; }
        [Required] [Display(Name = "Invoice Amount")] public decimal InvoiceAmount { get; set; }
        [Display(Name = "Invoice Balance")] public decimal Balance { get; set; }
        [Required] [Display(Name = "Received")] public decimal ReceiptAmount { get; set; }
        [Display(Name = "Tax")] public int TaxId { get; set; }
        [Display(Name = "Tax Amount")] public decimal TaxAmount { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }


        public bool IsDeleted { get; set; }

        //navigation property
        public ARReceipt Receipt { get; set; }
        public APPurchase Invoice { get; set; }

    }
}
