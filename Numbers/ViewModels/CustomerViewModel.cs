using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.ViewModels
{
    public class CustomerViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(15, ErrorMessage = "CNIC is required")]
        public string CNIC { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage = "Customer is required")]
        public string Name { get; set; }

        [MaxLength(150)]
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

        public string GSTNo { get; set; }
        public string NTNNo { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int AccountId { get; set; }

        public List<SelectListItem> CompanyList { get; set; }
        public List<SelectListItem> AccountList { get; set; }

        public CustomerViewModel()
        {

        }
        //public BkgCustomerViewModel(IEnumerable<GLAccount> gLAccounts, IEnumerable<AppCompany> appCompanies)
        //{
        //    AccountList = gLAccounts.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
        //}
    }
}
