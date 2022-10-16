using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class AppCompany
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50,ErrorMessage ="Company name is required")]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Logo { get; set; }
        [MaxLength(100)]
        public string AddressLine1 { get; set; }
        [MaxLength(100)]
        public string AddressLine2 { get; set; }
        [MaxLength(30)]
        public string City { get; set; }
        [MaxLength(30)]
        public string Country { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(20)]
        public string Phone1 { get; set; }
        [MaxLength(20)]
        public string Phone2 { get; set; }
        [MaxLength(20)]
        public string Fax { get; set; }
        [MaxLength(20)]
        public string SalesTaxNumber { get; set; }
        [MaxLength(20)]
        public string NationalTaxNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlock { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(100)]
        public string BlockReason { get; set; }
        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public IList<GLAccount> glAccounts { get; set; }
        public IList<BkgCustomer> bkgCustomers { get; set; }
        public int Code { get; set; }
    }
}
