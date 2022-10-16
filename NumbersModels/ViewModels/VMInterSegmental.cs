using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class VMInterSegmental
    {
        public GRInterSegmental GRInterSegmental { get; set; } = new GRInterSegmental();
        public GRInterSegmentalDetail[] GRInterSegmentalDetailList { get; set; }
        public GRInterSegmentalDetail GRInterSegmentalDetail { get; set; }
    }
}
