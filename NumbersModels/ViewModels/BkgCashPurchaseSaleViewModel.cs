using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Numbers.Entity.Models;
namespace Numbers.Entity.ViewModels
{
    public class BkgCashPurchaseSaleViewModel
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
        [Display(Name = "Booking No")]
        public string BookingNo { get; set; }

        [Range(1, 10000000)]
        [Required]
        [Display(Name = "Estimated Purchase")]
        public decimal EstimatedPurchase { get; set; }

        [MaxLength(500)]
        [Display(Name = "Booking Remarks")]
        public string BookingRemarks { get; set; }

        [Display(Name = "Booking Status")]
        [MaxLength(50)]
        public string BookingStatus { get; set; }

        [MaxLength(50)]
        [Display(Name = "IGP No")]
        public string IGPNo { get; set; }

        [Display(Name = "IGP Date")]
        [DataType(DataType.Date)]
        public DateTime IGPDate { get; set; }

        [MaxLength(50)]
        [Display(Name = "Engine No")]
        public string EngineNo { get; set; }

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

        [MaxLength(30)]
        [DisplayName("Invoice Number")]
        public string InvoiceNo { get; set; }

        [MaxLength(50)]
        [Display(Name = "OGP Status")]
        public string OGPStatus { get; set; }

        [Required]
        [Display(Name = "Item")]
        public int ItemId { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }
        
        [Range(1, 10000000)]
        [Required]
        [Display(Name = "Purchase Rate")]
        public decimal PurchaseRate { get; set; }

        [Required]
        [MaxLength(500)]
        [Display(Name = "Purchase Remarks")]
        public string PurchaseRemarks { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Purchase Party")]
        public string PurchaseParty { get; set; }

        [Display(Name = "Advance Purchase")]
        public bool AdvancePurchase { get; set; }

        public bool IsDeleted { get; set; }

        [Display(Name = "Payment Amount")]
        public decimal PaymentAmount { get; set; }



        [MaxLength(50)]
        [Display(Name = "Sales Customer CNIC")]
        public string SalesCustomerCNIC { get; set; }

        [Required]
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



        [MaxLength(50)]
        [Display(Name = "Sales Status")]
        public string SalesStatus { get; set; }

        [MaxLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; }

        //Payments
        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        //[Range(1, 10000000)]
        [DataType(DataType.Currency)]
        [Display(Name = "Payment Amount")]
        public decimal PaymentAmountD { get; set; }

        [MaxLength(100)]
        [Display(Name = "Reference")]
        public string PaymentReference { get; set; }

        [MaxLength(500)]
        [Display(Name = "Remarks")]
        public string PaymentRemarksD { get; set; }

        [Display(Name = "Bank/Cash Account")]
        public int PaymentGLAccountId { get; set; }

        public int CashPurchaseSaleId { get; set; }

        //Receipts


        [Display(Name = "Receipt Date")]
        [DataType(DataType.Date)]
        public DateTime ReceiptDate { get; set; }

        //[Range(1, 10000000)]
        [DataType(DataType.Currency)]
        [Display(Name = "Receipt Amount")]
        public decimal ReceiptAmount { get; set; }

        [MaxLength(100)]
        [Display(Name = "Reference")]
        public string ReceiptsReference { get; set; }

        [MaxLength(500)]
        [Display(Name = "Remarks")]
        public string ReceiptsRemarks { get; set; }

        [Display(Name = "Bank/Cash Account")]
        public int ReceiptsGLAccountId { get; set; }


        public bool SavePurchaseInformation { get; set; }
        public bool SaveSalesInformation { get; set; }
        public bool SavePayemnt { get; set; }
        public bool SaveReceipt { get; set; }

        [Display(Name = "Voucher Id")]
        public int VoucherId { get; set; }

        [Display(Name = "PurchaseVoucher Id")]
        public int PurchaseVoucherId { get; set; }

        [Display(Name = "SalesVoucher Id")]
        public int SalesVoucherId { get; set; }

        [Display(Name = "Payment Voucher Id")]
        public int PaymentVoucherId { get; set; }

        [Display(Name = "Receipt Voucher Id")]
        public int ReceiptVoucherId { get; set; }

        public List<BkgCashPurchaseSalePayment> PurchasePayments { get; set; }
        public List<BkgCashPurchaseSaleReceipt> SaleReceipts { get; set; }

        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
