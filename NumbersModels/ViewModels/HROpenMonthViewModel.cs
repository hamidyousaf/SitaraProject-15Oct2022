using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class HROpenMonthViewModel
    {
        [Display(Name = "Month")] public string MonthDescrption { get; set; }
        public string ShortDescription { get; set; }
        public int RousterId { get; set; }
        public string PeriodId { get; set; }
        public int GroupId { get; set; }
        public bool Closed { get; set; }

        public DateTime EndDate { get; set; }
        //RosterItems
        public DateTime Rouster_Date { get; set; }
        public int ShiftId { get; set; }


        //navigation property
        public AppPeriod Period { get; set; }

        public int ModuleId { get; set; }


    }
}
