using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Repository.HR
{
    public class EmployeeRepo: SessionBase
    {
        //private static readonly IHttpContextAccessor _ihttpContextAccessor ;

        //for photo uploading function
        public async static Task<string> UploadFile(IFormFile img)
        {
            string filesList = "";
            if (img != null)
            {
                if (img.Length > 0)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\HR\\employee-images", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(Fstream);
                        var fullPath = "/uploads/HR/employee-images/" + fileName;
                        filesList += fullPath;
                    }
                }
            }
            return filesList;
        }

        public static IEnumerable<HREmployee> GetAll()
        {
            IEnumerable<HREmployee> employees = _dbContext.HREmployees.ToList().Where(e=>e.CompanyId==_companyId && !e.IsDeleted);
            return employees;
        }
        public static HREmployee GetById(int id)
        { 
            HREmployee employee = _dbContext.HREmployees.Where(e => e.CompanyId == _companyId && e.Id == id).FirstOrDefault();
            return employee;
        }
        public static HREmployee Create(int id)
        {
            HREmployee employee = _dbContext.HREmployees.Find(id);
            return employee;
        }
        public static async Task <bool> Create(HREmployee employee, IFormCollection collection, IFormFile Photo)
        {
            try
            {
                var model = new HREmployee();
                if (Photo != null)
                {
                    model.Photo =await UploadFile(Photo);
                }
                model.CompanyId = _companyId;
                model.IsDeleted = false;
                model.CreatedBy = _userId;
                model.CreatedDate = DateTime.Now;
                HREmployeeFamily[] familyMembers = JsonConvert.DeserializeObject<HREmployeeFamily[]>(collection["familyDetail"]);
                HREmployeeExperience[] employeeExperinces = JsonConvert.DeserializeObject<HREmployeeExperience[]>(collection["experienceDetail"]);
                HRDeductionEmployee[] employeeDeductions = JsonConvert.DeserializeObject<HRDeductionEmployee[]>(collection["deductionDetail"]);
                HREmployeeSalaryBreakUp[] employeeSalaries = JsonConvert.DeserializeObject<HREmployeeSalaryBreakUp[]>(collection["salaryDetail"]);
                HREmployeeQualification[] employeeQualifications = JsonConvert.DeserializeObject<HREmployeeQualification[]>(collection["degreeDetail"]);    
                _dbContext.HREmployees.Add(employee);
                await _dbContext.SaveChangesAsync();

                foreach (HREmployeeFamily familyMember in familyMembers)
                {
                    HREmployeeFamily member = new HREmployeeFamily();
                    member = familyMember;
                    member.EmployeeId = employee.Id;
                    member.CreatedBy = _userId;
                    member.CreatedDate = DateTime.Now;
                    _dbContext.HREmployeeFamilies.Add(member);
                    await _dbContext.SaveChangesAsync();

                }
                foreach (HREmployeeExperience employeeExperince in employeeExperinces)
                {
                    HREmployeeExperience experience = new HREmployeeExperience();
                    experience = employeeExperince;
                    experience.EmployeeId = employee.Id;
                    experience.CreatedBy = _userId;
                    experience.CreatedDate = DateTime.Now;
                    _dbContext.HREmployeeExperiences.Add(experience);
                    await _dbContext.SaveChangesAsync();

                }
                foreach (HRDeductionEmployee employeeDeduction in employeeDeductions)
                {
                    HRDeductionEmployee deduction = new HRDeductionEmployee();
                    deduction = employeeDeduction;
                    deduction.EmployeeId = employee.Id;
                    deduction.CreatedBy = _userId;
                    deduction.CreatedDate = DateTime.Now;
                    _dbContext.HRDeductionEmployees.Add(deduction);
                    await _dbContext.SaveChangesAsync();

                }
                foreach (HREmployeeSalaryBreakUp employeeSalary in employeeSalaries)
                {
                    HREmployeeSalaryBreakUp salary = new HREmployeeSalaryBreakUp();
                    salary = employeeSalary;
                    salary.EmployeeId = employee.Id;
                    salary.CreatedBy = _userId;
                    salary.CreatedDate = DateTime.Now;
                    _dbContext.HREmployeeSalaryBreakUps.Add(salary);
                    await _dbContext.SaveChangesAsync();

                }
                foreach (HREmployeeQualification employeeQualification in employeeQualifications)
                {
                    HREmployeeQualification qualification = new HREmployeeQualification();
                    qualification = employeeQualification;
                    qualification.EmployeeId = employee.Id;
                    qualification.CreatedBy = _userId;
                    qualification.CreatedDate = DateTime.Now;
                    _dbContext.HREmployeeQualifications.Add(qualification);
                    await _dbContext.SaveChangesAsync();

                }
                return true;
            }
            catch(Exception ex)
            {
                string msg = ex.InnerException.Message.ToString();
                return false;
            }
        }
        public static HREmployee Update(int id)
        {
            HREmployee employee = _dbContext.HREmployees.Find(id);
            return employee;
        }
        [HttpPost]
        public async static Task<bool> Update(HREmployee employee,IFormCollection collection, IFormFile Photo)
        {
            HREmployee emp = _dbContext.HREmployees.Find(employee.Id);
            if (Photo != null)
            {
                emp.Photo =await UploadFile(Photo);
            }
            else
            {
                _dbContext.Entry(emp).State = EntityState.Modified;
                _dbContext.Entry(emp).Property(x => x.Photo).IsModified = false;
            }
            emp.CompanyId = _companyId;
            emp.UpdatedBy = _userId;
            emp.UpdatedDated = DateTime.Now;
            var entry = _dbContext.HREmployees.Update(emp);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            HREmployeeFamily[] familyMembers = JsonConvert.DeserializeObject<HREmployeeFamily[]>(collection["familyDetail"]);
            HREmployeeExperience[] employeeExperinces = JsonConvert.DeserializeObject<HREmployeeExperience[]>(collection["experienceDetail"]);
            HRDeductionEmployee[] employeeDeductions = JsonConvert.DeserializeObject<HRDeductionEmployee[]>(collection["deductionDetail"]);
            HREmployeeSalaryBreakUp[] employeeSalaries = JsonConvert.DeserializeObject<HREmployeeSalaryBreakUp[]>(collection["salaryDetail"]);
            HREmployeeQualification[] employeeQualifications = JsonConvert.DeserializeObject<HREmployeeQualification[]>(collection["degreeDetail"]);
            foreach(var familymember in familyMembers)
            {
                var id = familymember.Id;
                if (id == 0)
                {
                    HREmployeeFamily member = new HREmployeeFamily();
                    member = familymember;
                    member.EmployeeId = emp.Id;
                    member.CreatedBy = _userId;
                    member.CreatedDate = DateTime.Now;
                    member.CompanyId = _companyId;
                    _dbContext.HREmployeeFamilies.Add(member);
                    await  _dbContext.SaveChangesAsync();
                }
                else
                {
                    HREmployeeFamily member = _dbContext.HREmployeeFamilies.Where(f => f.Id == id).FirstOrDefault();
                    member.RelativeName = familymember.RelativeName;
                    member.Relation = familymember.Relation;
                    member.Dob = familymember.Dob;
                    member.UpdatedBy = _userId;
                    member.UpdatedDate = DateTime.Now;
                    var tracker = _dbContext.HREmployeeFamilies.Update(member);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            foreach (var employeeExperince in employeeExperinces)
            {
                if (employeeExperince.Id == 0)
                {
                    HREmployeeExperience experience = new HREmployeeExperience();
                    experience = employeeExperince;
                    experience.CompanyId = _companyId;
                    experience.EmployeeId = emp.Id;
                    experience.CreatedBy = _userId;
                    experience.CreatedDate = DateTime.Now;
                    _dbContext.HREmployeeExperiences.Add(experience);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    HREmployeeExperience experience = _dbContext.HREmployeeExperiences.Find(employeeExperince.Id);
                    experience.Company = employeeExperince.Company;
                    experience.Position = employeeExperince.Position;
                    experience.Pay = employeeExperince.Pay;
                    experience.JoiningDate = employeeExperince.JoiningDate;
                    experience.ResigningDate = employeeExperince.ResigningDate;
                    experience.Duration = employeeExperince.Duration;
                    experience.UpdatedBy = _userId;
                    experience.UpdatedDate = DateTime.Now;
                    var tracker = _dbContext.HREmployeeExperiences.Update(experience);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            foreach (var employeeDeduction in employeeDeductions)
            {
                if (employeeDeduction.Id == 0)
                {
                    HRDeductionEmployee deduction = new HRDeductionEmployee();
                    deduction = employeeDeduction;
                    deduction.EmployeeId = emp.Id;
                    deduction.CreatedBy = _userId;
                    deduction.CompanyId = _companyId;
                    _dbContext.HRDeductionEmployees.Add(deduction);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    HRDeductionEmployee deduction = _dbContext.HRDeductionEmployees.Find(employeeDeduction.Id);
                    deduction.DeductionId = employeeDeduction.DeductionId;
                    deduction.Amount = employeeDeduction.Amount;
                    deduction.UpdatedBy = _userId;
                    deduction.UpdatedDate = DateTime.Now;
                    var tracker = _dbContext.HRDeductionEmployees.Update(deduction);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            foreach (var employeeSalary in employeeSalaries)
            {
                if (employeeSalary.Id == 0)
                {
                    HREmployeeSalaryBreakUp salary = new HREmployeeSalaryBreakUp();
                    salary = employeeSalary;
                    salary.CompanyId = _companyId;
                    salary.EmployeeId = emp.Id;
                    salary.CreatedBy = _userId;
                    _dbContext.HREmployeeSalaryBreakUps.Add(salary);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    HREmployeeSalaryBreakUp salary = _dbContext.HREmployeeSalaryBreakUps.Find(employeeSalary.Id);
                    salary.EmployeeAllowanceId = employeeSalary.EmployeeAllowanceId;
                    salary.Amount = employeeSalary.Amount;
                    salary.UpdatedBy = _userId;
                    salary.UpdatedDate = DateTime.Now;
                    var tracker = _dbContext.HREmployeeSalaryBreakUps.Update(salary);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            foreach (var employeeQualification in employeeQualifications)
            {
                if (employeeQualification.Id == 0)
                {
                    HREmployeeQualification qualification = new HREmployeeQualification();
                    qualification = employeeQualification;
                    qualification.EmployeeId = emp.Id;
                    qualification.CreatedBy = _userId;
                    _dbContext.HREmployeeQualifications.Add(qualification);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    HREmployeeQualification qualification = _dbContext.HREmployeeQualifications.Find(employeeQualification.Id);
                    qualification.Qualification = employeeQualification.Qualification;
                    qualification.PassingYear = employeeQualification.PassingYear;
                    qualification.Institute = employeeQualification.Institute;
                    qualification.UpdatedBy = _userId;
                    qualification.UpdatedDate = DateTime.Now;
                    var tracker = _dbContext.HREmployeeQualifications.Update(qualification);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            dynamic[] deletedFamily = collection["deletedFamily"];
            dynamic[] deletedExperience = collection["deletedExperience"];
            dynamic[] deletedDeduction = collection["deletedDeduction"];
            dynamic[] deletedSalary = collection["deletedSalary"];
            dynamic[] deletedDegree = collection["deletedDegree"];
            
            foreach(dynamic item in deletedFamily)
            {
                if (item != "")
                {
                    int id = Convert.ToInt32(item);
                    HREmployeeFamily member = _dbContext.HREmployeeFamilies.Find(id);
                    member.IsDeleted = true;
                    member.UpdatedBy = _userId;
                    member.CompanyId = _companyId;
                    var tracker = _dbContext.HREmployeeFamilies.Update(member);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            foreach(dynamic item in deletedExperience)
            {
                if (item != "")
                {
                    int id = Convert.ToInt32(item);
                    HREmployeeExperience experience = _dbContext.HREmployeeExperiences.Find(id);
                    experience.IsDeleted = true;
                    experience.UpdatedBy = _userId;
                    var tracker = _dbContext.HREmployeeExperiences.Update(experience);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            foreach(dynamic item in deletedDeduction)
            {
                if (item != "")
                {
                    int id = Convert.ToInt32(item);
                    HRDeductionEmployee deduction = _dbContext.HRDeductionEmployees.Find(id);
                    deduction.IsDeleted = true;
                    deduction.UpdatedBy = _userId;
                    deduction.CompanyId = _companyId;
                    var tracker = _dbContext.HRDeductionEmployees.Update(deduction);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            foreach(dynamic item in deletedSalary)
            {
                if (item != "")
                {
                    int id = Convert.ToInt32(item);
                    HREmployeeSalaryBreakUp salary = _dbContext.HREmployeeSalaryBreakUps.Find(id);
                    salary.IsDeleted = true;
                    salary.UpdatedBy = _userId;
                    var tracker = _dbContext.HREmployeeSalaryBreakUps.Update(salary);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            foreach(dynamic item in deletedDegree)
            {
                if (item != "")
                {
                    int id = Convert.ToInt32(item);
                    HREmployeeQualification qualification = _dbContext.HREmployeeQualifications.Find(id);
                    qualification.IsDeleted = true;
                    qualification.UpdatedBy = _userId;
                    var tracker = _dbContext.HREmployeeQualifications.Update(qualification);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            return true; 
        }

        public static dynamic GetEmployeeFamilyMembers(int employeeId)
        {
             var familyMembers =  _dbContext.HREmployeeFamilies.ToArray().Where(q => q.EmployeeId == employeeId && !q.IsDeleted);
            return familyMembers;
        }
        public static dynamic GetEmployeeExperiences(int employeeId)
        {
            var employeeExperiences =  _dbContext.HREmployeeExperiences.ToArray().Where(q => q.EmployeeId == employeeId && !q.IsDeleted);
            return employeeExperiences;
        }
        public static dynamic GetEmployeeDeductions(int employeeId)
        {
            var employeeDeductions = _dbContext.HRDeductionEmployees.Include(d=>d.Employee).Where(q => q.EmployeeId == employeeId && !q.IsDeleted).ToArray();
            return employeeDeductions;
        }
        public static dynamic GetEmployeeSalaries(int employeeId)
        {
            var employeeSalaries = _dbContext.HREmployeeSalaryBreakUps.ToArray().Where(q => q.EmployeeId == employeeId && !q.IsDeleted);
            return employeeSalaries;
        }
        public static dynamic GetEmployeeDegrees(int employeeId)
        {
            var employeeDegrees = _dbContext.HREmployeeQualifications.ToArray().Where(q => q.EmployeeId == employeeId && !q.IsDeleted);
            return employeeDegrees;
        }

        [HttpGet]
        public static dynamic  GetEmployees(string q = "")
        {
            var employees = _dbContext.HREmployees.Where(
                                                (a => a.Name.Contains(q) && a.CompanyId == _companyId && a.IsDeleted==false))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Id, " - ", a.Name)
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return employees;
        }
        [HttpGet]
        public static dynamic GetEmployee(int id)
        {
            var employee = _dbContext.HREmployees.Where(a => a.Id == id && a.CompanyId == _companyId && a.IsDeleted==false)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Id, " - ", a.Name)
                                               })
                                               .FirstOrDefault();
            return employee;
        }
    }
}
