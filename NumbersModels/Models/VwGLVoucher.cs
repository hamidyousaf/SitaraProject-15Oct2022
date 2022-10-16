using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class VwGLVoucher
    {
        public string VoucherType { get; set; }
        public string Description { get; set; }
        public int VoucherId { get; set; }
        public int VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string VoucherDescription { get; set; }
        public int PeriodId { get; set; }
        public string Currency { get; set; }
        public decimal CurrencyExchangeRate { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public int VoucherDetailId { get; set; }
        public int AccountId { get; set; }
        public short Sequence { get; set; }
        public string VoucherDetailDescription { get; set; }
        public int ProjectId { get; set; }
        public int SubAccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public int CompanyId { get; set; }
    }
}
