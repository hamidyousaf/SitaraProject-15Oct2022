using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
  public class HRRestdayRosterItems
    {
        [Key]
        public int Rouster_Line_id { get; set; }


        public int Rouster_id { get; set; }

        public int Group_ID { get; set; }

        public string Period_ID { get; set; }

        public int ShiftId { get; set; }

        public DateTime Rouster_Date { get; set; }

        public bool Rest_Day { get; set; }
    }
}
