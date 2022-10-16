using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class VMARCommissionPayment
    {
        public int Id { get; set; }
        public int DetailId { get; set; }
        public int CommissionPaymentId { get; set; }
        public int ReceiptId { get; set; }
        public DateTime ReceiptDate { get; set; }
        public int CustomerId { get; set; }
        public string Customer { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public Decimal ReceiptAmount { get; set; }
        public Decimal CommissionAmount { get; set; }
        public Decimal PayAmount { get; set; }

        
        public int TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public int CityId { get; set; }
        public string City { get; set; }
        public int AgentId { get; set; }
        public string Agent { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ApprovedDate { get; set; }

        public DateTime DeletedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string DeletedBy { get; set; }
        public string UnApprovedBy { get; set; }
        public string Updatedby { get; set; }
        public int CompanyId { get; set; }
        public int Resp_Id { get; set; }

        public virtual  ARCommissionPayment ARCommissionPayment { get; set; }
        public List<ARCommissionPaymetDetail> ARCommissionPaymetDetail { get; set; }
        public List<VMCommissionPaymentDetail> DetailList { get; set; }
    }
}
