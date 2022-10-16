using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    [Area("GL")]
 public class CostCenter
    {   [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Code { get; set; }

        public string Description { get; set; }

        public int DivisionId { get; set; }



        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
  
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int SubDivisionId { get; set; }
        public bool? IsApproved { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Status { get; set; }
        //Navigational Property
        public GLDivision Division { get; set; }
        public GLSubDivision SubDivision { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
    }
}
