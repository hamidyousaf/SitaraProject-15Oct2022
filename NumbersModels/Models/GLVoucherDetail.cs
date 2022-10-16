using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class GLVoucherDetail
    {
        public int Id { get; set; }
        [ForeignKey("Voucher")]
        public int VoucherId { get; set; }
        public GLVoucher Voucher { get; set; }
        public int AccountId { get; set; }
        public Int16? Sequence { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public int? ProjectId { get; set; }
        public int? SubAccountId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Debit { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Credit { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public DateTime? Realization { get; set; }

        [ForeignKey("SubAccount")]
        public int SubAccountIdName { get; set; }

        [ForeignKey("CostCenter")]
        public int CostCenterName { get; set; }
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
        //Navigational Property
        public GLAccount Account { get; set; }
        public GLDivision Department { get; set; }
        public GLSubDivision SubDepartment { get; set; }
        public virtual CostCenter CostCenter { get; set; }
        public virtual GLSubAccount SubAccount { get; set; }
    }
}
