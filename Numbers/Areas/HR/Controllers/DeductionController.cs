using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using Numbers.Repository.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class DeductionController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public DeductionController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<HRDeduction> list = DeductionRepo.GetAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int id)
   {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            ViewBag.Counter = 0;
            ViewBag.Department = configValues.GetConfigValues("HR", "Department", companyId);
            if (id == 0)
            {
                ViewBag.EntityState = "Create";
                var model = new HRDeductionViewModel();
             //   model.EmployeeType = DeductionRepo.GetEmployeeTypes();
                TempData["DeductionNo"] = DeductionRepo.GetDeductionNo(id);
                return View(model);
            }
            else
            {
                HRDeductionViewModel model = DeductionRepo.GetById(id);
               // HRDeductionEmployee[] deductions = DeductionRepo.GetEmployeeDeduction(id);
              //  ViewBag.Items = deductions;
                TempData["DeductionNo"] = model.DeductionNo;
                if (model.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(HRDeductionViewModel model, IFormCollection collection)
        {
            if (model.Id == 0)
            {
                var success = await DeductionRepo.Create(model, collection);
                if (success)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Deduction has been Added Successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong while adding Increment";
                    return View();
                }
            }
            else
            {
                var success = await DeductionRepo.Update(model, collection);
                if (success)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Deduction has been Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong while updating Increment";
                    return View();
                }
            }
        }

        public IActionResult Delete(int id)
        {
            dynamic isSuccess = DeductionRepo.Delete(id);
            if (isSuccess = true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Deduction has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeductionItems(int? counter)
        {
            ViewBag.Counter = counter;
            //var model = new HRIncrementViewModel();
            //model.EmployeeTypes = IncrementRepo.GetEmployeeTypes();
            return PartialView("_partialIncrementItems");
        }
        public IActionResult GetEmployeeDeductions(int id, int itemId)
        {
            ViewBag.Counter = id;
            HRDeductionViewModel viewModel = DeductionRepo.GetEmployeeDeduction(id, itemId);
            return PartialView("_partialIncrementItems", viewModel);
        }
    }
}
