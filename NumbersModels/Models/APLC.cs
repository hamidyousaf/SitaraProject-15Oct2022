using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Numbers.Entity.Models
{
     public class APLC
    {
        [Key]
        public int Id { get; set; }
        public int TransctionNo { get; set; }
        public string LCNo { get; set; }
        public DateTime LCOpenDate { get; set; }
        public DateTime LatestShipmentDate { get; set; }
        public DateTime LCCloseDate { get; set; }
        public DateTime LCExpiryDate { get; set; }
        public int BankId { get; set; }
        public decimal TotalAmount { get; set; }
        public int LCTypeId { get; set; }
        public decimal FCAmount { get; set; }
        public decimal PKRAmount { get; set; }
        public string EIFNo { get; set; }
        public DateTime EIFDate { get; set; }
        public string Attachment { get; set; }
        public string CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int Resp_Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string UnApprovedBy { get; set; }
        public DateTime UnApprovedDate { get; set; }
        public bool IsApproved { get; set; }
        [NotMapped]
        public  string Date { get; set; }
        [NotMapped]
        public string BankName { get; set; }
        [NotMapped]
        public string LCName { get; set; }
        [NotMapped]
        public string VendorName { get; set; }
        [NotMapped]
        public string CurrencyName { get; set; }
        public int VendorId { get; set; }
        public int POId { get; set; }
        public int PrDetailId { get; set; }
        public bool IsShipment { get; set; }
        public int SubAccountId { get; set; }
    }
}
