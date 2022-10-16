using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARSalePersonItemCategory
    {
        public int ID { get; set; }
        public int SalePerson_ID { get; set; }
        [ForeignKey("InvItemCategories")]
        public int ItemCategory_ID { get; set; }
        public int? CityId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int Company_ID { get; set; }
        public int Resp_ID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        //navigation property
        public InvItemCategories InvItemCategories { get; set; }
        public AppCitiy City { get; set; }
    }
}
