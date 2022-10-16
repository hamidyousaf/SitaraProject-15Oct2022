using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class BkgAdvanceBooking
    {
        public int Id { get; set; }

       [Display(Name="Booking No")]
       [MaxLength(450)]
       public string BookingNo { get; set; }

        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Range(1, 10000000)]
        [Display(Name = "Payment Amount")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal PaymentAmount { get; set; }

        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [MaxLength(500)]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Old Booking No")]
        [MaxLength(450)]
        public string OldBookingNo { get; set; }

        [Range(1, 10000000)]
        [Display(Name = "Old Payment Amount")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal OldPaymentAmount { get; set; }

        [Display(Name = "Old Customer")]
        public int OldCustomerId { get; set; }

        [MaxLength(30)]
        [Display(Name ="Purchase or Sales")]
        public string Type { get; set; }

        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public int VehicleId { get; set; }
        BkgVehicle Vehicle { get; set; }
    }
}
