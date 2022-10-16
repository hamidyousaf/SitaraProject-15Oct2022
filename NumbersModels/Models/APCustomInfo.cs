using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APCustomInfo
    {
        public int Id { get; set; }
        public int CustomNo { get; set; }
        public int GDNo { get; set; }
        public DateTime GDDate { get; set; }
        public int IGMNo { get; set; }
        public DateTime IGMDate { get; set; }
        public int ClearingAgent_Id { get; set; }
        public int SROBenefit_Id { get; set; }
        public decimal AssetValue { get; set; }
        public string Attachment { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int Resp_Id { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int ShipmentId { get; set; }


    }
}
