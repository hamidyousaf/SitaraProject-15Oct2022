using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREmployee
    {
        public int Id { get; set;}
        [Required][Display(Name = "Supervisor")] public int SupervisorId { get; set;}
        [Required] [Display(Name = "Replaced with")] public int ReplaceEmployeeId { get; set;}
        [Display(Name ="Probation Period")]public int ProbationPeriod { get; set;}
        public int OrgId { get; set;}
        [MaxLength(450)]public string JoinApprovedBy { get; set;}
        [Display(Name = "Type")] public int TypeId { get; set;}
        [Display(Name = "Company")] public int CompanyId { get; set;}
        [Display(Name ="City")]public int City { get; set;}
        [Display(Name = "Bank Name")] public int BankId { get; set;}

        [Required] [Display(Name = "Name")] [MaxLength(50)] public string Name { get; set; }
        [Required] [Display(Name = "Photo")] [MaxLength(450)] public string Photo { get; set; }
        [Required] [Display(Name = "Father Name")] [MaxLength(50)] public string FatherName { get; set; }
        [Required] [Display(Name = "CNIC")] [MaxLength(15)] public string Nic { get; set; }
        [Display(Name = "N.T.N")] [MaxLength(15)] public string Ntn { get; set; }
        [Display(Name = "S.S.N")] [MaxLength(100)] public string SocialSecurityNo { get; set; }
        [Display(Name = "Temp Address")] [MaxLength(200)] public string TemporaryAddress { get; set; }
        [Required] [Display(Name = "Permanent Address")] [MaxLength(200)] public string PermanentAddress { get; set; }
        [Display(Name = "Father C.N.I.C")] [MaxLength(15)] public string FatherCnic { get; set; }
        [Required] [Display(Name = "Mobile")] [MaxLength(15)] public string MobilePhone { get; set; }
        [Display(Name = "Home Phone")] [MaxLength(15)] public string HomePhone { get; set; }
        [Display(Name = "Email")] [MaxLength(100)] public string EmailAddress { get; set; }
        [Display(Name = "EOBI Remarks")] [MaxLength(200)] public string EobiRemarks { get; set; }
        [Required] [Display(Name = "Designation")] [MaxLength(50)] public string Designation { get; set; }
        [Display(Name = "Blood Group")] [MaxLength(10)] public string BloodGroup { get; set; }
        [Display(Name = "Travel Check")] [MaxLength(50)] public string TravelCheck { get; set; }
        [Required] [Display(Name="Gender")][MaxLength(10)]public string Sex { get; set; }
        [Required] [Display(Name="Next To kin")][MaxLength(50)]public string NextToKin { get; set; }
        [Required] [Display(Name="Department")][MaxLength(50)]public string Department { get; set; }
        [Display(Name = "Resign Approved By")] [MaxLength(450)] public string ResignApprovedBy { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Stop Name")] public string StopName { get; set;}
        public string SsCheck { get; set;}
        [Display(Name = "Rest Day 1")] [MaxLength(7)] public string RestDay1 { get; set;}
        [Display(Name = "Rest Day 2")] [MaxLength(7)] public string RestDay2 { get; set;}
        [Display(Name = "Relation")] [MaxLength(50)] public string Relation { get; set;}
        [Display(Name = "Reference")] [MaxLength(50)] public string Reference { get; set;}
        [Display(Name = "Qualification")] public string Qualification { get; set;}
        public string PieceRate { get; set;}
        [Display(Name = "Old Employee No")] public string OldEmployeeNo { get; set;}
        public string MedicalChk { get; set;}
        [Display(Name = "Medical Card No")] public string MedicalCardNo { get; set;}
        [Display(Name = "Grade")] public string Grade { get; set;}
        [Display(Name = "Final Settlement")] public string FinalSettlement { get; set;}
        [Display(Name = "Family Code")] public string FamilyCode { get; set;}
        [Display(Name = "Experience")] public string Experience { get; set;}
        public string EobiCheck { get; set;}
        [Display(Name = "Employee#")][MaxLength(50)] public string No { get; set;}
        public string Group { get; set;}
        [Display(Name = "Attendance Card No")] public string CardId { get; set;}
        [Display(Name = "Branch")] public string BranchId { get; set;}
        [Display(Name = "Bank Account")] public string BankAccountNo { get; set;}

        [Column(TypeName = "numeric(18,2)")] public decimal TravelAllowance { get; set;}
        [Display(Name = "OT Rate")] [Column(TypeName = "numeric(18,2)")] public decimal OtRate { get; set;}
        [Column(TypeName = "numeric(18,2)")] public decimal MedicalAllowance { get; set;}
        [Column(TypeName = "numeric(18,2)")] public decimal LunchRate { get; set;}
        [Display(Name = "First Pay")] [Column(TypeName = "numeric(18,2)")] public decimal FirstPay { get; set;}
        [Column(TypeName = "numeric(18,2)")] public decimal CurrentPay { get; set;}
        [Column(TypeName = "numeric(18,2)")] public decimal ConveyanceRate { get; set;}
        [Column(TypeName = "numeric(18,2)")] public decimal Conveyance { get; set;}

        public bool IsBlocked { get; set; }
        public bool IsProbition { get; set; }
        public bool OtCheck { get; set; }
        public bool Pf { get; set; }
        public bool Gratuity { get; set; }
        public bool IsDeleted { get; set; }



        [Display(Name = "Resign Date")] public DateTime ResignDate { get; set;}
        public DateTime ResignApprovalDate { get; set;}
        [Display(Name = "PF Start Date")] public DateTime PfStartDate { get; set;}
        public DateTime UpdatedDated { get; set;}
        public DateTime JoinApprovalDate { get; set;}
        [Display(Name = "Insurance Date")] public DateTime InsuranceDate { get; set;}
        [Display(Name = "Date of Joining")] public DateTime Doj { get; set;}
        public DateTime Doc { get; set;}
        [Required] [Display(Name = "Date of Birth")] public DateTime Dob { get; set;}
        [Display(Name = "Blocked From")] public DateTime BlockedFrom { get; set;}
        [Display(Name = "Blocked To")] public DateTime BlockedTo { get; set;}
        public DateTime CreatedDate { get; set;}
    }
}
