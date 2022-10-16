using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class PlanMonthlyPlanning
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public int Resp_Id { get; set; }
        public int SPId { get; set; }
        public string Planofmonth { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedDate { get; set; }
        public DateTime UnApprovedDate { get; set; }
        public string UnApprovedBy { get; set; }
        public string DeletedBy { get; set; }
        public bool IsApproved { get; set; }
        public int PlanSpecificationId { get; set; }
        public int TotalMonthlyDesignCount { get; set; }
        public int TotalMonthlyRunQty { get; set; }
        public int TotalMonthlyFabricCons { get; set; }
        // navigational property
        //public virtual ICollection<PlanMonthlyPlanningItems> PlanMonthlyPlanningItems { get; set; }  
        public SeasonalPlaning SP { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
    }
}
