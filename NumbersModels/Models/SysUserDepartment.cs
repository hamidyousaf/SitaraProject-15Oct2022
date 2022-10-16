using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
  public  class SysUserDepartment
    {
        [Key]
        [MaxLength(20)]
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int SubDepartmentId { get; set; }
        public string SubDepartment { get; set; }
        public string DepartmentKey { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
