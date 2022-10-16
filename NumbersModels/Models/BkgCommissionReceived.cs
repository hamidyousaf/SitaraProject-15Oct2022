using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class BkgCommissionReceived
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Received Date")]
        [DataType(DataType.Date)]
        public DateTime ReceivedDate { get; set; }

        [Required]
        [Display(Name = "Booking Commission")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal BookingCommission { get; set; }

        [Required]
        [Display(Name = "Sales Promotion")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal AreaCommission { get; set; }

        [Required]
        [Display(Name = "Delivery Commission")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal DeliveryCommission { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [MaxLength(50)]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Booking No")]
        public int VehicleId { get; set; }
        public BkgVehicle Vehicle { get; set; }


        [Required]
        [Display(Name = "Bank Account")]
        public int BankCashAccountId { get; set; }
        public GLBankCashAccount BankCashAccount { get; set; }
        public int VoucherId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        //public GLAccount GLAccount { get; set; }
        [MaxLength(450)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
