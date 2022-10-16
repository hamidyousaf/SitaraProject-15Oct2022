using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class APPurchaseOrderViewModel
    {
        public APPurchaseOrderViewModel()
        {
            CurrencyExchangeRate = 1;
        }
        //APPurchaseOrder
        public int Id { get; set; }
        [Display(Name = "P.O. No.")] public int PONo { get; set; }
        [Display(Name = "P.O. Date")] [DataType(DataType.Date)] public DateTime PODate { get; set; } 
        public int PeriodId { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "P.O. Type")] public int POTypeId { get; set; }
        [Display(Name = "Supplier")] public int SupplierId { get; set; }
        [Display(Name = "Delivery Terms")] public int DeliveryTermId { get; set; }
        [Display(Name = "Payment Terms")] public int PaymentTermId { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        public string Brand { get; set; }
        [Display(Name = "Reference No")] [MaxLength(25)] public string ReferenceNo { get; set; }
        [Display(Name = "Attachments")] [MaxLength(450)] public string Attachment { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
        [Display(Name = "Total  Sale Tax")] [Column(TypeName = "numeric(18,2)")] public decimal TotalTaxAmount { get; set; }
        [Display(Name = "Freight")] [Column(TypeName = "numeric(18,2)")] public decimal Freight { get; set; }
        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }
        public int CompanyId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime ApprovedDate { get; set; }

        public bool IsDeleted { get; set; }

        //more fields
        [Display(Name = "Payment Mode")]  public string PaymentMode { get; set; }
        [Display(Name = "Cost Terms")]  public string CostTerms { get; set; }
        [Display(Name = "Shipping Mode")]  public string ShippingMode { get; set; }
        [Display(Name = "Shipping Port")]  public string ShippingPort { get; set; }
        [Display(Name = "Discharge Port")]  public string DischargePort { get; set; }
        [Display(Name = "Origin")]  public string Origin { get; set; }
        [Display(Name = "Import Type")]  public string ImportType { get; set; }

        public int OrganizationId { get; set; }
        public int DepartmentId { get; set; }
        public int OperationId { get; set; }
        public int Resp_ID { get; set; }


        //APPurchaseOrderDetail
        [Display(Name = "PO Id")] public int POId { get; set; }
        [Display(Name = "PR Item")] public int PRItemId { get; set; }
        [Display(Name = "Sequence No")] public int SequenceNo { get; set; }
        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "Qty")] public decimal Qty { get; set; }
        [Display(Name = "Rate")] public decimal Rate { get; set; }
        [Display(Name = "Total")] public decimal Total_ { get; set; }
        [Display(Name = "Tax")] public int TaxId { get; set; }
        [Display(Name = "Tax Amount")] public decimal TaxAmount { get; set; }
        [Display(Name = "Line Total")] public decimal LineTotal { get; set; }
        [Display(Name = "PKR Value")] public decimal PKRValue { get; set; }

        [Display(Name = "Delivery Date")] public DateTime DeliveryDate { get; set; }

        [Display(Name = "HS Code")] public int HSCode { get; set; }
        [Display(Name = "Net Total")] public decimal NetTotal { get; set; }
        [Display(Name = "FCValue")] public decimal FCValue { get; set; }
        [Display(Name = "Performa No")] public int PerformaNo { get; set; }
        public int CostCenter { get; set; }
        public int DetailCostCenter { get; set; }




        //navigation property
        public APPurchaseOrder PO { get; set; }
        public List<APPurchaseOrderItem> PurchaseItems { get; set; }
        public InvItem Item { get; set; }
        public APSupplier Supplier { get; set; }
        public List<AppTax> TaxList { get; set; }
        public List<CostCenter> CostCenterList { get; set; }
        public List<AppCurrency> Currencies { get; set; }
        public List<ListOfValue> BrandList { get; set; }
    }
}
