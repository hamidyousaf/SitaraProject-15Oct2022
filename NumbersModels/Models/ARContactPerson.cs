using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARContactPerson
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int SupplierId { get; set; }

        [Display(Name = "Person Name")]
        public string PersonName { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Phoneno")]
        public string PhoneNo { get; set; }

        [Display(Name = "Ext")]
        public string Ext { get; set; }

        [Display(Name = "Cellno")]
        public string CellNo { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public int ARCustomerID { get; set; }

  //      public int APSupplierId { get; set; }

    }
}
