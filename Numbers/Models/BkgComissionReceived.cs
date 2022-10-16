using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class BkgComissionReceived
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Received Date")]
        [DataType(DataType.Date)]
        public DateTime ReceivedDate { get; set; }

        [Required]
        [Display(Name = "Booking Comission")]
        public decimal BookingComission { get; set; }

        [Required]
        [Display(Name = "Area Comission")]
        public decimal AreaComission { get; set; }

        [Required]
        [Display(Name = "Delivery Comission")]
        public decimal DeliveryComission { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [MaxLength(50)]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Booking Vehicle")]
        public int BkgVehicleId { get; set; }

        [Required]
        [Display(Name = "Bank Account")]
        public int GLAccountId { get; set; }

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
