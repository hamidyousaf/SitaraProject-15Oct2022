using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ProductionOrderItem
    {
        [Key]
        public int Id { get; set; }
        public int ProductionOrderId { get; set; }
        [Display(Name = "Item Category 4")]
        public int FourthItemCategoryId { get; set; }
        public int ItemCategorySecondId { get; set; }
        [Display(Name = "Greige Quality")]
        [ForeignKey("GreigeQuality")]
        public int GreigeQualityId { get; set; }
        [Display(Name = "Greige Quality")]
        public string GreigeQualityDesc { get; set; }
        [Display(Name = "Item")]
        public int ItemId { get; set; }
        public int TypeId { get; set; }
        public int MPDetailId { get; set; }
        public string Month { get; set; }
        [Display(Name = "Version")]
        public int VersionId { get; set; }
        [Display(Name = "Process Type")]
        public int ProcessTypeId { get; set; }
        public decimal VersionConversion { get; set; }
        [Display(Name = "Version Qty (mtrs)")]
        public int VersionQuantity { get; set; }
        [Display(Name = "Colors Variation")]
        public int ColorVariations { get; set; }
        public int Pcs { get; set; }
        public int SuitMeters { get; set; }
        public int GroupId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        //public PlanSpecification GreigeQuality { get; set; }
        public GRQuality GreigeQuality { get; set; }
        public InvItemCategories FourthItemCategory { get; set; }
        public InvItem Item { get; set; }
        public PlanMonthlyPlanningItems MPDetail { get; set; }
        public AppCompanyConfig Type { get; set; }
        public AppCompanyConfig Version { get; set; }
        public AppCompanyConfig ProcessType { get; set; }
    }
}