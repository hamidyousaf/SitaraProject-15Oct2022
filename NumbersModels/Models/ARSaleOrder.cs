using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARSaleOrder
    {
        public int Id { get; set; }
        [Display(Name = "S.O. No.")] public int SaleOrderNo { get; set; }
        [Display(Name = "S.O. Date")] [DataType(DataType.Date)] public DateTime SaleOrderDate { get; set; }
        [Display(Name = "Validity")] public int Validity { get; set; }
        [Display(Name = "Ware House")] public int WareHouseId { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "Currency Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "Customer")] public int CustomerId { get; set; }
        public int PeriodId { get; set; }
        [Display(Name ="Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Refrence No")] [MaxLength(30)] public string ReferenceNo { get; set; }
        [Display(Name = "Freight")] [Column(TypeName = "numeric(18,2)")] public decimal Freight { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
        [Display(Name = "Total Sale Tax")] [Column(TypeName = "numeric(18,2)")] public decimal TotalTaxAmount { get; set; }
        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }
        public int CompanyId { get; set; }
        public int ResponsibilityId { get; set; }
        [Display(Name = "Delivery Terms")] public int DeliveryTermId { get; set; }
        [Display(Name = "Payment Terms")] public int PaymentTermId { get; set; }
        [Display(Name = "Status")] [MaxLength(10)] public string Status { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int CostCenter { get; set; }

        //navigation property
        public AppCompanyConfig DeliveryTerm { get; set; }
        public AppCompanyConfig PaymentTerm { get; set; }
        public ARCustomer Customer { get; set; }
        public AppCompanyConfig WareHouse { get; set; }
        public AppPeriod Period { get; set; }
        public AppCompanyConfig ProductType { get; set; }
        public InvItemCategories ItemCategory { get; set; }
        public int ProductTypeId { get; set; }
        public int ItemCategoryId { get; set; }
        public int FourthCategoryId { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
    }
}
