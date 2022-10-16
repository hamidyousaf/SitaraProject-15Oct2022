using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APSupplier
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Supplier Name")]
        public string Name { get; set; }

        [Display(Name = "City")]
        public int City { get; set; }

        [Display(Name = "Country")]
        public int Country { get; set; }

        [MaxLength(500)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [MaxLength(15)]
        [Display(Name = "Phone 1")]
        public string Phone1 { get; set; }

        [MaxLength(15)]
        [Display(Name = "Phone 2")]
        public string Phone2 { get; set; }

        [MaxLength(20)]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }


        [MaxLength(15)]
        [Display(Name = "GST No")]
        public string GSTNo { get; set; }

        [MaxLength(15)]
        [Display(Name = "STRN #")]
        public string STRNNo { get; set; }

        [MaxLength(15)]
        [Display(Name = "NTN #")]
        public string NTNNo { get; set; }

        public bool IsActive { get; set; }

        public int CompanyId { get; set; }
        public int BussinessType { get; set; }

        [Required]
        [Display(Name = "GL Account")]
        public int AccountId { get; set; }

        public AppCompany Company { get; set; }
        public GLAccount Account { get; set; }

        [MaxLength(450)]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }



        //[MaxLength(20)]
        //[Display(Name = "Fax")]
        //public string Fax { get; set; }

        //[MaxLength(50)]
        //[Display(Name = "E-Mail")]
        //public string Email { get; set; }

        [MaxLength(50)]
        [Display(Name = "Web Address")]
        public string Web { get; set; }

        public List<ARContactPerson> ARContactPerson { get; set; }
        public List<ARShippingDetail> ARShippingDetail { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        //public string CNIC {get;set;}
        public string Registered {get;set;}
        public string ChamberMemebership { get; set; }
        public DateTime RegistrationDate { get; set; }
        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }
        public string UnApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? UnApprovedDate { get; set; }
        public bool? IsApproved { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
        //navigational property
        [ForeignKey("CreatedBy")]
        public ApplicationUser CreatedUser { get; set; }
        [ForeignKey("ApprovedBy")]
        public ApplicationUser ApprovalUser { get; set; }
        [ForeignKey("City")]
        public AppCitiy Cities { get; set; }

    }
}
