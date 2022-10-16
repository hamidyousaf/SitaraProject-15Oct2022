using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
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
        public decimal PaymentAmount { get; set; }

        [Required]
        [Display(Name = "Reference #")]
        [MaxLength(50)]
        public string Refrence { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [MaxLength(50)]
        public string Status { get; set; }


        [Required]
        [Display(Name = "Bank/Cash Account")]
        public int GLAccountId { get; set; }

        [Display(Name = "Voucher ID")]
        public int VoucherID { get; set; }

        [Required]
        [Display(Name = "Booking Vehicle")]
        public int BkgVehicleId { get; set; }

        public BkgVehicle BkgVehicle { get; set; }
        public GLAccount GLAccount { get; set; }

        [Required]
        public int CompanyId { get; set; }


        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
