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
    public class OpenMonthController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NumbersDbContext _dbContext;
        private int _companyId = 0;
        public OpenMonthController(NumbersDbContext context, IHttpContextAccessor http)
        {
            _dbContext = context;
            _httpContextAccessor = http;
           // _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        }
        public IActionResult Index()
        {
            IEnumerable<AppPeriod> list = _dbContext.AppPeriods.Where(x => x.Type == "Month" && x.IsGLClosed == false && x.CompanyId== _companyId).ToList();
            return View(list);
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.Modules = new SelectList(_dbContext.AppModules.OrderBy(c => c.Description).ToList(), "Id", "Description");
            var model = new HROpenMonthViewModel();
            ViewBag.Counter = 0;
            ViewBag.EntityState = "Create";
             model = new OpenMonthRepo(_dbContext, userId, companyId).Getmonth();
            ViewBag.Modules = new SelectList(_dbContext.AppModules.FromSql("select * from AppModules where Id not in (Select ModuleId from AppPeriods where Description ={0})", model.MonthDescrption).ToList(), "Id", "Description");
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
                var success = await new OpenMonthRepo(_dbContext, userId, companyId).Create(model, collection);
                if (success)
                {

                    TempData["error"] = "false";
                    TempData["message"] = "Month is Open Successfully";
                    return RedirectToAction(nameof(Create));
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong while Opening Month";
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
        public async Task<IActionResult> MonthClose(int id)
        {
            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            var userId = HttpContext.Session.GetString("UserId");
            var openMonthRepo = new OpenMonthRepo(_dbContext,userId,companyId);
            bool isSuccess = await openMonthRepo.Close(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Month Closed successfully.";
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