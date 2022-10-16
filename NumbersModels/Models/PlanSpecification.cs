using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    [Area("Planning")]
  public class PlanSpecification
    {   [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Code { get; set; }
        [ForeignKey("ItemCategoryLevel2Id")]
        public int ItemCategoryLevel2Id { get; set; }
        [ForeignKey("ItemCategoryLevel4Id")]
        [Display(Name = "Item Category 4")]
        public int ItemCategoryLevel4Id { get; set; }
        [ForeignKey("GRQuality")]
        [Display(Name = "Greige Quality")]
        public int GRQualityId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsApproved { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Status { get; set; }
        //Navigational Property
        public InvItemCategories ItemCategoryLevel2 { get; set; }
        public InvItemCategories ItemCategoryLevel4 { get; set; }
        public GRQuality GRQuality { get; set; }
        [NotMapped]
        public string StartDate { get; set; }
        [NotMapped]
        public string EndDate { get; set; }
    }
}
