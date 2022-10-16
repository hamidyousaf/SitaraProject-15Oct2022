using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
  public  class OnScreenTrialBalance
    {
        public Int64 Id { get; set; }
        public string Code1 { get; set; }
        public string Name1 { get; set; }
        public string Code2 { get; set; }
        public string Name2 { get; set; }
        public string Code3 { get; set; }
        public string Name3 { get; set; }
        public string Code4{ get; set; }
        public string Name4 { get; set; }
        public decimal Opening { get; set; }
        public decimal OpeningCr { get; set; }

        public decimal OpeningDr { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
