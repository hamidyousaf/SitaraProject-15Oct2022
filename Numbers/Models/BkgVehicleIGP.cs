using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class BkgVehicleIGP
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "IGP No")]
        public int IGPNo { get; set; }

        [Required]
        [Display(Name = "IGP Date")]
        [DataType(DataType.Date)]
        public DateTime IGPDate { get; set; }

        [Required]
        [Display(Name = "Deliver Comission")]
        public decimal DeliveryComission { get; set; }

        [Required]
        [Display(Name = "Engine No")]
        [MaxLength(50)]
        public string EngineNo { get; set; }

        [Required]
        [Display(Name = "Chassis No")]
        [MaxLength(50)]
        public string ChassisNo { get; set; }


        [Required]
        [Display(Name = "Reference No")]
        [MaxLength(50)]
        public string MTLRefrenceNo { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [MaxLength(50)]
        public string Status { get; set; }


        [Display(Name = "Voucher ID")]
        public int VoucherID { get; set; }

        [Required]
        [Display(Name = "Booking Vehicle")]
        public int BkgVehicleId { get; set; }

        public BkgVehicle BkgVehicle { get; set; }

        [Required]
        public int CompanyId { get; set; }


        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
