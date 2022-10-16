using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;

namespace Numbers.Areas.Setup.Controllers
{
    [Area("Setup")]
    public class AppSetupController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public AppSetupController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public IActionResult Index()
        {
            ViewBag.Navbarheading = "Company Setup";
            ViewBag.Company = new SelectList(_dbContext.AppCompanies.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(IEnumerable<AppCompanySetup> model)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            if (ModelState.IsValid)
            {
                foreach(var item in model)
                {
                    var appSetup = _dbContext.AppCompanySetups.Find(item.Id);
                    //appSetup.Name = model.Name; 
                    appSetup.Value = item.Value;
                    //appSetup.DataType = model.DataType;
                    //appSetup.IsRequired = model.IsRequired;
                    //appSetup.CompanyId = companyId;
                    //appSetup.CreatedBy = userId;
                    _dbContext.AppCompanySetups.Update(appSetup);
                }
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "SetUp has been added successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }
        public IActionResult GetCompanySetup(int companyId)
        {
            //ViewBag.GLAccounts = new SelectList(_dbContext.GLAccounts.Where(x => !x.IsDeleted).ToList(), "Code", "Code");
            List<AppCompanySetup> setupList = _dbContext.AppCompanySetups.Where(s => s.CompanyId == companyId).ToList();
            return PartialView("_CompanySetupPartial", setupList);
        }
    }
}