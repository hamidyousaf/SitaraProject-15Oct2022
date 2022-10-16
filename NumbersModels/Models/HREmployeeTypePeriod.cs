using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREmployeeTypePeriod
    {
        public int Id { get; set; }
        public int PeriodId { get; set; }
        //public int EmployeeTypeId { get; set; }

        public string Closed { get; set; }

        public DateTime PayrollStart { get; set; }
        public DateTime PayrollEnd { get; set; }

        //navigation property
        public HREmployeeType EmployeeType { get; set; }
    }
}
