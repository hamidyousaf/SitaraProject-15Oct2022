using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;

namespace Numbers.Areas.GL.Controllers
{
    [Area("GL")]    
    public class ReportController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public ReportController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Ledger()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Vouchertypes = new SelectList(_dbContext.GLVoucherTypes.Where(d => d.IsActive == true && d.CompanyId == companyId), "VoucherType", "VoucherType");
            ViewBag.Costcenters = new SelectList(_dbContext.CostCenterDetails.Where(x => x.IsDeleted == false).ToList(), "Id", "Name");
            ViewBag.GLDivision = new SelectList(_dbContext.GLDivision.Where(x => x.IsDeleted == false && x.IsActive==true && x.IsApproved==true).ToList(), "Id", "Name");
            ViewBag.GLAccounts = new SelectList(from a in _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.AccountLevel == 4
                                               ).ToList() select new { Code=a.Code,Name=a.Code + " - " + a.Name }, "Code", "Name");
            var Period = _dbContext.AppPeriods.Where(x => x.PayrollOpen == true).ToList();
            Period.ForEach(x =>
            {
                x.Description = x.Description.Substring(4, 4);
            });
            ViewBag.Year = new SelectList(Period.Where(x => x.CompanyId == companyId).ToList(), "Description", "Description");
            LedgerViewModel ledgerViewModel = new LedgerViewModel();
            ledgerViewModel.GLAccounts = _dbContext.GLAccounts.Where(a => a.IsDeleted == false && a.AccountLevel == 4 /*&& a.IsActive == true*/)
                .Select(a => new GLAccount
                {
                    Id = a.Id,
                    Name = string.Concat(a.Code, " - ", a.Name),
                }).OrderBy(a => a.Code).ToList();

            ViewBag.NavbarHeading = "Ledger";
            return View(ledgerViewModel);
        }
        public IActionResult Ledger_v2()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Vouchertypes = new SelectList(_dbContext.GLVoucherTypes.Where(d => d.IsActive == true && d.CompanyId == companyId), "VoucherType", "VoucherType");
            ViewBag.Costcenters = new SelectList(_dbContext.CostCenterDetails.Where(x => x.IsDeleted == false).ToList(), "Id", "Name");
            ViewBag.GLDivision = new SelectList(_dbContext.GLDivision.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsApproved == true).ToList(), "Id", "Name");
            ViewBag.AppCompanies = new SelectList(_dbContext.AppCompanies.Where(x => x.IsDeleted == false).ToList(), "Id", "Name");
            ViewBag.GLAccounts = new SelectList(from a in _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.AccountLevel == 4
                                               ).ToList()
                                                select new { Code = a.Code, Name = a.Code + " - " + a.Name }, "Code", "Name");
            var Period = _dbContext.AppPeriods.Where(x => x.PayrollOpen == true).ToList();
            Period.ForEach(x =>
            {
                x.Description = x.Description.Substring(4, 4);
            });
            ViewBag.Year = new SelectList(Period.Where(x => x.CompanyId == companyId).ToList(), "Description", "Description");
            LedgerViewModel ledgerViewModel = new LedgerViewModel();
            ledgerViewModel.GLAccounts = _dbContext.GLAccounts.Where(a => a.IsDeleted == false && a.AccountLevel == 4 /*&& a.IsActive == true*/)
                .Select(a => new GLAccount
                {
                    Id = a.Id,
                    Name = string.Concat(a.Code, " - ", a.Name),
                }).OrderBy(a => a.Code).ToList();

            ViewBag.NavbarHeading = "Ledger";
            return View(ledgerViewModel);
        }

        public IActionResult LedgerDepWise()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Vouchertypes = new SelectList(_dbContext.GLVoucherTypes.Where(d => d.IsActive == true && d.CompanyId == companyId), "VoucherType", "VoucherType");
            ViewBag.Costcenters = new SelectList(_dbContext.CostCenterDetails.Where(x => x.IsDeleted == false).ToList(), "Id", "Name");
            ViewBag.GLDivision = new SelectList(_dbContext.GLDivision.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsApproved == true).ToList(), "Id", "Name");
            ViewBag.GLAccounts = new SelectList(from a in _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.AccountLevel == 4
                                               ).ToList()
                                                select new { Code = a.Code, Name = a.Code + " - " + a.Name }, "Code", "Name");
            var Period = _dbContext.AppPeriods.Where(x => x.PayrollOpen == true).ToList();
            Period.ForEach(x =>
            {
                x.Description = x.Description.Substring(4, 4);
            });
            ViewBag.Year = new SelectList(Period.Where(x => x.CompanyId == companyId).ToList(), "Description", "Description");
            LedgerViewModel ledgerViewModel = new LedgerViewModel();
            ledgerViewModel.GLAccounts = _dbContext.GLAccounts.Where(a => a.IsDeleted == false && a.AccountLevel == 4 /*&& a.IsActive == true*/)
                .Select(a => new GLAccount
                {
                    Id = a.Id,
                    Name = string.Concat(a.Code, " - ", a.Name),
                }).OrderBy(a => a.Code).ToList();

            ViewBag.NavbarHeading = "Ledger";
            return View(ledgerViewModel);
        }
        public IActionResult DailyActivitySummary()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Vouchertypes = new SelectList(_dbContext.GLVoucherTypes.Where(d => d.IsActive == true && d.CompanyId == companyId), "VoucherType", "VoucherType");
            ViewBag.Costcenters = new SelectList(_dbContext.CostCenterDetails.Where(x => x.IsDeleted == false).ToList(), "Id", "Name");
            ViewBag.GLDivision = new SelectList(_dbContext.GLDivision.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsApproved == true).ToList(), "Id", "Name");
            ViewBag.GLAccounts = new SelectList(_dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.AccountLevel == 4
                                               ).ToList(), "Code", "Code", " - ", "Name");
            var Period = _dbContext.AppPeriods.Where(x => x.PayrollOpen == true).ToList();
            Period.ForEach(x =>
            {
                x.Description = x.Description.Substring(4, 4);
            });
            //ViewBag.Year = new SelectList(Period.Where(x => x.CompanyId == companyId).ToList(), "Description", "Description");
            ViewBag.Year = new SelectList(Period.ToList(), "Description", "Description");

            ViewBag.NavbarHeading = "Daily Activity Summary";
            return View();
        }

        public IActionResult VoucherPrinting()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Vouchertypes = new SelectList(_dbContext.GLVoucherTypes.Where(d => d.IsActive == true && d.CompanyId == companyId && d.Id<18), "VoucherType", "VoucherType");
            ViewBag.Costcenters = new SelectList(_dbContext.CostCenterDetails.Where(x => x.IsDeleted == false).ToList(), "Id", "Name");
            ViewBag.GLDivision = new SelectList(_dbContext.GLDivision.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsApproved == true).ToList(), "Id", "Name");
            ViewBag.GLAccounts = new SelectList(_dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.AccountLevel == 4
                                               ).ToList(), "Code", "Code", " - ", "Name");
            var Period = _dbContext.AppPeriods.Where(x => x.CompanyId == companyId && x.PayrollOpen == true).ToList();
            Period.ForEach(x =>
            {
                x.Description = x.Description.Substring(4, 4);
            });
            ViewBag.Year = new SelectList(Period.Where(x => x.CompanyId == companyId).ToList(), "Description", "Description");

            ViewBag.NavbarHeading = "Voucher Printing";
            return View();
        }

        //public IActionResult Ledger()
        //{
        //    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    string userid = HttpContext.Session.GetString("UserId");
        //    ViewBag.Vouchertypes = new SelectList(_dbContext.GLVoucherTypes.Where(d => d.IsActive == true && d.CompanyId == companyId), "VoucherType", "VoucherType");
        //    //ViewBag.Branch = new CommonDDL(_dbContext).GetBranchesbyId(userid);

        //    return View();
        //}
        public IActionResult TrialBalance()
        {
            /*
            AppCompanyConfig path = _dbContext.AppCompanyConfigs
                                 .Where(n => n.ConfigName == "Report Path").FirstOrDefault();
            string startCode = _dbContext.GLAccounts
                                .OrderBy(x=>x.Code)
                                .Where(x => x.IsDeleted == false)
                                .FirstOrDefault()
                                .Code;
            string endCode = _dbContext.GLAccounts
                    .OrderByDescending(x => x.Code)
                    .Where(x => x.IsDeleted == false)
                    .FirstOrDefault()
                    .Code;
            ViewBag.ReportPath = path.ConfigValue;
            ViewBag.StartCode = startCode;
            ViewBag.EndCode = endCode;
    */
            ViewBag.NavbarHeading = "Trial Balance";
            return View();
        }
        public IActionResult ChartOfAccount()
        {
            ViewBag.NavbarHeading = "Chart of Account";
            return View();
        }
        public IActionResult ProfitLossComparison()
        {
            ViewBag.GLDivision = new SelectList(_dbContext.GLDivision.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsApproved == true).ToList(), "Id", "Name");
            ViewBag.NavbarHeading = "Profit Loss Comparison";
            return View();
        }
        public IActionResult Range(string type)
        {
            if (type == "DayActivity")
            {
                ViewBag.ReportType = "Day Activity";
                ViewBag.NavbarHeading = "Day Activity";
            }
            else if (type == "Profit/LossDetail")
            {
                ViewBag.ReportType = "Profit & Loss Detail";
                ViewBag.NavbarHeading = "Profit & Loss Detail";

            }
            else
            {
                ViewBag.ReportType = "Profit & Loss";
                ViewBag.NavbarHeading = "Profit & Loss";
                ViewBag.GLDivision = new SelectList(_dbContext.GLDivision.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsApproved == true).ToList(), "Id", "Name");
                ViewBag.Costcenters = new SelectList(_dbContext.CostCenterDetails.Where(x => x.IsDeleted == false ).ToList(), "Id", "Name");
            }
            return View();
        }

        public IActionResult BalanceSheet()
        {
            ViewBag.NavbarHeading = "Balance Sheet";
            return View();
        }
        public IActionResult OnScreenTrialBalance()
        {
            return View();
        }
        public IActionResult BalanceSheetDetail()
        {
            ViewBag.NavbarHeading = "Balance Sheet Detail";
            return View();
        }
        public IActionResult Realization()
        {
            ViewBag.NavbarHeading = "Realization";
            return View();
        }
    }
}