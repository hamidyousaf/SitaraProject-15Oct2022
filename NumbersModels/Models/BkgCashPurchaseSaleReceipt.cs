using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class BkgCashPurchaseSaleReceipt
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "Receipt Date")]
        [DataType(DataType.Date)]
        public DateTime ReceiptDate { get; set; }

        [Range(1, 10000000)]
        [DataType(DataType.Currency)]
        [Required]
        [Display(Name = "Receipt Amount")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal ReceiptAmount { get; set; }

        [MaxLength(100)]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [MaxLength(500)]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Required]
        [Display(Name = "Bank/Cash Account")]
        public int BankCashAccountId { get; set; }

        [Required]
        public int CashPurchaseSaleId { get; set; }

        [Display(Name ="Voucher Id")]
        public int VoucherId { get; set; }


        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
