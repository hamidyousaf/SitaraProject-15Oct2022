using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.ViewModels
{
    public class BkgReceiptViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Receipt Date")]
        [DataType(DataType.Date)]
        public DateTime ReceiptDate { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Receipt Account")]
        public string ReceiptAccount { get; set; }

        [Required]
        [Display(Name = "Receipt Amount")]
        public decimal ReceiptAmount { get; set; }

        [Required]
        [Display(Name = "Receipt Type")]
        [MaxLength(50)]
        public string ReceiptType { get; set; }

        [Required]
        [Display(Name = "MTL Reference")]
        [MaxLength(50)]
        public string MTLReference { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "CR Account")]
        [MaxLength(50)]
        public string CRAccount { get; set; }

        [Required]
        [Display(Name = "Status")]
        [MaxLength(50)]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Booking Vehicle")]
        public int VehicleId { get; set; }

        public string CustomerName { get; set; }
        public decimal ReceivedAmount { get; set; }

        public BkgVehicle BkgVehicle { get; set; }

        [MaxLength(450)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
