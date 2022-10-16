using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class PlanInvoiceViewModel
    {
        public PlanInvoice PlanInvoice { get; set; } = new PlanInvoice();
        public PlanInvoiceDetail PlanInvoiceDetails { get; set; }
        public string Vendor { get; set; }
        public string Address { get; set; }
        public SelectList ProductionOrderLOV { get; set; }
        public List<GreigeIssuance> GreigeIssuance { get; set; }
    }
}
