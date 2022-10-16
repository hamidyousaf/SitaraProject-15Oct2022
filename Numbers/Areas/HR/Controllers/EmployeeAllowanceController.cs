using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Numbers.Entity.Models;
using Numbers.Repository.HR;

namespace Numbers.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class EmployeeAllowanceController : Controller
    {
        //private readonly NumbersDbContext _dbContext;

        //public EmployeeAllowanceController(NumbersDbContext context)
        //{
        //    _dbContext = context;
        //}
         
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<HREmployeeAllowance> list = EmployeeAllowanceRepo.GetAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            if (id == 0)
            {
                return View(new HREmployeeAllowance());
            }
            else
            {
                HREmployeeAllowance AllowanceCreate = EmployeeAllowanceRepo.GetById(id);
                return View(AllowanceCreate);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(HREmployeeAllowance model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                if (model.Id == 0)
                {
                    bool isSuccess = await EmployeeAllowanceRepo.Create(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Leave has been created successfully.";
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "Something went wrong.";
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    bool isSuccess = await EmployeeAllowanceRepo.Update(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Leave has been updated successfully.";
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "Something went wrong.";
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            bool isSuccess = await EmployeeAllowanceRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Leave has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}