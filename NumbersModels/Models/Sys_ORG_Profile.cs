using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class Sys_ORG_Profile
    {
        public int ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string ProfileName { get; set; }
        public int CalssificationId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime Creation_Date { get; set; }
        public DateTime? Updation_Date { get; set; }
        public int CompanyId { get; set; }
        public int Resp_Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
