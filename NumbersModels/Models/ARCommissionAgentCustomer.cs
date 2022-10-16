using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class ARCommissionAgentCustomer
    {
        public int Id { get; set; }
        [ForeignKey("CommissionAgent")]
        public int CommissionAgent_Id { get; set; }
        [ForeignKey("Customer")]
        public int Customer_Id { get; set; }
        //CategoryId
        public int CategoryId { get; set; }
        public virtual ARCustomer Customer { get; set; }
        public virtual ARCommissionAgent CommissionAgent{ get; set; }
        //public virtual InvItemCategories InvItemCategories { get; set; }
    }
}
