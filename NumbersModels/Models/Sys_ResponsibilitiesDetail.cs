using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
  public  class Sys_ResponsibilitiesDetail
    {
        [Key]
        [MaxLength(20)]
        public int Id { get; set; }
        public int ResponsibilityId { get; set; }
        public string Responsibility { get; set; }
        public string ResponsibilityKey { get; set; }
        public DateTime Date_from { get; set; }
        public DateTime Date_To { get; set; }
        public string Remarks { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public bool Approve { get; set; }
        public bool UnApprove { get; set; }
    }
}
