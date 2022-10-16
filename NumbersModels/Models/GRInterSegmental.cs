using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRInterSegmental
    {
        public int Id { get; set; }
        public int TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public int CompanyId { get; set; }
        public int TransactionType { get; set; }
        public int SegmentsId    { get; set; }
        public int CustomerId { get; set; }
        public string Remarks { get; set; }
        public int GRNId { get; set; }
        public int ItemCategory2Id { get; set; }
        public int ItemId    { get; set; }
        public int BrandId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public DateTime DeletedDate { get; set; }
    }
}
