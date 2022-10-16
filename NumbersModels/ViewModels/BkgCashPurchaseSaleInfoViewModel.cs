using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Numbers.Entity.Models;
namespace Numbers.Entity.ViewModels
{
    public class BkgCashPurchaseSaleInfoViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Trans Date")]
        [DataType(DataType.Date)]
        public DateTime TransDate { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Required]
        [Display(Name = "Booking No")]
        public string BookingNo { get; set; }

        [Range(1, 10000000)]
        [Required]
        [Display(Name = "Estimated Purchase")]
        public decimal EstimatedPurchase { get; set; }

        [Required]
        [MaxLength(500)]
        [Display(Name = "Booking Remarks")]
        public string BookingRemarks { get; set; }

        [Display(Name = "Booking Status")]
        [MaxLength(50)]
        public string BookingStatus { get; set; }

        [MaxLength(50)]
        [Display(Name = "IGP No")]
        public string IGPNo { get; set; }

        [Display(Name = "IGP Date")]
        [DataType(DataType.Date)]
        public DateTime IGPDate { get; set; }

        [MaxLength(50)]
        [Display(Name = "Engine No")]
        public string EngineNo { get; set; }

        [MaxLength(50)]
        [Display(Name = "Chassis No")]
        public string ChassisNo { get; set; }

        [MaxLength(500)]
        [Display(Name = "Receiving Remarks")]
        public string ReceivingRemarks { get; set; }

        [MaxLength(50)]
        [Display(Name = "IGP Status")]
        public string IGPStatus { get; set; }


        [MaxLength(50)]
        [Display(Name = "OGP No")]
        public string OGPNo { get; set; }

        [Display(Name = "OGP Date")]
        [DataType(DataType.Date)]
        public DateTime OGPDate { get; set; }

        [MaxLength(500)]
        [Display(Name = "Issuance Remarks")]
        public string IssuanceRemarks { get; set; }

        [MaxLength(50)]
        [Display(Name = "OGP Status")]
        public string OGPStatus { get; set; }

        [Required]
        [Display(Name = "Item")]
        public int ItemId { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Required]
        public int CompanyId { get; set; }



        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
