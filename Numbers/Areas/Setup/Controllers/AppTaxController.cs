using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Repository.Setup;

namespace Numbers.Areas.Setup.Controllers
{
   [Area("Setup")]
    public class AppTaxController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public AppTaxController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            IEnumerable<AppTax> list = appTaxRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Taxes";
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            ViewBag.Company = new SelectList(_dbContext.AppCompanies.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
            if (id == 0)
            {
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Tax";
                return View(new AppTax());
            }
            else
            {
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Tax";
                var appTaxRepo = new AppTaxRepo(_dbContext);
                AppTax model = appTaxRepo.GetById(id);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppTax model)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var appTaxRepo = new AppTaxRepo(_dbContext);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                if (model.Id == 0)
                {
                    model.CreatedBy = userId;
                    //model.CompanyId = companyId;
                    model.CompanyId = model.CompanyId;
                    bool isSuccess = await appTaxRepo.Create(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Tax has been Created successfully.";
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
                    model.UpdatedBy = userId;
                    //model.CompanyId = companyId;
                    model.CompanyId = model.CompanyId;
                    bool isSuccess = await appTaxRepo.Update(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Tax has been updated successfully.";
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
            var appTaxRepo = new AppTaxRepo(_dbContext);
            bool isSuccess = await appTaxRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Tax has been deleted successfully.";
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