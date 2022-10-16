using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class GLBankCashAccount
    {

        public int Id { get; set; }
        public DateTime Date { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Bank/Cash Account Name is required")]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        [MaxLength(50, ErrorMessage = "Account Number is required")]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [MaxLength(15)]
        [Display(Name = "Receipt Voucher Type")]
        public string VoucherType { get; set; }

        [MaxLength(15)]
        [Display(Name = "Payment Voucher Type")]
        public string PaymentVoucherType { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "GL Account")] public int AccountId { get; set; }
        public GLAccount Account { get; set; }
        public int CompanyId { get; set; }
        public AppCompany Company { get; set; }
        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [NotMapped]
        public string shortdate { get; set; }
        public bool? IsApproved { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? Status { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
    }
}
