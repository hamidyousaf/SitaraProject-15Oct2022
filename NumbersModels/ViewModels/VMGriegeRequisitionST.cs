using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class VMGriegeRequisitionST
    {
        public GRGriegeRequisitionST GriegeRequisition { get; set; } = new GRGriegeRequisitionST();
        public GRGriegeRequisitionDetailsST GriegeRequisitionDetails { get; set; } = new GRGriegeRequisitionDetailsST();
        public GRGriegeRequisitionDetailsST VMGRL { get; set; } = new GRGriegeRequisitionDetailsST();
        public List<VMGRGriegeRequisitionDetailsST> GRGriegeRequisitionDetailsList { get; set; }
        public string ListDate { get; set; }
        public string DepartmentName { get; set; }
        public string SubDepartmentName { get; set; }
    }
}
