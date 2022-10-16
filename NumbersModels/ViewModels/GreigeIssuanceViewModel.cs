using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
   public class GreigeIssuanceViewModel
    {

        public bool IsApproved { get; set; }
        public bool IsDelete { get; set; }
        public GreigeIssuance GreigeIssuance { get; set; } = new GreigeIssuance();
        public GreigeIssuanceDetail[] GreigeIssuanceDetails { get; set; } 
        public GreigeIssuanceDetail GreigeIssuanceDetail { get; set; } 
        public GreigeIssuanceBal GreigeIssuanceBal { get; set; } 
        public GreigeIssuanceBalance GreigeIssuanceBalance { get; set; } 
        public SelectList SpecificationLOV { get; set; }
        public SelectList VendorLOV { get; set; }
        public SelectList IssueTypeLOV { get; set; }
        public SelectList WareHouseLOV { get; set; }

        public string Period { get; set; }
        public string Productionorder { get; set; }
        public decimal IssuanceQty { get; set; }
        public string Date { get; set; }
        
    }
}
