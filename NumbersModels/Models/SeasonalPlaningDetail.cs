using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class SeasonalPlaningDetail
    {
        public int Id { get; set; }
        public int SeasonalPlaningId { get; set; }
        public int SpecificationId { get; set; }

        [Display(Name = "Item Category 4 ")]
        public int FourthItemCategoryId { get; set; }

        [Display(Name = "Greige Quality ")]
        public int GreigeQualityId { get; set; }
        public int SeasonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Volume { get; set; }
        public int DesignPerVolume { get; set; }
        public int DesignCount { get; set; }
        public int DesignRun { get; set; }
        public int FabricConsumption { get; set; }
        public int BalanceDesignCount { get; set; }
        //public int BalanceDesignRun { get; set; }
        public int BalanceFabricConsumption { get; set; }
        //navigational property
        public InvItemCategories FourthItemCategory { get; set; }
        public GRQuality GreigeQuality { get; set; }
        public AppCompanyConfig Season { get; set; }
        public SeasonalPlaning SeasonalPlaning { get; set; }
    }
}
