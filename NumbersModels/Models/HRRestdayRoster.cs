using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRRestdayRoster
    {
        [Key]
        public int Rouster_id { get; set; }

        public string Period_id { get; set; }

        public int Group_ID { get; set; }



        public bool Closed { get; set; }

    }
}
