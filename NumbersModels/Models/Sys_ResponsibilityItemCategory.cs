using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class Sys_ResponsibilityItemCategory
    {
        [Key]
        public int Id { get; set; }
        public int ResponsibilityId { get; set; }
        [ForeignKey("ItemCategory")]
        public int ItemCategoryId { get; set; }
       public InvItemCategories ItemCategory { get; set; }


    }
}
