using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HROt
    {
        public int Id { get; set; }
        public int OrgId { get; set; }
       // public int EmployeeId { get; set; }
        public int Half { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string PeriodId { get; set; }
        [MaxLength(200)] public string Remarks { get; set; }
        public string EmployeeType { get; set; }
        public string Designation { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal W1Hrs { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W2Hrs { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W3Hrs { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W4Hrs { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W5Hrs { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W6Hrs { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W1Single { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W2Single { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W3Single { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W4Single { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W5Single { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W6Single { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W1Double { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W2Double { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W3Double { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W4Double { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W5Double { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W6Double { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W1Leaves { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W2Leaves { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W3Leaves { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W4Leaves { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W5Leaves { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal W6Leaves { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Pay { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Rate { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal TotalSingleHrs { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal TotalDoubleHrs { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal FoodQty { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal FoodRate { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal ConvRate { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal ConvQty { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal ArrearFood { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal ArrearConv { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal TotalAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal TotalLeaves { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal ArrearOthers { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal FoodAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal ConvAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal TotalWorkingHrs { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal TotalOtHours { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal TotalSingleAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal TotalDoubleAmount { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal OtRate { get; set; }
         
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }


        //navigation property
        public HREmployee Employee { get; set; }
    }
}
