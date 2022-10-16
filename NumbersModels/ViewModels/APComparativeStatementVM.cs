using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class APComparativeStatementVM
    {
        public APComparativeStatement APComparativeStatement { get; set; }
        public string OperatingName { get; set; }
        public string ValidityDate { get; set; }
        public string CreatedBy { get; set; }
        public APCSRequest APCSRequests { get; set; }
        public List<APCSRequest> APCSRequestsList { get; set; }
        public List<APCSRequestDetail> APCSRequestDetails { get; set; }

        
    }
}
