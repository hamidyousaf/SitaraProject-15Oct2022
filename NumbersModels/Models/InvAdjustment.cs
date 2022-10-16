using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class InvAdjustment
    {   //Integer type variables                    //
        public int Id { get; set; }
        [Display(Name = "Adjustment No")]
        public int AdjustmentNo { get; set; }




        [Required(ErrorMessage = "Ware House is required")]
        [Display(Name ="Ware House")]
        public int WareHouseId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Display(Name = "Quantity")]
        public int Qty { get; set; }

        public int CompanyId { get; set; }
        public int VoucherId { get; set; }


        //string type                               //
        [Display(Name ="Adjustment Type")]
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
        [Display(Name ="Adustment Date")]
        [DataType(DataType.Date)]
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

        public AppCompanyConfig WareHouse { get; set; }


        //Default constructor                       //
        public InvAdjustment()
        {
            Amount = 0;
            Rate = 0;
            IsDeleted = false;
            IsSystem = false;
        }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
    }
}
