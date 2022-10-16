using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;

namespace Numbers.Areas.ApplicationModule.Controllers
{
    [Area("ApplicationModule")]
    [Authorize]
    public class AppFormController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public AppFormController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Application Forms";
            return View(_dbContext.AppMenus.ToList());
        }
        [HttpGet]
        public IActionResult Create(int? id)
        {
            ViewBag.AppModules = new SelectList(_dbContext.AppModules.OrderBy(c => c.Name).ToList(), "Id", "Name");

            if (id == null)
            {
                ViewBag.EntityState = "Create";
                ViewBag.Parent = new SelectList(_dbContext.AppMenus.Where(a => a.HasSubMenus == true).ToList(), "Id", "Name");
                ViewBag.NavbarHeading = "Create Application Form";
                ViewBag.Id = null;
                AppMenu model = new AppMenu();
                return View(model);
            }
            else
            {
                ViewBag.EntityState = "Update";
                ViewBag.Parent = new SelectList(_dbContext.AppMenus.Where(a => a.HasSubMenus == true).ToList(), "Id", "Name");
                ViewBag.NavbarHeading = "Update Application Form";
                AppMenu model = _dbContext.AppMenus.Find(id);
                ViewBag.ItemId = model.ModuleId;

                return View(model);
            }
        }
        [HttpPost]
        public IActionResult Create(AppMenu model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            if (model.Id == 0)
            {
                int Id = _dbContext.AppMenus.Select(x => x.Id).Max() + 1;
                model.Id = Id;
                model.CreatedBy = userId;
                model.CreatedDate = DateTime.UtcNow;
                model.CompanyId = companyId;
                if (model.ParentId != 0)
                    model.HasSubMenus = true;
                else
                    model.HasSubMenus = false;
                _dbContext.AppMenus.Add(model);
                TempData["error"] = "false";
                TempData["message"] = "Form has been saved successfully.";

            }
            else
            {
                //avoiding null insertion in model attributes
                var data = _dbContext.AppMenus.Find(model.Id);
                data.Name = model.Name;
                data.Url = model.Url;
                data.ModuleId = model.ModuleId;
                data.MenuLevel = model.MenuLevel;
                data.Sequence = model.Sequence;
                data.IconClass = model.IconClass;
                data.CompanyId = companyId;
                data.UpdatedBy = userId;
                data.UpdatedDate = DateTime.Now;
                data.IsActive = model.IsActive;
                data.ParentId = model.ParentId;
                if (model.ParentId != 0)
                    data.HasSubMenus = true;
                else
                    data.HasSubMenus = false;
                _dbContext.AppMenus.Update(data);
                TempData["error"] = "false";
                TempData["message"] = "Application Form has been updated successfully.";
            }
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "AppForm");
        }
        //public async Task<IActionResult> DeleteSupplier(int Id)
        //{

        //    var supplier = await _dbContext.APSuppliers.Where(n => n.Id == Id).FirstAsync();
        //    if (supplier != null)
        //    {
        //        supplier.IsActive = false;
        //        _dbContext.APSuppliers.Update(supplier);
        //        await _dbContext.SaveChangesAsync();
        //    }
        //    return RedirectToAction(nameof(Index));
        //}

    }
}