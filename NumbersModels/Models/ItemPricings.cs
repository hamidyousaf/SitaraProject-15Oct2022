using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ItemPricings
    {
        public int ID { get; set; }
        [Display(Name ="Trans.#")]
        public int TrancationNo { get; set; }
        [ForeignKey("ProductType")]
        [Display(Name = "Product Type")]
        public int ProductType_Id { get; set; }
        [ForeignKey("InvItemSecond")]
        [Display(Name = "Item Category 2")]
        public int ItemCategory_Id { get; set; }
        [Display(Name = "Item Category 3")]
        public int ItemCategory_Id3 { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int Resp_ID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public bool IsClosed { get; set; }
        public string Status { get; set; }
        //navigational property
        public AppCompanyConfig ProductType { get; set; }
        public InvItemCategories InvItemSecond { get; set; }
    }
}
