using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class BkgVehicleOGP
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "OGP No")]
        public int OGPNo { get; set; }

        [Required]
        [Display(Name = "OGP Date")]
        [DataType(DataType.Date)]
        public DateTime OGPDate { get; set; }


        //[Required]
        //[Display(Name = "Engine No")]
        //[MaxLength(50)]
        //public string EngineNo { get; set; }

        //[Required]
        //[Display(Name = "Chassis No")]
        //[MaxLength(50)]
        //public string ChassisNo { get; set; }


        [Required]
        [Display(Name = "Received By")]
        [MaxLength(50)]
        public string ReceivedBy { get; set; }

        [Required]
        [Display(Name = "Bilty No")]
        [MaxLength(50)]
        public string BiltyNo { get; set; }

        [Required]
        [Display(Name = "Vehicle No")]
        [MaxLength(50)]
        public string VehicleNo { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [MaxLength(50)]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Posted")]
        public bool Posted { get; set; }

        [Required]
        [Display(Name = "Cash Purchase")]
        public bool CashPurchase { get; set; }

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
