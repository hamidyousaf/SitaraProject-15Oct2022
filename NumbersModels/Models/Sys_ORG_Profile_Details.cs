using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class Sys_ORG_Profile_Details
    {
        public int ID { get; set; }
        public int SysORGProfile_ID { get; set; }
        public int Organization_ID { get; set; }
        public bool IsEnable { get; set; }
    }
}
