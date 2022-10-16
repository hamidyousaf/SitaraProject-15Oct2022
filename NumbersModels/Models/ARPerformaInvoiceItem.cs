using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARPerformaInvoiceItem
    {
        public int Id { get; set; }
        [Display(Name = "Performa Invoice Id")] public int PerInvId { get; set; }

        [Display(Name = "Code")] public int Code { get; set; }
       
        [Display(Name = "Item")] public int ItemId { get; set; }

        [Display(Name = "Item Description")] public string ItemDescription { get; set; }
        public string UOM { get; set; }

        [Display(Name = "HS Code")] public int HSCode { get; set; }
        [Display(Name = "Quantity")]  public decimal Qty { get; set; }
        [Display(Name = "Packing")]  public decimal Packing { get; set; }
        [Display(Name = "Rate")]  public decimal Rate { get; set; }
        [Display(Name = "FC Value")]  public decimal FCValue { get; set; }
        [Display(Name = "Pkr")] public int Pkr { get; set; }

        [Display(Name = "Company")] public int CompanyId { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }



        //navigation property
        public ARSaleOrder SaleOrder { get; set; }
        public InvItem Item { get; set; }
      
        public string UnitName { get; set; }

        public int PerformaInvoiceId { get; set; }



        public List<ARPerformaInvoiceItem> ARPerformaInvoiceItemList { get; set; }
    }
}
