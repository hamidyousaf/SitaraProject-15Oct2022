using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class VMMonthllyPlanning
    {
        public string ApprovedBy { get; set; }
        public int SeasonalId { get; set; }
        public PlanMonthlyPlanning PlanMonthly { get; set; } = new PlanMonthlyPlanning();
        public PlanMonthlyPlanningItems PlanMonthlyItems { get; set; } = new PlanMonthlyPlanningItems();
        public List<PlanMonthlyPlanningItems> PlanMonthlyPlanningDetail { get; set; }
    }
}
