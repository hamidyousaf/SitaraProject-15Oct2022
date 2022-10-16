using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARQuotationDetail
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int PartNO { get; set; }
        public string UOM { get; set; }
        public int Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscuontedRate { get; set; }
        public decimal Value { get; set; }
        public decimal PkrValue { get; set; }

        public int Percentage { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalVAlue { get; set; }
        public DateTime DeliveryDate { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [Display(Name = "Approved By")] 
        public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime ApprovedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [Required] public int CompanyId { get; set; }

        [Display(Name = "Item Description")] public string ItemDescription { get; set; }

        public int QuotaionId { get; set; }

    }
}
