using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Numbers.Models;
namespace Numbers.ViewModels
{
    public class BkgCashPurchaseSalePurchaseInfoViewModel
    {
        public int Id { get; set; }

        

        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }
        //[Range(1, 10000000)]
        [Display(Name = "Purchase Rate")]
        public decimal PurchaseRate { get; set; }

        [MaxLength(500)]
        [Display(Name = "Purchase Remarks")]
        public string PurchaseRemarks { get; set; }

        [MaxLength(150)]
        [Display(Name = "Purchase Party")]
        public string PurchaseParty { get; set; }


        [Display(Name = "Advance Purchase")]
        public bool AdvancePurchase { get; set; }

        public bool IsDeleted { get; set; }

        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
