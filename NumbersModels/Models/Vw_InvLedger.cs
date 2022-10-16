using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class Vw_InvLedger
    {
        [Display(Name = "TransType")] [MaxLength(12)] public string TransType { get; set; }
        [Display(Name = "TransNo")] public int TransNo { get; set; }
        [Display(Name = "TransDate")] public DateTime TransDate { get; set; }
        [ForeignKey("Item")]
        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "Qty")] [Column(TypeName = "numeric(18,2)")] public decimal Qty { get; set; }
        [Display(Name = "Rate")] [Column(TypeName = "numeric(20,4)")] public decimal Rate { get; set; }
        [Display(Name = "Amount")] [Column(TypeName = "numeric(37,4)")] public decimal Amount { get; set; }
        [Display(Name = "Brand")] [MaxLength(200)] public string Brand { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string TransRemarks { get; set; }
        [Display(Name = "Ware House")] public int WareHouseId { get; set; }
        public int BrandId { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "Sequence")] public int Sequence { get; set; }

        public InvItem Item { get; set; }

    }
}
