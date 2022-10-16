using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARSalePerson
    {
        public int ID { get; set; }
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public decimal Comission { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int Resp_ID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        
    }
}
