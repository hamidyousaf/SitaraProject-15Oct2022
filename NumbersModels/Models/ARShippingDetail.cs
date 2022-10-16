using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARShippingDetail
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int SupplierId { get; set; }

        [Display(Name = "Location ")]
        public string Location { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [MaxLength(20)]
        [Display(Name = "Phone No")]
        public string PhoneNo { get; set; }

        public string Type { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [DataType(DataType.Date)] public DateTime? ApprovedDate { get; set; }
        public int ARCustomerID { get; set; }
        //public int APSupplierId { get; set; }
        
    }
}
