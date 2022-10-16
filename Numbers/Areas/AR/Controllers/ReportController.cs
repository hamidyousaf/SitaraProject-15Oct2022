using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class ReportController : Controller
    {
        public IActionResult CustomerLedger()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Range(string name)
        {
            if (name == "SalesSummaryCustomerWise")
                ViewBag.ReportType = "Sales Summary Customer Wise";

            else if(name== "CustomerOutstanding")
                ViewBag.ReportType = "Customer Outstanding";

            else if(name== "SalesLedgerInvoiceWise")
                ViewBag.ReportType = "Sales Ledger Invoice Wise";

            else if (name == "SalesJournal")
                ViewBag.ReportType = "Sales Journal";

            else if (name == "SalesLedgerItemWise")
                ViewBag.ReportType = "Sales Ledger Item Wise";
            return View();
        }
    }
}