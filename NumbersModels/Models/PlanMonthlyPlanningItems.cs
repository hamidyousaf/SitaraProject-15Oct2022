using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class PlanMonthlyPlanningItems
    {
        public int Id { get; set; }
        public int PlannigId { get; set; }
        public int ForthLevelCategoryId { get; set; }
        public int SeasonalDetailId { get; set; }
        public string CategoryName { get; set; }
        public string PlanSpecificationId { get; set; }
        public string MonthId { get; set; }
        public int SeasonalDesignCount { get; set; }
        public int SeasonalRunQty { get; set; }
        public int SeasonalFabricCons { get; set; }
        public int MonthlyDesignCount { get; set; }
        public int MonthlyFabricConsBalance { get; set; }
        public int MonthlyRunQty { get; set; }
        public int MonthlyFabricCons { get; set; }
        public int spec_Id { get; set; }

    }
}
