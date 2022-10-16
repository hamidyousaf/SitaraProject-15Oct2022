using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class VMGriegeRequisition
    {
        public GRGriegeRequisition GriegeRequisition { get; set; } = new GRGriegeRequisition();
        public GRGriegeRequisitionDetails GriegeRequisitionDetails { get; set; } = new GRGriegeRequisitionDetails();
        public VMGRGriegeRequisitionDetails VMGRL { get; set; } = new VMGRGriegeRequisitionDetails();
        public List<VMGRGriegeRequisitionDetails> GRGriegeRequisitionDetailsList { get; set; }
        public string ListDate { get; set; }
        public string DepartmentName { get; set; }
        public string SubDepartmentName { get; set; }
    }
}
