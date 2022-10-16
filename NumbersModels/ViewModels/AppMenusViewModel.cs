using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class AppMenusViewModel
    {


        public int Id { get; set; }
         
        public string Name { get; set; }
        public string ChildName { get; set; }
         
        public string Url { get; set; }
        
        public string IconClass { get; set; }
        public int MenuLevel { get; set; }
        [MaxLength(10)]
        public string MenuType { get; set; }
        public int Sequence { get; set; }
        [Required]
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
     
        public int ParentId { get; set; }
        public int ModuleId { get; set; }

    }
}
