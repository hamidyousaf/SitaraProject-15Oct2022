using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class SysProfileValues
    {
        [Key]
        public int Doc_Id { get; set; }

        [MaxLength(50)] 
        public string SecurityType { get; set; }
        [MaxLength(200)] 
        public string ProfileOption { get; set; } 

        public int ApplicationId { get; set; } 
        public int RespId { get; set; }

        public int UserId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string SiteValues { get; set; }
        public string ApplicationValues { get; set; }
        public string RespValues { get; set; }
        public string UserValues { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public SysProfileValues MyProperty { get; set; }
        public List<SysProfileValues> SysProfileValuesDetailList { get; set; }

    }
}
