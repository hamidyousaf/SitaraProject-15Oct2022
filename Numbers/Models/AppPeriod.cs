using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class AppPeriod
    {
        public int Id { get; set; }
        [MaxLength(6)]
        public string Code { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
        [Column(TypeName ="Date")]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
