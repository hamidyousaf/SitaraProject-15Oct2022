using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class GLSubDivisionVM
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string DepartmentName { get; set; }
        public string SubDepartmentName { get; set; }
        public string Status { get; set; }
        public GLDivision GLDivision { get; set; }
        public GLSubDivision GLSubDivision { get; set; }
    }
}
