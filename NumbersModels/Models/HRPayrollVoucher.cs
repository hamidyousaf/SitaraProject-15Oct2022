using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRPayrollVoucher
    {
        public int Id { get; set; }
        public int GLVoucherId { get; set; }
        public int PeriodId { get; set; }
        public int CompanyId { get; set; }

        public string BranchId { get; set; }
        [MaxLength(10)] public string Status { get; set; }
    }
}
