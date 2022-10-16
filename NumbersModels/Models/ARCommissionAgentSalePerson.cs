using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class ARCommissionAgentSalePerson
    {
        public int Id { get; set; }
        public int CommissionAgent_Id { get; set; }
        public int SalePerson_Id { get; set; }
    }
}
