using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class InvAdjustmentViewModel
    {
        //Master
        public int Id { get; set; }
        [Display(Name = "Adjustment No")]
        public int AdjustmentNo { get; set; }

        [Required(ErrorMessage = "Item is required")]
        [Display(Name = "Item Name")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Ware House is required")]
        [Display(Name = "Ware House")]
        public int WareHouseId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Display(Name = "Quantity")]
        public int Qty { get; set; }
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public int VoucherId { get; set; }


        //string type                               //
        [Display(Name = "Adjustment Type")]
        [MaxLength(10)]
        public string AdjustmentType { get; set; }

        [Display(Name = "Remarks")]
        [MaxLength(100)]
        public string Remarks { get; set; }

        [MaxLength(450)]
        public string CreatedBy { get; set; }

        [MaxLength(450)]
        public string UpdatedBy { get; set; }

        [MaxLength(450)]
        public string ApprovedBy { get; set; }

        [MaxLength(10)]
        public string Status { get; set; }

        //decimal members                           //
        [Required(ErrorMessage = "Rate is required")]
        [Display(Name = "Rate")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal Rate { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Column(TypeName = "numeric(18,2)")]
        public decimal Amount { get; set; }





        //date type members                         //
        [Required(ErrorMessage = "Adjustment Date is required")]
        [Display(Name = "Adustment Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        //  [DataType(DataType.Date)]
        public DateTime AdjustmentDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ApprovedDate { get; set; }


        //bool type members                         //
        public bool IsDeleted { get; set; }
        public bool IsSystem { get; set; }

        //Navigation properties or data accessor variables from related tables       
        public InvItem Item { get; set; }
        public AppCompanyConfig WareHouse { get; set; }


        //Detail item
        public int AdjustmentId { get; set; }
        public int AdjustmentItemId { get; set; }

        public InvAdjustment InvAdjustments { get; set; }
        public List<InvAdjustmentItem> InvAdjustmentItems { get; set; }
       
        [Display(Name = "UOM")] public int UOM { get; set; }
        [Display(Name = "Stock")] public decimal Stock { get; set; }
        [Display(Name = "Qty")] public decimal PhysicalStock { get; set; }
        [Display(Name = "Balance")] public decimal Balance { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }
      public string WareHouseName { get; set; }
        public string AdjDate { get; set; }
    }
}
