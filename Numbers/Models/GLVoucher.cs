using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class GLVoucher
    {
        public int Id { get; set; }
        [Required]
        public int VoucherNo { get; set; }
        [Display(Name ="Voucher date")]
        [Column(TypeName = "Date")]
        public DateTime VoucherDate { get; set; }
        public GLVoucherType VoucherType { get; set; }
        [MaxLength(10)]
        public string Status { get; set; }
        [MaxLength(30)]
        public string Reference { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public int PeriodId { get; set; }
        public AppCompany Company { get; set; }
        [Required]
        [MaxLength(3)]
        public string Currency { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

    }
}
