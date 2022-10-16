using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class InvStoreIssueItem
    {
        public int Id { get; set; }
        public int StoreIssueId { get; set; }
        public int StoreIssueItemId { get; set; }
        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "Qty")] public decimal Qty { get; set; }
        [Display(Name = "Qty")] public decimal ReturnQty { get; set; }
        [Display(Name = "Rate")] public decimal Rate { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }
        [Display(Name = "Remarks")] [MaxLength(400)] public string Remarks { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Created Date")] [DataType(DataType.Date)] public DateTime? CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Updated Date")] [DataType(DataType.Date)] public DateTime? UpdatedDate { get; set; }
        //Navigation properties or data accessor variables from related tables     
        public InvItem Item { get; set; }
        public InvStoreIssue StoreIssue { get; set; }
        [NotMapped]
        public string ItemName { get; set; }
        [NotMapped]
        public string UnitName { get; set; }
    }
}
