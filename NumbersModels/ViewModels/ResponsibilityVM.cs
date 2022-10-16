using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
  public  class ResponsibilityVM
    {
        public int Resp_Id { get; set; }
        public int Menu_Id { get; set; }
        public string Resp_Name { get; set; }
        public string Description { get; set; }
        public DateTime Effective_From_Date { get; set; }
        public DateTime Effective_To_Date { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Module_Id { get; set; }
        public int CompanyId { get; set; }

        public List<Responsibilities> ResponsibilitiesList { get; set; }


    }
}
