using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class BkgReceipt
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Receipt Date")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime ReceiptDate { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Receipt Account")]
        public string ReceiptAccount { get; set; }

        [Range(1,10000000)]
        [DataType(DataType.Currency)]
        [Required]
        [Display(Name = "Receipt Amount")]
        public decimal ReceiptAmount { get; set; }


        [Required]
        [Display(Name = "Reference #")]
        [MaxLength(50)]
        public string Reference { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "CR Account")]
        [MaxLength(50)]
        public string CRAccount { get; set; }

        [Display(Name = "Status")]
        [MaxLength(50)]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Bank/Cash Account")]
        public int GLAccountId { get; set; }

        [Required]
        [Display(Name = "Booking Vehicle")]
        public int BkgVehicleId { get; set; }


        [Required]
        public int CompanyId { get; set; }

        public BkgVehicle BkgVehicle { get; set; }
        //public GLAccount GLAccount { get; set; }


        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
