using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class GLVoucherDetail
    {
        public int Id { get; set; }
        public GLVoucher Voucher { get; set; }
        public int AccountId { get; set; }
        public GLAccount Account { get; set; }
        public Int16? Sequence { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public int? ProjectId { get; set; }
        public int? SubAccountId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Debit { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Credit { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
