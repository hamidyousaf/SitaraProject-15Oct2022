using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
   public class ARCustomer
    {
        public int Id { get; set; } 

        [MaxLength(15)]
        [Display(Name = "CNIC")]
        public string CNIC { get; set; }

     
        [MaxLength(150)]
        [Display(Name = "Customer Name")]
        public string Name { get; set; }

       
        [Display(Name = "Customer Category")]
        public int   CustomerCategoryID{ get; set; }


        [Display(Name = "City")]
        public int CityId { get; set; }

        [Display(Name = "Country")]
        public int CountryId { get; set; }

        [Display(Name = "Shipping Country")]
        public int ShippingCountry { get; set; }

        [Display(Name = "Shipping City")]
        public int ShippingCity { get; set; }
        [Display(Name = "Sales Person")]
        public int SalesPersonId { get; set; }

        [MaxLength(500)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [MaxLength(500)]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }

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
        [Display(Name = "NTN No")]
        public string NTNo { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int CompanyId { get; set; }
        public int ResponsibilityId { get; set; }

        [Display(Name="GL Account")]
        public int AccountId { get; set; }

        public AppCompany Company { get; set; }
        public GLAccount Account { get; set; }
        public AppCitiy City { get; set; }
        public AppCountry Country { get; set; }

        [MaxLength(450)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }



        [MaxLength(20)]
        [Display(Name = "Fax")]
        public string Fax { get; set; }

        [MaxLength(50)]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [MaxLength(50)]
        [Display(Name = "Web Address")]
        public string Web { get; set; }


        public List<ARContactPerson> ARContactPerson { get; set; }

        public  List<ARShippingDetail> ARShippingDetail { get; set; }
        public List<ARSuplierItemsGroup> ARSuplierItemsGroup { get; set; }
        public List<ARCreditLimit> CreditLimitList { get; set; }
        public virtual ARSalePerson SalesPerson { get; set; }
        public virtual ICollection<ARCommissionAgentCustomer> CommissionAgentCustomers { get; set; }
        public string CreditLimit { get; set; }
        public int ProductTypeId { get; set; }
    }
}
