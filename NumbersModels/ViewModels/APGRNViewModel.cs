using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class APGRNViewModel
    {
        public APGRNViewModel()
        {
            CurrencyExchangeRate = 1;
        }

        //APGRN

        public int Id { get; set; }

        [Display(Name = "GRN No #")] public int GRNNO { get; set; }
        [Display(Name = "GRN Date")] public DateTime GRNDate { get; set; }
        [Display(Name = "PONo #")] public int IRNNo { get; set; }
        [Display(Name = "Vendor")] public string Vendor { get; set; }
        [Display(Name = "Vendor Name")] public string VendorName { get; set; }
        [Display(Name = "Warehouse")] public string Warehouse { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "P.O. Type")] public int POTypeId { get; set; }
        [Display(Name = "Supplier")] public int SupplierId { get; set; }
        [Display(Name = "Brand")] public int BrandId { get; set; }
        public string Brand { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        [Display(Name = "Attachments")] [MaxLength(450)] public string Attachment { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
        [Display(Name = "Total Sale Tax")] [Column(TypeName = "numeric(18,2)")] public decimal TotalSaleTax { get; set; }
        [Display(Name = "Freight")] [Column(TypeName = "numeric(18,2)")] public decimal Freight { get; set; }
        [Display(Name = "Total Expense")] [Column(TypeName = "numeric(18,2)")] public decimal TotalExpense { get; set; }

        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }


        public int CompanyId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime ApprovedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int CostCenter { get; set; }
        public int DepartmentId { get; set; }
        public int OperationId { get; set; }
        public int Resp_ID { get; set; }
        public int IRNItemId { get; set; }

        //APGRNItem

        [Display(Name = "PO Id")] public int POId { get; set; }
        [Display(Name = "PR Item")] public int GRNItemId { get; set; }
        [Display(Name = "PR Item")] public int PRItemId { get; set; }
        [Display(Name = "Item")] public int ItemId { get; set; }
        [Display(Name = "HS Code")] public string HSCode { get; set; }

        [Display(Name = "GRN Qty")] public decimal GRNQty { get; set; }

        [Display(Name = "IRN Qty")] public decimal IRNQty { get; set; }
        [Display(Name = "Rejected Qty")] public decimal RejectedQty { get; set; }
        [Display(Name = "Accepted Qty")] public decimal AcceptedQty { get; set; }
        [Display(Name = "Balance Qty")] public decimal BalanceQty { get; set; }

        [Display(Name = "Rate")] public decimal Rate { get; set; }
        [Display(Name = "Total")] public decimal Total_ { get; set; }
        [Display(Name = "FCValue")] public decimal FCValue { get; set; }
        [Display(Name = "Expense")] public decimal Expense { get; set; }
        [Display(Name = "PKR Value")] public decimal PKRValue { get; set; }
        [Display(Name = "PKR Rate(Per Item)")] public decimal PKRRate { get; set; }


        //Expense Popup
        [Display(Name = "GL Code")] public decimal GLCode { get; set; }
        [Display(Name = "Account Name")] public decimal AccountName { get; set; }
        [Display(Name = "Expense Amount")] public decimal ExpenseAmount { get; set; }
        [Display(Name = "Total Amount")] public decimal TotalAmount { get; set; }
        [Display(Name = "Total Value")] public decimal TotalValue { get; set; }
        [Display(Name = "Total PKR Value")] public decimal TotalPKRValue { get; set; }
        [Display(Name = "Total PKR Value")] public decimal TotalValueIncTax { get; set; }

        public int DetailCostCenter { get; set; }
        public int PRDetailId { get; set; }


        //[Display(Name = "Tax")] public int TaxId { get; set; }
        //[Display(Name = "Tax Amount")] public decimal TaxAmount { get; set; }
        //[Display(Name = "Line Total")] public decimal LineTotal { get; set; }


        //navigation property
        public List<APGRNItem> APGRNItems { get; set; }
        public APPurchaseOrder PO { get; set; }
        public InvItem Item { get; set; }
        public APSupplier Supplier { get; set; }
        public List<AppTax> TaxList { get; set; }
        public List<CostCenter> CostCenterList { get; set; }
        public List<AppCurrency> Currencies { get; set; }
        public APGRN APGRN { get; set; }
        public int SaleTaxId { get; set; }
        public int LocationId { get; set; }
        public string Date { get; set; }
        public string BrandName { get; set; }
        //public List<string> Brand { get; set; } = new List<string>();
    }
}
