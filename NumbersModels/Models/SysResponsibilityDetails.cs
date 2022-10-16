using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class SysResponsibilityDetails
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int CostCenterId { get; set; }
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
        public int RespId { get; set; }
        public bool IsDeleted { get; set; }

    }
}
