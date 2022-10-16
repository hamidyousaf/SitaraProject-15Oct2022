using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class AgentCustomerDTO
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string SalesPerson { get; set; }
        public string City { get; set; }
        public string ItemCategory { get; set; }
        public int CityId { get; set; }
        public int CommissionAgent_Id { get; set; }
        public int ItemCategoryId { get; set; }
        public List<string> ItemCategoryNames { get; set; }
    }
}
