using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRGRNInvoice
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "Invoice #")] public int PurchaseNo { get; set; }
        [Display(Name = "Invoice Date")] public DateTime PurchaseDate { get; set; }
        [Display(Name = "Supplier Inv No")] [MaxLength(50)] public string SupplierInvoiceNo { get; set; }
        [Display(Name = "Supplier Inv Date")] [DataType(DataType.Date)] public DateTime SupplierInvoiceDate { get; set; }
        [Display(Name = "GRN #")] public int GRNId { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }

        [Display(Name = "Status")] [MaxLength(10)] public string Status { get; set; }

        [MaxLength(450)] public string CreatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [MaxLength(450)] public string UpdatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime? ApprovedDate { get; set; }

        public bool IsDeleted { get; set; }
        public int DepartmentId { get; set; }
        public int OperationId { get; set; }
        public int Resp_ID { get; set; }
        public int VoucherId { get; set; }
        public GRGRN GRN { get; set; }

    }
}
