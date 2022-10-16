using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Repository.AppModule;
using System.Linq.Dynamic.Core;
using Numbers.Entity.ViewModels;

namespace Numbers.Areas.ApplicationModule.Controllers
{
    [Area("ApplicationModule")]
    [Authorize]
    public class ModuleController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public ModuleController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<AppCompanyModule> Module = await _dbContext.AppCompanyModules.Where(x => x.Is_Active == true).ToListAsync();
            return View(Module);
        }


        [HttpGet]
        public IActionResult Create(int? id)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            AppCompanyModule module;
            if (id == null)
            {
                module = new AppCompanyModule();
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create App Company Module";

                return View(module);
            }
            if (id != null)
            {
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Module";

                module = _dbContext.AppCompanyModules.Find(id);

                return View(module);
            }


            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(AppCompanyModule appCompanyModule, IFormCollection collection)
            {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string UserId = HttpContext.Session.GetString("UserId");

            if (appCompanyModule.Id == 0)
            {
                appCompanyModule.CompanyId = companyId;
                appCompanyModule.CreatedBy = UserId;
                var ModuleRepo = new ModuleRepo(_dbContext);
                bool isSuccess = await ModuleRepo.Create(appCompanyModule, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Module has been added successfully.";
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
                appCompanyModule.UpdatedDate = DateTime.Now;
                appCompanyModule.UpdatedBy = UserId;
                var ModuleRepo = new ModuleRepo(_dbContext);
                bool isSuccess = await ModuleRepo.Update(appCompanyModule, collection);

                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Module has been Updated successfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }

                return RedirectToAction(nameof(Index));

            }
        }

        public async Task<IActionResult> DeleteModule(int Id)
        {
            var ModuleRepo = new ModuleRepo(_dbContext);
            bool isSuccess = await ModuleRepo.Delete(Id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Module has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult GetList()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from Company in _dbContext.AppCompanyModules.Where(x => x.Is_Active == true && x.CompanyId == companyId) select Company);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                
                recordsTotal = Data.Count();
                var data = Data.Skip(skip).Take(pageSize).ToList();
                List<AppCompanyModulesViewModel> Details = new List<AppCompanyModulesViewModel>();
                foreach (var grp in data)
                {
                    AppCompanyModulesViewModel appCompanyModulesViewModel = new AppCompanyModulesViewModel();
                    appCompanyModulesViewModel.CreatedDate = grp.CreatedDate.ToString(Helpers.CommonHelper.DateFormat);
                    appCompanyModulesViewModel.AppCompanyModules = grp;
                    Details.Add(appCompanyModulesViewModel);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}