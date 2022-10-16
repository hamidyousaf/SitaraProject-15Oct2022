using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class BkgVehicle
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Booking No")]
        public string BookingNo { get; set; }


        [Required]
        [Display(Name = "Receipt Type")]
        public int ReceiptType { get; set; }

        [Required]
        [Display(Name = "Booking Type")]
        [MaxLength(50)]
        public string BookingType { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Reference No")]
        public string ReferenceNo { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Range(1, 10000000)]
        [Display(Name = "Received Amount")]
        public decimal ReceivedAmount { get; set; }


        [Range(1, 10000000)]
        [Display(Name = "Payment Amount")]
        public decimal PaymentAmount { get; set; }

        [Required]
        [Display(Name = "Delivery Date")]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }

        [MaxLength(500)]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [MaxLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Cash/Bank Receipt Date")]
        [DataType(DataType.Date)]
        public DateTime CashBankReceiptDate { get; set; }

        [MaxLength(50)]
        [Display(Name = "Company Receipt No")]
        public string CompanyReceiptNo { get; set; }

        [Required]
        [Display(Name = "Booking Comission")]
        public decimal BookingComission { get; set; }

        [Display(Name = "Area Comission")]
        public decimal AreaComission { get; set; }

        [Display(Name = "Cash/Bank Account")]
        public int GLAccountId { get; set; }

        [Required]
        [Display(Name = "Item")]
        public int ItemId { get; set; }
        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        [Required]
        public int CompanyId { get; set; }

        public BkgItem Item { get; set; }
        public BkgCustomer Customer { get; set; }
        //public GLAccount GLAccount { get; set; }

        public IList<BkgReceipt> BkgReceipts { get; set; }
        public IList<BkgPayment> BkgPayments { get; set; }
        public IList<BkgVehicleIGP> BkgVehicleIGPs { get; set; }
        public IList<BkgVehicleOGP> BkgVehicleOGPs { get; set; }
        public IList<BkgComissionReceived> BkgComissionReceiveds { get; set; }
        public IList<BkgComissionPayment> BkgComissionPayments { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }
}
