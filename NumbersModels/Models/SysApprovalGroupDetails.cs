using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class SysApprovalGroupDetails
    {
        [Key]
        public int Id { get; set; }
        public int Approval_Group_Id { get; set; }

        public string User_ID { get; set; }

        public string Description { get; set; }
    }
}
