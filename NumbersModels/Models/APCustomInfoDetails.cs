using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
   public class APCustomInfoDetails
    {
        public int Id { get; set; }
        public int CustomInfo_Id { get; set; }
        public int PDCNo { get; set; }
        public DateTime PDCDate { get; set; }
        public decimal PDCAmount { get; set; }
        public int PDCBank_Id { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }

    }
}
