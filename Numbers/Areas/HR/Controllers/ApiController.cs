using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Repository.HR;

namespace Numbers.Areas.HR.Controllers
{
   // [Area("HR")]
    [Authorize]
    public class ApiController : Controller
    {
        public dynamic GetEmployeeFamilyMembers(int id)
        {
            return JsonConvert.SerializeObject(EmployeeRepo.GetEmployeeFamilyMembers(id));
        }
        [HttpGet]
        public IActionResult GetEmployees(string q = "")
        {

            var employees = EmployeeRepo.GetEmployees();
            return Ok(employees);
        }
        [HttpGet]
        public IActionResult GeEmployee(int id)
        {
            var employee = EmployeeRepo.GetEmployee(id);
            return Ok(employee);
        }

    }
}