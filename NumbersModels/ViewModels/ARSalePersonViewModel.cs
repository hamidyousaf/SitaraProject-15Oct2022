using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ARSalePersonViewModel
    {
        public ARSalePerson ARSalePerson { get; set; }
        public List<ARAnnualSaleTargets> ARAnnualSaleTargets { get; set; }
        public List<ARMonthlySaleTargets> ARMonthlySaleTargets { get; set; }
        public List<ARSalePersonCity> ARSalePersonCities { get; set; }
        public List<ARSalePersonItemCategory> ARSalePersonItemCategories { get; set; }
        public SelectList ListOfItemCategories { get; set; }
        public SelectList Cities { get; set; }
        public SelectList Customer { get; set; }
        public SelectList AnualSaleTargetCaogoriesOnEdit { get; set; }
        public SelectList MonthlySaleTargetCaogoriesOnEdit { get; set; }
        public List<InvItemCategories> ItemCategories { get; set; }
        public List<AppCitiy> City { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
