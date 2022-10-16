using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class VwGRFolding
    {
        public GRFolding grFolding { get; set; } = new GRFolding();
        public GRFoldingItems grFoldingItems { get; set; } = new GRFoldingItems();
        public List<GRFoldingItems> GRFoldingDetails { get; set; }
        public string ContractQualityDesc { get; set; }
        public string LoomQualityDesc { get; set; }
        public string Date { get; set; }
   
        public SelectList WeavingContractLOV { get; set; }
        public SelectList PurchaseContractLOV { get; set; }
    }
}
