using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class BkgVehicle
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "Trans. Date")]
        [DataType(DataType.Date)]
        public DateTime TransDate { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Booking No")]
        public string BookingNo { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Receipt Type")]
        public string ReceiptType { get; set; }

        [Required]
        [Display(Name = "Booking Type")]
        [MaxLength(50)]
        public string BookingType { get; set; }

        [MaxLength(150)]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [MaxLength(150)]
        [Display(Name = "Receipt No")]
        public string ReferenceNo { get; set; }

        [Required]
        [Range(1, 10000000)]
        [Column(TypeName = "numeric(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Received Amount")]
        [Range(1, 10000000)]
        [Column(TypeName = "numeric(18,2)")]
        public decimal ReceivedAmount { get; set; }

        [Display(Name = "Delivery Date")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
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

        [MaxLength(50)]
        [Display(Name = "Payment Advice No")]
        public string PaymentAdviceNo { get; set; }

        [MaxLength(50)]
        [Display(Name = "MTL. Ref. No")]
        public string MTLReferenceNo{ get; set; }


        public int PaymentBankCashAccountId { get; set; }
        [Display(Name = "Cash/Bank Account")]
        public int BankCashAccountId { get; set; }
        public GLBankCashAccount BankCashAccount { get; set; }

        [Required]
        [Display(Name = "Item")]
        public int ItemId { get; set; }
        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        [Required]
        public int CompanyId { get; set; }
        public int CancelVoucherId { get; set; }

        public InvItem Item { get; set; }
        public BkgCustomer Customer { get; set; }
        //public GLAccount GLAccount { get; set; }

        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [MaxLength(450)]
        public string CancelledBy { get; set; }
        [Display(Name ="Cancelled Date")]
        public DateTime? CancelledDate { get; set; }

        [MaxLength(500)]
        [Display(Name = "Canceling Remarks")]
        public string CancelRemarks { get; set; }


        [Range(1, 10000000)]
        [Display(Name = "Payment Amount")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal PaymentAmount { get; set; }
       
        [Display(Name = "Engine No")]
        [MaxLength(50)]
        public string EngineNo { get; set; }
       
        [Display(Name = "Chassis No")]
        [MaxLength(50)]
        public string ChassisNo { get; set; }

        public int OGPNo { get; set; }
        public int VoucherId { get; set; }
        public int PaymentVoucherId { get; set; }
        public GLVoucher Voucher { get; set; }

        public IList<BkgReceipt> BkgReceipts { get; set; }
        public IList<BkgPayment> BkgPayments { get; set; }
        public IList<BkgVehicleIGP> BkgVehicleIGPs { get; set; }
        public IList<BkgVehicleOGP> BkgVehicleOGPs { get; set; }
        public IList<BkgCommissionReceived> BkgCommissionReceiveds { get; set; }
        public IList<BkgCommissionPayment> BkgCommissionPayments { get; set; }

    }
}
