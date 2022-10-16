using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class GRQualityViewModel
    {
        public GRQuality GRQuality { get; set; } = new GRQuality();
        public SelectList GRCategoryLOV { get; set; }
        public SelectList GRConstructionLOV { get; set; }
        public SelectList LoomTypeLOV { get; set; }
    }
}
