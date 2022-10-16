using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using Numbers.Repository.HR;

namespace Numbers.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NumbersDbContext _dbContext;
        public EmployeeController(NumbersDbContext context, IHttpContextAccessor http)
        {
            _dbContext = context;
            _httpContextAccessor = http;
        }
        public IActionResult Index()
        {
            IEnumerable<HREmployee> list = EmployeeRepo.GetAll();
            return View(list);

        }
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            ViewBag.DeductionTypes = configValues.GetConfigValues("HR", "Deduction Type", companyId);
            ViewBag.Allowances = configValues.GetAllowances(companyId);
            if (id == 0)
            {
                ViewBag.EntityState = "Create";
                ViewBag.FamilyDetail = "[]";
                ViewBag.ExperienceDetail = "[]";
                ViewBag.DeductionDetail = "[]";
                ViewBag.SalaryDetail = "[]";
                ViewBag.DegreeDetail = "[]";
                return View(new HREmployee());
            }
            else
            {
                ViewBag.EntityState = "Update";
                var employee = EmployeeRepo.GetById(id);                
                ViewBag.FamilyDetail = JsonConvert.SerializeObject(EmployeeRepo.GetEmployeeFamilyMembers(id));
                ViewBag.ExperienceDetail = JsonConvert.SerializeObject(EmployeeRepo.GetEmployeeExperiences(id));
                ViewBag.DeductionDetail = JsonConvert.SerializeObject(EmployeeRepo.GetEmployeeDeductions(id));
                ViewBag.SalaryDetail = JsonConvert.SerializeObject(EmployeeRepo.GetEmployeeSalaries(id));
                ViewBag.DegreeDetail = JsonConvert.SerializeObject(EmployeeRepo.GetEmployeeDegrees(id));
                return View(employee);
            }
        }
        [HttpPost]
        public async Task <IActionResult> Create(HREmployee employee, IFormCollection collection,IFormFile Photo)
        {
            if (employee.Id == 0)
            {
                var success = await EmployeeRepo.Create(employee, collection,Photo);
                if (success==true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Employee has been Added Successfully";
                    return RedirectToAction(nameof(Index));
                }
                return View();
            }
            else
            {
                var success =await EmployeeRepo.Update(employee, collection, Photo);
                //var success = await Update(employee, collection);
                if (success==true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Employee has been Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
                return View();
            }
        }
    }
}