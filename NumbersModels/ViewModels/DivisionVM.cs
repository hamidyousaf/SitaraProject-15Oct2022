using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class DivisionVM
    {
        public GLDivision GLDivision { get; set; }
        public GLSubDivision GLSubDivision { get; set; }

        public List<GLDivision> GLDivisionList { get; set; }
        public List<GLSubDivision> GLSubDivisionList { get; set; }
    }
}
