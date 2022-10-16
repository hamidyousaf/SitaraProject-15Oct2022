using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ProductionOrderViewModel
    {
        public ProductionOrder ProductionOrder { get; set; } = new ProductionOrder();
        public SelectList MonthlyPlanningLOV { get; set; }
        public SelectList VersionLOV { get; set; }
        public SelectList ProcessTypeLOV { get; set; }
        public SelectList VersionConversionLOV { get; set; }
        public SelectList SecondLevelCategoryLOV { get; set; }
        public SelectList ProcessLOV { get; set; }
        public SelectList TypeLOV { get; set; }
        public ProductionOrderItem[] ProductionOrderItems { get; set; }
        public string Date { get; set; }
        public int SuiteMeters { get; set; }
    }
}
