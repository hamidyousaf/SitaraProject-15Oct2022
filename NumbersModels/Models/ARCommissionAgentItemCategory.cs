using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class ARCommissionAgentItemCategory
    {
        public int Id { get; set; }
        public int CommissionAgent_Id { get; set; }
        public int Category_Id { get; set; }
    }
}
