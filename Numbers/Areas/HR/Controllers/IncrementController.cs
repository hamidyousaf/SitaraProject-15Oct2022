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
    public class IncrementController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public IncrementController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<HRIncrement> list = IncrementRepo.GetAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            ViewBag.Counter =0;
            ViewBag.Department = configValues.GetConfigValues("HR", "Department", companyId);
            if (id == 0)
            {
                ViewBag.EntityState = "Create";
                var model = new HRIncrementViewModel();
                model.EmployeeTypes = IncrementRepo.GetEmployeeTypes();
                TempData["IncrementNo"] = IncrementRepo.GetIncrementNo(id);
                return View(model);
            }
            else
            {
                HRIncrementViewModel model = IncrementRepo.GetById(id);
                HRIncrementEmployee[] increments = IncrementRepo.GetIncrementEmployees(id);
                ViewBag.Items =increments;
                TempData["IncrementNo"] = model.IncrementNo;
                if (model.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(HRIncrementViewModel model, IFormCollection collection)
        {
            if (model.Id == 0)
            {
                var success = await IncrementRepo.Create(model, collection);
                if (success)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Increment has been Added Successfully";
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
                var success = await IncrementRepo.Update(model, collection);
                if (success)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Increment has been Updated Successfully";
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
            dynamic isSuccess = IncrementRepo.Delete(id);
            if (isSuccess = true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Increment has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult PartialIncrementItems(int? counter)
        {
            ViewBag.Counter = counter;
            //var model = new HRIncrementViewModel();
            //model.EmployeeTypes = IncrementRepo.GetEmployeeTypes();
            return PartialView("_partialIncrementItems");
        }
        public IActionResult GetEmployeeIncrements(int id, int itemId)
        {
            ViewBag.Counter = id;
            HRIncrementViewModel viewModel = IncrementRepo.GetEmployeeIncrements(id, itemId);
            return PartialView("_partialIncrementItems", viewModel);
        }
    }
}
