using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
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

        [Display(Name = "DLNo")]
        public int? DLNo { get; set; }


        [Required]
        [Display(Name = "Delivery Commission")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal DeliveryCommission { get; set; }

        [Display(Name ="Booking Commission")]
        [Column(TypeName ="numeric(18,2)")]
        public decimal BookingCommission { get; set; }

        [Display(Name = "Sales Promotion")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal AreaCommission { get; set; }

        [Required]
        [Display(Name = "Insurance Amount")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal InsuranceAmount { get; set; }

        [Display(Name = "Advance Booking Purchase")]
        public bool AdvanceBookingPurchase { get; set; }

        [Required]
        [Display(Name = "Engine No")]
        [MaxLength(50)]
        public string EngineNo { get; set; }

        [Required]
        [Display(Name = "Chassis No")]
        [MaxLength(50)]
        public string ChassisNo { get; set; }


        [Required]
        [Display(Name = "Invoice No")]
        [MaxLength(50)]
        public string InvoiceNo { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [MaxLength(50)]
        public string Status { get; set; }


        [Display(Name = "Voucher Id")]
        public int VoucherId { get; set; }

        [Required]
        [Display(Name = "Booking No")]
        public int VehicleId { get; set; }
        public BkgVehicle Vehicle { get; set; }

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
