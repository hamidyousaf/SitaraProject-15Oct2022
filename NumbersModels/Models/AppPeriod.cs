using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class AppPeriod
    {
        public int Id { get; set; }
        [MaxLength(6)]
        public string Code { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        [Column(TypeName ="Date")]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [MaxLength(10)]
        public string Type { get; set; }
        public int CompanyId { get; set; }
        public bool IsGLClosed { get; set; }
        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool PayrollOpen { get; set; }
        public bool PayrollClose { get; set; }
        public int ModuleId { get; set; }

    }
}
