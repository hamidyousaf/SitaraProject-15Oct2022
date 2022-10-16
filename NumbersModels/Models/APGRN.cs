using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APGRN
    {
        //AP goods receive notes
        [Key]
        public int Id { get; set; }

        [Display(Name = "GRN No #")] public int GRNNO { get; set; }
        [Display(Name = "GRN Date")] public DateTime GRNDate { get; set; }
        [Display(Name = "PONo #")] public int IRNNo { get; set; }
        [Display(Name = "Vendor")] public string Vendor { get; set; }
        [Display(Name = "Vendor Name")] public string VendorName { get; set; }
        [Display(Name = "Warehouse")] public string Warehouse { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }


        public int PeriodId { get; set; }
        public int VoucherId { get; set; }
        [Display(Name = "Currency")] [MaxLength(3)] public string Currency { get; set; }
        [Display(Name = "Currency Exchange Rate")] [Column(TypeName = "numeric(18,2)")] public decimal CurrencyExchangeRate { get; set; }
        [Display(Name = "P.O. Type")] public int POTypeId { get; set; }
        [Display(Name = "Supplier")] public int SupplierId { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        [Display(Name = "Attachments")] [MaxLength(450)] public string Attachment { get; set; }
        [Display(Name = "Total")] [Column(TypeName = "numeric(18,2)")] public decimal Total { get; set; }
        [Display(Name = "Tax Amount")] [Column(TypeName = "numeric(18,2)")] public decimal TotalTaxAmount { get; set; }
        [Display(Name = "Freight")] [Column(TypeName = "numeric(18,2)")] public decimal Freight { get; set; }
        [Display(Name = "Grand Total")] [Column(TypeName = "numeric(18,2)")] public decimal GrandTotal { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public int OperationId { get; set; }
        public int Resp_ID { get; set; }


        //more fields expense
        [Display(Name = "GL Code")] [MaxLength(450)] public string GLCode { get; set; }
        [Display(Name = "Account Name")] [MaxLength(450)] public string AccountName { get; set; }
        [Display(Name = "Expense Amount")]  public decimal ExpenseAmount { get; set; }
        //New Entries
        [Display(Name = "Total Amount")] [Column(TypeName = "numeric(18,2)")] public decimal TotalAmount { get; set; }
        [Display(Name = "Total PKR Value")] public decimal TotalPKRValue { get; set; }

        public int CostCenter { get; set; }
        [Display(Name = "Total Value")] public decimal TotalValue { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime ApprovedDate { get; set; }

        public bool IsDeleted { get; set; }

        //navigation property
        public APSupplier Supplier { get; set; }
        public AppCompanyConfig DeliveryTerm { get; set; }
        public AppCompanyConfig PaymentTerm { get; set; }
        public AppCompanyConfig POType { get; set; }
        public SysOrganization Operation { get; set; }
        public AppPeriod Period { get; set; }
        [NotMapped]
        public string VendorNme { get; set; }
        [NotMapped]
        public string WarehouseName { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
    }
}
