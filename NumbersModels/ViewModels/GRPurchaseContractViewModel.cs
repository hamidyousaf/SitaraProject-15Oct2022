using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class GRPurchaseContractViewModel
    {
        public SelectList VendorLOV { get; set; }
        public SelectList GRQualityLOV { get; set; }
        public SelectList ContractQualityLOV { get; set; }
        public SelectList GRWeavingContractsLOV { get; set; }
        public SelectList SalesTaxLOV { get; set; }
        public GRPurchaseContract GRPurchaseContract { get; set; } = new GRPurchaseContract();
    }
}
