using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARAnnualSaleTargets
    {
        public int ID { get; set; }
        public int No { get; set; }
        public int SalePerson { get; set; }
        [ForeignKey("InvItemCategories")]
        public int ItemCategory { get; set; }
        public int CityId { get; set; }
        public decimal AnnualTarget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int Resp_ID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        //navigation property
        public InvItemCategories InvItemCategories { get; set; }
        public AppCitiy City { get; set; }
    }
}
