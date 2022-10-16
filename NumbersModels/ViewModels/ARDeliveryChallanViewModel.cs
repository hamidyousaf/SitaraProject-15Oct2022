using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ARDeliveryChallanViewModel
    {
        //ARDeliveryChallans
        public int Id { get; set; }
        [Display(Name = "D.C. No.")] public int DCNo { get; set; }
        [Display(Name = "D.C. Date")] public DateTime DCDate { get; set; }
        [Display(Name = "Manual D.C. No.")] public int ManualDCNo { get; set; }
        public int SaleOrderId { get; set; }
        public DateTime ExpiryDate { get; set; }
        [Display(Name = "Customer")] public int CustomerId { get; set; }
        [Display(Name = "Ship To")] public int ShipToId { get; set; }
        [Display(Name = "Store")] public int Storemaster { get; set; }
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public int ResponsibilityId { get; set; }
        [Display(Name = "Item Group")] public int ItemGroupId { get; set; }
        [Display(Name = "Sales Category")] public int SalesCategoryId { get; set; }
        [Display(Name = "Driver Contact No.")] public string DriverContactNo { get; set; }
        [Display(Name = "Vehicle No.")] public string VehicleNo { get; set; }
        [Display(Name = "Vehicle Type")] public int VehicleType { get; set; }
        [Display(Name = "Driver Name")] [MaxLength(50)] public string DriverName { get; set; }
        [Display(Name = "Transport Company")] [MaxLength(50)] public string TransportCompany { get; set; }
        [Display(Name = "Builty No.")] public string BuiltyNo { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        public string Status { get; set; }
        [Display(Name = "Attachment")] [MaxLength(450)] public string Attachment { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
       
        public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsDeleted { get; set; }

        //navigation porperty
        public ARCustomer Customer { get; set; }


        //ARDeliveryChallanItems

        [Display(Name = "D.C. No")] public int DCId { get; set; }
        [Display(Name = "Sale Order No.")] public int SaleOrderNo { get; set; }

        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "PR Item")] public int PRItemId { get; set; }
        [Display(Name = "Qty")] [Column(TypeName = "numeric(18,2)")] public decimal Qty { get; set; }
        [Display(Name = "Sale Order Balance")] [Column(TypeName = "numeric(18,2)")] public decimal SaleOrderBalance { get; set; }
        [Display(Name = "Bonus")] [Column(TypeName = "numeric(18,2)")] public decimal Bonus { get; set; }
        [Display(Name = "DC Balance")] [Column(TypeName = "numeric(18,2)")] public decimal DCBalance { get; set; }
        [Display(Name = "Stock In Store")] public int StockInStore { get; set; }
        [Display(Name = "Company")] public string Company { get; set; }

        //navigation property
        public ARSaleOrder SaleOrder { get; set; }
        public InvItem Item { get; set; }
        public string UOM { get; set; }
        public string CustomerName { get; set; }
        public int StoreDetail { get; set; }
        public decimal Total { get; set; }

        public List<ARDeliveryChallanItem> ARDeliveryChallanItemList { get; set; }
        public string DDate { get; set; }
        public string BaleType { get; set; }
        public ARDeliveryChallan ARDeliveryChallans { get; set; }
    }
}