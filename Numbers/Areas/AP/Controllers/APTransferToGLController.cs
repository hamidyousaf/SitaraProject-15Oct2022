using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Areas.AP.Controllers
{
    [Area("AP")]
    [Authorize]
    public class APTransferToGLController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public APTransferToGLController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index()
        {
            var data = _dbContext.APLC.Where(a => a.IsActive == true && a.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value).ToList();
            List<APLC> aPLCList = new List<APLC>();
            foreach (var grp in data)
            {
                APLC aPLC = new APLC();
                grp.BankName = _dbContext.GLBankCashAccounts.FirstOrDefault(x => x.Id == grp.BankId).AccountName;
                grp.LCName = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == grp.LCTypeId).ConfigValue;
                aPLC = grp;
                aPLCList.Add(aPLC);
            }
            ViewBag.details = _dbContext.APGRNExpense.Where(x => x.IsDeleted == false && x.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value).ToList();
            ViewBag.NavbarHeading = "Transfer To GL";
            return View(aPLCList);
        }
        public IActionResult LCData(int[] LcIDs)
        {
            string _userId = HttpContext.Session.GetString("UserId");
            var ids = LcIDs.Select(id => (id)).ToList();
            var master = _dbContext.APLC.Where(x => ids.Contains(x.Id));
            var detail = _dbContext.APGRNExpense.Where(x => ids.Contains(x.LCId));
            var data = (from m in master
                        join d in detail on m.Id equals d.LCId
                         select new
                         {
                            m.Id,
                            m.LCNo,
                            m.LCTypeId,
                            m.LCOpenDate,
                            m.BankId, 
                            m.BankName,
                            m.TotalAmount,
                            m.CreatedDate,
                            d.GLCode,
                            d.AccountName,
                            d.ExpenseAmount,
                            d.ExpenseDate
                         }).ToList();

            
            return View();
        }
    }
}