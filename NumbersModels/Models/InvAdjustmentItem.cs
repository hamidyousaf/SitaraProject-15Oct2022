using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class InvAdjustmentItem
    {
        public int Id { get; set; }
        public int AdjustmentId { get; set; }
        public InvAdjustment Adjustment { get; set; }
        public int ItemId { get; set; }
        public InvItem Item { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Rate { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Stock { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal PhysicalStock { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Balance { get; set; }
        [Display(Name = "Line Total")] [Column(TypeName = "numeric(18,2)")] public decimal LineTotal { get; set; }

        [Display(Name = "Remarks")] [MaxLength(100)] public string Remarks { get; set; }
        [NotMapped]
        public string UnitName { get; set; }
        [NotMapped]
        public string ItemName{get;set;}
        public bool IsDeleted { get; set; }
    }
}
