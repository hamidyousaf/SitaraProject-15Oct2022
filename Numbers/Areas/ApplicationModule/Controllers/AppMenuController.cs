using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using Numbers.Repository.Inventory;

namespace Numbers.Areas.ApplicationModule.Controllers
{
    [Authorize]
    [Area("ApplicationModule")]
    public class AppMenuController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public AppMenuController(NumbersDbContext context)
        {
            _dbContext = context;
        }

        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userid = HttpContext.Session.GetString("UserId");
            var storeIssueRepo = new AppMenuRepo(_dbContext);
            IEnumerable<SYS_MENU_M> list = storeIssueRepo.GetAll(companyId, userid);
            ViewBag.NavbarHeading = "List of App Menus";
            return View(list);
        }
 
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var storeIssueRepo = new AppMenuRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            ViewBag.Counter = 0;
            ViewBag.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            ViewBag.CostCenter = configValues.GetConfigValues("Inventory", "Cost Center", companyId);
            if (id == 0)
            {
                TempData["Mode"] = false;
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Application Menu";
          
                var model = new AppMenuViewModel();
                return View(model);
            }
            else
            {
                TempData["Mode"] = true;
                ViewBag.Id = id;
                AppMenuViewModel modelEdit = storeIssueRepo.GetById(id);
                // SYS_MENU_D[] storeIssueItems = storeIssueRepo.GetStoreIssueItems(id);
                //modelEdit.InvStoreIssueItems = _dbContext.SYS_MENU_D
                //                        .Include(i => i.SYS_FORMS)
                //                        .Include(i => i.SYS_MENU_M)
                //                          .Where(i => i.MENU_M_ID == id)
                //                          .ToList();

                modelEdit.InvStoreIssueItems = _dbContext.SYS_MENU_D.Where(i => i.MENU_M_ID == id).ToList();
                //foreach (var item in modelEdit.InvStoreIssueItems)
                //{
                //    item.Item.Name = item.Item.ItemFullName;
                //}
                // ViewBag.Items = storeIssueItems;
                // TempData["IssueNo"] = modelEdit.IssueNo;

                ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Application Menu";
                    ViewBag.TitleStatus = "Created";
               
                return View(modelEdit);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(AppMenuViewModel model,IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var menuRepo = new AppMenuRepo(_dbContext);
            if (model.MENU_ID == 0)
            {
                model.CREATED_BY = userId;
                bool isSuccess = await menuRepo.Create(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Menu  has been created successfully.";
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
                model.LAST_UPDATED_BY = userId;
                bool isSuccess = await menuRepo.Update(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Menu has been updated successfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var sysResp = _dbContext.Sys_Responsibilities.Where(x=>x.Menu_Id==Id).ToList();
            if (sysResp.Count()==0)
            {
                var ModuleRepo = new AppMenuRepo(_dbContext);
                bool isSuccess = await ModuleRepo.Delete(Id);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Menu has been deleted successfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
            }
            
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Menu is used in SystemResponsibilty cannot be deleted..!";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetStoreIssueItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var storeIssueRepo = new StoreIssueRepo(_dbContext);
            var viewModel = storeIssueRepo.GetStoreIssueItems(id, itemId);
            ViewBag.Counter = id;
            ViewBag.ItemId = viewModel.ItemId;
            return PartialView("_partialStoreIssueItem", viewModel);
        }

        
    }
}