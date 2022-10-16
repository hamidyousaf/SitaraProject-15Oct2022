using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Numbers.Entity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Numbers.Entity.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Numbers.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public DashboardController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index(int id = 0)
        {
            HttpContext.Session.SetInt32("Resp_ID", id);
            try
            {
                HttpContext.Session.SetInt32("Resp_ID", id);
                HttpContext.Session.SetInt32("Resp_ID", id);
                int CompanyId = _dbContext.Sys_Responsibilities.Where(x => x.Resp_Id == id).FirstOrDefault().CompanyId;
                var companyName = _dbContext.AppCompanies.Where(x => x.Id == CompanyId).FirstOrDefault();
                ViewBag.CurrentCompany = companyName.Name;
                HttpContext.Session.SetInt32("CompanyId", CompanyId);
                HttpContext.Session.SetString("CompanyName",companyName.Name);
                HttpContext.Session.SetString("CompanyLogo", companyName.Logo);
                if (User.Identity.IsAuthenticated)
                {
                    var menu = JsonConvert.SerializeObject("");
                    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                    GetBankPosition();
                    GetStockPosition();
                    GetUnApprovedVouchers();
                    GetUnApprovedSales();
                    ViewBag.NavbarHeading = "Dashboard";
                    if (id != 0)
                    {
                        var countCat = 0;
                        var resMenu = _dbContext.Sys_Responsibilities.Where(x => x.Resp_Id == id).FirstOrDefault();
                        var newMenu1 = JsonConvert.SerializeObject((from m in _dbContext.SYS_MENU_M.Where(x => x.MENU_ID == resMenu.Menu_Id)
                                                                    from d in _dbContext.SYS_MENU_D
                                                                    from mm in _dbContext.AppMenus
                                                                    where m.IS_ACTIVE == true && m.MENU_ID == d.MENU_M_ID && d.SUBMENU_ID == mm.Id
                                                                    select new AppMenusViewModel
                                                                    {
                                                                        Id = m.MENU_ID,
                                                                        Name = m.MENU_NAME,
                                                                        ParentId = mm.ParentId,
                                                                        ChildName = mm.Name
                                                                    }).ToList());
                        ViewBag.AllMenus = JsonConvert.SerializeObject(_dbContext.AppMenus.ToList());
                        AppMenu mnu = _dbContext.AppMenus.Where(x => x.Id == resMenu.Menu_Id).FirstOrDefault();
                        ViewBag.ParaentMenu = mnu;

                        var newMenu = JsonConvert.SerializeObject(
                                               from rm in _dbContext.SYS_MENU_D
                                               where rm.MENU_M_ID == resMenu.Menu_Id
                                               orderby rm.SUBMENU_.ParentId
                                               select rm.SUBMENU_
                                             );
                        HttpContext.Session.SetString("menus", newMenu);
                    }
                    return View();
                }
                else
                    return RedirectToAction("Login", "Account", new { area = "" });
            }
            catch (Exception ex)
            {
                TempData["error"] = "true";
                TempData["message"] = ex.Message == null ? ex.InnerException.Message.ToString() : ex.Message.ToString();
                return RedirectToAction("Login", "Account");
            }
        }
        public IActionResult Responsibility(string q ="")
        {
            ViewBag.Company = new SelectList(_dbContext.AppCompanies.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
            var userId = HttpContext.Session.GetString("UserId");
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    ViewBag.NavbarHeading = "Responsibility";
                    return View();
                }
                else
                    return RedirectToAction("Login", "Account", new { area = "" });
            }
            catch (Exception ex)
            {
                TempData["error"] = "true";
                TempData["message"] = ex.Message == null ? ex.InnerException.Message.ToString() : ex.Message.ToString();
                return RedirectToAction("Login", "Account");
            }
        }
        public IActionResult GetResponsibility(int companyId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            ResponsibilityVM model = new ResponsibilityVM();
            model.ResponsibilitiesList = new List<Responsibilities>();
            var totalRes = _dbContext.Sys_ResponsibilitiesDetail.Where(a => a.UserId == userId && a.IsDeleted == false).ToList();
            model.ResponsibilitiesList = (from r in _dbContext.Sys_Responsibilities.Include(x => x.Company).Where(x=>x.CompanyId == companyId)
                                          join rd in totalRes on r.Resp_Id equals rd.ResponsibilityId

                                          select r).ToList();

            ViewBag.Company = new SelectList(_dbContext.AppCompanies.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
            ViewBag.Responsibilites = model.ResponsibilitiesList.Where(x => x.Effective_From_Date.Date <= DateTime.Now.Date && x.Effective_To_Date.Date >= DateTime.Now.Date);
            return PartialView("_ResponsibilityPartial", model);
        }
        public void GetBankPosition()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.BankPosition = _dbContext.VwGLVouchers.Where(c => c.CompanyId == companyId).GroupBy(v => v.AccountName).Select(y => new
            {
                Name = y.Key,
                Balance = y.Sum(x => x.Debit) - y.Sum(x => x.Credit)
            }).ToArray().Where(b => b.Balance > 0);
        }
        public void GetStockPosition()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.StockPosition = _dbContext.InvItems.Where(c => c.CompanyId == companyId && c.IsDeleted == false).OrderByDescending(o=>o.StockQty).Take(10).ToArray();
        }
        public void GetUnApprovedVouchers()
        {
            var vouchers = _dbContext.GLVouchers
               .Where(v => v.Status == "Created" && v.IsDeleted == false && v.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value
               && v.IsDeleted == false);
            ViewBag.UnApprovedVouchers = vouchers.Count();
        }
        public void GetUnApprovedSales()
        {
            var vouchers = _dbContext.ARInvoices
               .Where(v => v.Status == "Created" && v.Status !="Approved" && v.IsDeleted == false && v.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value
               && v.IsDeleted == false);
            ViewBag.UnApprovedSales = vouchers.Count();
        }
    }
}