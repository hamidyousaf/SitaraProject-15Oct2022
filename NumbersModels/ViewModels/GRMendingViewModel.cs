using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class GRMendingViewModel
    {
        public GRMending GRMending { get; set; } = new GRMending();
        public SelectList InwardGatePassLOV { get; set; }
        public SelectList DamageTypeLOV { get; set; }
        public GRMendingDetail[] GRMendingDetail { get; set; }
        public string ContractNo { get; set; }
        public string Date { get; set; }
        public GRPurchaseContract PurchaseContract { get; set; }
        public GRWeavingContract WeavingContract { get; set; }
    }
}
