using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class InvItem
    {
        public int Id { get; set; }

        [Display(Name = "Barcode")] public string Barcode { get; set; }
        [Required] [Display(Name = "Item Code")]public string Code { get; set; }
        [Required] [Display(Name = "Item Name")] [Column(TypeName = "nvarchar(150)")] public string Name { get; set; }
        [Required] [Display(Name = "Sales Rate")] [Column(TypeName = "numeric(18,2)")] public decimal SalesRate { get; set; }
        [Required] [Display(Name = "Discount %")] [Column(TypeName = "numeric(18,2)")] public decimal DiscountPercentage { get; set; }
        [Required] [Display(Name = "Purchase Rate")] [Column(TypeName = "numeric(18,2)")] public decimal PurchaseRate { get; set; }
        [Display(Name = "Stock Account")] public int StockAccountId { get; set; }
        public int CustomerStockAccountId { get; set; }
        [Display(Name = "Sale Account")] public int SaleAccountId { get; set; }
        [Display(Name = "Cost of Sale Account")] public int CostofSaleAccountId { get; set; }
        [Display(Name = "UOM")][ForeignKey("UOM")] public int Unit { get; set; }
        [Display(Name = "Manufacture By")] public int? ManufacturedId { get; set; }
        [Display(Name = "Manufacture Code")] [Column(TypeName = "nvarchar(50)")] public string ManufacturedCode { get; set; }
        [Column(TypeName = "numeric(18,2)")] public  decimal PackQty  { get; set; }
        public  int PackUnit  { get; set; }
       
        [Column(TypeName = "numeric(18,6)")] public decimal AvgRate { get; set; }
        [Column(TypeName = "numeric(18,4)")] public decimal StockQty { get; set; }
        [Column(TypeName = "numeric(18,6)")] public decimal StockValue { get; set; }
        public string SerialNo { get; set; }
        public string ModelNo { get; set; }
        public bool IsPurchaseable  { get; set; }
        public bool IsSaleable  { get; set; }
        [Display(Name = "Active")] public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal MinStockLevel { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal MaxStockLevel { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal ReorderStockLevel { get; set; }
        [Display(Name = "Item Account")] public int InvItemAccountId { get; set; }
        [Display(Name = "Detail Description")] [Column(TypeName = "nvarchar(450)")] public string Description { get; set; }
        [Display(Name = "Image")] [MaxLength(450)] public string Photo { get; set; }
        [Display(Name = "Item Type")] public int ItemType { get; set; }
        [Display(Name = "Consumption Account")] public int GLConsumptionAccountId { get; set; }
        [Display(Name = "Item Category")] public int CategoryId { get; set; }
        [Required] public int CompanyId { get; set; }
        [MaxLength(450)] [Display(Name = "Created By")] public string CreatedBy { get; set; }
        [Display(Name = "Created Date")] [DataType(DataType.Date)] public DateTime? CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Updated Date")] [DataType(DataType.Date)] public DateTime? UpdatedDate { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Size { get; set; }
        public string Radius { get; set; }

        public int UOMHeight { get; set; }
        public int UOMWeight { get; set; }
        public int UOMLength { get; set; }
        public int UOMWidth { get; set; }
        public int UOMSize { get; set; }
        public int UOMRadius { get; set; }
        public int Season { get; set; }
        public int FabricConstruction { get; set; }
        public string IsLocation { get; set; }
        public int LocationIfTrue { get; set; }
        public bool IsExpired { get; set; }
        public bool IsLabTest { get; set; }
        public int WareHouse { get; set; }
        public string OrderType { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Material { get; set; }
        public string LotNo { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public InvItemAccount InvItemAccount { get; set; }
        public InvItemCategories Category { get; set; }

        public InvItem()
        {
            AvgRate = 0;
            StockQty = 0;
            StockValue = 0;
        }
        [NotMapped]
        public string Value { get; set; }
        //navigational property
        public AppCompanyConfig UOM { get; set; }
    }
}

