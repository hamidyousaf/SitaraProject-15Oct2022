using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARSuplierItemsGroup
    {
        public int ID { get; set; }
        public int ApSuplierId { get; set; }
        public int CategoryId { get; set; }
        public int CategoryLevId { get; set; }
        public int ARCustomerId { get; set; }
        public string Type { get; set; }
        public ARCustomer ARCustomer { get; set; }
    }
}
