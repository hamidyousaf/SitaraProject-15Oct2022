using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class InvStockTransferViewModel
    {
        //InvStockTransfer
        public int Id { get; set; }
        [Display(Name = "Transfer No.")] public int TransferNo { get; set; }
        [Display(Name = "Transfer Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime TransferDate { get; set; }
        [Display(Name = "Ware House To")] public int WareHouseToId { get; set; }
        [Display(Name = "Ware House From")] public int WareHouseFromId { get; set; }
        [Display(Name = "Status")] public string Status { get; set; }
        [Display(Name = "Remarks")] [MaxLength(400)] public string Remarks { get; set; }
        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Created Date")] [DataType(DataType.Date)] public DateTime? CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Updated Date")] [DataType(DataType.Date)] public DateTime? UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [Display(Name = "Approved Date")] [DataType(DataType.Date)] public DateTime? ApprovedDate { get; set; }
        public decimal Box { get; set; }
        public decimal Pcs { get; set; }
        public decimal SQM { get; set; }
        public int BranchId { get; set; }
        //InvStockTransferItem
        public int StockTransferId { get; set; }
        public int StockTransferItemId { get; set; }
        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "Qty")] public decimal Qty { get; set; }
        [Display(Name = "UOM")] public int UOM { get; set; }
        [Display(Name = "Stock")] public decimal Stock { get; set; }

        //Navigation properties or data accessor variables from related tables       
        public InvItem Item { get; set; }
        public InvStockTransfer StockTransfer { get; set; }
        public List<InvStockTransferItem> InvStockTransferItems { get; set; }
        public string UOMName { get; set; }
        public string WareHouseFromName { get; set; }
        public string WareHouseToName { get; set; }
        public string StockTransferDate { get; set; }
    }
}
