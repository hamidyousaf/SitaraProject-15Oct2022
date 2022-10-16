using Microsoft.AspNetCore.Http;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class VoucherViewModel
    {
        public VoucherViewModel()
        {
            CurrencyExchangeRate = 1;
        }
        public int Id { get; set; }
        public string VoucherType { get; set; }
        public int VoucherNo { get; set; }
        public int BankCashAccountId { get; set; }

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
        public string Photo { get; set; }
        [NotMapped]
        public List<IFormFile> Attachments { get; set; }
        [NotMapped]
        public List<string> file { get; set; }
        //public GLVoucherDetail VoucherDetails { get; set; }
    }
    public class VoucherDetailViewModel
    {
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public short Sequence { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public int SubAccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public int SubAccountIdName { get; set; }


        public int CostCenterName { get; set; }
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
    }
}
