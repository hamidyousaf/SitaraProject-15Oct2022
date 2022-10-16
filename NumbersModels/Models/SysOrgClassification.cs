using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class SysOrgClassification
    {
        [Key]
        [MaxLength(20)]
        public int ClassificationDetailId { get; set; }
        public int OrganizationId { get; set; }
        [MaxLength(50)] 
        public int DocId { get; set; }
        [MaxLength(200)]
        public int ClassificationId { get; set; } 
        public bool IsEnable { get; set; } 
        [MaxLength(200)]
        public int Ledger_Id { get; set; }
        [MaxLength(200)]
        public int OperationUnitId { get; set; }
        [MaxLength(200)]
        public int BusinessUnitId { get; set; }
        public int CompanyId { get; set; }
      

    }
}
