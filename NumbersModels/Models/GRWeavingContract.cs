using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRWeavingContract
    {
        public int Id { get; set; }

        [Display(Name = "Trans No")] public int TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        [Display(Name = "Vendor")] public int VendorId { get; set; }
        public DateTime DeliveryDate { get; set; }
        [ForeignKey("GreigeQuality")]
        public int GreigeQualityId { get; set; }
        [ForeignKey("GreigeQualityLoom")]
        public int GreigeQualityLoomId { get; set; }
        public int NoOfLooms { get; set; }
        [Display(Name = "Contract Qty")] public decimal ContractQty { get; set; }
        [Display(Name = "Balance Contract Qty")] public decimal BalanceContractQty { get; set; }
        [Display(Name = "Width")] public decimal Width { get; set; }
        [Display(Name = "Picks")] public decimal Picks { get; set; }
        [Display(Name = "RatePerPick")] public decimal RatePerPicks { get; set; }
        [Display(Name = "Reed")] public decimal Reed { get; set; }
        [Display(Name = "Add.Reed")] public decimal AdditionalReed { get; set; }
        [Display(Name = "Sale Tax %")] public decimal SaleTax { get; set; }
        public string Warp { get; set; }
        public decimal WarpCount { get; set; }
        public decimal WarpWeightPerMeter { get; set; }
        [Display(Name = "Total Warp Bags")] public decimal? TotalWarpBags { get; set; }
        [Display(Name = "Warp Rate Pound")] public decimal? WarpRatePound { get; set; }
        public string Weft { get; set; }
        public decimal WeftCount { get; set; }
        public decimal WeftWeightPerMeter { get; set; }
        [Display(Name = "Total Warp Bags")] public decimal? TotalWeftBags { get; set; }
        [Display(Name = "Warp Rate Pound")] public decimal? WeftRatePound { get; set; }


        [Column(TypeName = "numeric(18,6)")] public decimal RateOfConversionExcTax { get; set; }
        [Column(TypeName = "numeric(18,4)")] public decimal SaleTaxAmount { get; set; }
        [Column(TypeName = "numeric(18,4)")] public decimal RateOfConversionIncTax { get; set; }
        [Column(TypeName = "numeric(18,6)")] public decimal PriceOfYarn { get; set; }
        [Column(TypeName = "numeric(18,6)")] public decimal ValueOfGreige { get; set; }
        [Column(TypeName = "numeric(18,6)")] public decimal TotalContractAmount { get; set; }

        public bool IsDeleted { get; set; }

        [Required] public int CompanyId { get; set; }
        [MaxLength(450)][Display(Name = "Created By")] public string CreatedBy { get; set; }
        [Display(Name = "Created Date")][DataType(DataType.Date)] public DateTime? CreatedDate { get; set; }
        [Display(Name = "Updated By")][MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Updated Date")][DataType(DataType.Date)] public DateTime? UpdatedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; }
        public int GRRequisitionId { get; set; }



        public GRQuality GreigeQuality { get; set; }
        public GRQuality GreigeQualityLoom { get; set; }

        public APSupplier Vendor { get; set; }
        [NotMapped]
        public GRWeavingContract WeavingContract { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }

        
    }
}

