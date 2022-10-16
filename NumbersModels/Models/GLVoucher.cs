using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class GLVoucher
    {
        public int Id { get; set; }
        [Required][Display(Name ="Voucher No.")]public int VoucherNo { get; set; }
        [Required][DataType(DataType.Date)]
        [Display(Name ="Voucher Date")][DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime VoucherDate { get; set; }
        public int BankCashAccountId { get; set; }
        [Required][MaxLength(10)][Display(Name ="Voucher Type")]public string VoucherType { get; set; }
        [MaxLength(10)]public string Status { get; set; }
        [MaxLength(30)]public string Reference { get; set; }
       public string Description { get; set; }
        public int PeriodId { get; set; }
        public int CompanyId { get; set; }
        [Required] [Display(Name = "Currency")] [MaxLength(3)]public string Currency { get; set; }
        [Display(Name ="Exchange Rate")][Column(TypeName = "decimal(18,4)")]public decimal CurrencyExchangeRate { get; set; }
        [Column(TypeName = "decimal(18,4)")]public decimal Amount { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsSystem { get; set; }
        [MaxLength(50)]
        public string ModuleName { get; set; }
        public int? ModuleId { get; set; }
        public int? ReferenceId { get; set; }
        [MaxLength(450)][Display(Name ="Created By")]public string CreatedBy { get; set; }
        [Display(Name ="Created Date")]public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")][MaxLength(450)]public string UpdatedBy { get; set; }
        [Display(Name = "Updated Date")]public DateTime? UpdatedDate { get; set; }
        [Display(Name = "Approved By")][MaxLength(450)]public string ApprovedBy { get; set; }
        [Display(Name = "Approval Date")]public DateTime? ApprovedDate { get; set; }

        public AppCompany Company { get; set; }
        // public GLVoucherDetail GLVoucherDetail { get; set; }
        public string Photo { get; set; }
        [NotMapped]
        public string Auser { get; set; }
        [NotMapped]
        public List<IFormFile> Attachments { get; set; }
        [NotMapped]
        public List<string> file { get; set; }
        [NotMapped]
        public string shortdate { get; set; }
        public List<AppCurrency> Currencies { get; set; }
        public GLVoucher()
        {
            Status = "Created";
//            Currency = "PKR";
            CurrencyExchangeRate = 1;
            IsDeleted = false;
            IsSystem = false;
            Amount = 0;
        }
        public virtual ICollection<GLVoucherDetail> GLVoucherDetail { get; set; }
        //Navigational Property
        public GLBankCashAccount BankCashAccount { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
        [ForeignKey("CreatedBy")]
        public ApplicationUser User { get; set; }
        [ForeignKey("ApprovedBy")]
        public ApplicationUser ApprovalUser { get; set; }
    }
}
