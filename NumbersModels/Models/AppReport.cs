using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class AppReport
    {
        public int Id { get; set; }
        [MaxLength(100)] public string Name { get; set; }
        [MaxLength(100)] public string Description { get; set; }
        [MaxLength(450)] public string ReportPath { get; set; }
        public int CompanyId { get; set; }
        [MaxLength(10)] public string Module { get; set; }
        [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsListed { get; set; }
        public bool IsActive { get; set; }
    }
}
