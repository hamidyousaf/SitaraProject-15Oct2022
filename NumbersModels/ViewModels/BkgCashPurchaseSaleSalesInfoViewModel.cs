using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Numbers.Entity.Models;
namespace Numbers.Entity.ViewModels
{
    public class BkgCashPurchaseSaleSalesInfoViewModel
    {
        public int Id { get; set; }



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

        public bool IsDeleted { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
