using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class GLVoucherViewModel
    {
        public GLVoucher GLVouchers { get; set; }
        public List<GLVoucherDetail> GLVoucherDetails { get; set; }
        public decimal Debit { get; set; }
        public decimal DetailDebit { get; set; }
        public decimal Credit { get; set; }
        public decimal DetailCredit { get; set; }
        public decimal Difference { get; set; }
        public string Account { get; set; }
        public string Seq { get; set; }
        public string VoucherType { get; set; }
        public string VoucherDate { get; set; }
        public string subAccount { get; set; }
        public string Department { get; set; }
        public string subDepartment { get; set; }
        public string CostCenter { get; set; }
        public string Voucher { get; set; }
        public bool Approve { get; set; }
        public bool Unapprove { get; set; }
    }
}
