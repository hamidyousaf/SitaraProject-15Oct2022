using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class GRInwardGatePassViewModel
    {
        public GRInwardGatePass GRInwardGatePass { get; set; } = new GRInwardGatePass();
        public SelectList WeavingContractLOV { get; set; }
        public SelectList PurchaseContractLOV { get; set; }
        public GRInwardGatePassDetail[] GRInwardGatePassDetail { get; set; }
        public string Weaver { get; set; }
        public string Date { get; set; }
        public int ContractNo { get; set; }
        public string ContractQuality { get; set; }
        public string LoomQuality { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public string LotNo { get; set; }
        public string Address { get; set; }
        public GRPurchaseContract PurchaseContract { get; set; }
        public GRWeavingContract WeavingContract { get; set; }
        public SelectList DamageTypeLOV { get; set; }

    }
}
