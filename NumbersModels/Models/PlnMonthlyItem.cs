using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class PlnMonthlyItem
    {
        public int Id { get; set; }
        public int PlnMonthlyId { get; set; }
        [Display(Name = "Item")] public int ItemCategory4Id { get; set; }
        [Display(Name = "Item")] public int SpecificationId { get; set; }
        [Display(Name = "Item")] public int MonthId { get; set; }
        [Display(Name = "Item")] public int SeasonalDetailId { get; set; }

        [Display(Name = "Qty")] public decimal SeasonalRunQty { get; set; }
        [Display(Name = "Qty")] public decimal SeasonalDesignCount { get; set; }
        [Display(Name = "Qty")] public decimal SeasonalFabricQty { get; set; }
        [Display(Name = "Qty")] public decimal MonthlyDesignCount { get; set; }
        [Display(Name = "Qty")] public decimal MonthlyRunQty { get; set; }
        [Display(Name = "Qty")] public decimal MonthlyFabicCons { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Created Date")] [DataType(DataType.Date)] public DateTime? CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Updated Date")] [DataType(DataType.Date)] public DateTime? UpdatedDate { get; set; }
        //Navigation properties or data accessor variables from related tables     
        public InvItemCategories ItemCategory4 { get; set; }
        public PlnMonthly PlnMonthly { get; set; }
        public PlanSpecification Specification { get; set; }
        public AppCompanyConfig  Month { get; set; }
        [NotMapped]
        public string ItemName { get; set; }
        [NotMapped]
        public string UnitName { get; set; }
    }
}
