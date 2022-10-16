using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class VwGLVoucher
    {
        [Key]
        public Guid  Id { get; set; }
        public string VoucherType { get; set; }
        public int VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public string Reference { get; set; }
        public string Currency { get; set; }
        public decimal CurrencyExchangeRate { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public int PeriodId { get; set; }
        public string Status { get; set; }
        public string VoucherDetail { get; set; }
        public string DeletedVoucherDetail { get; set; }
        public decimal Amount { get; set; }
        public int BranchId { get; set; }

        public string RefNum { get; set; }
        //public GLVoucherDetail VoucherDetails { get; set; }

        
        public Int32 VoucherId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public short Sequence { get; set; }
        [MaxLength(200)]
        public string VoucherDescription { get; set; }
        public int ProjectId { get; set; }
        public int SubAccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public int CompanyId { get; set; }

    }
}
