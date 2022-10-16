using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
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

        [Display(Name = "Insurance Receipt No")]
        [MaxLength(50)]
        public string InsuranceReceiptNo { get; set; }

        [Display(Name = "Insurance Amount")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal InsuranceAmount { get; set; }


        [Required]
        [Display(Name = "Received By")]
        [MaxLength(450)]
        public string ReceivedBy { get; set; }

        [Display(Name = "Bilty No")]
        [MaxLength(50)]
        public string BiltyNo { get; set; }

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

        [Display(Name = "Delivery Status")]
        [MaxLength(50)]
        public string DeliveryStatus { get; set; }

        [Display(Name = "RTP")]
        public bool RTP { get; set; }

        [Display(Name = "MF IGP")]
        [MaxLength(20)]
        public string MFIGP { get; set; }

        [Display(Name = "MF OGP")]
        [MaxLength(20)]
        public string MFOGP { get; set; }

        [Display(Name = "DL No")]
        [MaxLength(20)]
        public string DLNo { get; set; }

        [Display(Name = "PA No")]
        [MaxLength(20)]
        public string PANo { get; set; }


        [Required]
        [Display(Name = "Posted")]
        public bool Posted { get; set; }

        [Required]
        [Display(Name = "Cash Purchase")]
        public bool CashPurchase { get; set; }

        [Required]
        [Display(Name = "Booking No")]
        public int VehicleId { get; set; }
        public BkgVehicle Vehicle { get; set; }

        public int VoucherId { get; set; }

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
