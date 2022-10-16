using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARCommissionAgent
    {
        public int Id { get; set; }

        [MaxLength(15)]
        [Display(Name = "CNIC")]
        public string CNIC { get; set; }

        [MaxLength(150)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "City")]
        public int CityId { get; set; }
        [Display(Name = "Country")]
        public int CountryId { get; set; }
        public int ItemCategoryId { get; set; }
        public int CustomerId { get; set; }

        [Display(Name = "Sales Person")]
        public int SalesPersonId { get; set; }

        [MaxLength(500)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [MaxLength(15)]
        [Display(Name = "Phone 1")]
        public string Phone1 { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Commission %")]
        public decimal CommissionPer { get; set; }
        public int CompanyId { get; set; }
        public int ResponsibilityId { get; set; }
        public AppCompany Company { get; set; }
        public AppCitiy City { get; set; }
        public AppCountry Country { get; set; }

        [MaxLength(450)]
        public string CreatedBy { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<ARCommissionAgentCustomer> CommissionAgentCustomers { get; set; }
        [NotMapped]
        public AgentCustomerDTO AgentCustomerDTO { get; set; }

    }
}
