using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ARSaleOrderViewModel
    {
        public ARSaleOrderViewModel()
        {
            CurrencyExchangeRate = 1;
        }
        //ARSalesOrder model data....
        public int Id { get; set; }
        [Display(Name = "S.O. No.")] public int SaleOrderNo { get; set; }
        [Display(Name = "S.O. Date")] public DateTime SaleOrderDate { get; set; } = DateTime.Now.Date;
        [Display(Name = "Validity")] public int Validity { get; set; }
        [Display(Name = "Ware House")] public int WareHouseId { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "Customer")] public int CustomerId { get; set; }
        public int PeriodId { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Reference #")] [MaxLength(30)] public string ReferenceNo { get; set; }
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


        //navigation property
        public ARCustomer Customer { get; set; }
        public AppPeriod Period { get; set; }


        //AR_SalesOrderItems
        public int SaleOrderId { get; set; }
        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "PR Item")] public int PRItemId { get; set; }
        [Display(Name = "Rate")] public decimal Rate { get; set; }
        [Display(Name = "Qty")] public decimal Qty { get; set; }
        [Display(Name = "Lot No")] public string BaleNumber { get; set; }
        [Display(Name = "Total")] public decimal Total_ { get; set; }
        [Display(Name = "Tax Amount")] public decimal TaxAmount { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }
        [Display(Name = "Tax")] public int TaxId { get; set; }

        public int CostCenter { get; set; }
        public decimal PartyLimit { get; set; }
        //navigation property
        public ARSaleOrder SaleOrder { get; set; }
        public InvItem Item { get; set; }
        public List<AppTax> TaxList { get; set; }
        public List<AppCurrency> Currencies { get; set; }
        public List<CostCenter> CostCenterList { get; set; }

       // public string DepartmentName { get; set; }
        public string UserName { get; set; }
        public string UOM { get; set; }
        public List<string> UOmName { get; set; }
        public ARSaleOrder APSaleOrder { get; set; }
        public List<ARSaleOrderItem> ARSaleOrderDetails { get; set; }
        public int DetailCostCenter { get; set; }
        public string CustomerName { get; set; }
        public string SODate { get; set; }
        public int ProductTypeId { get; set; }
        public int ItemCategoryId { get; set; }
        public int FourthCategoryId { get; set; }
        public SelectList ItemCategoryThird { get; set; }
        public SelectList ProductTypeLOV { get; set; }
        public SelectList SecondLevelCategoryLOV { get; set; }
        public SelectList CustomersLOV { get; set; }
        public SelectList WareHouseLOV { get; set; }
        public SelectList DeliveryTermLOV { get; set; }
        public SelectList PaymentTermLOV { get; set; }
    }
}
