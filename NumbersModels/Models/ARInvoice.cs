using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARInvoice
    {
        public int Id { get; set; }
        [Display(Name = "Invoice No")] public int InvoiceNo { get; set; }
        [Display(Name = "Ware House")] public int WareHouseId { get; set; }
        [Display(Name = "Voucher Id")] public int VoucherId { get; set; }
        [Display(Name = "Period Id")] public int PeriodId { get; set; }
        [Display(Name = "Item Rate Id")] public int ItemRateId { get; set; }
        public int CompanyId { get; set; }
        public int ResponsibilityId { get; set; }
        [Display(Name = "Customer")] public int CustomerId { get; set; }

        [Display(Name = "SalesPersonId")] public int SalesPersonId { get; set; }
        
        [Display(Name = "Total")] [Column (TypeName="numeric(18,2)")] public decimal Total { get; set; }
        [Display(Name = "Total Discount Amount")] [Column(TypeName ="numeric(18,2)")] public decimal DiscountAmount { get; set; }
        [Display(Name = "Sale Tax Percentage")] [Column(TypeName = "numeric(18,2)")] public decimal SalesTaxPercentage { get; set; }
        [Display(Name = "Sale Tax")] [Column(TypeName = "numeric(18,2)")] public decimal SalesTaxAmount { get; set; }
        [Display(Name = "Excise Tax Percentage")] [Column(TypeName = "numeric(18,2)")] public decimal ExciseTaxPercentage { get; set; }
        [Display(Name = "Excise Tax")] [Column(TypeName = "numeric(18,2)")] public decimal ExciseTaxAmount { get; set; }
        [Display(Name = "Cash")] [Column(TypeName = "numeric(18,2)")] public decimal Cash { get; set; }
        [Display(Name = "Change")] [Column(TypeName = "numeric(18,2)")] public decimal Change { get; set; }
        [Display(Name ="Freight Amount")] [Column(TypeName = "numeric(18,2)")] public decimal FreightAmount { get; set; }
        [Display(Name ="Invoice Amount")] [Column(TypeName = "numeric(18,2)")] public decimal InvoiceAmount { get; set; }
        [Display(Name = "Commission Percentage")] [Column(TypeName = "numeric(18,2)")] public decimal CommissionPercentage { get; set; }
        [Display(Name = "Commission Amount")] [Column(TypeName = "numeric(18,2)")] public decimal CommissionAmount { get; set; }
        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }
        [Display(Name = "Receipt Total")] [Column(TypeName = "numeric(18,2)")] public decimal ReceiptTotal { get; set; }
        [Display(Name = "Discount Percentage")] [Column(TypeName = "numeric(18,2)")] public decimal DiscountPercentage { get; set; }
        [Display(Name = "Currency Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "Currency Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal AdjustmentTotal { get; set; }
        
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Transaction Type")] [MaxLength(15)] public string TransactionType { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "OGP No")] [MaxLength(50)] public string OGPNo { get; set; }
        [Display(Name = "Reference No")] [MaxLength(30)] public string ReferenceNo { get; set; }
        [Display(Name = "Customer P.O No")] [MaxLength(50)] public string CustomerPONo { get; set; }
        [Display(Name = "Vehicle")] [MaxLength(50)] public string Vehicle { get; set; }
        [Display(Name = "Status")] [MaxLength(10)] public string Status { get; set; }
        [Display(Name = "Location")] [MaxLength(50)] public string Location { get; set; }
        [Display(Name = "Old No")] [MaxLength(50)] public string OldNo { get; set; }
        [Display(Name = "Booking No")] [MaxLength(50)] public string BookingNo { get; set; }
        [Display(Name = "Name")] [MaxLength(2000)] public string Name { get; set; }
        [Display(Name = "Customer Invoice Address")] [MaxLength(2000)] public string CustomerInvAddress { get; set; }
        [Display(Name = "Sales Man")] [MaxLength(450)] public string SalesMan { get; set; }
        [Display(Name = "Sale Man")] [MaxLength(2)] public string SaleMan { get; set; }
        [Display(Name = "Invoice Type")] [MaxLength(10)] public string InvoiceType { get; set; }

        [Display(Name = "Invoice Date")] [DataType(DataType.Date)] public DateTime InvoiceDate { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [DataType(DataType.Date)] public DateTime? ApprovedDate { get; set; }
        [Display(Name = "Invoice Due Date")] [DataType(DataType.Date)] public DateTime InvoiceDueDate { get; set; }

        public int CostCenter { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsClosed { get; set; }
        public ARInvoice()
        {
            Currency = "PKR";
            CurrencyExchangeRate = 1;
            IsDeleted = false;
            IsClosed = false;
            ReceiptTotal = 0;
            AdjustmentTotal = 0;
        }

        //navigation property
        public AppCompanyConfig WareHouse { get; set; }
        public ARCustomer Customer { get; set; }
        public List<AppCurrency> Currencies { get; set; }
        virtual
        //public List<ARInvoiceItem> InvoiceItems { get; set; }
        public bool IsDiscount { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
    }
}

