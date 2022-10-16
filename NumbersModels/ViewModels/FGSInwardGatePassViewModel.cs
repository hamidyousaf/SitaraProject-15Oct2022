using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class FGSInwardGatePassViewModel
    {
        public FGSInwardGatePass FGSInwardGatePass { get; set; } = new FGSInwardGatePass();
        public FGSInwardGatePassDetail[] FGSInwardGatePassDetails { get; set; }
        public SelectList WarehouseLOV { get; set; }
        public SelectList CustomerLOV { get; set; }
        public SelectList VehicleTypeLOV { get; set; }
        public SelectList FGSOutwardGatePassesLOV { get; set; }
        public SelectList VendorLOV { get; set; }
        public string Date { get; set; }
    }
}
