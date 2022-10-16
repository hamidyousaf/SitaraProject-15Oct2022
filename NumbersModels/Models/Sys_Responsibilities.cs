using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
  public  class Sys_Responsibilities
    {
        [Key]
        [MaxLength(20)]
        public int Resp_Id { get; set; }
        [MaxLength(50)]
        public string Resp_Name { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }

        public DateTime Effective_From_Date { get; set; }
        public DateTime Effective_To_Date { get; set; }

        [MaxLength(450)]
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public int Module_Id { get; set; }
        public int CompanyId { get; set; }

    }
}
