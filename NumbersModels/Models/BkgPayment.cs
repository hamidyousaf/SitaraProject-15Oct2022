using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class BkgPayment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Payment Date")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }


        [Required]
        [Display(Name = "Payment Amount")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal PaymentAmount { get; set; }

        [Display(Name = "Reference")]
        [MaxLength(50)]
        public string Reference { get; set; }

        [Display(Name = "Description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [MaxLength(50)]
        public string Status { get; set; }


        [Required]
        [Display(Name = "Bank/Cash Account")]
        public int BankCashAccountId { get; set; }

        [Display(Name = "Voucher Id")]
        public int VoucherId { get; set; }

        [Required]
        [Display(Name = "Booking No")]
        public int VehicleId { get; set; }

        public BkgVehicle Vehicle { get; set; }
        public GLBankCashAccount BankCashAccount { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public BkgPayment()
        {
            IsDeleted = false;
            
        }
    }
}
