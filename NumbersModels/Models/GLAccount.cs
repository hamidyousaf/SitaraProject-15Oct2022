using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
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
        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }


        public IList<GLBankCashAccount> gLBankCashAccounts { get; set; }
        public IList<GLVoucherDetail> GLVoucherDetails { get; set; }


        public IList<BkgCustomer> bkgCustomers { get; set; }

       
        //public IList<BkgReceipt> bkgReceipts { get; set; }
        //public IList<BkgCommissionReceived> bkgCommissionReceiveds { get; set; }
        //public IList<BkgCommissionPayment> bkgCommissionPayments { get; set; }
        //public IList<BkgVehicle> bkgVehicles { get; set; }
    }
}
