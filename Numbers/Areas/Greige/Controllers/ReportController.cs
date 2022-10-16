using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;

namespace Numbers.Areas.Greige.Controllers
{
    public class ReportController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public ReportController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult StockLedger()
        {
            ViewBag.NavbarHeading = "Stock Ledger";
            return View();
        }

        public IActionResult StockBalance()
        {
            ViewBag.NavbarHeading = "Stock Balance";
            return View();
        }
    }
}