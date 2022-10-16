using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
   public class Sys_Rules_Approval_Details
    {
        [Key]
        public int Id { get; set; }
        public int Rule_Id { get; set; }
        public string Attribute_Name { get; set; }

        public string Operator { get; set; }

        public string Table_Name { get; set; }
        public string Value { get; set; }
    }
}
