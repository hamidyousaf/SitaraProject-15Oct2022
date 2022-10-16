using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
  public  class Responsibilities
    {
        [Key]
        public int Resp_Id { get; set; }
        public int Menu_Id { get; set; }
        public string Resp_Name { get; set; }
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Effective From Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime Effective_From_Date { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Effective To Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime Effective_To_Date { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Module_Id { get; set; }
        public int CompanyId { get; set; }
        public int TypeId { get; set; }

        public  AppCompany Company { get; set; }
        [NotMapped]
        public List< Sys_ResponsibilityItemCategory> ResponsibilityItemCategory { get; set; }
    }
}
