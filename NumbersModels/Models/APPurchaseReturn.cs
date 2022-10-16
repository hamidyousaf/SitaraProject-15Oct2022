using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APPurchaseReturn
    {
        public int Id { get; set; }
        [Display(Name = "Return No.")] public int ReturnNo { get; set; }
        [Display(Name = "Return Date")] [DataType(DataType.Date)] public DateTime ReturnDate { get; set; }
        public int PeriodId { get; set; }
        public int VoucherId { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "Currency Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "WareHouse")] public int WareHouseId { get; set; }
        [Display(Name = "Supplier")] public int SupplierId { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        [Display(Name = "Reference No")] [MaxLength(25)] public string ReferenceNo { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
        [Display(Name = "Total Discount Amount")] [Column(TypeName = "numeric(18,2)")] public decimal TotalDiscountAmount { get; set; }
        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }
        public int CompanyId { get; set; }


        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime ApprovedDate { get; set; }

        public bool IsDeleted { get; set; }

        //navigation property
        public APSupplier Supplier { get; set; }
        public AppPeriod Period { get; set; }

    }
}
