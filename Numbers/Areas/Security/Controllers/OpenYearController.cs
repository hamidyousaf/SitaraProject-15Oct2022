using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using Numbers.Repository.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Areas.Security.Controllers
{

    [Area("Security")]
    [Authorize]
    public class OpenYearController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NumbersDbContext _dbContext;
        public OpenYearController(NumbersDbContext context, IHttpContextAccessor http)
        {
            _dbContext = context;
            _httpContextAccessor = http;
        }
        public IActionResult Index()
        {
            IEnumerable<AppPeriod> list = _dbContext.AppPeriods.Where(x => x.Type == "Year" && x.CompanyId == HttpContext.Session.GetInt32("CompanyID").Value && x.IsGLClosed==false).ToList();
            return View(list);
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            var userId = HttpContext.Session.GetString("UserId");
           // ViewBag.Modules = new SelectList(_dbContext.AppModules.OrderBy(c => c.Description).ToList(), "Id", "Description");
            var model = new HROpenMonthViewModel();
            ViewBag.Counter = 0;
            ViewBag.EntityState = "Create";
             model = new OpenYearRepo(_dbContext, userId, companyId).GetYear();
            ViewBag.Modules = new SelectList(_dbContext.AppModules.FromSql("select * from AppModules where Id not in (Select ModuleId from AppPeriods where Description ={0})",model.MonthDescrption).ToList(), "Id", "Description");
            TempData["Description"] = model.MonthDescrption;
            TempData["ShortDescription"] = model.ShortDescription;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HROpenMonthViewModel model, IFormCollection collection)
        {
            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            var userId = HttpContext.Session.GetString("UserId");
            if (model.PeriodId != string.Empty)
            {
                var success = await new OpenYearRepo(_dbContext, userId, companyId).Create(model, collection);
                if (success)
                {

                    TempData["error"] = "false";
                    TempData["message"] = "Year is Open Successfully";
                   // return View();
                    return RedirectToAction(nameof(Create));
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong while Opening Year";
                    return View();
                }
            }
            else
            {
                //var success = await new MedicalRepo(_dbContext, userId, companyId).Update(model, collection);
                //if (success)
                //{
                //    TempData["error"] = "false";
                //    TempData["message"] = "Medical Bill has been Updated Successfully";
                //    return RedirectToAction(nameof(Index));
                //}
                //else
                //{
                //    TempData["error"] = "true";
                //    TempData["message"] = "Something went wrong while updating Medical Bill";
                   return View();
                //}
            }
        }

        public async Task<IActionResult> YearClose(int id)
        {
            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            var userId = HttpContext.Session.GetString("UserId");
            var openYearRepo = new OpenYearRepo(_dbContext, userId, companyId);
            bool isSuccess = await openYearRepo.Close(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Year Closed successfully.";
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