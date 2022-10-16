using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
  public  class CostCenterVM
    {
        public string Division { get; set; }
        public string SubDivision { get; set; }
        public CostCenter CostCenter { get; set; }


        public List<CostCenter> CostCenterList { get; set; }
        public string Date { get; set; }

    }
}
