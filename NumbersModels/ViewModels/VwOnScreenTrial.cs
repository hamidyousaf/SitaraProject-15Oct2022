using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class VwOnScreenTrial
    {
        
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public string VoucherType { get; set; }
        public string StartCode { get; set; }
        public string EndCode { get; set; }
        public string VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string Currency { get; set; }
        public decimal CurrencyExchangeRate { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public int VoucherDetailId { get; set; }
        public int AccountId { get; set; }
        public int Sequence { get; set; }
        public string VoucherDetailDescription { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

    }
}
