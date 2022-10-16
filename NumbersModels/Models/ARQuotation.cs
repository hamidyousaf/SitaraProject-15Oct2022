using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARQuotation
    {
        [Key]
        public int Id { get; set; }

      
        [Display(Name = "Quotation Id")]
        public string QuotationNo { get; set; }

       
        [Display(Name = "Quotation Date")]
        public DateTime QuotationDate{ get; set; }

     
        [Display(Name = "Delivery Mode")]
        public int DeliveryMode { get; set; }

        [Display(Name = "Phote")]
        public string Photo { get; set; }


        [Display(Name = "Currency")] 
        public string Currency { get; set; } 

        [Display(Name = "Customer Qut Id")]
        public string CustomerQuotationNo { get; set; }
        
        
        [Display(Name = "Customer Qut Date")]
        public DateTime CustomerQuotationDate { get; set; }

        [Display(Name = "Payment Mode")]
        public int PaymentMode { get; set; }

        [Display(Name = "Exch. Rate")]
        public decimal ExchRate { get; set; }

        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Display(Name = "Exch. Date")]
        public DateTime ExchDate { get; set; }

        [Display(Name = "Ship To")]
        public int ShipTo { get; set; }

        [Display(Name = "Sale Person")]
        public string SalePerson { get; set; }

        [Display(Name = "Rate Days")]
        public int RateDays { get; set; }

        [Display(Name = "Delivery Days")]
        public int DeliveryDays { get; set; }

        [Display(Name = "Attachments")]
        public string Attachments { get; set; }

        [Display(Name = "Warranty")]
        public string Warranty { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Total Quantity")]
        public int TotalQuantity { get; set; }

        [Display(Name = "Total Discount")]
        public decimal TotalDiscount { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }




        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime ApprovedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [Required] public int CompanyId { get; set; }
        [Display(Name = "Status")] [MaxLength(10)] public string Status { get; set; }


        public List<ARQuotationDetail> ARQuotationsDetailList { get; set; }
        //public List<AppCurrency> Currencies { get; set; }
    }
}
