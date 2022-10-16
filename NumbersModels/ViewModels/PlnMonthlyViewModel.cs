using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class PlnMonthlyViewModel
    {
        //InvStoreIssue
        public int Id { get; set; }
        [Display(Name = "Trans No.")] public int IssueNo { get; set; }
        [Display(Name = "Transaction Date")] [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")] public DateTime IssueDate { get; set; }
        [Display(Name = "Seasonal Planning")] public int SeasonId { get; set; }
        [Display(Name = "Plan of ")] public int MonthId { get; set; }
        [Display(Name = "Sub Department")] public int SubDepartmentId { get; set; }
        [Display(Name = "Department")] public int DepartmentId { get; set; }
        [Display(Name = "Status")] public string Status { get; set; }
        [Display(Name = "Remarks")] [MaxLength(400)] public string Remarks { get; set; }
        public int VoucherId { get; set; }
        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Created Date")] [DataType(DataType.Date)] public DateTime? CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Updated Date")] [DataType(DataType.Date)] public DateTime? UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [Display(Name = "Approved Date")] [DataType(DataType.Date)] public DateTime? ApprovedDate { get; set; }
        public int BranchId { get; set; }
        //InvStoreIssueItem
        public int StoreIssueId { get; set; }
        public int StoreIssueItemId { get; set; }
        public decimal Box { get; set; }
        public decimal Pcs { get; set; }
        public decimal SQM { get; set; }

        public decimal Boxes { get; set; }
        public decimal Tiles { get; set; }

        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "ItemCode")] public string ItemCode { get; set; }
        [Display(Name = "Item Name")] public string ItemName { get; set; }
        [Display(Name = "Qty")] public decimal Qty { get; set; }
        [Display(Name = "Rate")] public decimal Rate { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }
        [Display(Name = "UOM")] public string UOM { get; set; }
        [Display(Name = "Stock")] public decimal Stock { get; set; }

        //Navigation properties or data accessor variables from related tables       
        public InvItem Item { get; set; }
        public PlnMonthly PlnMonthly { get; set; }
        public InvItemCategories ItemCategory4 { get; set; }
        public List<PlnMonthlyItem> InvStoreIssueItems { get; set; }
        public List<AppTax> TaxList { get; set; }
        public string CostCenterName { get; set; }
       // public string WareHouseName { get; set; }
        public String Season { get; set; }
        public string Month { get; set; }
        public string StoreIssuesDate { get; set; }


        public List<PlnMonthlyItem> PlnMonthlyItems { get; set; }


        public List<PlnMonthly> PlnMonthlys { get; set; }
    }
}
