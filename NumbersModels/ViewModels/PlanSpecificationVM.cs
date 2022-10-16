using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
  public  class PlanSpecificationVM
    {
        public string ItemCategoryLevel2 { get; set; }
        public string ItemCategoryLevel4 { get; set; }
        public string GreigeQuality { get; set; }
        public PlanSpecification PlanSpecifications { get; set; }


        public List<PlanSpecification> PlanSpecificationList { get; set; }
        public string Date { get; set; }

    }
}
