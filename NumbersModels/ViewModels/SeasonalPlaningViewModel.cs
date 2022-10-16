using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
   public class SeasonalPlaningViewModel
    {

        public bool IsApproved { get; set; }
        public bool IsDelete { get; set; }
        public SeasonalPlaning SeasonalPlaning { get; set; } = new SeasonalPlaning();
        public SeasonalPlaningDetail[] SeasonalPlaningDetail { get; set; } 
        public SeasonalPlaningDetail SeasonalPlaningDetails { get; set; } 
        public SelectList SeasonLOV { get; set; }

        public string Period { get; set; }
        public string Date { get; set; }
        
    }
}
