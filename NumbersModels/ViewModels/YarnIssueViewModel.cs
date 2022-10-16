using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class YarnIssueViewModel
    {
        //InvStoreIssue
        public int Id { get; set; }
        [Display(Name = "Issue No.")] public int IssueNo { get; set; }
        [Display(Name = "Issue Date")] [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")] public DateTime IssueDate { get; set; }
        [Display(Name = "Ware House")] public int WareHouseId { get; set; }
        [Display(Name = "Weaving Contract #")] public int WeavingContractId { get; set; }
        [Display(Name = "Sub Department")] public int SubDepartmentId { get; set; }
        [Display(Name = "Department")] public int DepartmentId { get; set; }
        [Display(Name = "Cost Center")] public int VendorId { get; set; }
        [Display(Name = "Brand")] public string Brand { get; set; }
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
        public int YarnIssuanceId { get; set; }
        public int YarnIssuanceItemId { get; set; }
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
        public YarnIssuance YarnIssuance { get; set; } = new YarnIssuance();
        public List<YarnIssuanceItem> YarnIssuanceItems { get; set; }
        public List<AppTax> TaxList { get; set; }
        public string CostCenterName { get; set; }
        public string WareHouseName { get; set; }
        public string StoreIssuesDate { get; set; }
        public SelectList BrandLOV { get; set; }
        public WarpIssuance[] WarpDetails { get; set; }
        public WeftIssuance[] WeftDetails { get; set; }
    }
}
