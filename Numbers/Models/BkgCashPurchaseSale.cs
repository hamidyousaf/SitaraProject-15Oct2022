﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
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
        [Display(Name = "Booking No")]
        public string BookingNo { get; set; }


        [Required]
        [Display(Name = "Estimated Purchase")]
        public decimal EstimatedPurchase { get; set; }

        [Required]
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

        [MaxLength(50)]
        [Display(Name = "OGP Status")]
        public string OGPStatus { get; set; }

        [Required]
        [Display(Name = "Item")]
        public int ItemId { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }

        [Display(Name = "Purchase Rate")]
        public decimal PurchaseRate { get; set; }

        [MaxLength(500)]
        [Display(Name = "Purchase Remarks")]
        public string PurchaseRemarks { get; set; }

        [MaxLength(150)]
        [Display(Name = "Purchase Party")]
        public string PurchaseParty { get; set; }

        [Display(Name = "Payment Amount")]
        public decimal PaymentAmount { get; set; }

        [Display(Name = "Payment Bank Account")]
        public int PaymentBankAccount { get; set; }

        [MaxLength(50)]
        [Display(Name = "Payment Cheque No")]
        public string PaymentChequeNo { get; set; }

        [MaxLength(50)]
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; }

        [MaxLength(500)]
        [Display(Name = "Payment Remarks")]
        public string PaymentRemarks { get; set; }


        [MaxLength(50)]
        [Display(Name = "Sales Customer CNIC")]
        public string SalesCustomerCNIC { get; set; }

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

        [Display(Name = "Advance Purchase")]
        public bool AdvancePurchase { get; set; }

        public bool IsDeleted { get; set; }

        public BkgItem Item { get; set; }
        public BkgCustomer Customer { get; set; }
        //public GLAccount GLAccount { get; set; }



        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
