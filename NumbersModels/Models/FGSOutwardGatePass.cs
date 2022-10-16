using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class FGSOutwardGatePass
    {
        public int Id { get; set; }
        public int OGPNo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime OGPDate { get; set; }
        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        [Display(Name = "Item Category 2")]
        public int SecondItemCategoryId { get; set; }
        [Display(Name = "Item Category 4")]
        public int FourthItemCategoryId { get; set; }
        public string Remarks { get; set; }
        public string FileAttachment { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int Resp_ID { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public string UnApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime UnApprovedDate { get; set; }
        // navigational property
        public AppCompanyConfig Warehouse { get; set; }
        public ARCustomer Customer { get; set; }
        public InvItemCategories SecondItemCategory { get; set; }
        public InvItemCategories FourthItemCategory { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
    }
}
