using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class GLAccount
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(15)]
        public string Code { get; set; }

        [Required(ErrorMessage = "Account name is required")]
        [MaxLength(150)]
        public string Name { get; set; }

        [Display(Name = "Level")]
        public Int16 AccountLevel { get; set; }

        
        [Required]
        public int ParentId { get; set; }
        public int CompanyId { get; set; }
        public AppCompany Company { get; set; }
        
        public bool RequireSubAccount { get; set; }
        [MaxLength(50)]
        public string SubAccountId { get; set; }
        public bool RequireCostCenter { get; set; }
        public bool IsCash { get; set; }
        public bool IsBank { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }


        public IList<BkgCustomer> bkgCustomers { get; set; }
        public IList<BkgPayment> bkgPayments { get; set; }
        //public IList<BkgReceipt> bkgReceipts { get; set; }
        //public IList<BkgComissionReceived> bkgComissionReceiveds { get; set; }
        //public IList<BkgComissionPayment> bkgComissionPayments { get; set; }
        //public IList<BkgVehicle> bkgVehicles { get; set; }
    }
}
