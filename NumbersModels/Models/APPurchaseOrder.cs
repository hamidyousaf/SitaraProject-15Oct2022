using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APPurchaseOrder
    {
        public int Id { get; set; }
        [Display(Name = "P.O. No.")] public int PONo { get; set; }
        [Display(Name = "P.O. Date")] [DataType(DataType.Date)] public DateTime PODate { get; set; }
        public int PeriodId { get; set; }
        public int VoucherId { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "Currency Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "P.O. Type")] public int POTypeId { get; set; }
        [Display(Name = "Supplier")] public int SupplierId { get; set; }
        [Display(Name = "Delivery Terms")] public int DeliveryTermId { get; set; }
        [Display(Name = "Payment Terms")] public int PaymentTermId { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        [Display(Name = "Reference No")] [MaxLength(25)] public string ReferenceNo { get; set; }
        [Display(Name = "Attachments")] [MaxLength(450)] public string Attachment { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
        [Display(Name = "Tax Amount")] [Column(TypeName = "numeric(18,2)")] public decimal TotalTaxAmount { get; set; }
        [Display(Name = "Freight")] [Column(TypeName = "numeric(18,2)")] public decimal Freight { get; set; }
        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }
        public int CompanyId { get; set; }

        //more fields
        [Display(Name = "Payment Mode")] public string PaymentMode { get; set; }
        [Display(Name = "Cost Terms")] public string CostTerms { get; set; }
        [Display(Name = "Shipping Mode")] public string ShippingMode { get; set; }
        [Display(Name = "Shipping Port")] public string ShippingPort { get; set; }
        [Display(Name = "Discharge Port")] public string DischargePort { get; set; }
        [Display(Name = "Origin")] public string Origin { get; set; }
        [Display(Name = "Import Type")] public string ImportType { get; set; }



        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime ApprovedDate { get; set; }

        public int OrganizationId { get; set; }
        public int DepartmentId { get; set; }
        public int OperationId { get; set; }
        public int Resp_ID { get; set; }
        public int CSNo { get; set; }

        public bool IsDeleted { get; set; }    

        //navigation property
        public APSupplier Supplier { get; set; }
        public AppCompanyConfig DeliveryTerm { get; set; }
        public AppCompanyConfig PaymentTerm { get; set; }
        public AppCompanyConfig POType { get; set; }
        public SysOrganization Operation { get; set; }
        public AppPeriod Period { get; set; }



        [Display(Name = "Performa No #")] public int PerformaNo { get; set; }
        [Display(Name = "Performa Date")] public DateTime PerformaDate { get; set; }
        [Display(Name = "LC No #")] public int LCNo { get; set; }
        [Display(Name = "LCOpening Date")] public DateTime LCOpeningDate { get; set; }
        [Display(Name = "LCExpiry Date")] public DateTime LCExpiryDate { get; set; }
        [Display(Name = "LC Bank")] public string LCBank { get; set; }
        [Display(Name = "Customer Bank")] public string CustomerBank { get; set; }
        [Display(Name = "Swift Code")] public string SwiftCode { get; set; }
        public int CostCenter { get; set; }
        public int APComparativeStatementId { get; set; }
        [NotMapped]
        public string VendorName { get; set; }
        [NotMapped]
        public string CurrencyName { get; set; }
        public string LCNumber { get; set; }
      
        [NotMapped]
        public string Date { get; set; }
        [NotMapped]
        public decimal? Qty { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
        public List<APPurchaseOrderItem> APPurchaseOrderItems { get; set; }
    }
}
