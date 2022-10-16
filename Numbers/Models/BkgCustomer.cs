using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class BkgCustomer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(15, ErrorMessage = "CNIC is required")]
        public string CNIC { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage = "Customer is required")]
        public string Name { get; set; }

        [MaxLength(150)]
        [Display(Name = "Father Name")]
        public string FatherName { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(500)]
        public string Remarks { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        [Required]
        [MaxLength(15, ErrorMessage = "Mobile is required")]
        public string Mobile { get; set; }

        [MaxLength(15)]
        [Display(Name = "GST No")]
        public string GSTNo { get; set; }
        [MaxLength(15)]
        [Display(Name = "NTN No")]
        public string NTNNo { get; set; }

        public bool IsActive { get; set; }


        public int CompanyId { get; set; }
        public int AccountId { get; set; }

        public IList<BkgVehicle> BkgVehicles { get; set; }

        public AppCompany Company { get; set; }
        public GLAccount Account { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
