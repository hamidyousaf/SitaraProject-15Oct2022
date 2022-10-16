using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARPerformaInvoice
    {
        public int Id { get; set; }
        [Display(Name ="Performa Inv #.")] public int PerformaInvNo { get; set; }
        [Display(Name = "LC No.")] public int LCNo { get; set; }
        [Display(Name = "LC Opening Date")] public DateTime LCOpeningDate { get; set; }
        [Display(Name = "LC Last Date")] public int LCLastDate { get; set; }
        [Display(Name = "LC Opening Bank")] public int LCOpeningBank { get; set; }
        [Display(Name = "Vendor")] public string Vendor { get; set; }
        public int ShippingPort { get; set; }
        public int DischargePort { get; set; }
        [Display(Name = "Origin")] public string Origin { get; set; }
        [Display(Name = "PaymentMode")] public string PaymentMode { get; set; }
        [Display(Name = "Cost Terms")] public string CostTerms { get; set; }
        [Display(Name = "ShippingModes")] public string ShippingModes { get; set; }
        [Display(Name = "Currency")] public int Currency { get; set; }
        [Display(Name = "Exchange Rate Date")]  public string ExchangeRateDate { get; set; }
        [Display(Name = "Exchange Rate")] [MaxLength(50)] public string ExchangeRate { get; set; }
        [Display(Name = "Vendor Account")] public int VendorAccount { get; set; }
        [Display(Name = "Remarks")][MaxLength(200)] public string Remarks { get; set; }


        public int CompanyId { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsDeleted { get; set; }

        //navigation porperty
        //public ARCustomer Customer { get; set; }

        //public List<ARDeliveryChallan> ARDeliveryChallanList { get; set; }

    }
}