using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Numbers.Areas.AP.Controllers
{
    [Area("AP")]
    [Authorize]
    public class ReportController : Controller
    {
        public IActionResult SupplierLedger()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Range(string name)
        {
           if (name == "PurchaseLedgerInvoiceWise")
                ViewBag.ReportType = "Purchase Ledger Invoice Wise";

            else if (name == "PurchaseJournal")
                ViewBag.ReportType = "Purchase Journal";
            return View();
        }
    }
}