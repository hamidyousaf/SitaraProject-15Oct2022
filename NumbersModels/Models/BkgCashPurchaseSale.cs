using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class BkgCashPurchaseSale
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "Trans Date")]
        [DataType(DataType.Date)]
        public DateTime TransDate { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Booking No")]
        public string BookingNo { get; set; }

        [MaxLength(30)]
        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }


        [Required]
        [Column(TypeName = "numeric(18,2)")]
        [Display(Name = "Estimated Purchase")]
        [Range(1, 10000000)]
        public decimal EstimatedPurchase { get; set; }

        [MaxLength(500)]
        [Display(Name = "Booking Remarks")]
        public string BookingRemarks { get; set; }

        [Display(Name = "Booking Status")]
        [MaxLength(50)]
        public string BookingStatus { get; set; }

        [Required(ErrorMessage ="This field is required.")]
        [MaxLength(50)]
        [Display(Name = "IGP No")]
        public string IGPNo { get; set; }

        [Required]
        [Display(Name = "IGP Date")]
        [DataType(DataType.Date)]
        public DateTime IGPDate { get; set; }

        [Required(ErrorMessage ="This field is required.")]
        [MaxLength(50)]
        [Display(Name = "Engine No")]
        public string EngineNo { get; set; }

        [Required(ErrorMessage="This Field is required.")]
        [MaxLength(50)]
        [Display(Name = "Chassis No")]
        public string ChassisNo { get; set; }

        [MaxLength(500)]
        [Display(Name = "Receiving Remarks")]
        public string ReceivingRemarks { get; set; }

        [MaxLength(50)]
        [Display(Name = "IGP Status")]
        public string IGPStatus { get; set; }


        [MaxLength(50)]
        [Display(Name = "OGP No")]
        public string OGPNo { get; set; }

        [Display(Name = "OGP Date")]
        [DataType(DataType.Date)]
        public DateTime OGPDate { get; set; }

        [MaxLength(500)]
        [Display(Name = "Issuance Remarks")]
        public string IssuanceRemarks { get; set; }

        [MaxLength(50)]
        [Display(Name = "OGP Status")]
        public string OGPStatus { get; set; }

        [Required]
        [Display(Name = "Item")]
        public int ItemId { get; set; }

       
        [Display(Name = "Vehicle Id")]
        public int VehicleId { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        [Display(Name = "Purchase Rate")]
        public decimal PurchaseRate { get; set; }

        [MaxLength(500)]
        [Display(Name = "Purchase Remarks")]
        public string PurchaseRemarks { get; set; }

        [MaxLength(150)]
        [Display(Name = "Purchase Party")]
        public string PurchaseParty { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        [Display(Name = "Payment Amount")]
        public decimal PaymentTotalAmount { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        [Display(Name = "Receipt Amount")]
        public decimal ReceiptTotalAmount { get; set; }

        public int SalesCustomerId { get; set; }

        [MaxLength(50)]
        [Display(Name = "Sales Customer CNIC")]
        public string SalesCustomerCNIC { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        [Display(Name = "Sales Rate")]
        public decimal SalesRate { get; set; }


        [Display(Name = "Sales Date")]
        [DataType(DataType.Date)]
        public DateTime SalesDate { get; set; }

        [MaxLength(500)]
        [Display(Name = "Sales Remarks")]
        public string SalesRemarks { get; set; }

        [MaxLength(150)]
        [Display(Name = "Sales Party")]
        public string SalesParty { get; set; }

        [Display(Name = "Sales Party Account")]
        public int SalesPartyAccount { get; set; }

        [Display(Name = "Voucher Id")]
        public int VoucherId { get; set; }

        [Display(Name ="PurchaseVoucherId")]
        public int PurchaseVoucherId { get; set; }

        [Display(Name ="SalesVoucherId")]
        public int SalesVoucherId { get; set; }

        [MaxLength(50)]
        [Display(Name = "Sales Status")]
        public string SalesStatus { get; set; }

        [MaxLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; }

        public bool IsDeleted { get; set; }


        public InvItem Item { get; set; }
        public BkgVehicle Vehicle { get; set; }
        public BkgCustomer Customer { get; set; }
        public int SalePaymentId { get; set; }
        public int SaleReceiptId { get; set; }
        public BkgCashPurchaseSalePayment SalePayment { get; set; }
        public BkgCashPurchaseSaleReceipt SaleReceipt { get; set; }
        //public GLAccount GLAccount { get; set; }

        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
