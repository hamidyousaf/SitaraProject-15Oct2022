using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class SysOrganization
    {
        [Key]
        [MaxLength(20)]
        public int Organization_Id { get; set; }
        [MaxLength(50)] 
        public string OrgName { get; set; }
        [MaxLength(200)] 
        public string OrgType { get; set; } 

        public DateTime EffectiveFromDate { get; set; }
        public DateTime EffectiveToDate { get; set; }

        [MaxLength(450)]
        public string OrgLocation { get; set; }
        public string OrgInternalExternal { get; set; }
        public string OrgAddress { get; set; }
        public string OrgIntAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string ShortName { get; set; }

        public int CompanyId { get; set; }

               
    }
}
