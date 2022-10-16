using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class BkgCustomer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(15)]
        public string CNIC { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Father Name")]
        public string FatherName { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(500)]
        public string Remarks { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        [MaxLength(15)]
        public string Mobile { get; set; }

        [MaxLength(15)]
        [Display(Name = "GST No")]
        public string GSTNo { get; set; }

        [MaxLength(15)]
        [Display(Name = "NTN No")]
        public string NTNNo { get; set; }

        [MaxLength(200)]
        [Display(Name="Image")]
        public string Photo { get; set; }

        public bool IsActive { get; set; }
        public bool IsBooking { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }


        public int CompanyId { get; set; }
        public int AccountId { get; set; }

        public IList<BkgVehicle> BkgVehicles { get; set; }

        public AppCompany Company { get; set; }
        public GLAccount Account { get; set; }
        
        [MaxLength(450)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
